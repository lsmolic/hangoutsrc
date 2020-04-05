using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Server.StateServer;
using Hangout.Shared;
using log4net;

namespace Hangout.Server
{
    public static class AccountsServiceAPI
    {
        private static ILog mLogger = LogManager.GetLogger("AccountServiceAPI");

		/// <summary>
		/// the expected output of the returned xml is:
		/// 	<Accounts>
		///			<Account AccountId="1000002" FBAccountId="1520031799" PIAccountId="728" PISecureKey="B3EF252C9CFFC2BEF3A098CF128CB0511C4B923B4062960B41B01AC6440AA82D" NickName="Matt" FirstName="Matt" LastName"Schmeer">
		///			</Account>
		///		</Accounts>
		/// </summary>
        public static void CreateServerAccount(string fbAccountId, string fbSessionKey, string nickName, string firstName, string lastName, string campaignId, string referrerId, Action<XmlDocument> createServerAccountCallback)
        {
			mLogger.DebugFormat("CreateServerAccount called fbAccountId={0} sessionKey={1} nickname={2} firstname={3} lastname={4}", fbAccountId, fbSessionKey, nickName, firstName, lastName);
            WebServiceRequest createServerAccountService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Accounts", "CreateAccountFromFacebook");
            createServerAccountService.AddParam(ConstStrings.kFbAccountId, fbAccountId);
			createServerAccountService.AddParam(ConstStrings.kFbSessionKey, fbSessionKey);
			createServerAccountService.AddParam(ConstStrings.kNickName, nickName);
			createServerAccountService.AddParam(ConstStrings.kFirstName, firstName);
			createServerAccountService.AddParam(ConstStrings.kLastName, lastName);
			createServerAccountService.AddParam(ConstStrings.kCampaignId, campaignId);
			createServerAccountService.AddParam(ConstStrings.kReferrerId, referrerId);
            createServerAccountService.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
            {
                mLogger.DebugFormat("CreateServerAccount responded fbAccountId={0} sessionKey={1} nickname={2}", fbAccountId, fbSessionKey, nickName);
                createServerAccountCallback(xmlResponse);
            });
        }

        public static void GetServerAccountForFacebookId(string fbAccountId, Action<XmlDocument> getServerAccountForFacebookIdCallback)
        {
            mLogger.DebugFormat("GetServerAccountForFacebookId called fbAccountId={0}", fbAccountId);
            WebServiceRequest getServerAccountForFacebookIdService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Accounts", "GetAccounts");
			getServerAccountForFacebookIdService.AddParam(ConstStrings.kFbAccountId, fbAccountId);
            getServerAccountForFacebookIdService.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
            {
                mLogger.DebugFormat("GetServerAccountForFacebookId responded fbAccountId={0}", fbAccountId);
                getServerAccountForFacebookIdCallback(xmlResponse);
            });
        }

        public static void GetServerAccountForAccountId(string accountId, Action<XmlDocument> getServerAccountForAccountIdCallback)
        {
            mLogger.DebugFormat("GetServerAccountForAccountId called accountId={0}", accountId);
            WebServiceRequest getServerAccountForAccountIdService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Accounts", "GetAccounts");
			getServerAccountForAccountIdService.AddParam(ConstStrings.kAccountId, accountId);
            getServerAccountForAccountIdService.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
            {
                mLogger.DebugFormat("GetServerAccountForAccountId responded accountId={0}", accountId);
                getServerAccountForAccountIdCallback(xmlResponse);
            });
        }

        public static void UpdateServerPaymentItemsAccount(string accountId, string paymentItemsAccountId, string secureKey, Action<XmlDocument> updatePaymentItemAccountCallback)
        {
            mLogger.InfoFormat("UpdateServerPaymentItemsAccount called accountId={0} PIAccountId={1}", accountId, paymentItemsAccountId);
            WebServiceRequest updatePaymentItemsAccountService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Accounts", "UpdateAccountWithPaymentItemsInfo");
			updatePaymentItemsAccountService.AddParam(ConstStrings.kAccountId, accountId);
			updatePaymentItemsAccountService.AddParam(ConstStrings.kPaymentItemsAccountId, paymentItemsAccountId);
			updatePaymentItemsAccountService.AddParam(ConstStrings.kPiSecureId, secureKey);
            updatePaymentItemsAccountService.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
            {
                mLogger.InfoFormat("UpdateServerPaymentItemsAccount responded accountId={0} PIAccountId={1}", accountId, paymentItemsAccountId);
                updatePaymentItemAccountCallback(xmlResponse);
            });
        }

		public static void UpdateAccountData(AccountId accountId, XmlDocument accountDataXml, Action<XmlDocument> updatedAccountDataCallback)
		{
            mLogger.DebugFormat("UpdateAccountData called accountId={0}", accountId);
            WebServiceRequest updateAccountDataService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Accounts", "UpdateAccountData");
			updateAccountDataService.AddParam(ConstStrings.kAccountId, accountId.ToString());
			updateAccountDataService.AddParam(ConstStrings.kAccountData, accountDataXml.OuterXml);
			updateAccountDataService.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
            {
                mLogger.DebugFormat("UpdateAccountData responded accountId={0}", accountId);
                updatedAccountDataCallback(xmlResponse);
            });
		}
    }
}
