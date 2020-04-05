using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;
using PureMVC.Patterns;
using Hangout.Shared;

namespace Hangout.Client
{
    /// <summary>
    /// Contains information about connection
    /// IPAddress - Address of stateserver
    /// Port - Port we connect to on stateserver
    /// BaseUrl - Base url of unity3d file on webserver, not including query params
    /// ApplicationUrl - Full url of unity3d, including query params
    /// FacebookAccountId - Facebook account id
    /// SessionKey - Facebook session key
    /// Nickname - Facebook nickname
    /// WebEntryPointId - ID of the entry point into the game from the web
    /// SelectedAvatar - Avatar selected on initial onramp to game
    /// AssetBaseUrl - Base url of the externally loaded assets
    /// WebServicesBaseUrl - IP of web services
    /// </summary>
    public class ConnectionProxy : Proxy
    {
        private Logger mLogger;
        private ClientConnectionManager mConnectionManager = null;

        // Default ip/port for stateserver.  Overridden by value passed in by javascript
        // DON'T HARDCODE THIS VALUE, USE state_server_location IN CONFIG.JSON
        private string mIpAddress;
        public string IpAddress
        {
            get { return mIpAddress; }
            set { mIpAddress = value; }
        }

        private int mPort = 8000;
        public int Port
        {
            get { return mPort; }
            set { mPort = value; }
        }

        private long mFacebookAccountId;
        public long FacebookAccountId
        {
            get { return mFacebookAccountId; }
            set { mFacebookAccountId = value; }
        }

        private string mSessionKey;
        public string SessionKey
        {
            get { return mSessionKey; }
            set { mSessionKey = value; }
        }

        private string mStageName;
        public string StageName
        {
            get { return mStageName; }
            set { mStageName = value; }
        }
 
        /// <summary>
        /// The id of the entry point we enter the client from.  This will determine if we load in
        /// to a minigame, public room, etc
        /// </summary>
        private string mWebEntryPointId;
        public string WebEntryPointId
        {
            get { return mWebEntryPointId; }
            set { mWebEntryPointId = value; }
        }

        private string mCampaignId;
        public string CampaignId
        {
            get { return mCampaignId; }
            set { mCampaignId = value; }
        }

		private string mReferrerId = string.Empty;
		public string ReferrerId 
		{
			get { return mReferrerId; }
		}

        private string mNickName;
        public string NickName
        {
            get { return mNickName; }
            set { mNickName = value; }
        }

		private string mFirstName = string.Empty;
		public string FirstName
		{
			get { return mFirstName; }
		}

		private string mLastName = string.Empty;
		public string LastName
		{
			get { return mLastName; }
		}

        private AvatarId mSelectedAvatar;
        public AvatarId SelectedAvatar
        {
            get { return mSelectedAvatar; }
        }

        private string mAssetBaseUrl;
        public string AssetBaseUrl
        {
            get { return mAssetBaseUrl; }
            set { mAssetBaseUrl = value; }
        }

        private string mWebServicesBaseUrl;
        public string WebServicesBaseUrl
        {
            get { return mWebServicesBaseUrl; }
            set { mWebServicesBaseUrl = value; }
        }


        // ---------------- BEGIN USEFUL METHODS ----------------

        public ConnectionProxy()
        {
            mLogger = new Logger();
            mLogger.AddReporter(new DebugLogReporter());

            mConnectionManager = new ClientConnectionManager();
        }

        // Retrieve all the config values we need to start up
        public void SetConfiguration(ConfigManagerClient configManager)
        {
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("SetConfiguration start", LogLevel.Info);
			mIpAddress = configManager.GetString("state_server_location", "127.0.0.1");
            mStageName = configManager.GetString("instance_name", "DEV");


            mWebEntryPointId = configManager.GetString("web_entry_point", FunnelGlobals.FASHION_MINIGAME);

            mAssetBaseUrl = configManager.GetString("asset_base_url", ProtocolUtility.GetAssetDataPath());
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("asssetBaseUrl " + mAssetBaseUrl, LogLevel.Info);

            mWebServicesBaseUrl = configManager.GetString("web_services_base_url", "http://services.hangoutdev.net");
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("web_services_base_url " + mWebServicesBaseUrl, LogLevel.Info);

            mCampaignId = configManager.GetString("campaign_id", "NO_VALUE_FROM_JS");
			mReferrerId = configManager.GetString("inviter_id", "NO_VALUE_FROM_JS");
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("campaign_id " + mCampaignId, LogLevel.Info);

			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("inviter_id " + mReferrerId, LogLevel.Info);
			mSelectedAvatar = new AvatarId((uint)configManager.GetInt("selected_avatar", 1));
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("mSelectedAvatar " + mSelectedAvatar, LogLevel.Info);

            mFacebookAccountId = configManager.GetLong("fb_account_id", 0);
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("mFacebookAccountId " + mFacebookAccountId, LogLevel.Info);
			mSessionKey = configManager.GetString("fb_session_key", "");
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("mSessionKey " + mSessionKey, LogLevel.Info);
			mNickName = configManager.GetString("fake_nickname", "");
            mFirstName = configManager.GetString("fake_first_name", "");
            mLastName = configManager.GetString("fake_last_name", "");
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("fake_nickname " + mNickName, LogLevel.Info);
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("fake_first_name " + mFirstName, LogLevel.Info);
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("fake_last_name " + mLastName, LogLevel.Info);
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("SetConfiguration finished", LogLevel.Info);
		}

        public void ReceiveMessage(Message message)
        {
            mConnectionManager.ReceiveMessage(message);
        }
    }
}
