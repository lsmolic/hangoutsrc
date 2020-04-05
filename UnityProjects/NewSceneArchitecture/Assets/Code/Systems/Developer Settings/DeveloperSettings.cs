using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Client
{

    public class DeveloperSettings
    {

        private const string SETTINGS_PATH = "resources://DeveloperSettings.tweaks";

        [Tweakable("Mock Facebook AccountId")]
        private long mMockFacebookAccountId = 0;

        [Tweakable("Mock Facebook SessionKey")]
        private string mMockFacebookSessionKey = "";

        [Tweakable("Mock Facebook NickName")]
        private string mMockFacebookNickName = "";

        [Tweakable("Mock Default AvatarId")]
        private AvatarId mMockDefaultAvatarId = null;

		[Tweakable("Mock FirstName")]
		private string mFirstName = string.Empty;

		[Tweakable("Mock LastName")]
		private string mLastName = string.Empty;

		[Tweakable("CampaignId")]
		private string mCampaignId = string.Empty;

		[Tweakable("ReferrerId")]
		private string mReferrerId = string.Empty;

        private static DeveloperSettings mInstance = null;

        public string MockFacebookNickName
        {
            get { return mMockFacebookNickName; }
        }
        
        public string MockFacebookSessionKey
        {
            get { return mMockFacebookSessionKey; }
        }

        public long MockFacebookAccountId
        {
            get { return mMockFacebookAccountId; }
        }

        public AvatarId MockDefaultAvatarId
        {
            get { return mMockDefaultAvatarId; }
        }

		public string MockFirstName
		{
			get { return mFirstName; }
		}

		public string MockLastName
		{
			get { return mLastName; }
		}

		public string CampaignId
		{
			get { return mCampaignId; }
		}

		public string ReferrerId
		{
			get { return mReferrerId; }
		}

        public DeveloperSettings()
        {
            new TweakablesHandler(SETTINGS_PATH, this);
        }

        public static DeveloperSettings Instance()
        {
            if(mInstance == null)
            {
                mInstance = new DeveloperSettings();
            }
            return mInstance;
        }

    }
}
