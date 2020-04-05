using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using MySql.Data.MySqlClient;


namespace Hangout.Server.WebServices
{
	public class MiniGamesService : ServiceBaseClass
	{
		private ServicesLog mServiceLog = new ServicesLog("MiniGames");

		public SimpleResponse GetMiniGameUserData(string accountId, string miniGameId, string miniGameName, string dataKey, string dataKeyCsvList)
		{
			mServiceLog.Log.InfoFormat("GetMiniGameUserData: accountId={0}, miniGameId={1}, miniGameName={2}, dataKey={3}, dataKeyCSVList={4}",
							accountId, miniGameId, miniGameName, dataKey, dataKeyCsvList);
			StringBuilder xmlBuilder = new StringBuilder();
			try
			{
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.MiniGameDBConnectionString))
				{
					mysqlConnection.Open();

					Dictionary<string, string> gameList = new Dictionary<string,string>();
					Dictionary<string, List<XElement>> userDataList = new Dictionary<string, List<XElement>>();

					StringBuilder sqlOptions = new StringBuilder();

					using (MySqlCommand getMiniGameUserDataCommand = mysqlConnection.CreateCommand())
					{
						if (!String.IsNullOrEmpty(accountId))
						{
							sqlOptions.Append("AND MiniGameUserData.AccountId=@AccountId ");
							getMiniGameUserDataCommand.Parameters.AddWithValue("@AccountId", accountId);
						}
						if (!String.IsNullOrEmpty(miniGameId))
						{
							sqlOptions.Append("AND MiniGames.MiniGameId = @MiniGameId ");
							getMiniGameUserDataCommand.Parameters.AddWithValue("@MiniGameId", miniGameId);
						}
						if (!String.IsNullOrEmpty(miniGameName))
						{
							sqlOptions.Append("AND MiniGames.MiniGameName = @MiniGameName ");
							getMiniGameUserDataCommand.Parameters.AddWithValue("@MiniGameName", miniGameName);
						}

						if (dataKey != null)
						{
							if (accountId == "")
							{
								accountId = "-1";
							}
							sqlOptions.Append("AND MiniGameUserData.DataKey=@DataKey ");
							getMiniGameUserDataCommand.Parameters.AddWithValue("@DataKey", dataKey);

						}
						else if (dataKeyCsvList != null)
						{
							string csvList = "";
							if (dataKeyCsvList != "")
							{
								string[] dataKeys = dataKeyCsvList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
								string delimiter = "";
								foreach (string s in dataKeys)
								{

									csvList += delimiter + "'" + s + "'";
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
							sqlOptions.Append("AND MiniGameUserData.DataKey IN ( " + csvList + " ) ");
						}

						//we join accounts here because i don't know if we want the account Id sent down at some point.
						string getMiniGameUserDataQuery = "SELECT * FROM MiniGames " +
														"LEFT JOIN MiniGameUserData ON (MiniGames.MiniGameId = MiniGameUserData.MiniGameId) " +
														"WHERE 1 " + sqlOptions.ToString();

						getMiniGameUserDataQuery += " ORDER BY MiniGames.MiniGameId, MiniGameUserData.AccountId ";
						getMiniGameUserDataCommand.CommandText = getMiniGameUserDataQuery;
						using (MySqlDataReader getMiniGameUserDataReader = getMiniGameUserDataCommand.ExecuteReader())
						{
							while (getMiniGameUserDataReader.Read())
							{
								XElement dataKeyElement = new XElement("DataKey");
								dataKeyElement.SetAttributeValue("AccountId", getMiniGameUserDataReader["AccountId"].ToString());
								dataKeyElement.SetAttributeValue("KeyName", getMiniGameUserDataReader["DataKey"].ToString());
								dataKeyElement.SetAttributeValue("UpdatedOn", getMiniGameUserDataReader["UpdatedOn"].ToString());
								dataKeyElement.Value = getMiniGameUserDataReader["DataValue"].ToString();
								
								if(!gameList.ContainsKey(getMiniGameUserDataReader["MiniGameId"].ToString()))
								{
									gameList.Add(getMiniGameUserDataReader["MiniGameId"].ToString(),getMiniGameUserDataReader["MiniGameName"].ToString());
								}
								if (!userDataList.ContainsKey(getMiniGameUserDataReader["MiniGameId"].ToString()))
								{
									List<XElement> newList = new List<XElement>();
									newList.Add(dataKeyElement);
									userDataList.Add(getMiniGameUserDataReader["MiniGameId"].ToString(), newList);
								} else
								{
									userDataList[getMiniGameUserDataReader["MiniGameId"].ToString()].Add(dataKeyElement);
								}
								
							}
						}
					}
					foreach (KeyValuePair<string, string> gamePair in gameList)
					{
						xmlBuilder.AppendFormat("<MiniGame MiniGameId='{0}' MiniGameName='{1}'>",
										gamePair.Key,
										gamePair.Value);
						foreach (XElement element in userDataList[gamePair.Key])
						{
							xmlBuilder.Append(element.ToString());
						}
						xmlBuilder.Append("</MiniGame>");
					}
				}
				return new SimpleResponse("MiniGameUserData", xmlBuilder.ToString());
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Use To store a single key/value pair per user you wish to update again and again or only once
		/// This creates a unique relationship between an ACCOUNT, MINIGAME, and DATAKEY .. saving it's value and then 
		/// overwriting the value on each following save
		/// </summary>
		/// <param name="indexId"></param>
		/// <param name="accountId"></param>
		/// <param name="miniGameId"></param>
		/// <param name="miniGameName"></param>
		/// <param name="dataKey"></param>
		/// <param name="dataValue"></param>
		/// <returns></returns>
		public SimpleResponse SaveMiniGameUserData(string accountId, string miniGameId, 
			string miniGameName, string dataKey, string dataValue)
		{
			mServiceLog.Log.InfoFormat("SaveMiniGameUserData: accountId={0}, miniGameId={1}, miniGameName={2}, dataKey={3}, dataValue={4}",
							accountId, miniGameId, miniGameName, dataKey, dataValue);

			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.MiniGameDBConnectionString))
			{
				mysqlConnection.Open();

				bool foundKey = false;
				string selectMiniGameuserDataQuery = "SELECT MiniGameId FROM MiniGameUserData " +
					"WHERE AccountId=@AccountId AND MiniGameId=@MiniGameId AND DataKey=@DataKey";

				string updateMiniGameUserDataQuery = "UPDATE MiniGameUserData SET DataValue=@DataValue " +
					"WHERE AccountId=@AccountId AND MiniGameId=@MiniGameId AND DataKey=@DataKey;";

				string insertMiniGameUserDataQuery = "INSERT INTO MiniGameUserData "+
					"(AccountId, MiniGameId, DataKey, DataValue) " +
					"VALUES (@AccountId, @MiniGameId, @DataKey, @DataValue ) ";

				using (MySqlCommand selectMiniGameUserDataCommand = mysqlConnection.CreateCommand())
				{
					try
					{
						selectMiniGameUserDataCommand.CommandText = selectMiniGameuserDataQuery;
						selectMiniGameUserDataCommand.Parameters.AddWithValue("@AccountId", accountId);
						selectMiniGameUserDataCommand.Parameters.AddWithValue("@MiniGameId", miniGameId);
						selectMiniGameUserDataCommand.Parameters.AddWithValue("@DataKey", dataKey);
						object obj = selectMiniGameUserDataCommand.ExecuteScalar();
						if(obj != null)
						{
							int gameId = Convert.ToInt32(obj.ToString());
							if(gameId > 0)
							{
								foundKey = true;
							}
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
				string insertOrUpdateQuery = insertMiniGameUserDataQuery;
				if(foundKey)
				{
					insertOrUpdateQuery = updateMiniGameUserDataQuery;
				}
				using (MySqlCommand updateMiniGameUserDataCommand = mysqlConnection.CreateCommand())
				{
					try
					{
						updateMiniGameUserDataCommand.CommandText = insertOrUpdateQuery;
						updateMiniGameUserDataCommand.Parameters.AddWithValue("@AccountId", accountId);
						updateMiniGameUserDataCommand.Parameters.AddWithValue("@MiniGameId", miniGameId);
						updateMiniGameUserDataCommand.Parameters.AddWithValue("@DataKey", dataKey);
						updateMiniGameUserDataCommand.Parameters.AddWithValue("@DataValue", dataValue);
						updateMiniGameUserDataCommand.ExecuteNonQuery();
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
				return new SimpleResponse("Success", "true");
			}
		}

	}
}
