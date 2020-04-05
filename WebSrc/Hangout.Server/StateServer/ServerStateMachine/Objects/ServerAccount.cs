using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;
using System.Diagnostics;

namespace Hangout.Server
{
    public class ServerAccount : IAccount
    {
        private AccountId mAccountId;
        private long mFbAccountId;
        private string mNickname;
		private string mFirstName;
		private string mLastName;

        private ulong mFacebookFriendsReadyCallbackId;
        public ulong FacebookFriendsReadyCallbackId
        {
            get { return mFacebookFriendsReadyCallbackId; }
            set { mFacebookFriendsReadyCallbackId = value; }
        }
        private bool mFacebookFriendsRequested = false;
        public bool FacebookFriendsRequested
        {
            get { return mFacebookFriendsRequested; }
            set { mFacebookFriendsRequested = value; }
        }

		// All the facebook friends, indexed by FacebookID
		private readonly Dictionary<long, FacebookFriendInfo> mAllFacebookFriends = new Dictionary<long, FacebookFriendInfo>();

		private readonly List<FacebookFriendInfo> mHangoutFacebookFriends = new List<FacebookFriendInfo>();
        private string mPiAccountId;
        private string mPiSecureKey;
        private RoomId mLastRoomId;
        private string mIpAddress;
        private bool mFacebookFriendsPending = true;

        public bool FacebookFriendsPending
        {
            get { return mFacebookFriendsPending; }
        }

		private Stopwatch mLoggedInTimeLength = null;
		public TimeSpan LoggedInTimeLength
		{
			get { return mLoggedInTimeLength.Elapsed; }
		}

		private UserProperties mUserProperties = null;
		public UserProperties UserProperties
		{
			set { mUserProperties = value; }
			get { return mUserProperties; }
		}

		public ICollection<FacebookFriendInfo> AllFacebookFriends
		{
			get { return mAllFacebookFriends.Values; }
		}

		public IDictionary<long, FacebookFriendInfo> FacebookFriendsLookupTable
		{
			get { return mAllFacebookFriends; }
		}

		public int EntourageSize
		{
			get { return mHangoutFacebookFriends.Count; }
		}
		public List<FacebookFriendInfo> HangoutFacebookFriends
        {
			get { return mHangoutFacebookFriends; } 
        }

        public long FacebookAccountId
        {
            get { return mFbAccountId; }
        }

        public AccountId AccountId
        {
            get { return mAccountId; }
        }

        public string Nickname
        {
            get { return mNickname; }
        }

		public string FirstName
		{
			get { return mFirstName; }
		}

		public string LastName
		{
			get { return mLastName; }
		}

		public RoomId LastRoomId
        {
            set { mLastRoomId = value; }
            get { return mLastRoomId; }
        }

        public string IpAddress
        {
            set { mIpAddress = value; }
            get { return mIpAddress;  }
        }

        public ServerAccount(AccountId accountId, long fbAccountId, string piAccountId, string piSecureKeyString, string nickname, string firstName, string LastName, UserProperties userProperties)
        {
			mUserProperties = userProperties;
            mAccountId = accountId;
            mFbAccountId = fbAccountId;
            mPiAccountId = piAccountId;
            mPiSecureKey = piSecureKeyString;
            mNickname = nickname;
			mFirstName = firstName;
			mLastName = LastName;

			mLoggedInTimeLength = new Stopwatch();
			mLoggedInTimeLength.Start();
        }

		~ServerAccount()
		{
			mLoggedInTimeLength.Stop();
		}

        public override string ToString()
        {
            return "\n----------- ServerAccount ---------------" +
                "\nFacebookId:\t" + mFbAccountId.ToString() +
                "\nAccountId:\t" + mAccountId.ToString() +
                "\nNickname:\t" + mNickname +
				"\nFirstname:\t" + mFirstName +
				"\nLastname:\t" + mLastName +
                "\nUserProperties:\n" + mUserProperties.ToString() +
                "\n-----------------------------------------\n";
        }

        public void SaveCurrentAccountData(System.Action<XmlDocument> finishedUpdatingAccountDataCallback)
        {
            XmlDocument accountDataXml = AccountsXmlUtil.CreateAccountDataXml(this);
            AccountsServiceAPI.UpdateAccountData(mAccountId, accountDataXml, finishedUpdatingAccountDataCallback);
        }

        public void AddFacebookFriends(IEnumerable<FacebookFriendInfo> friendInfos)
        {
            mFacebookFriendsPending = false;
			foreach (FacebookFriendInfo friendInfo in friendInfos)
			{
				mAllFacebookFriends.Add(friendInfo.FbAccountId, friendInfo);
				if(friendInfo.AccountId != null)
				{
					mHangoutFacebookFriends.Add(friendInfo);
				}
			}
        }

        public string PaymentItemUserId
        {
            get
            {
                return mPiAccountId;
            }
            set
            {
                mPiAccountId = value;
            }
        }


        public string PaymentItemSecureKey
        {
            get
            {
                return mPiSecureKey;
            }
            set
            {
                mPiSecureKey = value;
            }
        }
   }
}
