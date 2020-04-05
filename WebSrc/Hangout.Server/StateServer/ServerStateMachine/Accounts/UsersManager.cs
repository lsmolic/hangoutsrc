using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using Hangout.Shared.Messages;
using System.Xml;
using Hangout.Server.StateServer;
using log4net;

namespace Hangout.Server
{
    public class UsersManager : AbstractExtension
    {
		private SessionManager mSessionManager = null;
		private static ILog mLogger = LogManager.GetLogger("UsersManager");

		public UsersManager(ServerStateMachine serverStateMachine) : base (serverStateMachine)
        {
			mSessionManager = serverStateMachine.SessionManager;

			mMessageActions.Add(MessageSubType.SaveUserProperties, SaveUserAccountForClient);
        }

		public void SaveUserAccountForClient(Message receivedMessage, Guid senderId)
		{
			UserProperties userProperties = CheckType.TryAssignType<UserProperties>(receivedMessage.Data[0]);

			ServerAccount serverAccount = mSessionManager.GetServerAccountFromSessionId(senderId);
            serverAccount.UserProperties = userProperties;
			serverAccount.SaveCurrentAccountData(delegate(XmlDocument returnedXmlDocument){});
		}

        public void GetAccountForUser(string fbAccountId, string fbSessionKey, string nickName, string firstName, string lastName, string userIpAddress, string campaignId, string referrerId, Action<ServerAccount> getAccountForUserCallback)
        {
            Action<XmlDocument> getAccountForFacebookIdCallback = delegate(XmlDocument receivedXmlGetAccount)
            {
                //<Accounts> 
                //  <Account accountId="" fbaccountid="" tfaccountId="" nickname="" firstname="" lastname=""/>
                //</Accounts>
                XmlNode accountXmlNode = receivedXmlGetAccount.SelectSingleNode("Accounts/Account");
                if (accountXmlNode != null)
                {
					ServerAccount serverAccount = AccountsXmlUtil.GetAccountFromXml(accountXmlNode);

					if (serverAccount != null)
					{
						serverAccount.IpAddress = userIpAddress;
						mLogger.Info("GetAccountForUser, ServerAccount: " + serverAccount.ToString());
						serverAccount.SaveCurrentAccountData(delegate(XmlDocument returnedXmlDocument)
						{
						});
						if (serverAccount.PaymentItemUserId.Trim().Length == 0)
						{
							mLogger.Debug("GetAccountForUser.CreatePaymentItemsAccount");
							CreatePaymentItemAccountForUser(serverAccount, userIpAddress, getAccountForUserCallback);
						}
						else
						{
							mLogger.Debug("GetAccountForUser, calling callback");
							getAccountForUserCallback(serverAccount);
						}
					}
					else
					{
						StateServerAssert.Assert(new Exception("Could not extract the account from the Xml"));
					}
                }
                else
                {
                    CreateAccountForUser(fbAccountId, fbSessionKey, nickName, firstName, lastName, userIpAddress, campaignId, referrerId, delegate(ServerAccount newServerAccount)
                        {
                            getAccountForUserCallback(newServerAccount);
                        }
                    );
                }
            };
            try
            {
                CallGetServerAccountForFacebookIdService(fbAccountId, getAccountForFacebookIdCallback);
            }
            catch (System.Exception)
            {
                mLogger.Warn("Could not get an account for facebook id: " + fbAccountId);
                getAccountForUserCallback(null);
            }
        }

		protected virtual void CallGetServerAccountForFacebookIdService(string fbAccountId, Action<XmlDocument> getAccountForFacebookIdCallback)
		{
            AccountsServiceAPI.GetServerAccountForFacebookId(fbAccountId, getAccountForFacebookIdCallback); 
		}

		public void CreateAccountForUser(string fbAccountId, string fbSessionKey, string nickName, string firstName, string lastName, string userIpAddress, string campaignId, string referrerId, Action<ServerAccount> createAccountForUserCallback)
		{
			mLogger.Debug(String.Format("CreateAccountForUser {0} {1} {2} {3} {4} {5} {6} {7}", fbAccountId, fbSessionKey, nickName, firstName, lastName, userIpAddress, campaignId, referrerId));
			Action<XmlDocument> createServerAccountServiceCallback = delegate(XmlDocument receivedXmlCreateNewAccount)
			{
				XmlNode newAccountXmlNode = receivedXmlCreateNewAccount.SelectSingleNode("Accounts/Account");
				//if we get a null xml node when trying to create an account, we need to throw an error
				if (newAccountXmlNode == null)
				{
					StateServerAssert.Assert(new System.Exception("Error: unable to create a new account.. do you have a valid facebook Account Id, Session key, nickname, firstname, lastname, campaignId, and referredId?  Check your client data file! Returned Xml: " + receivedXmlCreateNewAccount.OuterXml));
					createAccountForUserCallback(null);
				}
				else
				{
					ServerAccount serverAccount = AccountsXmlUtil.GetAccountFromXml(newAccountXmlNode);
                    serverAccount.IpAddress = userIpAddress;
                    SaveCurrentAccountData(serverAccount);
					CreatePaymentItemAccountForUser(serverAccount, userIpAddress, createAccountForUserCallback);

                    Metrics.Log(LogGlobals.CATEGORY_ACCOUNT, LogGlobals.EVENT_ACCOUNT_CREATED, LogGlobals.ACCOUNT_ID_LABEL, serverAccount.AccountId.ToString(), serverAccount.AccountId.ToString());
                }
			};
			CallCreateServerAccountService(fbAccountId, fbSessionKey, nickName, firstName, lastName, userIpAddress, campaignId, referrerId, createServerAccountServiceCallback);
		}

        /// <summary>
        /// Save the current account data to the db
        /// Make this virtual so we can override it in our unit tests, so we don't actually make a service call
        /// </summary>
        /// <param name="serverAccount"></param>
        protected virtual void SaveCurrentAccountData(ServerAccount serverAccount)
        {
            serverAccount.SaveCurrentAccountData(delegate(XmlDocument returnedXml) { });
        }

		protected virtual void CallCreateServerAccountService(string fbAccountId, string fbSessionKey, string nickName, string firstName, string lastName, string userIpAddress, string campaignId, string referrerId, Action<XmlDocument> createAccountForUserCallback)
		{
			AccountsServiceAPI.CreateServerAccount(fbAccountId, fbSessionKey, nickName, firstName, lastName, campaignId, referrerId, createAccountForUserCallback);
		}

		private void UpdateAccountDataForUser(ServerAccount serverAccount, UserProperties userProperties, System.Action<XmlDocument> finishedUpdatingAccountDataCallback)
		{
			serverAccount.UserProperties = userProperties;
			XmlDocument accountDataXml = AccountsXmlUtil.CreateAccountDataXml(serverAccount);
			AccountsServiceAPI.UpdateAccountData(serverAccount.AccountId, accountDataXml, finishedUpdatingAccountDataCallback);
		}

        /// <summary>
        /// Create the PaymentItem Account for a user
        /// </summary>
        /// <param name="accountXmlNode">The account information xml node</param>
        /// <param name="serverAccount">The account object </param>
        /// <param name="accountForUserCallback">The callback to call when finished</param>
        public virtual void CreatePaymentItemAccountForUser(ServerAccount serverAccount, string userIpAddress, Action<ServerAccount> accountForUserCallback)
        {
            mLogger.Debug("CreatePaymentItemAccountForUser " + serverAccount.AccountId.ToString());
            try
            {
                PaymentItemsProcess paymentItems = new PaymentItemsProcess();
                UserInfo userInfo = GetPaymentItemsUserInfo(serverAccount, userIpAddress);
                int initialCoinAmount = GetPaymentItemsUserInfoUserInitialCoinAmount(); 
                int initialCashAmount = GetPaymentItemsUserInfoUserInitialCashAmount();

                System.Action<string> asyncCallback = delegate(string paymentItemsResponse)
                {
                    try
                    {
                        XmlDocument response = new XmlDocument();
                        response.LoadXml(paymentItemsResponse);

                        if (response != null)
                        {
                            XmlNode userNode = response.SelectSingleNode("Response/user");
                            serverAccount.PaymentItemUserId = userNode.Attributes["id"].InnerText;
                            serverAccount.PaymentItemSecureKey = userNode.Attributes["secureKey"].InnerText;

                            UpdateServerPaymentItemsAccount(serverAccount, accountForUserCallback);
                            Metrics.Log(LogGlobals.CATEGORY_ACCOUNT, LogGlobals.EVENT_PAYMENT_ACCOUNT_CREATED, LogGlobals.PAYMENT_ACCOUNT_ID_LABEL, serverAccount.PaymentItemUserId, serverAccount.AccountId.ToString());

                        }
                    }

                    //if twoFish account creation blows an exception the we do not want to cause the game to fail
                    //so lets just continue
                    catch(System.Exception ex)
                    {
                        accountForUserCallback(serverAccount);
                    }
                };

   
                paymentItems.CreateNewUser(userInfo, initialCoinAmount, initialCashAmount, asyncCallback);
            }

            //if twoFish account creation blows an exception the we do not want to cause the game to fail
            //so lets just continue
            catch
            {
                accountForUserCallback(serverAccount);
            }
        }


        /// <summary>
        /// Calling the Accounts service to update the Hangout PaymentItems information 
        /// </summary>
        /// <param name="serverAccount">The account object </param>
        /// <param name="accountForUserCallback">The callback to call when finished</param>
        public void UpdateServerPaymentItemsAccount(ServerAccount serverAccount, Action<ServerAccount> accountForUserCallback)
        {
            Action<XmlDocument> updatePaymentItemAccountCallback = delegate(XmlDocument receivedXmlUpdatePaymentItemsAccount)
            {
                if (accountForUserCallback != null)
                {
                    accountForUserCallback(serverAccount);
                }
            };
            AccountsServiceAPI.UpdateServerPaymentItemsAccount(serverAccount.AccountId.ToString(), serverAccount.PaymentItemUserId, serverAccount.PaymentItemSecureKey, updatePaymentItemAccountCallback);
           
        }

        /// <summary>
        /// Get the PaymentItems User information
        /// </summary>
        /// <param name="serverAccount">The account object </param>
        /// <param name="userIpAddress">The ipAddress of the user</param>
        /// <returns></returns>
        public UserInfo GetPaymentItemsUserInfo(ServerAccount serverAccount, string userIpAddress)
        {
            UserInfo userInfo = new UserInfo();
            userInfo.Name = String.Format("PI{0}",  serverAccount.AccountId.ToString());
            userInfo.ExternalKey = serverAccount.Nickname;
            userInfo.IPAddress = userIpAddress;

            return userInfo;
        }

        /// <summary>
        /// Get the PaymentItems initial  coin amount for a new user
        /// </summary>
        /// <returns>returns the coin amount from the config file</returns>
        public int GetPaymentItemsUserInfoUserInitialCoinAmount()
        {
            string configCoinValue = StateServerConfig.PaymentItemsInitialCoin;

            int coinValue ;
            if (int.TryParse(configCoinValue, out coinValue) == false)
            {
                coinValue = 0;
            }

            return coinValue;
        }

        /// <summary>
        /// Get the PaymentItems initial cash amount for a new user
        /// </summary>
        /// <returns>returns the cash amount from the config file</returns>
        public int GetPaymentItemsUserInfoUserInitialCashAmount()
        {
            string configCashValue = StateServerConfig.PaymentItemsInitialCash;

            int cashValue;
            if (int.TryParse(configCashValue, out cashValue) == false)
            {
                cashValue = 0;
            }

            return cashValue;
        }
    }
}
