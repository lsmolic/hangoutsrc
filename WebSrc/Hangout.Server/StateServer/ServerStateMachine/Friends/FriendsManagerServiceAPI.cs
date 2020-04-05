using System.Xml;
using Hangout.Server.StateServer;
using log4net;

namespace Hangout.Server
{
    public static class FriendsManagerServiceAPI
    {
        private static ILog mLogger = LogManager.GetLogger("FriendManagerServiceAPI");
        private static string kFbAccountId = "fbAccountId";
        private static string kSessionKey= "fbSessionKey";

        /// <summary>
        /// NOTE: sessionKey is NOT the same as a sessionId (Guid).. a sessionKey is gotten from the webpage from facebook
        /// </summary>
        public static void GetHangoutFacebookFriends(long facebookAccountId, string sessionKey, System.Action<XmlDocument> getFacebookFriendsServiceCallback)
        {
            mLogger.InfoFormat("GetHangoutFacebookFriends called fbAccountId={0}", facebookAccountId);
            WebServiceRequest getFacebookFriendsService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Facebook", "GetOnlineFriendsWhoAreAppUsers");
            getFacebookFriendsService.AddParam(kFbAccountId, facebookAccountId.ToString());
            getFacebookFriendsService.AddParam(kSessionKey, sessionKey);
            getFacebookFriendsService.GetWebResponseAsync(delegate(XmlDocument xmlResponse)
            {
                mLogger.Info("GetHangoutFacebookFriends responded");
                getFacebookFriendsServiceCallback(xmlResponse);
            });
        }

		public static void GetAllFacebookFriends(long facebookAccountId, string sessionKey, System.Action<XmlDocument> getAllFacebookFriendsServiceCallback)
		{
            mLogger.InfoFormat("GetAllFacebookFriends called fbAccountId={0}", facebookAccountId);
            WebServiceRequest getFacebookFriendsService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "Facebook", "GetAllFacebookFriends");
            getFacebookFriendsService.AddParam(kFbAccountId, facebookAccountId.ToString());
            getFacebookFriendsService.AddParam(kSessionKey, sessionKey);
            getFacebookFriendsService.GetWebResponseAsync(delegate(XmlDocument xmlResponse)
            {
                mLogger.Info("GetAllFacebookFriends responded");
                getAllFacebookFriendsServiceCallback(xmlResponse);
            });
		}
    }
}
