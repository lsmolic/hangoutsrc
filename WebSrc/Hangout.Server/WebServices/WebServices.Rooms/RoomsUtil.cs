using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Hangout.Server.WebServices
{
	public class RoomsUtil
	{
		private ServicesLog mServiceLog = new ServicesLog("RoomsUtil");

		public void DisableAllRoomsForUser(string accountId)
		{
			List<string> rooms = new List<string>();
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.RoomsDBConnectionString))
			{
				mysqlConnection.Open();
				string getAllRoomsForAccountQuery = "SELECT RoomId, IsEnabled FROM AccountToRoomMapping WHERE HangoutAccountId='" + accountId + "';";

				using (MySqlCommand getAllRoomsForAccountCommand = mysqlConnection.CreateCommand())
				{
					getAllRoomsForAccountCommand.CommandText = getAllRoomsForAccountQuery;
					using (MySqlDataReader getAllRoomsReader = getAllRoomsForAccountCommand.ExecuteReader())
					{
						while (getAllRoomsReader.Read())
						{
							if (getAllRoomsReader["IsEnabled"].ToString() == "1")
							{
								rooms.Add(getAllRoomsReader["RoomId"].ToString());
							}
						}
					}
				}
			}
			if (rooms.Count > 0)
			{
				foreach (string roomId in rooms)
				{
					DisableRoom(roomId);
				}
			}
		}

		public void DisableRoom(string roomId)
		{
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.RoomsDBConnectionString))
			{
				mysqlConnection.Open();

				string table = "Rooms";
				int numRoomId = Convert.ToInt32(roomId);
				if (numRoomId < 1000001)
				{
					table = "SystemRooms";
				}
				string disableRoomQuery = "UPDATE " + table + " SET IsEnabled = '0' WHERE RoomId=@RoomId;" +
											"UPDATE AccountToRoomMapping SET IsDefault ='0' WHERE RoomId=@RoomId;";

				using (MySqlCommand disableRoomCommand = mysqlConnection.CreateCommand())
				{
					try{
						disableRoomCommand.CommandText = disableRoomQuery;
						disableRoomCommand.Parameters.AddWithValue("@RoomId", roomId);
						disableRoomCommand.ExecuteNonQuery();
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
		}

		public void EnableRoom(string roomId)
		{
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.RoomsDBConnectionString))
			{
				mysqlConnection.Open();

				string table = "Rooms";
				int numRoomId = Convert.ToInt32(roomId);
				if (numRoomId < 1000001)
				{
					table = "SystemRooms";
				}
				string enableRoomQuery = "UPDATE "+table+" SET IsEnabled = '1' WHERE RoomId=@RoomId;";

				using (MySqlCommand enableRoomCommand = mysqlConnection.CreateCommand())
				{
					try
					{
						enableRoomCommand.CommandText = enableRoomQuery;
						enableRoomCommand.Parameters.AddWithValue("@RoomId", roomId);
						enableRoomCommand.ExecuteNonQuery();
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
		}

		public void DeleteRoom(string roomId)
		{
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.RoomsDBConnectionString))
			{
				mysqlConnection.Open();

				string table = "Rooms";
				int numRoomId = Convert.ToInt32(roomId);
				if (numRoomId < 1000001)
				{
					table = "SystemRooms";
				}
				string deleteRoomQuery = "DELETE rm, ac FROM " + table + " AS rm,AccountToRoomMapping AS ac WHERE rm.RoomId=ac.RoomId AND rm.RoomId=@RoomId; ";
											

				using (MySqlCommand deleteRoomCommand = mysqlConnection.CreateCommand())
				{
					try
					{
						deleteRoomCommand.CommandText = deleteRoomQuery;
						deleteRoomCommand.Parameters.AddWithValue("@RoomId", roomId);
						deleteRoomCommand.ExecuteNonQuery();
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
		}

		public void SetAllUserRoomsToNotBeDefault(string accountId)
		{
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.RoomsDBConnectionString))
			{
				mysqlConnection.Open();
				string disableQuery = "UPDATE AccountToRoomMapping SET IsDefault = '0' WHERE HangoutAccountId=@HangoutAccountId;";

				using (MySqlCommand disableRoomCommand = mysqlConnection.CreateCommand())
				{
					try
					{
						disableRoomCommand.CommandText = disableQuery;
						disableRoomCommand.Parameters.AddWithValue("@HangoutAccountId", accountId);
						disableRoomCommand.ExecuteNonQuery();
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
		}
	}
}
