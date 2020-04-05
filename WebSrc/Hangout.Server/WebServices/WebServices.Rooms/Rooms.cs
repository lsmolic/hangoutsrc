using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using Hangout.Server;
using System.Xml;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using System.Data.SqlClient;



namespace Hangout.Server.WebServices
{
    public class Rooms : ServiceBaseClass
	{
		private ServicesLog mServiceLog = new ServicesLog("Rooms");
		private RoomsUtil mRoomUtil = new RoomsUtil();

		/// <summary>
		/// Returns ALL rooms for an account regardless of disabled or default tags in the DB
		/// </summary>
		/// <param name="accountId"></param>
		/// <returns></returns>
		public SimpleResponse GetRoomsForAccountId(uint accountId)
		{
			mServiceLog.Log.InfoFormat("GetRoomsForAccountId: accountId={0}", accountId);
			StringBuilder xmlBuilder = new StringBuilder();
			try
			{
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.RoomsDBConnectionString))
				{
					mysqlConnection.Open();
					string getRoomsForAccountQuery = "SELECT * FROM AccountToRoomMapping " +
													"LEFT JOIN Rooms ON (AccountToRoomMapping.RoomId = Rooms.RoomId) " +
													"WHERE AccountToRoomMapping.HangoutAccountId='" + accountId + "' " +
													"ORDER BY AccountToRoomMapping.RoomId;";

					List<XElement> roomList = new List<XElement>();
					using (MySqlCommand getRoomsCommand = mysqlConnection.CreateCommand())
					{
						getRoomsCommand.CommandText = getRoomsForAccountQuery;
						using (MySqlDataReader getRoomsReader = getRoomsCommand.ExecuteReader())
						{
							while (getRoomsReader.Read())
							{
								xmlBuilder.AppendFormat("<Room RoomId='{0}' AccountId='{1}' IsEnabled='{2}'>",
										getRoomsReader["RoomId"].ToString(),
										getRoomsReader["HangoutAccountId"].ToString(), 
										getRoomsReader["IsEnabled"].ToString());
								xmlBuilder.Append(getRoomsReader["RoomDNA"].ToString());
								xmlBuilder.Append("</Room>");
							}
						}
					}


				}
				return new SimpleResponse("Rooms", xmlBuilder.ToString());
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// returns a list of public rooms
		/// an optional parameters include: 
		/// -accountId to specify the public rooms for only one user
		/// -privacyLevel 
		/// -
		/// </summary>
		/// <param name="accountId"></param>
		/// <returns></returns>
		public SimpleResponse GetRooms(string accountId, string accountIdCsvList, string privacyLevel, string isDefault, string roomId)
		{
			mServiceLog.Log.InfoFormat("GetRooms: accountId={0}, accountIdCsvList={1}, privacyLevel={2}, isDefault={3}, roomId={4}",
				accountId, accountIdCsvList, privacyLevel, isDefault, roomId);
			StringBuilder sqlOptionsRooms = new StringBuilder();
			StringBuilder sqlOptionsSystemRooms = new StringBuilder();

			StringBuilder xmlBuilder = new StringBuilder();
			try
			{
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.RoomsDBConnectionString))
				{
					mysqlConnection.Open();
					string getRoomsForAccountQuery = "";
					string getSystemRoomsForAccountQuery = "";
					bool isUserAccount = false; //this is set if something needs to return something from 


					List<XElement> roomList = new List<XElement>();
					MySqlCommand getRoomsCommand = mysqlConnection.CreateCommand();
					MySqlCommand getSystemRoomsCommand = mysqlConnection.CreateCommand();
					

						if (accountId != null)
						{
							if (accountId == "")
							{
								accountId = "-1";
							}
							sqlOptionsRooms.Append("AND HangoutAccountId=@AccountId ");
							getRoomsCommand.Parameters.AddWithValue("@AccountId", accountId);
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
							sqlOptionsRooms.Append("AND HangoutAccountId IN ( " + csvList + " ) ");
							isUserAccount = true;
						}

						if (!String.IsNullOrEmpty(privacyLevel))
						{
							sqlOptionsSystemRooms.Append("AND SystemRooms.PrivacyLevel=@PrivacyLevel ");
							getSystemRoomsCommand.Parameters.AddWithValue("@PrivacyLevel", privacyLevel);
							sqlOptionsRooms.Append("AND Rooms.PrivacyLevel=@PrivacyLevel ");
							getRoomsCommand.Parameters.AddWithValue("@PrivacyLevel", privacyLevel);
						}
						if (!String.IsNullOrEmpty(isDefault))
						{
							if (isDefault == "0" || isDefault == "1")
							{
								sqlOptionsRooms.Append("AND AccountToRoomMapping.IsDefault=@IsDefault ");
								getRoomsCommand.Parameters.AddWithValue("@IsDefault", isDefault);
							}
						}
						if (roomId != null)
						{
							if (roomId == "")
							{
								roomId = "-1";
							}
							sqlOptionsSystemRooms.Append("AND SystemRooms.RoomId=@RoomId ");
							getSystemRoomsCommand.Parameters.AddWithValue("@RoomId", roomId);
							sqlOptionsRooms.Append("AND Rooms.RoomId=@RoomId ");
							getRoomsCommand.Parameters.AddWithValue("@RoomId", roomId);
						}
						getSystemRoomsForAccountQuery += "SELECT * FROM SystemRooms " +
													"WHERE 1 " + sqlOptionsSystemRooms.ToString() + "ORDER BY RoomId ";

						getSystemRoomsCommand.CommandText = getSystemRoomsForAccountQuery;
						if(!isUserAccount)
						{
							using (MySqlDataReader getSystemRoomsReader = getSystemRoomsCommand.ExecuteReader())
							{
								while (getSystemRoomsReader.Read())
								{
									xmlBuilder.AppendFormat(FormatRoomData(
											getSystemRoomsReader["RoomId"].ToString(),
											"1",
											"1",
											getSystemRoomsReader["IsEnabled"].ToString(),
											getSystemRoomsReader["PrivacyLevel"].ToString(),
											getSystemRoomsReader["RoomDNA"].ToString()
									));
								}
							}
						}
						
						getSystemRoomsCommand.Dispose();
						
						getRoomsForAccountQuery += "SELECT * FROM Rooms " +
													"LEFT JOIN AccountToRoomMapping ON ( AccountToRoomMapping.RoomId = Rooms.RoomId ) " +
													"WHERE 1 " + sqlOptionsRooms.ToString() + "ORDER BY Rooms.RoomId ";

						getRoomsCommand.CommandText = getRoomsForAccountQuery;
						using (MySqlDataReader getRoomsReader = getRoomsCommand.ExecuteReader())
						{
							while (getRoomsReader.Read())
							{
								xmlBuilder.AppendFormat(FormatRoomData(
										getRoomsReader["RoomId"].ToString(),
										getRoomsReader["HangoutAccountId"].ToString(),
										getRoomsReader["IsDefault"].ToString(),
										getRoomsReader["IsEnabled"].ToString(),
										getRoomsReader["PrivacyLevel"].ToString(),
										getRoomsReader["RoomDNA"].ToString()
								));
							}
						}
						getRoomsCommand.Dispose();

				}
				return new SimpleResponse("Rooms", xmlBuilder.ToString());
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		public SimpleResponse GetSystemRooms(string roomId, string isEnabled)
		{
			mServiceLog.Log.InfoFormat("GetSystemRooms: roomId={0}, isEnabled={1}", roomId, isEnabled);
			StringBuilder sqlOptions = new StringBuilder();

			StringBuilder xmlBuilder = new StringBuilder();
			try
			{
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.RoomsDBConnectionString))
				{
					mysqlConnection.Open();
					string getRoomsForAccountQuery = "SELECT * FROM SystemRooms " +
													"WHERE 1 ";


					List<XElement> roomList = new List<XElement>();
					using (MySqlCommand getRoomsCommand = mysqlConnection.CreateCommand())
					{
						if (!String.IsNullOrEmpty(roomId))
						{
							sqlOptions.Append("AND RoomId=@RoomId ");
							getRoomsCommand.Parameters.AddWithValue("@RoomId", roomId);
						}
						if (!String.IsNullOrEmpty(isEnabled))
						{
							string boolValue = "0";
							if (isEnabled == "1" || isEnabled.ToLower() == "true")
							{
								boolValue = "1";
							}
							sqlOptions.Append("AND SystemRooms.IsEnabled=@IsEnabled ");
							getRoomsCommand.Parameters.AddWithValue("@IsEnabled", boolValue);
						}

						getRoomsForAccountQuery += sqlOptions.ToString();
						getRoomsForAccountQuery += "ORDER BY RoomId; ";

						getRoomsCommand.CommandText = getRoomsForAccountQuery;
						using (MySqlDataReader getRoomsReader = getRoomsCommand.ExecuteReader())
						{
							while (getRoomsReader.Read())
							{
								xmlBuilder.AppendFormat(FormatRoomData(
										getRoomsReader["RoomId"].ToString(),
										"1",
										"1",
										getRoomsReader["IsEnabled"].ToString(),
										getRoomsReader["PrivacyLevel"].ToString(),
										getRoomsReader["RoomDNA"].ToString()
								));
							}
						}
					}


				}
				return new SimpleResponse("Rooms", xmlBuilder.ToString());
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		private string FormatRoomData(string roomId, string accountId, string isDefault, string isEnabled, string privacyLevel, string roomDNA)
		{
			StringBuilder xmlBuilder = new StringBuilder();
			xmlBuilder.AppendFormat("<Room RoomId=\"{0}\" AccountId=\"{1}\" IsDefault=\"{2}\" IsEnabled=\"{3}\" PrivacyLevel=\"{4}\">",
								roomId,
								accountId,
								isDefault,
								isEnabled,
								privacyLevel
								);

			xmlBuilder.Append(roomDNA);
			xmlBuilder.Append("</Room>");

			return xmlBuilder.ToString();
		}

		/// <summary>
		/// Returns a room marked as default in the AccountToRoomMapping Table. 
		/// This will return multiple rooms if the DB has more than one marked as default
		/// Set Default should fix that by marking all other rooms as not default.
		/// </summary>
		/// <param name="accountId"></param>
		/// <returns></returns>
		public SimpleResponse GetDefaultRoom(string accountId)
		{
			mServiceLog.Log.InfoFormat("GetDefaultRoom: accountId={0}", accountId);
			StringBuilder xmlBuilder = new StringBuilder();
			try
			{
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.RoomsDBConnectionString))
				{
					mysqlConnection.Open();

					//we join accounts here because i don't know if we want the account Id sent down at some point.
					string getDefaultRoomQuery = "SELECT * FROM AccountToRoomMapping " +
													"LEFT JOIN Rooms ON (AccountToRoomMapping.RoomId = Rooms.RoomId) " +
													"WHERE AccountToRoomMapping.IsDefault=1 AND HangoutAccountId = @AccountId; ";

					List<XElement> roomList = new List<XElement>();
					using (MySqlCommand getDefaultRoomCommand = mysqlConnection.CreateCommand())
					{
						getDefaultRoomCommand.Parameters.AddWithValue("@AccountId", accountId);
						getDefaultRoomCommand.CommandText = getDefaultRoomQuery;
						using (MySqlDataReader getDefaultRoomReader = getDefaultRoomCommand.ExecuteReader())
						{
							while (getDefaultRoomReader.Read())
							{
								xmlBuilder.AppendFormat("<Room RoomId='{0}' AccountId='{1}' IsEnabled='{2}' PrivacyLevel='{3}'>",
										getDefaultRoomReader["RoomId"].ToString(),
										accountId,
										getDefaultRoomReader["IsEnabled"].ToString(),
										getDefaultRoomReader["PrivacyLevel"].ToString());
								xmlBuilder.Append(getDefaultRoomReader["RoomDNA"].ToString());
								xmlBuilder.Append("</Room>");
							}
						}
					}


				}
				return new SimpleResponse("DefaultRoom", xmlBuilder.ToString());
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}

		public SimpleResponse SetDefaultRoom(string accountId, string roomId)
		{
			mServiceLog.Log.InfoFormat("GetDefaultRoom: accountId={0}, roomId={1}", accountId, roomId);
			RoomsUtil roomsUtil = new RoomsUtil();
			roomsUtil.SetAllUserRoomsToNotBeDefault(accountId);

			StringBuilder xmlBuilder = new StringBuilder();
			try
			{
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.RoomsDBConnectionString))
				{
					mysqlConnection.Open();

					//we join accounts here because i don't know if we want the account Id sent down at some point.
					string getDefaultRoomQuery = "UPDATE AccountToRoomMapping SET IsDefault = '1' WHERE RoomId=@RoomId;";

					using (MySqlCommand getDefaultRoomCommand = mysqlConnection.CreateCommand())
					{
						getDefaultRoomCommand.Parameters.AddWithValue("@RoomId", roomId);
						getDefaultRoomCommand.CommandText = getDefaultRoomQuery;
						getDefaultRoomCommand.ExecuteNonQuery();
					}
				}
			}
			catch (System.Exception ex)
			{
				throw ex;
			}

			return new SimpleResponse("Success", "true");
		}

		/// <summary>
		/// Overwrites the stored RoomDNA for a given roomId
		/// </summary>
		/// <param name="roomId"></param>
		/// <param name="roomDNA"></param>
		/// <returns></returns>
		public SimpleResponse UpdateRoomDNA(string roomId, string roomDNA)
		{
			mServiceLog.Log.InfoFormat("UpdateRoomDNA: roomId={0}, roomDNA={1}", roomId, roomDNA);
			if (roomId == "0")
			{
				throw new ArgumentOutOfRangeException("roomId");
			}

			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.RoomsDBConnectionString))
			{
				mysqlConnection.Open();
				string table = "Rooms";
				int numRoomId = Convert.ToInt32(roomId);
				if(numRoomId < 1000001)
				{
					table = "SystemRooms";
				}
				string updateRoomDNAQuery = "UPDATE " + table + " SET RoomDNA=@RoomDNA WHERE RoomId=@RoomId;";
				using (MySqlCommand updateRoomDNACommand = mysqlConnection.CreateCommand())
				{
					try
					{
						updateRoomDNACommand.CommandText = updateRoomDNAQuery;
						updateRoomDNACommand.Parameters.AddWithValue("@RoomId", roomId.ToString());
						updateRoomDNACommand.Parameters.AddWithValue("@RoomDNA", roomDNA);
						updateRoomDNACommand.ExecuteNonQuery();
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

		/// <summary>
		/// Creates a new foom for a given accountId and stores the provided
		/// roomDNA in the database for longterm storage
		/// </summary>
		/// <param name="accountId"></param>
		/// <param name="roomDNA"></param>
		/// <returns></returns>
		public SimpleResponse CreateRoom(string accountId, string roomDNA, string privacyLevel, string defaultRoomId, string roomName)
		{
			mServiceLog.Log.InfoFormat("CreateRoom: accountId={0}, roomDNA={1}, privacyLevel={2}, system RoomId to copy={3}, roomName={4}", accountId, roomDNA, privacyLevel, defaultRoomId, roomName);
			string roomIdFromDB = "0";
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.RoomsDBConnectionString))
			{
				mysqlConnection.Open();

				string createRoomQuery = "";
				using (MySqlCommand createRoomCommand = mysqlConnection.CreateCommand())
				{
					try
					{
						string pLevel = "1";
						if (!String.IsNullOrEmpty(privacyLevel))
						{
							pLevel = privacyLevel;
						}

						if (String.IsNullOrEmpty(roomDNA))
						{
							if (String.IsNullOrEmpty(defaultRoomId))
							{
								throw new NullReferenceException("Missing Argument: defaultRoomId");
							}
							if (defaultRoomId == "0")
							{
								defaultRoomId = "1";
							}
							createRoomQuery = "INSERT INTO Rooms ( Rooms.RoomDNA, Rooms.IsEnabled, PrivacyLevel ) " +
								"SELECT SystemRooms.RoomDNA, '1', @privacyLevel FROM SystemRooms WHERE SystemRooms.RoomId=@RoomId; ";
							createRoomCommand.Parameters.AddWithValue("@RoomId", defaultRoomId);
							createRoomCommand.Parameters.AddWithValue("@privacyLevel", pLevel);
						}
						else
						{
							createRoomQuery = "INSERT INTO Rooms ( RoomDNA, IsEnabled, PrivacyLevel ) VALUES (@RoomDNA, '1', @PrivacyLevel); ";
							createRoomCommand.Parameters.AddWithValue("@RoomDNA", roomDNA);
							createRoomCommand.Parameters.AddWithValue("@PrivacyLevel", pLevel);
						}

						createRoomQuery += "SELECT LAST_INSERT_ID();";
						createRoomCommand.CommandText = createRoomQuery;

						object o = createRoomCommand.ExecuteScalar();
						if (o != null)
						{
							roomIdFromDB = o.ToString();
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
				string mapAccountToRoomQuery = "INSERT INTO AccountToRoomMapping ( HangoutAccountId, RoomId, IsDefault ) VALUES ('" + accountId + "','" + roomIdFromDB + "', '1');";
				using (MySqlCommand insertAccountMappingToRoomCommand = mysqlConnection.CreateCommand())
				{
					try
					{
						insertAccountMappingToRoomCommand.CommandText = mapAccountToRoomQuery;
						insertAccountMappingToRoomCommand.ExecuteNonQuery();
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

			
			XmlDocument newRoomXml = GetRooms(null, null, null, null, roomIdFromDB);
			XmlNode roomDnaNode = newRoomXml.SelectSingleNode("/Rooms/Room/RoomDna");
			XmlAttribute roomNameAttr = roomDnaNode.Attributes["RoomName"];
			if(roomNameAttr != null)
			{
				if(!String.IsNullOrEmpty(roomName))
				{
					roomDnaNode.Attributes["RoomName"].Value = roomName;
				}
			}
			UpdateRoomDNA(roomIdFromDB, roomDnaNode.OuterXml);

			return GetRooms(null, null, null, null, roomIdFromDB);
		}

		/// <summary>
		/// THIS SHOULD NEVER BE USED IN PRODUCTION UNLESS STRICTLY REQUIRED
		/// USE "DisableRoom" INSTEAD... which acts as more of a soft delete
		/// TODO... add authentication to this service
		/// </summary>
		/// <param name="roomId"></param>
		/// <returns></returns>
		public SimpleResponse DeleteRoom(string roomId)
		{
			mServiceLog.Log.InfoFormat("DeleteRoom: roomId={0}", roomId);
			RoomsUtil roomsUtil = new RoomsUtil();
			roomsUtil.DeleteRoom(roomId);
			return new SimpleResponse("Success", "true");
		}

		/// <summary>
		/// Marks a room as Disabled (this should be used as delete)
		/// Allows rooms to be "un-deleted"
		/// </summary>
		/// <param name="roomId"></param>
		/// <returns></returns>
		public SimpleResponse DisableRoom(string roomId)
		{
			mServiceLog.Log.InfoFormat("DisableRoom: roomId={0}", roomId);
			RoomsUtil roomsUtil = new RoomsUtil();
			roomsUtil.DisableRoom(roomId);
			return new SimpleResponse("Success", "true");
		}

		/// <summary>
		/// Switches a disabled room to enabled in the DB
		/// This is to be used like an UN-Delete of sorts
		/// in case someone deleted a room they wanted back
		/// </summary>
		/// <param name="roomId"></param>
		/// <returns></returns>
		public SimpleResponse EnableRoom(string roomId)
		{
			mServiceLog.Log.InfoFormat("EnableRoom: roomId={0}", roomId);
			RoomsUtil roomsUtil = new RoomsUtil();
			roomsUtil.EnableRoom(roomId);
			return new SimpleResponse("Success", "true");
		}

	}
}
