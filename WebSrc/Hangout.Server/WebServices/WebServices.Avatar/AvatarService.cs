using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Hangout.Shared;
using System.Xml;

namespace Hangout.Server.WebServices
{
	public class AvatarService : ServiceBaseClass
	{
		private ServicesLog mServiceLog = new ServicesLog("AvatarService");

		public SimpleResponse GetAvatars(string accountId, string accountIdCsvList, string avatarId, string isDefault, string isEnabled)
		{
			mServiceLog.Log.InfoFormat("GetAvatars: accountId={0}, avatarId={1}, isDefault={2}, isEnabled={3}", accountId, avatarId, isDefault, isEnabled);
			StringBuilder sqlOptionsAvatars = new StringBuilder();
			StringBuilder sqlOptionsSystemAvatars = new StringBuilder();

			StringBuilder xmlBuilder = new StringBuilder();
			try
			{
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AvatarsDBConnectionString))
				{
					mysqlConnection.Open();
					string getAvatarsForAccountQuery = "";
					string getSystemAvatarsForAccountQuery = "";
					bool isUserAccount = false; //this is set if something needs to return something from 
					bool isUserAvatar = false;

					List<XElement> avatarList = new List<XElement>();
					MySqlCommand getAvatarsCommand = mysqlConnection.CreateCommand();
					MySqlCommand getSystemAvatarsCommand = mysqlConnection.CreateCommand();


					if (avatarId != null)
						{
							if (avatarId == "")
							{
								avatarId = "-1";
							}

							sqlOptionsAvatars.Append("AND AccountToAvatarMapping.AvatarId=@AvatarId ");
							getAvatarsCommand.Parameters.AddWithValue("@AvatarId", avatarId);
							sqlOptionsSystemAvatars.Append("AND AvatarId=@AvatarId ");
							getSystemAvatarsCommand.Parameters.AddWithValue("@AvatarId", avatarId);
					}
					if (accountId != null)
					{
						if (accountId == "")
						{
							accountId = "-1";
						}
						sqlOptionsAvatars.Append("AND AccountToAvatarMapping.HangoutAccountId=@HangoutAccountId ");
						getAvatarsCommand.Parameters.AddWithValue("@HangoutAccountId", accountId);
						isUserAccount = true;

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
						sqlOptionsAvatars.Append("AND AccountToAvatarMapping.HangoutAccountId IN ( " + csvList + " ) ");
						isUserAccount = true;
					}

					if (!String.IsNullOrEmpty(isDefault))
					{
						string boolValue = "0";
						if (isDefault == "1" || isDefault.ToLower() == "true")
						{
							boolValue = "1";
						}
						sqlOptionsAvatars.Append("AND AccountToAvatarMapping.IsDefault=@IsDefault ");
						getAvatarsCommand.Parameters.AddWithValue("@IsDefault", boolValue);
					}
					if (!String.IsNullOrEmpty(isEnabled))
					{
						string boolValue = "0";
						if (isEnabled == "1" || isEnabled.ToLower() == "true")
						{
							boolValue = "1";
						}
						sqlOptionsAvatars.Append("AND Avatars.IsEnabled=@IsEnabled ");
						sqlOptionsSystemAvatars.Append("AND SystemAvatars.IsEnabled=@IsEnabled ");

						getAvatarsCommand.Parameters.AddWithValue("@IsEnabled", boolValue);
						getSystemAvatarsCommand.Parameters.AddWithValue("@IsEnabled", boolValue);
					}


					getSystemAvatarsForAccountQuery += "SELECT * FROM SystemAvatars  " +
													"WHERE 1 " + sqlOptionsSystemAvatars.ToString() +
													"ORDER BY AvatarId ";

					getSystemAvatarsCommand.CommandText = getSystemAvatarsForAccountQuery;
					if (!isUserAccount && !isUserAvatar)
					{
						using (MySqlDataReader getSystemAvatarsReader = getSystemAvatarsCommand.ExecuteReader())
						{
							while (getSystemAvatarsReader.Read())
							{
								xmlBuilder.Append(FormatAvatarData(
									getSystemAvatarsReader["AvatarId"].ToString(),
									getSystemAvatarsReader["IsEnabled"].ToString(),
									"1",
									"1",
									getSystemAvatarsReader["AvatarDNA"].ToString()
									));
							}
						}
					}
					getSystemAvatarsCommand.Dispose();


					getAvatarsForAccountQuery += "SELECT * FROM Avatars  " +
													"LEFT JOIN AccountToAvatarMapping ON (AccountToAvatarMapping.AvatarId = Avatars.AvatarId) " +
													"WHERE 1 " + sqlOptionsAvatars.ToString() + " ORDER BY AccountToAvatarMapping.AvatarId ; ";


					getAvatarsCommand.CommandText = getAvatarsForAccountQuery;
					using (MySqlDataReader getAvatarsReader = getAvatarsCommand.ExecuteReader())
					{
						while (getAvatarsReader.Read())
						{
							xmlBuilder.Append(FormatAvatarData(
								getAvatarsReader["AvatarId"].ToString(),
								getAvatarsReader["IsEnabled"].ToString(),
								getAvatarsReader["IsDefault"].ToString(),
								getAvatarsReader["HangoutAccountId"].ToString(),
								getAvatarsReader["AvatarDNA"].ToString()
								));
						}
					}
					getAvatarsCommand.Dispose();
				}
				return new SimpleResponse("Avatars", xmlBuilder.ToString());
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		public SimpleResponse GetSystemAvatars(string accountId, string avatarId, string isDefault, string isEnabled)
		{
			mServiceLog.Log.InfoFormat("GetSystemAvatars: accountId={0}, avatarId={1}, isDefault={2}, isEnabled={3}", accountId, avatarId, isDefault, isEnabled);
			StringBuilder sqlOptions = new StringBuilder();

			StringBuilder xmlBuilder = new StringBuilder();
			try
			{
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AvatarsDBConnectionString))
				{
					mysqlConnection.Open();
					string getAvatarsForAccountQuery = "SELECT * FROM SystemAvatars  " +
													"LEFT JOIN AccountToAvatarMapping ON (AccountToAvatarMapping.AvatarId = SystemAvatars.AvatarId) " +
													"WHERE 1 ";


					List<XElement> avatarList = new List<XElement>();
					using (MySqlCommand getAvatarsCommand = mysqlConnection.CreateCommand())
					{
						if (!String.IsNullOrEmpty(avatarId))
						{
							sqlOptions.Append("AND AccountToAvatarMapping.AvatarId=@AvatarId ");
							getAvatarsCommand.Parameters.AddWithValue("@AvatarId", avatarId);
						}
						if (!String.IsNullOrEmpty(accountId))
						{
							sqlOptions.Append("AND AccountToAvatarMapping.HangoutAccountId=@HangoutAccountId ");
							getAvatarsCommand.Parameters.AddWithValue("@HangoutAccountId", accountId);
						}
						if (!String.IsNullOrEmpty(isDefault))
						{
							string boolValue = "0";
							if (isDefault == "1" || isDefault.ToLower() == "true")
							{
								boolValue = "1";
							}
							sqlOptions.Append("AND AccountToAvatarMapping.IsDefault=@IsDefault ");
							getAvatarsCommand.Parameters.AddWithValue("@IsDefault", boolValue);
						}
						if (!String.IsNullOrEmpty(isEnabled))
						{
							string boolValue = "0";
							if (isEnabled == "1" || isEnabled.ToLower() == "true")
							{
								boolValue = "1";
							}
							sqlOptions.Append("AND SystemAvatars.IsEnabled=@IsEnabled ");
							getAvatarsCommand.Parameters.AddWithValue("@IsEnabled", boolValue);
						}

						getAvatarsForAccountQuery += sqlOptions.ToString();
						getAvatarsForAccountQuery += "ORDER BY AccountToAvatarMapping.AvatarId; ";

						getAvatarsCommand.CommandText = getAvatarsForAccountQuery;
						using (MySqlDataReader getAvatarsReader = getAvatarsCommand.ExecuteReader())
						{
							while (getAvatarsReader.Read())
							{
								xmlBuilder.Append(FormatAvatarData(
									getAvatarsReader["AvatarId"].ToString(),
									getAvatarsReader["IsEnabled"].ToString(),
									getAvatarsReader["IsDefault"].ToString(),
									getAvatarsReader["HangoutAccountId"].ToString(),
									getAvatarsReader["AvatarDNA"].ToString()
									));
							}
						}
					}


				}
				return new SimpleResponse("Avatars", xmlBuilder.ToString());
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		private string FormatAvatarData(string avatarId, string isEnabled, string isDefault, string accountId, string avatarDNA)
		{
			StringBuilder xmlBuilder = new StringBuilder();
			xmlBuilder.AppendFormat("<Avatar AvatarId='{0}' IsEnabled='{1}' IsDefault='{2}' AccountId='{3}'>",
								avatarId,
								isEnabled,
								isDefault,
								accountId);

			xmlBuilder.Append(avatarDNA);
			xmlBuilder.Append("</Avatar>");

			return xmlBuilder.ToString();
		}

		public SimpleResponse UpdateAvatarDNA(string avatarId, string avatarDNA)
		{
			mServiceLog.Log.InfoFormat("UpdateAvatarDNA: avatarId={0}, avatarDNA={1}", avatarId, avatarDNA);
			if (String.IsNullOrEmpty(avatarId))
			{
				throw new ArgumentNullException("avatarId");
			}

			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AvatarsDBConnectionString))
			{
				mysqlConnection.Open();


				string table = "Avatars";
				int numAvatarId = Convert.ToInt32(avatarId);
				if (numAvatarId < 1000001)
				{
					table = "SystemAvatars";
				}

				string updateAvatarDNAQuery = "UPDATE " + table + " SET AvatarDNA=@AvatarDNA WHERE AvatarId=@AvatarId;";
				using (MySqlCommand updateAvatarDNACommand = mysqlConnection.CreateCommand())
				{
					try
					{
						updateAvatarDNACommand.CommandText = updateAvatarDNAQuery;
						updateAvatarDNACommand.Parameters.AddWithValue("@AvatarId", avatarId);
						updateAvatarDNACommand.Parameters.AddWithValue("@AvatarDNA", avatarDNA);
						updateAvatarDNACommand.ExecuteNonQuery();
					}
					catch (MySql.Data.MySqlClient.MySqlException ex)
					{
						StringBuilder errorMessages = new StringBuilder();

						errorMessages.Append("Error Number:" + ex.Number + "\n" +
							"ErrorCode: " + ex.ErrorCode + "\n" +
							"InnerException: " + ex.InnerException + "\n" +
							"Message: " + ex.Message + "\n" +
							"Method: " + ex.TargetSite + "\n" +
							"StackTrace: " + ex.StackTrace + "\n");

						mServiceLog.Log.Error(errorMessages.ToString());
						throw ex;
					}

				}
				return GetAvatars(null, null, avatarId, null, null);
			}
		}


		/// <summary>
		/// Takes a required AccountId and then either:
		/// 1. avatarDNA 
		/// 2. defaultAvatarId
		/// This will return the created avatar
		/// </summary>
		/// <param name="accountId"></param>
		/// <param name="avatarDNA"></param>
		/// <param name="defaultAvatarId"></param>
		/// <returns></returns>
		public SimpleResponse CreateAvatar(string accountId, string avatarDNA, string defaultAvatarId)
		{
			mServiceLog.Log.InfoFormat("CreateAvatar: accountId={0}, avatarDNA={1}, defaultAvatarId={2}", accountId, avatarDNA, defaultAvatarId);
			string avatarIdFromDB = "0";
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AvatarsDBConnectionString))
			{
				mysqlConnection.Open();

				string createAvatarQuery = "";

				using (MySqlCommand createAvatarCommand = mysqlConnection.CreateCommand())
				{
					try
					{
						if (String.IsNullOrEmpty(avatarDNA))
						{
							if (String.IsNullOrEmpty(defaultAvatarId))
							{
								throw new NullReferenceException("Missing Argument: defaultAvatarId");
							}
							if (defaultAvatarId == "0")
							{
								defaultAvatarId = "1";
							}
							createAvatarQuery = "INSERT INTO Avatars ( Avatars.AvatarDNA, Avatars.IsEnabled ) " +
								"SELECT SystemAvatars.AvatarDNA, SystemAvatars.IsEnabled FROM SystemAvatars WHERE SystemAvatars.AvatarId=@AvatarId; ";
							createAvatarCommand.Parameters.AddWithValue("@AvatarId", defaultAvatarId);
						}
						else
						{
							createAvatarQuery = "INSERT INTO Avatars ( AvatarDNA, IsEnabled ) VALUES (@avatarDNA, '1'); ";
							createAvatarCommand.Parameters.AddWithValue("@avatarDNA", avatarDNA);
						}

						createAvatarQuery += "SELECT LAST_INSERT_ID();";
						createAvatarCommand.CommandText = createAvatarQuery;

						object o = createAvatarCommand.ExecuteScalar();
						if (o != null)
						{
							avatarIdFromDB = o.ToString();
						}
						else
						{
							throw new Exception("Insert returned 0 results");
						}
					}
					catch (MySql.Data.MySqlClient.MySqlException ex)
					{
						StringBuilder errorMessages = new StringBuilder();

						errorMessages.Append("Error Number:" + ex.Number + "\n" +
							"ErrorCode: " + ex.ErrorCode + "\n" +
							"InnerException: " + ex.InnerException + "\n" +
							"Message: " + ex.Message + "\n" +
							"Method: " + ex.TargetSite + "\n" +
							"StackTrace: " + ex.StackTrace + "\n");

						mServiceLog.Log.Error(errorMessages.ToString());
						throw ex;
					}
				}
				string mapAccountToAvatarQuery = "INSERT INTO AccountToAvatarMapping ( HangoutAccountId, AvatarId ) VALUES (@HangoutAccountId,@AvatarId);";
				using (MySqlCommand insertAccountMappingToAvatarCommand = mysqlConnection.CreateCommand())
				{
					try
					{
						mServiceLog.Log.InfoFormat("CreateAvatar- INSERT INTO AccountToAvatarMapping: HangoutAccountId={0}, new auto incremented AvatarId={1}", accountId, avatarIdFromDB);
						insertAccountMappingToAvatarCommand.Parameters.AddWithValue("@HangoutAccountId", accountId);
						insertAccountMappingToAvatarCommand.Parameters.AddWithValue("@AvatarId", avatarIdFromDB);
						insertAccountMappingToAvatarCommand.CommandText = mapAccountToAvatarQuery;
						insertAccountMappingToAvatarCommand.ExecuteNonQuery();
					}
					catch (MySql.Data.MySqlClient.MySqlException ex)
					{
						StringBuilder errorMessages = new StringBuilder();

						errorMessages.Append("Error Number:" + ex.Number + "\n" +
							"ErrorCode: " + ex.ErrorCode + "\n" +
							"InnerException: " + ex.InnerException + "\n" +
							"Message: " + ex.Message + "\n" +
							"Method: " + ex.TargetSite + "\n" +
							"StackTrace: " + ex.StackTrace + "\n");

						mServiceLog.Log.Error(errorMessages.ToString());
						throw ex;
					}
				}
			}



			return SetDefaultAvatar(accountId, avatarIdFromDB);
		}

		public SimpleResponse DeleteAvatar(string avatarId)
		{
			mServiceLog.Log.InfoFormat("DeleteAvatar: avatarId={0}", avatarId);
			AvatarUtil avatarUtil = new AvatarUtil();
			avatarUtil.DeleteAvatar(avatarId);
			return new SimpleResponse("Success", "true");
		}

		public SimpleResponse DisableAvatar(string avatarId)
		{
			mServiceLog.Log.InfoFormat("DisableAvatar: avatarId={0}", avatarId);
			AvatarUtil avatarUtil = new AvatarUtil();
			avatarUtil.DisableAvatar(avatarId);
			return GetAvatars(null, null, avatarId, null, null);
		}

		public SimpleResponse EnableAvatar(string avatarId)
		{
			mServiceLog.Log.InfoFormat("EnableAvatar: avatarId={0}", avatarId);
			AvatarUtil avatarUtil = new AvatarUtil();
			avatarUtil.EnableAvatar(avatarId);
			return GetAvatars(null, null, avatarId, null, null);
		}

		public SimpleResponse SetDefaultAvatar(string accountId, string avatarId)
		{
			mServiceLog.Log.InfoFormat("SetDefaultAvatar: accountId={0}", accountId);
			AvatarUtil avatarUtil = new AvatarUtil();
			avatarUtil.SetAllUserAvatarsToNotBeDefault(accountId);

			StringBuilder xmlBuilder = new StringBuilder();
			try
			{
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AvatarsDBConnectionString))
				{
					mysqlConnection.Open();

					string getDefaultAvatarQuery = "UPDATE AccountToAvatarMapping SET IsDefault = '1' WHERE AvatarId=@AvatarId;";

					using (MySqlCommand getDefaultAvatarCommand = mysqlConnection.CreateCommand())
					{
						getDefaultAvatarCommand.Parameters.AddWithValue("@AvatarId", avatarId);
						getDefaultAvatarCommand.CommandText = getDefaultAvatarQuery;
						getDefaultAvatarCommand.ExecuteNonQuery();
					}
				}
			}
			catch (System.Exception ex)
			{
				throw ex;
			}

			return GetAvatars(null, null, avatarId, null, null);
		}

	}
}
