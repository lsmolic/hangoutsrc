using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hangout.Server;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Xml.Linq;
using System.Data;

namespace Hangout.Server.WebServices
{
	public class BossServer : ServiceBaseClass
	{
		private ServicesLog mServiceLog = new ServicesLog("BossServer");

		/// <summary>
		/// Returns the stateserver with the lowest amount of connections and is Enabled
		/// </summary>
		/// <returns></returns>
		public SimpleResponse GetStateServer()
		{
			XElement stateServer = new XElement("StateServer");
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.BossServerDBConnectionString))
			{
				mysqlConnection.Open();

				StringBuilder sqlOptions = new StringBuilder();

				if(String.IsNullOrEmpty(WebConfig.StateServerDesiredPopulationCap))
				{
					sqlOptions.Append(" AND Population <= 300");
				}
				else
				{
					sqlOptions.Append(" AND Population <= '" + WebConfig.StateServerDesiredPopulationCap + "' ");
				}

				string getStateServersQuery = "SELECT * FROM StateServers WHERE IsEnabled='1' AND LastUpdated > ( select unix_timestamp(current_date - interval 10 minute) ) " + sqlOptions.ToString() + " ORDER BY Population DESC;";

				using (MySqlCommand getStateServersCommand = mysqlConnection.CreateCommand())
				{
					getStateServersCommand.CommandText = getStateServersQuery;
					using (MySqlDataReader getStateServersReader = getStateServersCommand.ExecuteReader(CommandBehavior.SingleRow))
					{
						while (getStateServersReader.Read())
						{

							stateServer.SetAttributeValue("Id", getStateServersReader["Id"].ToString());
							stateServer.SetAttributeValue("Ip", getStateServersReader["StateServerIp"].ToString());
							stateServer.SetAttributeValue("Port", getStateServersReader["StateServerPort"].ToString());
							stateServer.SetAttributeValue("Population", getStateServersReader["Population"].ToString());
						}
					}
				}
			}
			return new SimpleResponse("ConnectTo", stateServer.ToString());
		}

		/// <summary>
		/// Removes the entries from UserTracking table for the
		/// particular StateServerIp
		/// IsEnabled is an optional parameter - defaults to 'online'
		/// </summary>
		/// <param name="stateServerIp"></param>
		/// <param name="isEnabled"></param>
		/// <returns></returns>
		public SimpleResponse InitStateServer(string stateServerIp, string port, string isEnabled)
		{
			string stateServerId = "0";
			try
			{

				if (String.IsNullOrEmpty(isEnabled))
				{
					isEnabled = "1";
				}
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.BossServerDBConnectionString))
				{
					mysqlConnection.Open();

					
					string sqlOptions = "";
					if(!String.IsNullOrEmpty(port))
					{
						sqlOptions += " AND StateServerPort=@StateServerPort ";
					}
					string updateStateServersQuery = "DELETE FROM UserTracking " +
						"WHERE StateServerId=(SELECT Id FROM StateServers WHERE StateServerIp=@StateServerIp " + sqlOptions + ");" +
						"UPDATE StateServers SET Population='0' WHERE StateServerIp=@StateServerIp " + sqlOptions + "; " +
						"SELECT Id FROM StateServers WHERE StateServerIp=@StateServerIp " + sqlOptions + ";";

					using (MySqlCommand updateStateServersCommand = mysqlConnection.CreateCommand())
					{
						updateStateServersCommand.Parameters.AddWithValue("@StateServerIp", stateServerIp);
						updateStateServersCommand.Parameters.AddWithValue("@StateServerPort", port);
						updateStateServersCommand.CommandText = updateStateServersQuery;
						object o = updateStateServersCommand.ExecuteScalar();
						if( o != null)
						{
							stateServerId = o.ToString();
						}
					}
				}
				return new SimpleResponse("StateServerId", stateServerId);
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

		public SimpleResponse UpdateStateServer(string stateServerId, string population, string isEnabled)
		{
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.BossServerDBConnectionString))
			{
				mysqlConnection.Open();


				StringBuilder additionalFields = new StringBuilder();
				string updateStateServersQuery = "";

				using (MySqlCommand updateStateServersCommand = mysqlConnection.CreateCommand())
				{
					if (!String.IsNullOrEmpty(isEnabled))
					{
						additionalFields.Append(", IsEnabled=@IsEnabled ");
						updateStateServersCommand.Parameters.AddWithValue("@IsEnabled", isEnabled);
					}

					updateStateServersQuery += "UPDATE StateServers SET Population=@Population, LastUpdated=UTC_TIMESTAMP() " + additionalFields + " WHERE Id=@StateServerId;";
					updateStateServersCommand.Parameters.AddWithValue("@Population", population);
					updateStateServersCommand.Parameters.AddWithValue("@StateServerId", stateServerId);
					updateStateServersCommand.CommandText = updateStateServersQuery;
					updateStateServersCommand.ExecuteNonQuery();
				}
			}
			return new SimpleResponse("success", "true");
		}

		public SimpleResponse GetUserConnections(string accountId, string accountIdCsvList, 
												string sessionId, string sessionIdCsvList, 
												string status, string statusCsvList, 
												string zone, string zoneCsvList, 
												string stateServerId, string stateServerIdCsvList)
		{
			StringBuilder userConnectionsXml = new StringBuilder();
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.BossServerDBConnectionString))
			{
				mysqlConnection.Open();

				StringBuilder sqlOptions = new StringBuilder();
				using (MySqlCommand getUserConnectionsCommand = mysqlConnection.CreateCommand())
				{
					 
					if (accountId != null)
					{
						if (accountId == "")
						{
							accountId = "-1";
						}
						sqlOptions.Append(" AND AccountId=@AccountId ");
						getUserConnectionsCommand.Parameters.AddWithValue("@AccountId", accountId.ToString());

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
						sqlOptions.Append(" AND AccountId IN ( " + csvList + " ) ");
					}

					if (sessionId != null)
					{
						if (sessionId == "")
						{
							throw new ArgumentOutOfRangeException("Invalid GUID");
						}
						sqlOptions.Append(" AND SessionId=@SessionId ");
						getUserConnectionsCommand.Parameters.AddWithValue("@SessionId", sessionId.ToString());

					}
					else if (sessionIdCsvList != null)
					{
						string csvList = "";
						if (sessionIdCsvList != "")
						{
							string[] sessionIds = sessionIdCsvList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							string delimiter = "";
							foreach (string s in sessionIds)
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
						sqlOptions.Append(" AND SessionId IN ( " + csvList + " ) ");
					}

					if (status != null)
					{
						if (status == "")
						{
							status = "-1";
						}
						sqlOptions.Append(" AND Status=@Status ");
						getUserConnectionsCommand.Parameters.AddWithValue("@Status", status);

					}
					else if (statusCsvList != null)
					{
						string csvList = "";
						if (statusCsvList != "")
						{
							string[] statuses = statusCsvList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							string delimiter = "";
							foreach (string s in statuses)
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
						sqlOptions.Append(" AND SessionId IN ( " + csvList + " ) ");
					}

					if (zone != null)
					{
						if (zone == "")
						{
							zone = "-1";
						}
						sqlOptions.Append(" AND Zone=@Zone ");
						getUserConnectionsCommand.Parameters.AddWithValue("@Zone", zone);

					}
					else if (zoneCsvList != null)
					{
						string csvList = "";
						if (zoneCsvList != "")
						{
							string[] zones = zoneCsvList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							string delimiter = "";
							foreach (string s in zones)
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
						sqlOptions.Append(" AND Zone IN ( " + csvList + " ) ");
					}

					if (stateServerId != null)
					{
						if (stateServerId == "")
						{
							stateServerId = "-1";
						}
						sqlOptions.Append(" AND StateServerId=@StateServerId ");
						getUserConnectionsCommand.Parameters.AddWithValue("@StateServerId", stateServerId);

					}
					else if (stateServerIdCsvList != null)
					{
						string csvList = "";
						if (stateServerIdCsvList != "")
						{
							string[] stateServers = stateServerIdCsvList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							string delimiter = "";
							foreach (string s in stateServers)
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
						sqlOptions.Append(" AND StateServerId IN ( " + csvList + " ) ");
					}


					string getUsersQuery = "SELECT * FROM UserTracking";
					getUsersQuery += " WHERE 1 " + sqlOptions + " ORDER BY status ASC;";
					getUserConnectionsCommand.CommandText = getUsersQuery;
					using (MySqlDataReader getUserConnectionsReader = getUserConnectionsCommand.ExecuteReader())
					{
						while (getUserConnectionsReader.Read())
						{
							XElement userConnection = new XElement("Connection");
							userConnection.SetAttributeValue("ConnectionId", getUserConnectionsReader["ConnectionId"].ToString());
							userConnection.SetAttributeValue("AccountId", getUserConnectionsReader["AccountId"].ToString());
							userConnection.SetAttributeValue("SessionId", getUserConnectionsReader["SessionId"].ToString());
							userConnection.SetAttributeValue("Status", getUserConnectionsReader["Status"].ToString());
							userConnection.SetAttributeValue("Zone", getUserConnectionsReader["Zone"].ToString());
							userConnection.SetAttributeValue("StateServerId", getUserConnectionsReader["StateServerId"].ToString());

							userConnectionsXml.Append(userConnection.ToString());

						}
					}
				}
			}
			return new SimpleResponse("UserConnections", userConnectionsXml.ToString());
		}

		public SimpleResponse RegisterNewSession(string accountId, string sessionId, string status, string zone, string stateServerId)
		{
			RemoveSession(accountId, null);
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.BossServerDBConnectionString))
			{
				mysqlConnection.Open();

				string registerNewSessionQuery = "INSERT INTO UserTracking (AccountId,SessionId,Status,Zone,StateServerId) " +
					"VALUES (@AccountId, @SessionId, @Status, @Zone, @StateServerId);";

				using (MySqlCommand registerNewSessionCommand = mysqlConnection.CreateCommand())
				{
					registerNewSessionCommand.Parameters.AddWithValue("@AccountId", accountId);
					registerNewSessionCommand.Parameters.AddWithValue("@SessionId", sessionId);
					registerNewSessionCommand.Parameters.AddWithValue("@Status", status);
					registerNewSessionCommand.Parameters.AddWithValue("@Zone", zone);
					registerNewSessionCommand.Parameters.AddWithValue("@StateServerId", stateServerId);

					registerNewSessionCommand.CommandText = registerNewSessionQuery;
					registerNewSessionCommand.ExecuteNonQuery();
				}
			}
			return new SimpleResponse("Success", "true");
		}

		public SimpleResponse UpdateSession(string accountId, string sessionId, string status, string zone, string stateServerId)
		{
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.BossServerDBConnectionString))
			{
				mysqlConnection.Open();

				string updateSessionQuery = "UPDATE UserTracking SET Status=@Status,Zone=@Zone,StateServerId=@StateServerId " + 
					"WHERE SessionId=@SessionId AND AccountId=@AccountId;";

				using (MySqlCommand updateSessionCommand = mysqlConnection.CreateCommand())
				{
					updateSessionCommand.Parameters.AddWithValue("@AccountId", accountId);
					updateSessionCommand.Parameters.AddWithValue("@SessionId", sessionId);
					updateSessionCommand.Parameters.AddWithValue("@Status", status);
					updateSessionCommand.Parameters.AddWithValue("@Zone", zone);
					updateSessionCommand.Parameters.AddWithValue("@StateServerId", stateServerId);

					updateSessionCommand.CommandText = updateSessionQuery;
					updateSessionCommand.ExecuteNonQuery();
				}
			}
			return new SimpleResponse("Success", "true");
		}

		public SimpleResponse RemoveSession(string accountId, string sessionId)
		{
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.BossServerDBConnectionString))
			{
				mysqlConnection.Open();

				string removeSessionQuery = "DELETE FROM UserTracking WHERE SessionId=@SessionId OR AccountId=@AccountId;";

				using (MySqlCommand removeSessionCommand = mysqlConnection.CreateCommand())
				{
					removeSessionCommand.Parameters.AddWithValue("@AccountId", accountId);
					removeSessionCommand.Parameters.AddWithValue("@SessionId", sessionId);

					removeSessionCommand.CommandText = removeSessionQuery;
					removeSessionCommand.ExecuteNonQuery();
				}
			}
			return new SimpleResponse("Success", "true");
		}
	}

}
