using System;
using System.Text;

namespace Hangout.Shared
{
	public static class ConstStrings
	{
		// Warning, changing these values is extremely fragile.  These must match up exactly with the parameter names
		// in web services, or the web service call will fail, probably silently.
		public const string kAccountData = "AccountData";
		public const string kUserProperties = "UserProperties";
		public const string kUserProperty = "UserProperty";
		public const string kAccountId = "AccountId";
		public const string kFbAccountId = "FBAccountId";
		public const string kFbSessionKey = "fbSessionKey";
		public const string kPaymentItemsAccountId = "paymentItemsAccountId";
		public const string kPiAccountId = "PIAccountId";
		public const string kPiSecureKey = "PISecureKey";
		public const string kPiSecureId = "secureKey";
		public const string kNickName = "NickName";
		public const string kFirstName = "FirstName";
		public const string kLastName = "LastName";
		public const string kAvatars = "Avatars";
		public const string kAvatar = "Avatar";
		public const string kCampaignId = "campaign";
		public const string kReferrerId = "referringFbAccountId";

		public const string kName = "Name";
		public const string kType = "Type";
		public const string kValue = "Value";
	}
}
