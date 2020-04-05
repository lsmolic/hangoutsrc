using System;

namespace Hangout.Shared
{
    [Serializable]
	public class FacebookFriendInfo
	{
		private AccountId mAccountId = null;
		private long mFbAccountId;
		public long FbAccountId
		{
			get { return mFbAccountId; }
		}
		private string mFirstName;
		private string mLastName;
		private string mImageUrl;

		public string ImageUrl
		{
			get { return mImageUrl; }
		}

		public AccountId AccountId
		{
			get { return mAccountId; }
		}

		public string FirstName
		{
			get { return mFirstName; }
		}

		public string LastName
		{
			get { return mLastName; }
		}

		public FacebookFriendInfo(AccountId accountId, long fbAccountId, string firstName, string lastName, string imageUrl)
		{
			mAccountId = accountId;
			mFbAccountId = fbAccountId;
			mFirstName = firstName;
			mLastName = lastName;
			mImageUrl = imageUrl;
		}
	}
}
