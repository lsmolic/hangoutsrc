using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Hangout.Shared;
using System.IO;

namespace Hangout.Server.WebServices
{
	public class LoggingService : ServiceBaseClass
	{
		private ServicesLog mServiceLog = new ServicesLog("LoggingService");

		public SimpleResponse LogMetric(string categoryName, string eventName, string subEvent, string eventData, string accountId)
		{
			WriteLogToDb(categoryName, eventName, subEvent, eventData, accountId);

			return new SimpleResponse("Success", "true");
		}
		public SimpleResponse LogBlobMetrics(string blobLog, HangoutPostedFile logFile)
		{
			if(!String.IsNullOrEmpty(blobLog))
			{
				WriteMultipleLogsToDb(blobLog);
				return new SimpleResponse("Success", "false");
			} 
			else if (logFile != null)
			{
				blobLog = "";

				MemoryStream filestream = (MemoryStream)logFile.InputStream;

				byte[] logArray = filestream.ToArray();

				if (logArray != null && logArray.Length > 0)
				{
					blobLog = Encoding.ASCII.GetString(logArray);

					WriteMultipleLogsToDb(blobLog);
				}
				
				return new SimpleResponse("Success", "false");
			}
			

			return new SimpleResponse("Success", "false");
		}

		public SimpleResponse GetApproveDenyWordList()
		{
			StringBuilder xmlBuilder = new StringBuilder();
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.LoggingDBConnectionString))
			{
				mysqlConnection.Open();

				string getNaughtyWordsQuery = "SELECT * FROM NaughtyWords";

				using (MySqlCommand getNaughtyWordsCommand = mysqlConnection.CreateCommand())
				{
					getNaughtyWordsCommand.CommandText = getNaughtyWordsQuery;
					using (MySqlDataReader getNaughtyWordsReader = getNaughtyWordsCommand.ExecuteReader())
					{
						xmlBuilder.Append("<NaughtyWords>");
						while (getNaughtyWordsReader.Read())
						{
							xmlBuilder.AppendFormat("<No>{0}</No>", getNaughtyWordsReader["WordToReplace"].ToString());
						}
						xmlBuilder.Append("</NaughtyWords>");
					}
				}

				string getNiceWordsQuery = "SELECT * FROM NiceWords";

				using (MySqlCommand getNiceWordsCommand = mysqlConnection.CreateCommand())
				{
					getNiceWordsCommand.CommandText = getNiceWordsQuery;
					using (MySqlDataReader getNiceWordsReader = getNiceWordsCommand.ExecuteReader())
					{
						xmlBuilder.Append("<NiceWords>");
						while (getNiceWordsReader.Read())
						{
							xmlBuilder.AppendFormat("<Yes>{0}</Yes>", getNiceWordsReader["WordToKeep"].ToString());
						}
						xmlBuilder.Append("</NiceWords>");
					}
				}
			}

			return new SimpleResponse("ApproveDenyWords", xmlBuilder.ToString());
		}

		public SimpleResponse GetNaughtyWords()
		{
			StringBuilder xmlBuilder = new StringBuilder();
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.LoggingDBConnectionString))
			{
				mysqlConnection.Open();

				string getNaughtyWordsQuery = "SELECT * FROM NaughtyWords";

				using (MySqlCommand getNaughtyWordsCommand = mysqlConnection.CreateCommand())
				{
					getNaughtyWordsCommand.CommandText = getNaughtyWordsQuery;
					using (MySqlDataReader getNaughtyWordsReader = getNaughtyWordsCommand.ExecuteReader())
					{
						while(getNaughtyWordsReader.Read())
						{
							xmlBuilder.AppendFormat("<No>{0}</No>", getNaughtyWordsReader["WordToReplace"].ToString());
						}
					}
				}
			}

			return new SimpleResponse("NaughtyWords", xmlBuilder.ToString());
		}
		
		private void WriteLogToDb(string categoryName, string eventName, string subEvent, string eventData, string accountId)
		{
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.LoggingDBConnectionString))
			{
				mysqlConnection.Open();
				string getAccountForUserQuery = "INSERT INTO GeneralEmetrics ( Category , EventName , SubEvent , EventData, HangoutAccountId, TimeStamp ) " +
					"VALUES ( @Category , @EventName , @SubEvent , @EventData, @HangoutAccountId, UTC_TIMESTAMP() )";
				using (MySqlCommand getAccountCommand = mysqlConnection.CreateCommand())
				{
					if (String.IsNullOrEmpty(categoryName))
					{
						categoryName = "";
					}
					if (String.IsNullOrEmpty(eventName))
					{
						eventName = "";
					}
					if (String.IsNullOrEmpty(subEvent))
					{
						subEvent = "";
					}
					if (String.IsNullOrEmpty(eventData))
					{
						eventData = "";
					}
					if (String.IsNullOrEmpty(accountId))
					{
						accountId = "";
					}
					getAccountCommand.Parameters.AddWithValue("@Category", categoryName);
					getAccountCommand.Parameters.AddWithValue("@EventName", eventName);
					getAccountCommand.Parameters.AddWithValue("@SubEvent", subEvent);
					getAccountCommand.Parameters.AddWithValue("@EventData", eventData);
					getAccountCommand.Parameters.AddWithValue("@HangoutAccountId", accountId);

					try
					{
						getAccountCommand.CommandText = getAccountForUserQuery;
						getAccountCommand.ExecuteNonQuery();
					}
					catch (System.Exception ex)
					{
						throw new Exception("TERRIBLE THINGS ON UPLOAD!!!" + ex.Message + ex.InnerException);
					}
					
					
				}
			}
		}

		private void WriteMultipleLogsToDb(string blobLog)
		{
			List<string> KAZAKHISTAN = new List<string>();

			string[] insertDelimiter = new string[1];
			insertDelimiter[0] = "||";
			if (!String.IsNullOrEmpty(blobLog))
			{
				string[] KURDISTAN = blobLog.Split(insertDelimiter, StringSplitOptions.RemoveEmptyEntries);
				KAZAKHISTAN = KURDISTAN.ToList<string>();
			}
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.LoggingDBConnectionString))
			{
				mysqlConnection.Open();

				try
				{
					using (MySqlCommand getAccountCommand = mysqlConnection.CreateCommand())
					{
						
						string logMetricsQuery = "INSERT INTO GeneralEmetrics ( Category , EventName , SubEvent , EventData, HangoutAccountId, TimeStamp ) VALUES ";
						string queryDelimiter = "";
						int replaceIndex = 0;
						foreach (string log in KAZAKHISTAN)
						{
							//string[] paramDelimiter = new string[1];
							//paramDelimiter[0] = "&";
	
							string[] UZBECKISTAN;
							UZBECKISTAN = log.Split(new char[] { '&' }, StringSplitOptions.None);

							string categoryName = "";
							string eventName = "";
							string subEvent = "";
							string eventData = "";
							string accountId = "";
							string timeStamp = "";

							if (UZBECKISTAN.Length > 0 &&!String.IsNullOrEmpty(UZBECKISTAN[0]))
							{
								categoryName = UZBECKISTAN[0].ToString();
							}
							if (UZBECKISTAN.Length > 1 && !String.IsNullOrEmpty(UZBECKISTAN[1]))
							{
								eventName = UZBECKISTAN[1];
							}
							if (UZBECKISTAN.Length > 2 && !String.IsNullOrEmpty(UZBECKISTAN[2]))
							{
								subEvent = UZBECKISTAN[2];
							}
							if (UZBECKISTAN.Length > 3 && !String.IsNullOrEmpty(UZBECKISTAN[3]))
							{
								eventData = UZBECKISTAN[3];
							}
							if (UZBECKISTAN.Length > 4 && !String.IsNullOrEmpty(UZBECKISTAN[4]))
							{
								accountId = UZBECKISTAN[4];
							}
							if (UZBECKISTAN.Length > 5 && !String.IsNullOrEmpty(UZBECKISTAN[5]))
							{
								timeStamp = UZBECKISTAN[5];
							}


							getAccountCommand.Parameters.AddWithValue("@Category" + replaceIndex.ToString(), categoryName);
							getAccountCommand.Parameters.AddWithValue("@EventName" + replaceIndex.ToString(), eventName);
							getAccountCommand.Parameters.AddWithValue("@SubEvent" + replaceIndex.ToString(), subEvent);
							getAccountCommand.Parameters.AddWithValue("@EventData" + replaceIndex.ToString(), eventData);
							getAccountCommand.Parameters.AddWithValue("@HangoutAccountId" + replaceIndex.ToString(), accountId);
							getAccountCommand.Parameters.AddWithValue("@TimeStamp" + replaceIndex.ToString(), timeStamp);
							logMetricsQuery += queryDelimiter + "( @Category" + replaceIndex.ToString() + 
																" , @EventName" + replaceIndex.ToString() + 
																" , @SubEvent" + replaceIndex.ToString() + 
																" , @EventData" + replaceIndex.ToString() +
																" , @HangoutAccountId" + replaceIndex.ToString() +
																" , @TimeStamp" + replaceIndex.ToString() + " ) ";
							replaceIndex++;
							queryDelimiter = ",";
						}
	
						getAccountCommand.CommandText = logMetricsQuery;
						getAccountCommand.ExecuteNonQuery();
	
					}
				}
				catch (MySqlException myEx)
				{
					StringBuilder errorMessages = new StringBuilder();

					errorMessages.Append("Error Number:" + myEx.Number + "\n" +
						"ErrorCode: " + myEx.ErrorCode + "\n" +
						"InnerException: " + myEx.InnerException + "\n" +
						"Message: " + myEx.Message + "\n" +
						"Method: " + myEx.TargetSite + "\n" +
						"StackTrace: " + myEx.StackTrace + "\n");

					mServiceLog.Log.Error(errorMessages.ToString());
					throw myEx;
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
		}
	}
}
