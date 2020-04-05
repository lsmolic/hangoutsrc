using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook;
using Microsoft.Xml.Schema.Linq;
using System.Xml;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Security;
using Facebook.Rest;
using Facebook.Session;
using Facebook.Utility;

namespace Hangout.Server.WebServices
{
	public class FacebookService : ServiceBaseClass
	{
		private ServicesLog mServiceLog = new ServicesLog("FacebookService");

		public SimpleResponse GetAllFacebookFriends(long fbAccountId, string fbSessionKey)
		{
			mServiceLog.Log.InfoFormat("GetAllFacebookFriends: fbAccountId={0}, fbSessionKey={1}", fbAccountId, fbSessionKey);
			StringBuilder xmlBuilder = new StringBuilder();

			FacebookSession facebookSession = new HangoutFacebookSession();
			facebookSession.ApplicationKey = WebConfig.FacebookAPIKey;
			facebookSession.ApplicationSecret = WebConfig.FacebookSecret;
			facebookSession.SessionKey = fbSessionKey;

			Api facebookApi = new Api(facebookSession);

			//IList<facebook.Schema.user> fbFriends = facebookApi.friends.getUserObjects(fbAccountId);//get(Convert.ToInt64(fbAccountId));
			mServiceLog.Log.InfoFormat("GetAllFacebookFriends: got fb response");

			Dictionary<long, string> accountMapping = new Dictionary<long, string>();

			List<long> fbAppUserFriendIds;
			try
			{
				fbAppUserFriendIds = (List<long>)facebookApi.Friends.GetAppUsers();
			}
			catch (FacebookException fbEx)
			{
				mServiceLog.Log.Error("Facebook Exception: errorcode=" + fbEx.ErrorCode + ", errorType=" + fbEx.ErrorType + ", message=" + fbEx.Message);
				HangoutException hangoutException = new HangoutException("FACEBOOK EXCEPTION: " + fbEx.Message + ", Facebook ErrorCode: " + fbEx.ErrorCode + ", FbErrorType: " + fbEx.ErrorType);
				
				
				hangoutException.Source = fbEx.Source;
				throw hangoutException;
			}
			catch (System.Exception ex)
			{
				throw new Exception("Facebook did not return any useful information or invalid xml " + ex.Message, ex.InnerException);
			}
			accountMapping = ListHangoutAccountsForFbAccounts(fbAppUserFriendIds);

			XmlDocument friendsXml = GetAllFriendsXml(fbAccountId, facebookApi);

			xmlBuilder.Append(ConstructFriendData(friendsXml, accountMapping));

			return new SimpleResponse("Friends", xmlBuilder.ToString());
		}

		public SimpleResponse BanUsers(string fbAccountId, string accountId, string fbAccountIdCsvList, string accountIdCsvList)
		{
			mServiceLog.Log.InfoFormat("BanUser: fbAccountId={0}, fbSessionKey={1}", fbAccountId, fbAccountIdCsvList);

			List<long> uids = new List<long>();

			if (!String.IsNullOrEmpty(fbAccountId))
			{
				uids.Add(Convert.ToInt64(fbAccountId));
			}
			if (fbAccountIdCsvList != null)
			{
				if (fbAccountIdCsvList != "")
				{
					string[] fbAccountIds = fbAccountIdCsvList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

					foreach (string s in fbAccountIds)
					{
						long sUint = Convert.ToInt64(s);
						uids.Add(sUint);
					}

				}
			}
			if (!String.IsNullOrEmpty(accountId))
			{
				if (!String.IsNullOrEmpty(accountIdCsvList))
				{
					accountIdCsvList += "," + accountId;
				}
				else
				{
					accountIdCsvList += accountId;
				}

			}
			if (accountIdCsvList != null)
			{
				if (accountIdCsvList != "")
				{
					AccountService accounts = new AccountService();
					XmlDocument accountXmlNodeList = accounts.GetAccounts(null, null, null, accountIdCsvList, null, null);
					foreach (XmlNode accountXmlNode in accountXmlNodeList)
					{
						if (accountXmlNode != null)
						{
							if (accountXmlNode.Attributes["FbAccountId"] != null)
							{
								long uInt = Convert.ToInt64(accountXmlNode.Attributes["FbAccountId"].Value);
								uids.Add(uInt);
							}

						}
					}

				}
			}

			FacebookSession facebookSession = new HangoutFacebookSession();
			facebookSession.ApplicationKey = WebConfig.FacebookAPIKey;
			facebookSession.ApplicationSecret = WebConfig.FacebookSecret;
			Api facebookApi = new Api(facebookSession);

			if (uids.Count == 0)
			{
				throw new Exception("You must include at least one fbAccountId to ban;");
			}
			bool response = facebookApi.Admin.BanUsers(uids);


			return new SimpleResponse("Success", response.ToString());
		}

		public SimpleResponse UnBanUsers(string fbAccountId, string accountId, string fbAccountIdCsvList, string accountIdCsvList)
		{
			mServiceLog.Log.InfoFormat("BanUser: fbAccountId={0}, fbSessionKey={1}", fbAccountId, fbAccountIdCsvList);

			List<long> uids = new List<long>();

			if (!String.IsNullOrEmpty(fbAccountId))
			{
				uids.Add(Convert.ToInt64(fbAccountId));
			}
			if (fbAccountIdCsvList != null)
			{
				if (fbAccountIdCsvList != "")
				{
					string[] fbAccountIds = fbAccountIdCsvList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

					foreach (string s in fbAccountIds)
					{
						long sUint = Convert.ToInt64(s);
						uids.Add(sUint);
					}

				}
			}
			if (!String.IsNullOrEmpty(accountId))
			{
				if (!String.IsNullOrEmpty(accountIdCsvList))
				{
					accountIdCsvList += "," + accountId;
				}
				else
				{
					accountIdCsvList += accountId;
				}

			}
			if (accountIdCsvList != null)
			{
				if (accountIdCsvList != "")
				{
					AccountService accounts = new AccountService();
					XmlDocument accountXmlNodeList = accounts.GetAccounts(null, null, null, accountIdCsvList, null, null);
					foreach (XmlNode accountXmlNode in accountXmlNodeList)
					{
						if (accountXmlNode != null)
						{
							if (accountXmlNode.Attributes["FbAccountId"] != null)
							{
								long uInt = Convert.ToInt64(accountXmlNode.Attributes["FbAccountId"].Value);
								uids.Add(uInt);
							}

						}
					}

				}
			}

			FacebookSession facebookSession = new HangoutFacebookSession();
			facebookSession.ApplicationKey = WebConfig.FacebookAPIKey;
			facebookSession.ApplicationSecret = WebConfig.FacebookSecret;
			Api facebookApi = new Api(facebookSession);

			if (uids.Count == 0)
			{
				throw new Exception("You must include at least one fbAccountId to ban;");
			}
			try
			{
				facebookApi.Admin.UnbanUsers(uids);
			}
			catch (System.Exception ex)
			{

			}



			return new SimpleResponse("Success", "true");
		}

		public SimpleResponse GetBannedUsers()
		{
			FacebookSession facebookSession = new HangoutFacebookSession();
			facebookSession.ApplicationKey = WebConfig.FacebookAPIKey;
			facebookSession.ApplicationSecret = WebConfig.FacebookSecret;
			Api facebookApi = new Api(facebookSession);

			IList<long> bannedUsers = facebookApi.Admin.GetBannedUsers();
			string bannedUserString = "";
			string delimiter = "";
			foreach (long bUser in bannedUsers)
			{
				bannedUserString += delimiter + bUser;
				delimiter = ",";
			}

			AccountService accounts = new AccountService();
			string response = "";
			XmlDocument accountsXmlDoc = accounts.GetAccounts(null, null, null, null, bannedUserString, "1");
			if (accountsXmlDoc != null)
			{
				response = accountsXmlDoc.OuterXml;
			}

			return new SimpleResponse("BannedUsers", response);
		}


		/// <summary>
		/// Returns a list of friends who are currently online (using facebook) and 
		/// have added the Application on facebook (have an account with us)
		/// </summary>
		/// <param name="fbAccountId"></param>
		/// <param name="fbSessionKey"></param>
		/// <returns></returns>
		public SimpleResponse GetOnlineFriendsWhoAreAppUsers(long fbAccountId, string fbSessionKey, string getMockData)
		{
			mServiceLog.Log.InfoFormat("GetOnlineFriendsWhoAreAppUsers: fbAccountId={0}, fbSessionKey={1}, getMockData={2}", fbAccountId, fbSessionKey, getMockData);
			bool isMockRequest = false;
			if (getMockData == "1" || getMockData == "true")
			{
				isMockRequest = true;
			}

			StringBuilder xmlBuilder = new StringBuilder();

			//instantiate connection to facebookApi
			FacebookSession facebookSession = new HangoutFacebookSession();
			facebookSession.ApplicationKey = WebConfig.FacebookAPIKey;
			facebookSession.ApplicationSecret = WebConfig.FacebookSecret;
			facebookSession.SessionKey = fbSessionKey;
			Api facebookApi = new Api(facebookSession);


			Dictionary<long, string> accountMapping = new Dictionary<long, string>();
			List<long> fbAppUserFriendIds;
			try
			{
				fbAppUserFriendIds = (List<long>)facebookApi.Friends.GetAppUsers();
			}
			catch (FacebookException fbEx)
			{
				mServiceLog.Log.Error("Facebook Exception: errorcode=" + fbEx.ErrorCode + ", errorType=" + fbEx.ErrorType + ", message=" + fbEx.Message);
				HangoutException hangoutException = new HangoutException("FACEBOOK EXCEPTION: " + fbEx.Message + ", Facebook ErrorCode: " + fbEx.ErrorCode + ", FbErrorType: " + fbEx.ErrorType);
				
				
				hangoutException.Source = fbEx.Source;
				throw hangoutException;
			}
			catch (System.Exception ex)
			{
				throw new Exception("Facebook did not return any useful information or invalid xml " + ex.Message, ex.InnerException);
			}

			accountMapping = ListHangoutAccountsForFbAccounts(fbAppUserFriendIds);

			//For each friend who is an app user, 
			XmlDocument friendsXml = GetOnlineFriendsWhoAreAppUsersXml(fbAccountId, facebookApi);

			string friendString = "";
			if (getMockData != null && isMockRequest)
			{
				friendString = GetMockFriendDataForFacebook();
			}
			else
			{
				friendString = ConstructFriendData(friendsXml, accountMapping);
			}

			return new SimpleResponse("Friends", friendString);

		}

		/// <summary>
		/// Returns a list of All friends who have added the facebook app
		/// regardless of whether they are online
		/// </summary>
		/// <param name="fbAccountId"></param>
		/// <param name="fbSessionKey"></param>
		/// <returns></returns>
		public SimpleResponse GetAllFriendsWhoAreAppUsers(long fbAccountId, string fbSessionKey, string getMockData)
		{
			mServiceLog.Log.InfoFormat("GetAllFriendsWhoAreAppUsers: fbAccountId={0}, fbSessionKey={1}, getMockData={2}", fbAccountId, fbSessionKey, getMockData);
			bool isMockRequest = false;
			if (getMockData == "1" || getMockData == "true")
			{
				isMockRequest = true;
			}
			StringBuilder xmlBuilder = new StringBuilder();

			//instantiate connection to facebookApi
			FacebookSession facebookSession = new HangoutFacebookSession();
			facebookSession.ApplicationKey = WebConfig.FacebookAPIKey;
			facebookSession.ApplicationSecret = WebConfig.FacebookSecret;
			facebookSession.SessionKey = fbSessionKey;
			Api facebookApi = new Api(facebookSession);


			Dictionary<long, string> accountMapping = new Dictionary<long, string>();
			List<long> fbAppUserFriendIds;
			try
			{
				fbAppUserFriendIds = (List<long>)facebookApi.Friends.GetAppUsers();
			}
			catch (FacebookException fbEx)
			{
				mServiceLog.Log.Error("Facebook Exception: errorcode=" + fbEx.ErrorCode + ", errorType=" + fbEx.ErrorType + ", message=" + fbEx.Message);
				HangoutException hangoutException = new HangoutException("FACEBOOK EXCEPTION: " + fbEx.Message + ", Facebook ErrorCode: " + fbEx.ErrorCode + ", FbErrorType: " + fbEx.ErrorType);
				
				
				hangoutException.Source = fbEx.Source;
				throw hangoutException;
			}
			catch (System.Exception ex)
			{
				throw new Exception("Facebook did not return any useful information or invalid xml " + ex.Message, ex.InnerException);
			}

			accountMapping = ListHangoutAccountsForFbAccounts(fbAppUserFriendIds);

			//For each friend who is an app user, 
			XmlDocument friendsXml = GetAllFriendsWhoAreAppUsersXml(fbAccountId, facebookApi);

			string friendString = "";
			if (getMockData != null && isMockRequest)
			{
				friendString = GetMockFriendDataForFacebook();
			}
			else
			{
				friendString = ConstructFriendData(friendsXml, accountMapping);
			}


			return new SimpleResponse("Friends", friendString);

		}

		/// <summary>
		/// finds hangoutAccountIds for FacebookAccountIds and creates a dictionary mapping them
		/// - this is intended to tie the information from facebook with our system
		/// </summary>
		/// <param name="fbAppUserFriendIds"></param>
		/// <returns></returns>
		private Dictionary<long, string> ListHangoutAccountsForFbAccounts(IList<long> fbAppUserFriendIds)
		{
			//Get list of friends from calling method
			//1. convert that list to a dictionary that will hold a HangoutAccountId as it's value
			//2. convert that list to a csv string for use in the query that will populate that dictionary

			Dictionary<long, string> accountMapping = new Dictionary<long, string>();
			string delimiter = "";
			string whereIn = "";
			foreach (long l in fbAppUserFriendIds)
			{
				accountMapping.Add(l, "");
				whereIn += delimiter + l.ToString();
				delimiter = ", ";
			}

			if (whereIn != String.Empty)
			{
				//Call the hangout db and populate the dictionary that ties fbAccounts to hoAccounts
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AccountsDBConnectionString))
				{
					mysqlConnection.Open();

					string getAccountInfoForFacebookFriendsQuery = "SELECT * FROM LocalAccountInfo WHERE FacebookAccountId IN ( " + whereIn + " );";

					using (MySqlCommand getAccountInfoCommand = mysqlConnection.CreateCommand())
					{
						getAccountInfoCommand.CommandText = getAccountInfoForFacebookFriendsQuery;
						using (MySqlDataReader getAccountInfoReader = getAccountInfoCommand.ExecuteReader())
						{
							while (getAccountInfoReader.Read())
							{
								if (getAccountInfoReader["HangoutAccountId"] != null && getAccountInfoReader["FacebookAccountId"] != null)
								{
									string hoAcct = getAccountInfoReader["HangoutAccountId"].ToString();
									long fbAcct = Convert.ToInt64(getAccountInfoReader["FacebookAccountId"]);
									accountMapping[fbAcct] = hoAcct;
								}
							}
						}
					}
				}
			}
			return accountMapping;
		}

		private XmlDocument GetAllFriendsXml(long fbAccountId, Api facebookApi)
		{
			string response = "";
			try
			{
				response = facebookApi.Fql.Query(
						"SELECT " +
							"birthday, birthday_date, email_hashes, first_name, has_added_app," +
							"is_app_user, last_name, name, pic, pic_big," +
							"pic_small, pic_square, sex, status, timezone, uid, online_presence " +
						"FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1='" + fbAccountId.ToString() + "');");

			}
			catch (FacebookException fbEx)
			{
				mServiceLog.Log.Error("Facebook Exception: errorcode=" + fbEx.ErrorCode + ", errorType=" + fbEx.ErrorType + ", message=" + fbEx.Message);
				HangoutException hangoutException = new HangoutException("FACEBOOK EXCEPTION: " + fbEx.Message + ", Facebook ErrorCode: " + fbEx.ErrorCode + ", FbErrorType: " + fbEx.ErrorType);
				
				
				hangoutException.Source = fbEx.Source;
				throw hangoutException;
			}
			catch (System.Exception ex)
			{
				throw new Exception("Facebook did not return any useful information or invalid xml " + ex.Message, ex.InnerException);
			}
			
			XmlDocument friendsXml = new XmlDocument();
			try
			{
				friendsXml.LoadXml(response);
			}
			catch (XmlException xmlEx)
			{
				throw xmlEx;
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
			return friendsXml;
		}

		/// <summary>
		/// Get all friend data for ONLINE users who regardless of whether they have the app
		/// </summary>
		/// <param name="fbAccountId"></param>
		/// <param name="facebookApi"></param>
		/// <returns></returns>
		private XmlDocument GetAllOnlineFriendsXml(long fbAccountId, Api facebookApi)
		{
			string response = "";
			try
			{
				response = facebookApi.Fql.Query(
					"SELECT " +
						"birthday, birthday_date, email_hashes, first_name, has_added_app," +
								"is_app_user, last_name, name, pic, pic_big," +
								"pic_small, pic_square, sex, status, timezone, uid, online_presence " +
					"FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1='" + fbAccountId.ToString() + "') AND ('active' IN online_presence OR 'idle' IN online_presence);");
			}
			catch (FacebookException fbEx)
			{
				mServiceLog.Log.Error("Facebook Exception: errorcode=" + fbEx.ErrorCode + ", errorType=" + fbEx.ErrorType + ", message=" + fbEx.Message);
				HangoutException hangoutException = new HangoutException("FACEBOOK EXCEPTION: " + fbEx.Message + ", Facebook ErrorCode: " + fbEx.ErrorCode + ", FbErrorType: " + fbEx.ErrorType);
				
				
				hangoutException.Source = fbEx.Source;
				throw hangoutException;
			}
			catch (System.Exception ex)
			{
				throw new Exception("Facebook did not return any useful information or invalid xml " + ex.Message, ex.InnerException);
			}
			
			XmlDocument friendsXml = new XmlDocument();
			try
			{
				friendsXml.LoadXml(response);
			}
			catch (XmlException xmlEx)
			{
				throw xmlEx;
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
			return friendsXml;
		}

		/// <summary>
		/// Get all friend data for ONLINE users who ARE app users
		/// </summary>
		/// <param name="fbAccountId"></param>
		/// <param name="facebookApi"></param>
		/// <returns></returns>
		private XmlDocument GetOnlineFriendsWhoAreAppUsersXml(long fbAccountId, Api facebookApi)
		{
			string response = "";
			try
			{
				response = facebookApi.Fql.Query(
					 "SELECT " +
						 "birthday, birthday_date, email_hashes, first_name, has_added_app," +
								 "is_app_user, last_name, name, pic, pic_big," +
								 "pic_small, pic_square, sex, status, timezone, uid, online_presence " +
					 "FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1='" + fbAccountId.ToString() + "') AND ('active' IN online_presence OR 'idle' IN online_presence) AND is_app_user='1';");
			}
			catch (FacebookException fbEx)
			{
				mServiceLog.Log.Error("Facebook Exception: errorcode=" + fbEx.ErrorCode + ", errorType=" + fbEx.ErrorType + ", message=" + fbEx.Message);
				HangoutException hangoutException = new HangoutException("FACEBOOK EXCEPTION: " + fbEx.Message + ", Facebook ErrorCode: " + fbEx.ErrorCode + ", FbErrorType: " + fbEx.ErrorType);
				
				
				hangoutException.Source = fbEx.Source;
				throw hangoutException;
			}
			catch (System.Exception ex)
			{
				throw new Exception("Facebook did not return any useful information or invalid xml " + ex.Message, ex.InnerException);
			}

			XmlDocument friendsXml = new XmlDocument();
			try
			{
				friendsXml.LoadXml(response);
			}
			catch (XmlException xmlEx)
			{
				throw xmlEx;
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
			return friendsXml;
		}

		/// <summary>
		/// Get all friend data for users who ARE app users regardless of presence
		/// </summary>
		/// <param name="fbAccountId"></param>
		/// <param name="facebookApi"></param>
		/// <returns></returns>
		private XmlDocument GetAllFriendsWhoAreAppUsersXml(long fbAccountId, Api facebookApi)
		{
			string response = "";
			try
			{
				response = facebookApi.Fql.Query(
					 "SELECT " +
						 "birthday, birthday_date, email_hashes, first_name, has_added_app," +
								 "is_app_user, last_name, name, pic, pic_big," +
								 "pic_small, pic_square, sex, status, timezone, uid, online_presence " +
					 "FROM user WHERE uid IN (SELECT uid2 FROM friend WHERE uid1='" + fbAccountId.ToString() + "') AND is_app_user='1';");
			}
			catch (FacebookException fbEx)
			{
				mServiceLog.Log.Error("Facebook Exception: errorcode=" + fbEx.ErrorCode + ", errorType=" + fbEx.ErrorType + ", message=" + fbEx.Message);
				HangoutException hangoutException = new HangoutException("FACEBOOK EXCEPTION: " + fbEx.Message + ", Facebook ErrorCode: " + fbEx.ErrorCode + ", FbErrorType: " + fbEx.ErrorType);
				
				
				hangoutException.Source = fbEx.Source;
				throw hangoutException;
			}
			catch (System.Exception ex)
			{
				throw new Exception("Facebook did not return any useful information or invalid xml " + ex.Message, ex.InnerException);
			}

			XmlDocument friendsXml = new XmlDocument();
			try
			{
				friendsXml.LoadXml(response);
			}
			catch (XmlException xmlEx)
			{
				throw xmlEx;
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
			return friendsXml;
		}

		/// <summary>
		/// Uses Xml from the API.Query() function
		/// </summary>
		/// <param name="friendsXml"></param>
		/// <param name="accountMapping"></param>
		/// <returns></returns>
		private string ConstructFriendData(XmlDocument friendsXml, Dictionary<long, string> accountMapping)
		{
			StringBuilder xmlBuilder = new StringBuilder();
			if (friendsXml != null)
			{
				XmlNamespaceManager nsMgr = new XmlNamespaceManager(friendsXml.NameTable);
				nsMgr.AddNamespace("fb", "http://api.facebook.com/1.0/");

				XmlNodeList friendNodes = friendsXml.SelectNodes("fb:fql_query_response/fb:user", nsMgr);
				foreach (XmlNode friendNode in friendNodes)
				{
					string accountId = "";
					XmlNode uidNode = friendNode.SelectSingleNode(".//fb:uid", nsMgr);
					if (uidNode != null)
					{
						long fbAcctId = Convert.ToInt64(uidNode.InnerText);
						if (fbAcctId > 0 && accountMapping.ContainsKey(fbAcctId))
						{
							accountId = accountMapping[fbAcctId].ToString();
						}
						else
						{
							accountId = null;
						}
					}
					string fbAccountId = "";
					if (uidNode != null)
					{
						fbAccountId = uidNode.InnerText;
					}

					string firstName = "";
					XmlNode firstNameNode = friendNode.SelectSingleNode(".//fb:first_name", nsMgr);
					if (firstNameNode != null)
					{
						firstName = SecurityElement.Escape(firstNameNode.InnerText);
					}
					string lastName = "";
					XmlNode lastNameNode = friendNode.SelectSingleNode(".//fb:last_name", nsMgr);
					if (lastNameNode != null)
					{
						lastName = SecurityElement.Escape(lastNameNode.InnerText);
					}
					string squarePic = "";
					XmlNode squarePicNode = friendNode.SelectSingleNode(".//fb:pic_square", nsMgr);
					if (squarePicNode != null)
					{
						squarePic = squarePicNode.InnerText;
					}
					string isAppUser = "";
					XmlNode isAppUserNode = friendNode.SelectSingleNode(".//fb:is_app_user", nsMgr);
					if (isAppUserNode != null)
					{
						isAppUser = isAppUserNode.InnerText;
					}
					string onlinePresence = "";
					XmlNode onlinePresenceNode = friendNode.SelectSingleNode(".//fb:online_presence", nsMgr);
					if (onlinePresenceNode != null)
					{
						onlinePresence = onlinePresenceNode.InnerText;
					}

					//if (!String.IsNullOrEmpty(accountId))
					//{
					xmlBuilder.AppendFormat("<Friend AccountId='{0}' FBAccountId='{1}' FirstName='{2}' LastName='{3}' PicSquare='{4}' IsAppUser='{5}' OnlinePresence='{6}' />",
					accountId, fbAccountId, SecurityElement.Escape(firstName), SecurityElement.Escape(lastName), squarePic, isAppUser, onlinePresence);
					//}
				}
			}

			return xmlBuilder.ToString();
		}

		/// <summary>
		/// uses a list of facebook.Schema.user 
		/// from one of the facebook api services that returns user objects
		/// </summary>
		/// <param name="fbFriends"></param>
		/// <returns></returns>
		private string ConstructFriendData(IList<Facebook.Schema.user> fbFriends)
		{
			StringBuilder xmlBuilder = new StringBuilder();
			foreach (Facebook.Schema.user user in fbFriends)
			{
				bool hasAddedApp = user.has_added_app.GetValueOrDefault();
				bool isAppUser = user.is_app_user.GetValueOrDefault();

				xmlBuilder.AppendFormat("<Friend FBAccountId='{0}' Name='{1}' Birthday='{2}' FirstName='{3}' LastName='{4}' PicSquare='{5}' PicSmall='{6}' IsAppUser='{7}' />",
					user.uid, SecurityElement.Escape(user.name), user.birthday,
					SecurityElement.Escape(user.first_name), SecurityElement.Escape(user.last_name), user.pic_square,
					user.pic_small, isAppUser.ToString());
			}
			return xmlBuilder.ToString();
		}


		private string GetMockFriendDataForFacebook()
		{
			string mockData =
				"<Friend AccountId='1000007' FBAccountId='912479' FirstName='Vilas' LastName='Tewari' PicSquare='http://profile.ak.fbcdn.net/v230/1674/37/q912479_8015.jpg' IsAppUser='1' OnlinePresence='active'/>" +
				"<Friend AccountId='1000000' FBAccountId='13001919' FirstName='Lucas' LastName='Smolic' PicSquare='http://profile.ak.fbcdn.net/v230/1199/7/q13001919_9801.jpg' IsAppUser='1' OnlinePresence='active'/>" +
				"<Friend AccountId='1000008' FBAccountId='505456151' FirstName='Pano' LastName='Anthos' PicSquare='http://profile.ak.fbcdn.net/v222/832/1/q505456151_9714.jpg' IsAppUser='1' OnlinePresence='active'/>" +
				"<Friend AccountId='1000005' FBAccountId='651220831' FirstName='Samir' LastName='Naik' PicSquare='http://profile.ak.fbcdn.net/v222/1162/10/q651220831_4504.jpg' IsAppUser='1' OnlinePresence='idle'/>" +
				"<Friend AccountId='1000006' FBAccountId='1520031799' FirstName='Matt' LastName='Hangout' PicSquare='' IsAppUser='1' OnlinePresence='active'/>" +
				"<Friend AccountId='1000009' FBAccountId='100000069628492' FirstName='Ian' LastName='Graham' PicSquare='' IsAppUser='1' OnlinePresence='active'/>";

			return mockData;
		}

	}
}
