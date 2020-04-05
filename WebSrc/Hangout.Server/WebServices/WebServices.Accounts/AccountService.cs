using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using Hangout.Shared;
using Facebook;
using Facebook.Utility;
using System.Xml;
using System.Security;
using Facebook.Rest;
using Facebook.Session;

namespace Hangout.Server.WebServices
{
	public class AccountService : ServiceBaseClass
	{
		private ServicesLog mServiceLog = new ServicesLog("AccountService");

		/// <summary>
		/// CreateAccountFromFacebook checks first to see if there is an existing account for this fbUser.
		/// If a user exists it simply returns that user, otherwise it inserts a new user into the db
		/// and returns their information
		/// </summary>
		/// <param name="fbAccountId"></param>
		/// <param name="fbSessionKey"></param>
		/// <param name="nickName"></param>
		/// <param name="campaign"></param>
		/// ONLY ONE of the following should be sent.
		/// <param name="referringAccountId"></param>
		/// <param name="referringFbAccountId"></param>
		/// <param name="campaign"></param>
		/// <returns></returns>
		public SimpleResponse CreateAccountFromFacebook(long fbAccountId, string fbSessionKey, string nickName, string referringAccountId, string referringFbAccountId, string campaign)
		{
			mServiceLog.Log.InfoFormat("CreateAccountFromFacebook: fbAccountId={0}, fbSessionKey={1}, nickName={2}, referringAccountId={3}, referringFbAccountId={4}, campaign={5}", fbAccountId, fbSessionKey, nickName, referringAccountId, referringFbAccountId, campaign);

			FacebookSession facebookSession = new HangoutFacebookSession();
			facebookSession.ApplicationKey = WebConfig.FacebookAPIKey;
			facebookSession.ApplicationSecret = WebConfig.FacebookSecret;
			facebookSession.SessionKey = fbSessionKey;

			Api facebookApi = new Api(facebookSession);

			long currentFbUserId;
			Facebook.Schema.user fbUser;
			
			try
			{
				currentFbUserId = facebookApi.Users.GetLoggedInUser();
				fbUser = facebookApi.Users.GetInfo(currentFbUserId);
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

			

			if (!fbUser.is_app_user.GetValueOrDefault())
			{
				throw new Exception("is_app_user returned false");
			}

			if (fbAccountId != fbUser.uid)
			{
				throw new Exception("logged in facebook UID does not match fbAccountId parameter");
			}

			SimpleResponse selectResponse = GetAccounts(fbAccountId.ToString(), null, null, null, null, null);

			XmlNode accountNode = selectResponse.SelectSingleNode("//Account");
			if (accountNode != null)
			{
				XmlAttribute accountIdAttr = accountNode.Attributes["AccountId"];
				if (accountIdAttr != null && !String.IsNullOrEmpty(accountIdAttr.Value))
				{
					return selectResponse;
				}
			}
			//pass through and insert a new user... because the old user don't EXIST!!

			uint accountId = 0;
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AccountsDBConnectionString))
			{
				mysqlConnection.Open();

				string createNewHangoutUserQuery = "INSERT INTO LocalAccountInfo " +
					" (FacebookAccountId, HangoutNickName, FirstName, LastName, Birthdate, Gender, CreatedDate, LastLoggedIn) " +
					" VALUES (@FBAccountId, @HangoutNickName, @FirstName, @LastName, @Birthdate, @Gender, UTC_TIMESTAMP(), UTC_TIMESTAMP()); SELECT LAST_INSERT_ID();";

				using (MySqlCommand createAccountCommand = mysqlConnection.CreateCommand())
				{
					createAccountCommand.CommandText = createNewHangoutUserQuery;
					createAccountCommand.Parameters.AddWithValue("@FBAccountId", fbAccountId);
					createAccountCommand.Parameters.AddWithValue("@HangoutNickName", fbUser.first_name);
					createAccountCommand.Parameters.AddWithValue("@FirstName", fbUser.first_name);
					createAccountCommand.Parameters.AddWithValue("@LastName", fbUser.last_name);
					createAccountCommand.Parameters.AddWithValue("@Birthdate", fbUser.birthday_date.ToShortDateString());
					createAccountCommand.Parameters.AddWithValue("@Gender", fbUser.sex);


					accountId = Convert.ToUInt32(createAccountCommand.ExecuteScalar());
					if (accountId == 0)
					{
						throw new Exception("AccountId returned 0");
					}
				}

				InsertReferralData(accountId.ToString(), referringAccountId, referringFbAccountId, campaign);
			}

			return GetAccounts(null, accountId.ToString(), null, null, null, null);
		}

		public SimpleResponse InsertReferralData(string accountId, string referringAccountId, string referringFbAccountId, string campaign)
		{
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AccountsDBConnectionString))
			{
				mysqlConnection.Open();
				using (MySqlCommand insertReferralInfoCommand = mysqlConnection.CreateCommand())
				{
					string getAccountIdForFacebookId = "";
					string getSourceCompaign = " (SELECT SourceCampaign FROM AccountReferrals AS ar1 WHERE AccountId=@TempAccountId)";
					string getSourceAccountId = " (SELECT SourceAccountId FROM AccountReferrals AS ar2 WHERE AccountId=@TempAccountId) ";
					
					if (!String.IsNullOrEmpty(referringAccountId))
					{
						int anIntegerNamedDesire;
						if (!Int32.TryParse(referringAccountId, out anIntegerNamedDesire))
						{
							getAccountIdForFacebookId = "0";
							getSourceCompaign = " @Campaign ";
							getSourceAccountId = " @AccountId ";
						}
						else
						{
							getAccountIdForFacebookId = "@ReferringAccountId";
							insertReferralInfoCommand.Parameters.AddWithValue("@ReferringAccountId", referringAccountId);
						}
						
					}
					else if (!String.IsNullOrEmpty(referringFbAccountId))
					{
						long aSixtyFourBitIntegerNamedDesire;
						if (!Int64.TryParse(referringFbAccountId, out aSixtyFourBitIntegerNamedDesire))
						{
							getAccountIdForFacebookId = "0";
							getSourceCompaign = " @Campaign ";
							getSourceAccountId = " @AccountId ";
						}
						else
						{
							getAccountIdForFacebookId = "(SELECT HangoutAccountId FROM LocalAccountInfo WHERE FacebookAccountId=@ReferringFbAccountId)";
							insertReferralInfoCommand.Parameters.AddWithValue("@ReferringFbAccountId", referringFbAccountId);
						}
					}
					else
					{
						getAccountIdForFacebookId = "0";
						getSourceCompaign = " @Campaign ";
						getSourceAccountId = " @AccountId ";
					}

					if (String.IsNullOrEmpty(campaign))
					{
						campaign = "UNKNOWN";
					}

					string insertReferralInformationQuery = "SET @TempAccountId := " + getAccountIdForFacebookId + ";" +
					"INSERT INTO AccountReferrals " +
					"(AccountId,ReferringAccountId,Campaign,SourceCampaign,SourceAccountId,CreatedDate) " +
					"VALUES (@AccountId,@TempAccountId,@Campaign, " +
					getSourceCompaign + ", " +
					getSourceAccountId + ", UTC_TIMESTAMP() );";

					insertReferralInfoCommand.CommandText = insertReferralInformationQuery;
					insertReferralInfoCommand.Parameters.AddWithValue("@AccountId", accountId);
					insertReferralInfoCommand.Parameters.AddWithValue("@Campaign", campaign);
					insertReferralInfoCommand.ExecuteNonQuery();

				}
			}
			return new SimpleResponse("Success", "true");
		}

		public SimpleResponse UpdateAccountWithPaymentItemsInfo(uint accountId, string paymentItemsAccountId, string secureKey)
		{
			mServiceLog.Log.InfoFormat("UpdateAccountWithPaymentItemsInfo: accountId={0}, paymentItemsAccountId={1}", accountId, paymentItemsAccountId);
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AccountsDBConnectionString))
			{
				mysqlConnection.Open();

				string updateHangoutUserQuery = "UPDATE LocalAccountInfo SET PaymentItemsAccountId=@PaymentItemsAccountId, PaymentItemsSecureKey=@PaymentItemsSecureKey WHERE HangoutAccountId=@HangoutAccountId";

				using (MySqlCommand updateAccountCommand = mysqlConnection.CreateCommand())
				{
					updateAccountCommand.CommandText = updateHangoutUserQuery;
					updateAccountCommand.Parameters.AddWithValue("@HangoutAccountId", accountId.ToString());
					updateAccountCommand.Parameters.AddWithValue("@PaymentItemsAccountId", paymentItemsAccountId);
					updateAccountCommand.Parameters.AddWithValue("@PaymentItemsSecureKey", secureKey);
					updateAccountCommand.ExecuteNonQuery();
				}
			}
			return GetAccounts(null, accountId.ToString(), null, null, null, null);
		}

		public SimpleResponse GetAccounts(string fbAccountId, string accountId, string nickName, string accountIdCsvList, string fbAccountIdCsvList, string ignoreUserData)
		{
			mServiceLog.Log.InfoFormat("GetAccounts: fbAccountId={0}, accountId={1}, nickName={2}, accountIdCsvList={3}, fbAccountIdCsvList={4}, ignoreUserData={5}", fbAccountId, accountId, nickName, accountIdCsvList, fbAccountIdCsvList, ignoreUserData);
			StringBuilder xmlBuilder = new StringBuilder();
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AccountsDBConnectionString))
			{
				mysqlConnection.Open();
				string getAccountForUserQuery = "SELECT * FROM LocalAccountInfo WHERE 1 ";
				using (MySqlCommand getAccountCommand = mysqlConnection.CreateCommand())
				{
					if (fbAccountId != null)
					{
						if (fbAccountId == "")
						{
							fbAccountId = "-1";
						}
						getAccountForUserQuery += " AND FacebookAccountId=@FBAccountId ";
						getAccountCommand.Parameters.AddWithValue("@FBAccountId", fbAccountId.ToString());

					}
					else if (fbAccountIdCsvList != null)
					{
						string csvList = "";
						if (fbAccountIdCsvList != "")
						{
							string[] accountIds = fbAccountIdCsvList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							string delimiter = "";
							foreach (string s in accountIds)
							{
								long sUint = Convert.ToInt64(s);
								csvList += delimiter + "'" + sUint.ToString() + "'";
								delimiter = ",";
							}
							if (csvList == "")
							{
								csvList = "-1";
							}
						}
						else
						{
							csvList = "-1";
						}
						getAccountForUserQuery += " AND FacebookAccountId IN ( " + csvList + " ) ";
					}
					if (accountId != null)
					{
						if (accountId == "")
						{
							accountId = "-1";
						}
						getAccountForUserQuery += " AND HangoutAccountId=@HangoutAccountId ";
						getAccountCommand.Parameters.AddWithValue("@HangoutAccountId", accountId.ToString());

					}
					else if (accountIdCsvList != null)
					{
						string csvList = "";
						if (accountIdCsvList != "")
						{
							string[] accountIds = accountIdCsvList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							string delimiter = "";
							foreach (string s in accountIds)
							{
								uint sUint = Convert.ToUInt32(s);
								csvList += delimiter + "'" + sUint.ToString() + "'";
								delimiter = ",";
							}
							if (csvList == "")
							{
								csvList = "-1";
							}
						}
						else
						{
							csvList = "-1";
						}
						getAccountForUserQuery +=" AND HangoutAccountId IN ( " + csvList + " ) ";
					}
					if (!String.IsNullOrEmpty(nickName))
					{
						getAccountForUserQuery += " AND HangoutNickName=@NickName ";
						getAccountCommand.Parameters.AddWithValue("@NickName", nickName);
					}

					getAccountCommand.CommandText = getAccountForUserQuery;
					using (MySqlDataReader accountReader = getAccountCommand.ExecuteReader())
					{
						while (accountReader.Read())
						{
							xmlBuilder.AppendFormat("<Account AccountId='{0}' FBAccountId='{1}' PIAccountId='{2}' PISecureKey='{3}'  NickName='{4}' FirstName='{5}' LastName='{6}' >",
									accountReader["HangoutAccountId"].ToString(),
									accountReader["FacebookAccountId"].ToString(),
									accountReader["PaymentItemsAccountId"].ToString(),
									accountReader["PaymentItemsSecureKey"].ToString(),
									SecurityElement.Escape(accountReader["HangoutNickName"].ToString()),
									SecurityElement.Escape(accountReader["FirstName"].ToString()),
									SecurityElement.Escape(accountReader["LastName"].ToString()));
							if (String.IsNullOrEmpty(ignoreUserData))
							{
								if (Convert.ToInt16(ignoreUserData) == 0)
								{
									xmlBuilder.Append(accountReader["AccountData"].ToString());
								}
							}
							xmlBuilder.Append("</Account>");
						}
					}
				}
			}
			return new SimpleResponse("Accounts", xmlBuilder.ToString());
		}

		public SimpleResponse LoginFacebook(long fbAccountId, string fbSessionKey)
		{
			mServiceLog.Log.InfoFormat("LoginFacebook: fbAccountId={0}, fbSessionKey={1}", fbAccountId, fbSessionKey);
			
			FacebookSession facebookSession = new HangoutFacebookSession();
			facebookSession.ApplicationKey = WebConfig.FacebookAPIKey;
			facebookSession.ApplicationSecret = WebConfig.FacebookSecret;
			facebookSession.SessionKey = fbSessionKey;

			Api facebookApi = new Api(facebookSession);

			long currentFbUserId;
			Facebook.Schema.user fbUser;

			try
			{
				currentFbUserId = facebookApi.Users.GetLoggedInUser();
				fbUser = facebookApi.Users.GetInfo(currentFbUserId);
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

			if (!fbUser.is_app_user.GetValueOrDefault())
			{
				throw new Exception("is_app_user returned false");
			}

			if (fbAccountId != fbUser.uid)
			{
				throw new Exception("logged in facebook UID does not match fbAccountId parameter");
			}
			SetUserMetricsData(fbAccountId.ToString());
			return GetAccounts(fbAccountId.ToString(), null, null, null, null, null);
		}
		public SimpleResponse LoginName(string nickName, string password)
		{
			throw new NotImplementedException();
		}

		public SimpleResponse SetUserMetricsData(string fbAccountId)
		{
			mServiceLog.Log.InfoFormat("SetUserMetricsData: fbAccountId={0}", fbAccountId);
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AccountsDBConnectionString))
			{
				mysqlConnection.Open();

				string setUserMetricsDataQuery = "UPDATE LocalAccountInfo SET LastLoggedIn=UTC_TIMESTAMP() WHERE FacebookAccountId=@FacebookAccountId";

				using (MySqlCommand setUserMetricsDataCommand = mysqlConnection.CreateCommand())
				{
					setUserMetricsDataCommand.CommandText = setUserMetricsDataQuery;
					setUserMetricsDataCommand.Parameters.AddWithValue("@FacebookAccountId", fbAccountId);
					setUserMetricsDataCommand.ExecuteNonQuery();
				}
			}
			return new SimpleResponse("success", "true");
		}

		public SimpleResponse UpdateAccountData(string accountId, string accountData)
		{
			mServiceLog.Log.InfoFormat("UpdateAccountData: accountId={0}, accountData={1}", accountId, accountData);
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AccountsDBConnectionString))
			{
				mysqlConnection.Open();

				string updateAccountDataQuery = "UPDATE LocalAccountInfo SET AccountData=@AccountData WHERE HangoutAccountId=@HangoutAccountId";

				using (MySqlCommand updateAccountCommand = mysqlConnection.CreateCommand())
				{
					updateAccountCommand.CommandText = updateAccountDataQuery;
					updateAccountCommand.Parameters.AddWithValue("@AccountData", accountData);
					updateAccountCommand.Parameters.AddWithValue("@HangoutAccountId", accountId);
					updateAccountCommand.ExecuteNonQuery();
				}
			}
			return GetAccounts(null, accountId, null, null, null, null);
		}

		public SimpleResponse RemoveAllTracesOfAccount(string accountId, string accountIdCsvList, string fbAccountId, string nickName)
		{
			mServiceLog.Log.InfoFormat("RemoveAllTracesOfAccount: accountId={0}, accountIdCsvList={1}, fbAccountId={2}, nickName={3}", accountId, accountIdCsvList, fbAccountId, nickName);

			List<string> accounts = new List<string>();
			StringBuilder sqlOptions = new StringBuilder();
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AccountsDBConnectionString))
			{
				mysqlConnection.Open();
				using (MySqlCommand getAccountsCommand = mysqlConnection.CreateCommand())
				{

					if (accountId != null)
					{
						if (accountId == "")
						{
							accountId = "-1";
						}
						sqlOptions.Append("OR HangoutAccountId=@HangoutAccountId ");
						getAccountsCommand.Parameters.AddWithValue("@HangoutAccountId", accountId);

					}
					else if (accountIdCsvList != null)
					{
						string csvList = "";
						if (accountIdCsvList != "")
						{
							string[] accountIds = accountIdCsvList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							string delimiter = "";
							foreach (string s in accountIds)
							{
								uint sUint = Convert.ToUInt32(s);
								csvList += delimiter + "'" + sUint.ToString() + "'";
								delimiter = ",";
							}
							if (csvList == "")
							{
								csvList = "-1";
							}
						}
						else
						{
							csvList = "-1";
						}
						sqlOptions.Append("OR HangoutAccountId IN ( " + csvList + " ) ");
					}
					
					if(!String.IsNullOrEmpty(nickName))
					{
						sqlOptions.Append("OR HangoutNickName = @HangoutNickName ");
						getAccountsCommand.Parameters.AddWithValue("@HangoutNickName", nickName);
					}

					if (!String.IsNullOrEmpty(fbAccountId))
					{
						sqlOptions.Append("OR FacebookAccountId=@FbAccountId ");
						getAccountsCommand.Parameters.AddWithValue("@FbAccountId", fbAccountId);
					}

					string getAccountsQuery = "SELECT * FROM LocalAccountInfo " +
												"WHERE 0 " + sqlOptions;

					getAccountsCommand.CommandText = getAccountsQuery;			
					using (MySqlDataReader getAccountsReader = getAccountsCommand.ExecuteReader())
					{
						while(getAccountsReader.Read())
						{
							if( getAccountsReader["HangoutAccountId"] != null )
							{
								accounts.Add(getAccountsReader["HangoutAccountId"].ToString());
							}
						}
					}
				}
			}
			AccountUtil accountUtil = new AccountUtil();
			foreach ( string account in accounts )
			{
				accountUtil.DeleteAccount(account);
				accountUtil.DeleteAvatars(account);
				accountUtil.DeleteRooms(account);
				accountUtil.DeleteMiniGameUserData(account);
			}
			return new SimpleResponse("Success", "true");
		}
	}
}
