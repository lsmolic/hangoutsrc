using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Reflection;
using System.Security.Policy;
using System.Xml;
using Hangout.Shared;
using Hangout.Server.StateServer;
using log4net;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;


namespace Hangout.Server
{
    public class PaymentItemsProcess : RemoteCallToService
    {
        private IPaymentItemInterface mPaymentItemProcess = null;
        private static ILog mLogger = LogManager.GetLogger("PaymentItemsProcess");

        /// <summary>
        /// Initialize
        /// </summary>
        public PaymentItemsProcess()
        {
        }

        /// <summary>
        // Activate the remote interface IPaymentItemInterface 
        /// </summary>
        /// <returns>true if activated else false</returns>
        public override bool ActivateTheInterface()
        {
            bool activated = false;

            try
            {
                WellKnownServiceTypeEntry WKSTE = new WellKnownServiceTypeEntry(typeof(IPaymentItemInterface), StateServerConfig.PaymentItemsServicesUrl, WellKnownObjectMode.SingleCall);
                if (RemotingConfiguration.ApplicationName == null)
                {
                    RemotingConfiguration.ApplicationName = "RestService";
                    RemotingConfiguration.RegisterWellKnownServiceType(WKSTE);
                }

                mPaymentItemProcess = (IPaymentItemInterface)Activator.GetObject(typeof(IPaymentItemInterface), WKSTE.ObjectUri);

                if (mPaymentItemProcess != null)
                {
                    activated = true;
                }
            }

            catch (Exception ex)
            {
                mLogger.Error("Error ActivateTheInterface ", ex);
            }

            return activated;
        }

        /// <summary>
        /// Invokes the remote PaymentItems ProcessMessage service call
        /// </summary>
        /// <param name="serviceCommand">PaymentItemCommand</param>
        /// <returns>xml string response from the PaymentItems service call</returns>
        protected override string InvokeServiceCommandHandler(ServiceCommand serviceCommand)
        {
            ServiceCommandSerializer serializer = new ServiceCommandSerializer();
            string xmlPaymentItemsMessage = serializer.SerializeCommandData(serviceCommand, typeof(PaymentCommand));

            return (mPaymentItemProcess.ProcessMessage(xmlPaymentItemsMessage));
        }

        /// <summary>
        /// The PaymentItems Service return, used in async PaymentItems remote method calls
        /// </summary>
        /// <param name="response">The PaymentItems response message</param>
        /// <param name="type">The message type not used for the PaymentItem calls</param>
        /// <param name="callback">Callback that is called on response from service</param>
        protected override void InvokeServiceReturn(string response, string type, System.Action<string> callback)
        {
            try
            {
                PaymentItemsProcessClientMessage clientMessage = new PaymentItemsProcessClientMessage();
                clientMessage.PaymentItemsReturn(response, callback);
            }

            catch (Exception ex)
            {
                mLogger.Error("Error InvokeServiceReturn ", ex);
            }
        }


        /// <summary>
        /// Method used to start a ProcessMessage async method call 
        /// This method processes PaymentItems messages
        /// If this is a cached message then retreive the data from the response data from the cache 
        /// </summary>
        /// <param name="xmlClientMessage">PaymentItem client message</param>
        /// <param name="sender">List of session GUID's to send the response to</param>
        /// <param name="callback">Callback that is called on response from service</param>
        /// <returns>blank if no error else the error message</returns>
        public string ProcessMessageAsync(string xmlClientMessage, System.Action<string> callback, ServerStores serverStores, getUserIdDelegate getUserIdCall)
        {
            string response = "";
            try
            {
                PaymentItemsProcessClientMessage clientMessage = new PaymentItemsProcessClientMessage();
                ServiceCommand serviceCommand = (ServiceCommand)clientMessage.ProcessMessage(xmlClientMessage, getUserIdCall);
                if (clientMessage.IsCachedMessage((PaymentCommand)serviceCommand, serverStores))
                {
                    response = clientMessage.GetCachedStoreData((PaymentCommand)serviceCommand, serverStores);
                    clientMessage.PaymentItemsReturn(response, callback);
                }
                else if (clientMessage.IsDoNothing((PaymentCommand)serviceCommand))
                {
                    response = clientMessage.createDoNothingResponse((PaymentCommand)serviceCommand);
                    clientMessage.PaymentItemsReturn(response, callback);
                }
                else
                {
                    InvokeServiceAsync(serviceCommand, callback);
                }
            }

            catch (Exception ex)
            {
                response = String.Format("<Error><Message>{0}</Message></Error>", ex);
                mLogger.Error(String.Format("Error ProcessMessageAsync: {0}", response));
            }

            return response;
        }

        /// <summary>
        /// Method used to start a ProcessMessage blocking method call 
        /// If this is a cached message then retreive the data from the response data from the cache 
        /// <param name="xmlClientMessage">PaymentItem client message</param>
        /// <param name="xmlClientMessage"></param>
        /// <returns>PaymentItems xml string response</returns>
        public string ProcessMessageBlocking(string xmlClientMessage, ServerStores serverStores, getUserIdDelegate getUserIdCall)
        {   
            string response = "";

            try
            {
                PaymentItemsProcessClientMessage clientMessage = new PaymentItemsProcessClientMessage();
                ServiceCommand serviceCommand = (ServiceCommand)clientMessage.ProcessMessage(xmlClientMessage, getUserIdCall);
                if (clientMessage.IsCachedMessage((PaymentCommand)serviceCommand, serverStores))
                {
                    response = clientMessage.GetCachedStoreData((PaymentCommand)serviceCommand, serverStores);
                }
                else
                {
                    response = InvokeServiceBlocking(serviceCommand);
                }
            }

            catch (Exception ex)
            {
                response = String.Format("<Error><Message>{0}</Message></Error>", ex);
                mLogger.Error(String.Format("Error ProcessMessageBlocking: {0}", response));
            }

            return response;
        }

        /// <summary>
        /// Creates a PaymentItem new user account and funds the VCOIN currency with an initial amount
        /// </summary>
        /// <param name="userInfo">userInfo object containing the new users account information</param>
        /// <param name="initialAmount">The initial amount of VCOIN</param>
        /// <returns>CreateNewUser PaymentCommand</returns>
        public string CreateNewUser(UserInfo userInfo, int initialCoinAmount, int initialCashAmount, System.Action<string> callback)
        {
            string response = "";

            try
            {
                PaymentCommand cmd = new PaymentCommand();

                cmd.Noun = "HangoutUsers";
                cmd.Verb = "CreateNewUser";

                cmd.Parameters.Add("name", userInfo.Name);
                cmd.Parameters.Add("ipAddress", userInfo.IPAddress);
                cmd.Parameters.Add("initialCoinAmount", initialCoinAmount.ToString());
                cmd.Parameters.Add("initialCashAmount", initialCashAmount.ToString());

                ServiceCommand serviceCommand = (ServiceCommand)cmd;

                if (callback != null)
                {
                    InvokeServiceAsync(serviceCommand, callback);
                }
                else
                {
                    response = InvokeServiceBlocking(serviceCommand);
                }
            }
            
            catch (Exception ex)
            {
                response = String.Format("<Error><Message>{0}</Message></Error>", ex);
                mLogger.Error(String.Format("Error CreateNewUser: {0}", response));
            }

            return response;
        }
    }

    public class PaymentItemsProcessClientMessage
    {
        private getUserIdDelegate mGetUserIdCall = null;
        private ILog mLogger;

        public PaymentItemsProcessClientMessage()
        {
            mLogger = LogManager.GetLogger("GeneralUse");
        }
        
        /// <summary>
        /// Converts the client PaymentItems command to a PaymentItems command 
        /// </summary>
        /// <param name="xmlClientMessage">xml client PaymentItems command</param>
        /// <returns>PaymentItems command</returns>
        public PaymentCommand ProcessMessage(string xmlClientMessage, getUserIdDelegate getUserIdCall)
        {
            mGetUserIdCall = getUserIdCall;
            ServiceCommandSerializer serializer = new ServiceCommandSerializer();
            PaymentCommand clientCommand = (PaymentCommand)serializer.DeserializeCommandData(xmlClientMessage, typeof(PaymentCommand));

            return (ParseClientPaymentCommand(clientCommand));
        }


        /// <summary>
        /// Async return method for PaymentItems
        /// Send the response to the client
        /// </summary>
        /// <param name="responseData">xml string response from PaymentItems service</param>
        /// <param name="sender">List of session GUID's to send the response to</param>
        /// <param name="callback">ServerMessageProcessor pointer used to call SendMessageToReflector</param>
        public void PaymentItemsReturn(string responseData, Action<string> callback)
        {
			RemoteCallToService.CallToServiceCallbackManager.Enqueue(callback, responseData);
        }
     
        /// <summary>
        /// Checks if this PaymentCommand is a special command that is a cached command
        /// </summary>
        /// <param name="command">PaymentCommand</param>
        /// <param name="serverStores">The itemObject pointer, to verify that it is not null</param>
        /// <returns>true of it is a cached message, otherwise false</returns>
        public bool IsCachedMessage(PaymentCommand command, object itemObject)
        {
            bool isCachedMessage = false;
            Dictionary<string, string> specialCommands = new Dictionary<string, string>();

            specialCommands.Add("GetStoreInventory", "HangoutStore");
            string value = SpecialCommand(command.Noun, command.Verb, specialCommands);

            switch (value)
            {
                case "GetStoreInventory":
                    if (itemObject != null)
                    {
                        string storeName = GetStringValueFromArgs(command.Parameters, "storeName", "");
                        ServerStores serverStores = (ServerStores)itemObject;
                        if (serverStores.IsCachedStore(storeName))
                        {
                            isCachedMessage = true;
                        }
                    }
                    break;
            }


            return isCachedMessage;
        }

        /// <summary>
        /// Retreive the Cached data from the store data cache
        /// Then filter it, sorted it and apply any paging.
        /// </summary>
        /// <param name="command">PaymentCommand</param>
        /// <param name="serverStores">serverStores object</param>
        /// <returns>The cached store response filtered, sorted and paged</returns>
        public string GetCachedStoreData(PaymentCommand command, ServerStores serverStores)
        {
            string response = "";

            if (serverStores != null)
            {
                string storeName = GetStringValueFromArgs(command.Parameters, "storeName", "");

                XmlDocument xmlResponse = serverStores.GetStoreResponseXML(storeName);
                Object  dataSetResponse = serverStores.GetStoreResponseDataSet(storeName);

                response = FilterSortStoreInventoryResponse(xmlResponse, dataSetResponse, command);
            }

            return response;
        }

        /// <summary>
        /// Checks if this PaymentCommand is a do nothing command that doesn't need to call out to services
        /// </summary>
        /// <param name="command">PaymentCommand</param>
        /// <returns>true of it is a do nothing message, otherwise false</returns>
        public bool IsDoNothing(PaymentCommand command)
        {
            bool isDoNothingMessage = false;
            Dictionary<string, string> specialCommands = new Dictionary<string, string>();

            specialCommands.Add("SecurePaymentInfo", "HangoutUsers");
            string value = SpecialCommand(command.Noun, command.Verb, specialCommands);

            switch (value)
            {
                case "SecurePaymentInfo":
                    isDoNothingMessage = true;
                   break;
            }

            return isDoNothingMessage;
        }

        /// <summary>
        /// Create a do nothing blank response with noun and verb
        /// </summary>
        /// <param name="command">PaymentCommand</param>
        /// <returns>a do nothing blank response with noun and verb</returns>
        public string createDoNothingResponse(PaymentCommand command)
        {
            return (String.Format("<Response noun='{0}' verb='{1}'></Response>", command.Noun, command.Verb));
        }


        /// <summary>
        /// Check if this is a special response
        /// If a special response then call the special response handler to modify the response xml
        /// </summary>
        /// <param name="responseData">The PaymentItems response string xml data</param>
        /// <returns>The response string xml data, modified if a special response else the inputed xml data</returns>
        public XmlDocument SpecialResponse(XmlDocument response, string noun, string verb, ServerStores serverStores)
        {
            MoneyTransactionLogging moneyLog = new MoneyTransactionLogging();

            Dictionary<string, string> specialResponses = new Dictionary<string, string>();
            specialResponses.Add("GetUserInventory", "HangoutUsers");
            specialResponses.Add("PurchaseGameCurrencyPayPal", "HangoutPurchase");
            specialResponses.Add("PurchaseGameCurrencyCreditCard", "HangoutPurchase");
            specialResponses.Add("PurchaseItems", "HangoutUsers");
            
            string value = SpecialCommand(noun, verb, specialResponses);

            switch (value)
            {
                case "GetUserInventory":
                    response = PaymentItemsSortFilter.AddItemsToUserInventory(response, serverStores);
                    break;

                case "PurchaseItems":
                    response = PaymentItemsSortFilter.AddAssetsToPurchaseResponse(response, serverStores);
                    break;

                case "PurchaseGameCurrencyPayPal":
                    moneyLog.PayPalLogResponse(response, "InProgress");
                    break;

                case "PurchaseGameCurrencyCreditCard":
                    moneyLog.CreditCardLogResponse(response);
                    break;

            }

            return response;
        }

        /// <summary>
        /// Check if this is a special command
        /// A command that matches the noun and verb with the item in the specialCommands dictionary
        /// If this is, then this command get special handling.
        /// </summary>
        /// <param name="commandNoun">PaymentCommand noun</param>
        /// <param name="commandVerb">PaymentCommand verb</param>
        /// <param name="specialCommands">The distionary of special command to find</param>
        /// <returns>Returns the verb as the special command name if found else returns an empty string</returns>
        private string SpecialCommand(string commandNoun, string commandVerb, Dictionary<string, string> specialCommands)
        {
            string specialName = "";

            foreach (KeyValuePair<string, string> kvp in specialCommands)
            {
                string noun = kvp.Value;
                string verb = kvp.Key;

                if ((commandNoun == noun) && (commandVerb == verb))
                {
                    specialName = verb;
                    break;
                }
            }
            return specialName;
        }

        /// <summary>
        /// Parse the client PaymentItems Command 
        /// Converts the client PaymentItems command to a PaymentItems command 
        /// </summary>
        /// <param name="clientCommand">Client PaymentItems command</param>
        /// <returns>PaymentItems command</returns>
        private PaymentCommand ParseClientPaymentCommand(PaymentCommand clientCommand)
        {
            MoneyTransactionLogging moneyLog = new MoneyTransactionLogging();
            PaymentCommand paymentCommand = null;
     
            switch (clientCommand.Verb)
            {
                case "GetUserBalance":
                    paymentCommand = CreateSimpleCommand("HangoutUsers", "GetUserBalance", clientCommand.Parameters);
                    break;

                case "AddVirtualCoinForUser":
                    paymentCommand = AddVirtualCoinForUser(clientCommand.Parameters);
                    break;

                case "GetUserInventory":
                    paymentCommand = GetUserInventory(clientCommand.Parameters);
                    break;

                case "GetStoreInventory":
                    paymentCommand = GetStoreInventory(clientCommand.Parameters);
                    break;

                case "PurchaseOffers":
                    paymentCommand = CreateSimpleCommand("HangoutPurchase", "PurchaseCashOffers", clientCommand.Parameters);
                    break;

                case "PurchaseCoinOffers":
                    paymentCommand = CreateSimpleCommand("HangoutPurchase", "PurchaseCoinOffers", clientCommand.Parameters);
                    break;

                case "PurchaseItems":
                    paymentCommand = PurchaseItems(clientCommand.Parameters, false);
                    break;

                case "PurchaseItemsGift":
                    paymentCommand = PurchaseItems(clientCommand.Parameters, true);
                    break;

                case "PurchaseGameCurrencyPayPal":
                    paymentCommand = PurchaseGameCurrencyPayPal(clientCommand.Parameters);
                    moneyLog.LogMoneyPaymentCommand("", "", paymentCommand.Parameters, "",  "Paypal");
                    break;

                case "PurchaseGameCurrencyCreditCard":
                    paymentCommand = PurchaseGameCurrencyCreditCard(clientCommand.Parameters);
                    moneyLog.LogMoneyPaymentCommand("", "", paymentCommand.Parameters, "", "CreditCard");
                    break;

                case "HealthCheck":
                    paymentCommand = new PaymentCommand();
                    paymentCommand.Noun = "HangoutInfo";
                    paymentCommand.Verb = "HealthCheck";
                    break;

                case "SecurePaymentInfo":
                    paymentCommand = new PaymentCommand();
                    paymentCommand.Noun = "HangoutUsers";
                    paymentCommand.Verb = "SecurePaymentInfo";
                    break;
                    
                default:
					StateServerAssert.Assert(new System.Exception("Invalid Payment Items Command"));
					break;
            }

            return paymentCommand;
        }


        /// <summary>
        /// Converts a simple client command to a PaymentCommmand
        /// A simple command is a a command that has a noun, verb and userId for parameters
        /// </summary>
        /// <param name="noun">The command noun</param>
        /// <param name="verb">The command verb</param>
        /// <param name="commandArgs">The command arguments</param>
        /// <returns>PaymentCommand</returns>
        public PaymentCommand CreateSimpleCommand(string noun, string verb, NameValueCollection commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();
            string userId = ConvertSessionIdToUserId(GetStringValueFromArgs(commandArgs, "userSession", ""));

            cmd.Noun = noun;
            cmd.Verb = verb;

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("startIndex", GetStringValueFromArgs(commandArgs, "startIndex", ""));
            cmd.Parameters.Add("blockSize", GetStringValueFromArgs(commandArgs, "blockSize", ""));

            return cmd;
        }


        /// <summary>
        /// Converts a AddVirtualCoinForUser client command to a PaymentCommmand
        /// </summary>
        /// <param name="commandArgs">The command arguments</param>
        /// <returns>AddVirtualCoinForUser PaymentCommand</returns>
        public PaymentCommand AddVirtualCoinForUser(NameValueCollection commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutUsers";
            cmd.Verb = "AddVirtualCoinForUser";

            string userId = GetStringValueFromArgs(commandArgs, "userId", "");
            if (userId == "")
            {
                userId = ConvertSessionIdToUserId(GetStringValueFromArgs(commandArgs, "userSession", ""));
            }

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("amount", GetStringValueFromArgs(commandArgs, "amount", ""));
            cmd.Parameters.Add("ipAddress", GetStringValueFromArgs(commandArgs, "ipAddress", ""));

            return cmd;
        }


        /// <summary>
        /// Converts a GetUserInventory client command to a PaymentCommmand
        /// </summary>
        /// <param name="commandArgs">The command arguments</param>
        /// <returns>GetUserInventory PaymentCommand</returns>
        public PaymentCommand GetUserInventory(NameValueCollection commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutUsers";
            cmd.Verb = "GetUserInventory";

            string userId = ConvertSessionIdToUserId(GetStringValueFromArgs(commandArgs, "userSession", ""));

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("startIndex", GetStringValueFromArgs(commandArgs, "startIndex", ""));
            cmd.Parameters.Add("blockSize", GetStringValueFromArgs(commandArgs, "blockSize", ""));
            cmd.Parameters.Add("itemTypeNames", GetStringValueFromArgs(commandArgs, "itemTypeNames", ""));

            return cmd;
        }


        /// <summary>
        /// Converts a GetStoreInventory client command to a PaymentCommmand
        /// </summary>
        /// <param name="commandArgs">The command arguments</param>
        /// <returns>GetStoreInventory PaymentCommand</returns>
        public PaymentCommand GetStoreInventory(NameValueCollection commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutStore";
            cmd.Verb = "GetStoreInventory";

            cmd.Parameters.Add("storeName", GetStringValueFromArgs(commandArgs, "storeName", ""));
            cmd.Parameters.Add("itemTypeNames", GetStringValueFromArgs(commandArgs, "itemTypeNames", ""));
            cmd.Parameters.Add("startIndex", GetStringValueFromArgs(commandArgs, "startIndex", ""));
            cmd.Parameters.Add("blockSize", GetStringValueFromArgs(commandArgs, "blockSize", ""));
            cmd.Parameters.Add("filter", GetStringValueFromArgs(commandArgs, "filter", ""));
            cmd.Parameters.Add("orderby", GetStringValueFromArgs(commandArgs, "orderby", ""));
            return cmd;
        }
        
        /// <summary>
        /// Filter, Sort, and Page the Store Inventory 
        /// </summary>
        /// <param name="response">XML Document response from the GetStoreInventory cached response</param>
        /// <param name="dataSetResponse">Dataset containing the GetStoreInventory cached response</param>
        /// <param name="command">PaymentItem Command sent, used to retrieve the Filter, OrderBy, StartIndex and BlockSize parameters</param>
        /// <returns>xml string response of the filtered, sorted and paged store inventory</returns>
        public string FilterSortStoreInventoryResponse(XmlDocument response, object dataSetResponse, PaymentCommand command)
        {
            ResultFilter resultFilter = PaymentItemsSortFilter.GetFilterParameters(command);

            string filter = resultFilter.Filter;            //"itemOffer_title Like '%basic%'";                  
            string sort = resultFilter.OrderBy;             //"itemOffer_endDate desc, item_itemTypeName asc";

            if (filter == null)
            {
                filter = "";
            }

            if (sort == null)
            {
                sort = "";
            }

            filter = AddItemTypeNamesToFilter(filter, resultFilter.ItemTypeNames);

            int startIndex;
            int blockSize;

            if (!int.TryParse(resultFilter.StartIndex, out startIndex))
            {
                startIndex = -1;
            }
            if (!int.TryParse(resultFilter.BlockSize, out blockSize))
            {
                blockSize = -1;
            }


            response = PaymentItemsSortFilter.FilterSortTheStoreResponse(response, dataSetResponse, filter, sort, startIndex, blockSize);

            return (response.InnerXml);
        }

        /// <summary>
        /// Add the ItemTypeNames into the filter.
        /// The ItemType Names are AND'ed with the filter and OR'ed with each other
        /// </summary>
        /// <param name="filter">The filter expression</param>
        /// <param name="itemTypeNames">The comma seperated list of ItemTypeNames</param>
        /// <returns>string containg the filter expression where the ItemType Names are AND'ed with the filter and OR'ed with each other</returns>
        private string AddItemTypeNamesToFilter(string filter, string itemTypeNames)
        {
            string returnFilter = filter;
            string itemTypeNameFilterString = "";

            if ((itemTypeNames != null) && (itemTypeNames.Trim().Length > 0))
            {
                string[] itemTypeNamesArray = itemTypeNames.Split(',');

                StringBuilder itemTypeNameFilter = new StringBuilder();
                string delim = "";

                foreach (string itemTypeName in itemTypeNamesArray)
                {
                    itemTypeNameFilter.Append(String.Format("{0} (item_itemTypeName LIKE '{1}') ", delim, itemTypeName.Trim()));
                    delim = "OR";
                }

                if (itemTypeNameFilter.Length > 0)
                {
                    itemTypeNameFilterString = String.Format("({0})", itemTypeNameFilter.ToString());
                }
                
                if (filter.Trim().Length > 0)
                {
                    filter += " AND ";
                }

                returnFilter = filter + itemTypeNameFilterString;
            }

            return returnFilter;
        }


        /// <summary>
        /// Converts a PurchaseItems client command to a PaymentCommmand for both PurchaseItems and PurchaseGift
        /// </summary>
        /// <param name="commandArgs">The command arguments</param>
        /// <param name="giftFlag">Is this a gift purchase</param>
        /// <returns>PurchaseItems or PurchaseGift PaymentCommand</returns>
        public PaymentCommand PurchaseItems(NameValueCollection commandArgs, bool giftFlag)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutUsers";
            cmd.Verb = "PurchaseItems";

            string userId = ConvertSessionIdToUserId(GetStringValueFromArgs(commandArgs, "userSession", ""));

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("currencyName", GetStringValueFromArgs(commandArgs, "currencyName", ""));
            cmd.Parameters.Add("offerIds", GetStringValueFromArgs(commandArgs, "offerIds", ""));
            cmd.Parameters.Add("ipAddress", GetStringValueFromArgs(commandArgs, "ipAddress", ""));
            mLogger.InfoFormat("PurchaseItems called user:{0} currency:{1} offerIds:{2} ipAddress:{3}", cmd.Parameters[0], cmd.Parameters[1], cmd.Parameters[2], cmd.Parameters[3]);

            if (giftFlag)
            {
                cmd.Verb = "PurchaseGift";
                cmd.Parameters.Add("recipientUserId", GetStringValueFromArgs(commandArgs, "recipientUserId", ""));
                cmd.Parameters.Add("noteToRecipient", GetStringValueFromArgs(commandArgs, "noteToRecipient", ""));
            }

            return cmd;
        }

        /// <summary>
        /// Converts a PurchaseGameCurrencyPayPal client command to a PaymentCommmand
        /// </summary>
        /// <param name="commandArgs">The command arguments</param>
        /// <returns>PurchaseGameCurrencyPayPal PaymentCommand</returns>
        public PaymentCommand PurchaseGameCurrencyPayPal(NameValueCollection commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutPurchase";
            cmd.Verb = "PurchaseGameCurrencyPayPal";

            string userId = ConvertSessionIdToUserId(GetStringValueFromArgs(commandArgs, "userSession", ""));

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("offerId", GetStringValueFromArgs(commandArgs, "offerId", ""));
            cmd.Parameters.Add("ipAddress", GetStringValueFromArgs(commandArgs, "ipAddress", ""));

            cmd.Parameters.Add("externalTxnId", Guid.NewGuid().ToString());

            return cmd;
        }

        /// <summary>
        /// Converts a PurchaseGameCurrencyCreditCard client command to a PaymentCommmand 
        /// </summary>
        /// <param name="commandArgs">The command arguments</param>
        /// <returns>PurchaseGameCurrencyCreditCard PaymentCommand</returns>
        public PaymentCommand PurchaseGameCurrencyCreditCard(NameValueCollection commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutPurchase";
            cmd.Verb = "PurchaseGameCurrencyCreditCard";

            string userId = ConvertSessionIdToUserId(GetStringValueFromArgs(commandArgs, "userSession", ""));

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("offerId", GetStringValueFromArgs(commandArgs, "offerId", ""));
            cmd.Parameters.Add("ipAddress", GetStringValueFromArgs(commandArgs, "ipAddress", ""));
            cmd.Parameters.Add("secureKey", GetStringValueFromArgs(commandArgs, "secureKey", ""));
            cmd.Parameters.Add("creditCardNumber", GetStringValueFromArgs(commandArgs, "creditCardNumber", ""));
            cmd.Parameters.Add("creditCardType", GetStringValueFromArgs(commandArgs, "creditCardType", ""));
            cmd.Parameters.Add("expireDate", GetStringValueFromArgs(commandArgs, "expireDate", ""));
            cmd.Parameters.Add("securityCode", GetStringValueFromArgs(commandArgs, "securityCode", ""));
            cmd.Parameters.Add("firstName", GetStringValueFromArgs(commandArgs, "firstName", ""));
            cmd.Parameters.Add("lastName", GetStringValueFromArgs(commandArgs, "lastName", ""));
            cmd.Parameters.Add("address", GetStringValueFromArgs(commandArgs, "address", ""));
            cmd.Parameters.Add("city", GetStringValueFromArgs(commandArgs, "city", ""));
            cmd.Parameters.Add("state", GetStringValueFromArgs(commandArgs, "state", ""));
            cmd.Parameters.Add("zipCode", GetStringValueFromArgs(commandArgs, "zipCode", ""));
            cmd.Parameters.Add("countryCode", GetStringValueFromArgs(commandArgs, "countryCode", ""));
            cmd.Parameters.Add("phoneNumber", GetStringValueFromArgs(commandArgs, "phoneNumber", ""));

            cmd.Parameters.Add("externalTxnId", Guid.NewGuid().ToString());

            return cmd;
        }


        /// <summary>
        /// If the data is not null returns the data else returns the defaultData
        /// </summary>
        /// <param name="data">input data to check for null</param>
        /// <param name="defaultData">default data</param>
        /// <returns>If the data is not null returns the data else returns the defaultData</returns>
        private string GetUserInfoData(string data, string defaultData)
        {
            string returnData = defaultData;
            if (data != null)
            {
                returnData = data;
            }
            return returnData;
        }

        /// <summary>
        /// Given the sessionId gets the PaymentItems userId
        /// </summary>
        /// <param name="sessionId">The sessionId</param>
        /// <returns>The PaymentItems userId</returns>
        private string ConvertSessionIdToUserId(string sessionId)
        {
            string userId = "";
            Guid sessionIdGuid = new Guid(sessionId);

            if (mGetUserIdCall != null)
            {
                userId = mGetUserIdCall(sessionIdGuid);
               }
            return userId;
        }

        /// <summary>
        /// Retrieve the value given the commandArgs and a key
        /// </summary>
        /// <param name="commandArgs">The command arguments</param>
        /// <param name="key">The key </param>
        /// <param name="defaultValue">If an error occurs or the key is not available then returns the default value</param>
        /// <returns>The command argument value or if an error occurs or the key is not available then returns the default value</returns>
        private string GetStringValueFromArgs(NameValueCollection commandArgs, string key, string defaultValue)
        {
            string value = defaultValue;
            try
            {
                value = commandArgs[key];
                if (value == null)
                {
                    value = defaultValue;
                }
            }

            catch (Exception Exception)
            {
                mLogger.Error("Error GetStringValueFromArgs", Exception);
            }

            return value;
        }

    }
}
