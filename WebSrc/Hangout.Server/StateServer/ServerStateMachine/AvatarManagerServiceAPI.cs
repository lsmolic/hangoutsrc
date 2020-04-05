using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Server.StateServer;
using Hangout.Shared;
using log4net;

namespace Hangout.Server
{
    public static class AvatarManagerServiceAPI
    {
        private static ILog mLogger = LogManager.GetLogger("AvatarManagerServiceAPI");
        private static string kAccountId = "accountId";
        private static string kAvatarId = "avatarId";
        private static string kAvatarDna = "avatarDNA";
		private static string kAccountIdCsvList = "accountIdCsvList";
        private static string kDefaultAvatarId = "defaultAvatarId";

        /// <summary>
        /// Gets all system avatars
        /// </summary>
        /// <param name="getSystemAvatarsCallback"></param>
        public static void GetSystemAvatars(Action<XmlDocument> getSystemAvatarsCallback)
        {
            mLogger.Debug("GetSystemAvatars called");
            // For now we just return all the system avatars to choose from
            WebServiceRequest getAvatarService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Avatars", "GetSystemAvatars");
            getAvatarService.Method = FormMethod.POST;
            getAvatarService.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
            {
                mLogger.Debug("GetAvatarForUser responded");
                getSystemAvatarsCallback(xmlResponse);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="avatarId">Id of system avatar we are requesting (ie. "1","2","3","4","5") </param>
        /// <param name="getSystemAvatarsCallback"></param>
        public static void GetSystemAvatar(string avatarId, Action<XmlDocument> getSystemAvatarsCallback)
        {
            mLogger.DebugFormat("GetSystemAvatar called, avatarId = {0}", avatarId);
            // For now we just return all the system avatars to choose from
            WebServiceRequest getAvatarService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Avatars", "GetSystemAvatars");
            getAvatarService.Method = FormMethod.POST;
            getAvatarService.AddParam(kAvatarId, avatarId);
            getAvatarService.GetWebResponseAsync(delegate(XmlDocument xmlResponse)
            {
                mLogger.DebugFormat("GetSystemAvatar responded, avatarId = {0}", avatarId);
                getSystemAvatarsCallback(xmlResponse);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverAccount"></param>
        /// <param name="getAvatarForUserServiceCallback"></param>
        public static void GetAvatarForUser(ServerAccount serverAccount, Action<XmlDocument> getAvatarForUserServiceCallback)
        {
            mLogger.DebugFormat("GetAvatarForUser called accountId={0}", serverAccount.AccountId);
            WebServiceRequest getAvatarList = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Avatars", "GetAvatars");
            getAvatarList.AddParam(kAccountId, serverAccount.AccountId.ToString());
            getAvatarList.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
            {
                mLogger.DebugFormat("GetAvatarForUser responded accountId={0}, xmlReponse={1}", serverAccount.AccountId, xmlResponse.OuterXml);
                getAvatarForUserServiceCallback(xmlResponse);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverAccount"></param>
        /// <param name="defaultAvatarId"></param>
        /// <param name="createAvatarForUserServiceCallback"></param>
        public static void CreateAvatarForUser(ServerAccount serverAccount, AvatarId defaultAvatarId, Action<XmlDocument> createAvatarForUserServiceCallback)
        {
            mLogger.DebugFormat("CreateAvatarForUser called accountId={0} defaultAvatarId={1}", serverAccount.AccountId, defaultAvatarId);
            WebServiceRequest getAvatarList = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Avatars", "CreateAvatar");
            getAvatarList.AddParam(kAccountId, serverAccount.AccountId.ToString());
            getAvatarList.AddParam(kDefaultAvatarId, defaultAvatarId.ToString());
            getAvatarList.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
            {
                mLogger.DebugFormat("CreateAvatarForUser responded accountId={0} defaultAvatarId={1}", serverAccount.AccountId, defaultAvatarId);
                createAvatarForUserServiceCallback(xmlResponse);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountIds"></param>
        /// <param name="getAvatarForUserServiceCallback"></param>
		public static void GetAvatarForUsers(IEnumerable<AccountId> accountIds, Action<XmlDocument> getAvatarForUserServiceCallback)
		{
            string accountIdsString = AccountsXmlUtil.GetCommaSeperatedListOfAccountIdsFromList(accountIds);
            mLogger.DebugFormat("GetAvatarForUsers called accountIds={0}", accountIdsString);

			WebServiceRequest getAvatarList = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Avatars", "GetAvatars");
			getAvatarList.AddParam(kAccountIdCsvList, accountIdsString);
			getAvatarList.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
            {
                mLogger.DebugFormat("GetAvatarForUsers responded accountIds={0} xmlReponse={1}", accountIdsString, xmlResponse.OuterXml);
                getAvatarForUserServiceCallback(xmlResponse);
            });
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="avatarId"></param>
        /// <param name="avatarDna"></param>
        /// <param name="updateAvatarDnaServiceFinishedCallback"></param>
        public static void UpdateAvatarDna(AvatarId avatarId, XmlDocument avatarDna, System.Action<XmlDocument> updateAvatarDnaServiceFinishedCallback)
        {
            mLogger.DebugFormat("UpdateAvatarDna called avatarId={0}", avatarId);
            WebServiceRequest updateAvatarDna = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Avatars", "UpdateAvatarDNA");
            updateAvatarDna.AddParam(kAvatarId, avatarId.ToString());
            updateAvatarDna.AddParam(kAvatarDna, avatarDna.OuterXml);
            updateAvatarDna.GetWebResponseAsync(delegate(XmlDocument xmlResponse) 
            {
                mLogger.DebugFormat("UpdateAvatarDna responded avatarId={0}", avatarId);
                updateAvatarDnaServiceFinishedCallback(xmlResponse);
            });
        }
    }
}
