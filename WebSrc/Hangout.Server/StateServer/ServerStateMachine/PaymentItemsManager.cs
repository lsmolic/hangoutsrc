using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;
using log4net;

namespace Hangout.Server
{
    public class PaymentItemsManager : AbstractExtension
    {
        private static ILog mLogger = LogManager.GetLogger("PaymentItemsManager");
        private readonly MoneyPaymentNotifyClient mMoneyPaymentNotifyClient = null;

        private readonly PaymentItemsProcess mPaymentItems = null;
        public PaymentItemsProcess PaymentItems
        {
            get { return mPaymentItems; }
        }

        private readonly RemoteCallToService mCallToService = null;
        private readonly ServerStores mServerStores = null;


        /// <summary>
        /// PaymentItemManager constuctor
        /// </summary>
        /// <param name="serverStateMachine">A pointer to the server state machine</param>
        /// <param name="sendMessageToReflectorCallback">A pointer to send a message to the reflector</param>
        public PaymentItemsManager(ServerStateMachine serverStateMachine) : base (serverStateMachine)
        {
            mServerStores = new ServerStores(this);
            mMoneyPaymentNotifyClient = new MoneyPaymentNotifyClient(SendMessageToClient);

            // Initialize the remoting interface
            try
            {
                mCallToService = new RemoteCallToService();
                mCallToService.RemoteConnect();
                mCallToService.ActivateTheInterface();
                mPaymentItems = new PaymentItemsProcess();
                mPaymentItems.ActivateTheInterface();
            }
            catch
            {
                mLogger.Warn("Error in activating the payment items interface");
            }
        }


        /// <summary>
        /// Parser for PaymentITems command
        /// </summary>
        /// <param name="message">The message from the client</param>
        /// <param name="senderId">The senderId GUID</param>
        public override void ReceiveRequest(Message message, Guid senderId)
        {
            string xmlData = (string)message.Data[0];
            System.Action<string> asyncCallback = delegate(string paymentItemsCommand)
            {
                try
                {
                    XmlDocument response = new XmlDocument();
                    response.LoadXml(paymentItemsCommand);

                    string noun = response.SelectSingleNode("/Response").Attributes["noun"].InnerText;
                    string verb = response.SelectSingleNode("/Response").Attributes["verb"].InnerText;

                    // Append special info to response for some payment items commands
                    PaymentItemsProcessClientMessage clientMessage = new PaymentItemsProcessClientMessage();
                    response = clientMessage.SpecialResponse(response, noun, verb, mServerStores);

                    // Award items in the hangout db that correspond with this purchase
                    AwardPurchasedItems(response, noun, verb, senderId);

                    response = GetPaymentItemSecurePaymentInfoEncrypted(response, noun, verb, senderId);

                    Message paymentItemsResponse = new Message();
                    List<object> dataObject = new List<object>();
                    dataObject.Add(response.InnerXml);
                    paymentItemsResponse.PaymentItemsMessage(dataObject);
                    paymentItemsResponse.Callback = message.Callback;

                    mLogger.DebugFormat("ProcessMessageAsync responded: {0},{1}.  Response Size:{2}", noun, verb, paymentItemsCommand.Length);

                    mServerStateMachine.SendMessageToReflector(paymentItemsResponse, senderId);
                }
                catch (System.Exception ex)
                {
                    mLogger.ErrorFormat("Error in PaymentItems asyncCallback {0} {1}", ex, paymentItemsCommand);
                }
            };
            getUserIdDelegate getUserId = new getUserIdDelegate(GetPaymentItemsUserId);
			XmlDocument xmlRequest = new XmlDocument();
			xmlRequest.LoadXml(xmlData);
            mLogger.DebugFormat("ProcessMessageAsync called: {0}", xmlRequest.SelectSingleNode("/Request").Attributes["verb"].InnerText);
            mPaymentItems.ProcessMessageAsync(xmlData, asyncCallback, mServerStores, getUserId);
        }

        private void AwardPurchasedItems(XmlDocument response, string noun, string verb, Guid senderId)
        {
            // If the response was for a PurchaseItems, let the individual managers parse the response and see if there are items to reward
            if ((noun == "HangoutUsers") && (verb == "PurchaseItems"))
            {
                // Check room purchase
                XmlNodeList purchasedRoomItems = response.SelectNodes("/Response/purchaseResults/purchaseResult/offer/item[@itemTypeName='" + ItemType.ROOM + "']");
                if (purchasedRoomItems.Count > 0) 
                {
                    mServerStateMachine.RoomManager.RoomItemPurchaseCallback(purchasedRoomItems, senderId);
                }

                // Energy refill purchase
                XmlNodeList purchasedEnergyItems = response.SelectNodes("/Response/purchaseResults/purchaseResult/offer/item[@itemTypeName='" + ItemType.ENERGY_REFILL + "']");
                if (purchasedEnergyItems.Count > 0)
                {
                    ServerAccount serverAccount = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(senderId);
                    mServerStateMachine.FashionMinigameServer.EnergyManager.RefillEnergy(serverAccount);
                }
            }
        }

        /// <summary>
        /// Given an ItemId retrieve the Asset XML data from the ServerAssetRepository
        /// </summary>
        /// <param name="itemId">The itemId for the assests to retrieve</param>
        /// <returns>xml document containing the Asset data</returns>
        public string GetAssetDataSetDna(string itemId)
        {
            uint rawUIntValue = 0;
            List<ItemId> itemIds = new List<ItemId>();

            rawUIntValue = Convert.ToUInt32(itemId);
            itemIds.Add(new ItemId(rawUIntValue));

            XmlDocument assetDna = mServerStateMachine.ServerAssetRepository.GetXmlDna(itemIds);

            XmlNode assetDnaNode = assetDna.SelectSingleNode("/Items/Item/Assets");
            if (assetDnaNode != null)
            {
                return assetDnaNode.InnerXml;
            }
            return "";
        }

        public void ProcessPaymentCommand(PaymentCommand paymentCommand, System.Action<string> asyncCallback)
        {
            mPaymentItems.InvokeServiceAsync(paymentCommand, asyncCallback);
        }


        /// <summary>
        /// Returns the current usrs encrypted payment information needed to start a 
        /// Money transaction. The payment information needed to start a Money transaction is 
        /// the combination of the sessionId GUID and the hangoutUserId 
        /// </summary>
        /// <param name="response">The response from the Payment Command</param>
        /// <param name="noun">The noun for the payment command</param>
        /// <param name="verb">The verb for the payment command</param>
        /// <param name="sessionId">The sessionId GUID for the current users session</param>
        /// <returns></returns>
        public XmlDocument GetPaymentItemSecurePaymentInfoEncrypted(XmlDocument response, string noun, string verb, Guid sessionId)
        {
            if ((noun == "HangoutUsers") && (verb == "SecurePaymentInfo"))
            {
                ServerAccount account = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);
                string hangoutUserId = account.AccountId.ToString();

                string secureInfo = String.Format("{0}&{1}", sessionId, hangoutUserId);
                SimpleCrypto crypt = new SimpleCrypto();

                XmlNode newNode = response.CreateNode("element", "SecureInfo", "");
                newNode.InnerText = crypt.TDesEncrypt(secureInfo);

                XmlNode xmlDocNode = response.SelectSingleNode("/Response");
                xmlDocNode.AppendChild(newNode);
            }
            return response;
        }


        /// <summary>
        /// Given the session returns the PaymentItems UserId
        /// If the paymentItems UserId is not available from ServerAccounts 
        /// (The paymentItems UserId is not in our db)
        /// then call CreateNewUser blocking to create account or get paymentItems UserId
        /// If teh account is still not available log the error and return a empty string
        /// Then call account update asynchronous without a return and continue.
        /// </summary>
        /// <param name="sessionId">The users session id</param>
        /// <returns>The PaymentItems userId</returns>
        public string GetPaymentItemsUserId(Guid sessionId)
        {
            string userId = "";
			string xmlResponseString = "";

            try
            {
                ServerAccount account = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(sessionId);
                userId = account.PaymentItemUserId.ToString();

                //call CreateNewUser blocking to create account or get user id 
                //since we do not have a paymentItemsId in our db.
                //Then call account update and continue.
                if (userId.Trim().Length == 0)
                {
                    mLogger.Info("GetPaymentItemsUserId: Creating new payment items user for hangout user: " + userId);
                    string ipAddress = mServerStateMachine.ServerMessageProcessor.ServerReflector.GetClientIPAddress(sessionId);
                    UserInfo userInfo = mServerStateMachine.UsersManager.GetPaymentItemsUserInfo(account, ipAddress);
                    int initialCoinAmount = mServerStateMachine.UsersManager.GetPaymentItemsUserInfoUserInitialCoinAmount();
                    int initialCashAmount = mServerStateMachine.UsersManager.GetPaymentItemsUserInfoUserInitialCashAmount();

					xmlResponseString = mPaymentItems.CreateNewUser(userInfo, initialCoinAmount, initialCashAmount, null);
                    XmlDocument responseDoc = new XmlDocument();
					responseDoc.LoadXml(xmlResponseString);

                    account.PaymentItemUserId = responseDoc.SelectSingleNode("/Response/user").Attributes["id"].InnerText;
                    account.PaymentItemSecureKey = responseDoc.SelectSingleNode("/Response/user").Attributes["secureKey"].InnerText;

                    userId = account.PaymentItemUserId;

                    mServerStateMachine.UsersManager.UpdateServerPaymentItemsAccount(account, null);
                }
            }

            catch (Exception ex)
            {
                mLogger.Error("Error getPaymentItemsUserId, response = " + xmlResponseString, ex);
            }

            return userId;
        }

    }
}
