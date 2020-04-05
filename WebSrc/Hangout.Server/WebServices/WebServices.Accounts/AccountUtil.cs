using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Hangout.Server.WebServices
{
	public class AccountUtil
	{
		private ServicesLog mServiceLog = new ServicesLog("AccountUtil");

		public void DeleteAccount(string accountId)
		{
			mServiceLog.Log.InfoFormat("DeleteRooms: accountId={0}", accountId);
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AccountsDBConnectionString))
			{
				mysqlConnection.Open();

				string deleteAccountQuery = "DELETE FROM LocalAccountInfo WHERE HangoutAccountId=@HangoutAccountId ";

				using (MySqlCommand deleteAccountCommand = mysqlConnection.CreateCommand())
				{
					deleteAccountCommand.CommandText = deleteAccountQuery;
					deleteAccountCommand.Parameters.AddWithValue("@HangoutAccountId", accountId);
					deleteAccountCommand.ExecuteNonQuery();
				}
			}
		}

		public void DeleteAvatars(string accountId)
		{
			mServiceLog.Log.InfoFormat("DeleteAvatars: accountId={0}", accountId);
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AvatarsDBConnectionString))
			{
				mysqlConnection.Open();

				string deleteAvatarsQuery = "DELETE AccountToAvatarMapping.* FROM AccountToAvatarMapping LEFT JOIN Avatars ON " +
												"(AccountToAvatarMapping.AvatarId = Avatars.AvatarId) WHERE HangoutAccountId=@HangoutAccountId ";

				using (MySqlCommand deleteAvatarsCommand = mysqlConnection.CreateCommand())
				{
					deleteAvatarsCommand.CommandText = deleteAvatarsQuery;
					deleteAvatarsCommand.Parameters.AddWithValue("@HangoutAccountId", accountId);
					deleteAvatarsCommand.ExecuteNonQuery();
				}
			}
		}

		public void DeleteRooms(string accountId)
		{
			mServiceLog.Log.InfoFormat("DeleteRooms: accountId={0}", accountId);
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.RoomsDBConnectionString))
			{
				mysqlConnection.Open();

				string deleteRoomsQuery = "DELETE AccountToRoomMapping.* FROM AccountToRoomMapping LEFT JOIN Rooms ON " +
												"(AccountToRoomMapping.RoomId = Rooms.RoomId) WHERE HangoutAccountId=@HangoutAccountId ";

				using (MySqlCommand deleteRoomsCommand = mysqlConnection.CreateCommand())
				{
					deleteRoomsCommand.CommandText = deleteRoomsQuery;
					deleteRoomsCommand.Parameters.AddWithValue("@HangoutAccountId", accountId);
					deleteRoomsCommand.ExecuteNonQuery();
				}
			}
		}

		public void DeleteMiniGameUserData(string accountId)
		{
			mServiceLog.Log.InfoFormat("DeleteMiniGameUserData: accountId={0}", accountId);
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.MiniGameDBConnectionString))
			{
				mysqlConnection.Open();

				string deleteMiniGameUserDataQuery = "DELETE FROM MiniGameUserData WHERE AccountId=@HangoutAccountId ";

				using (MySqlCommand deleteMiniGameUserDataCommand = mysqlConnection.CreateCommand())
				{
					deleteMiniGameUserDataCommand.CommandText = deleteMiniGameUserDataQuery;
					deleteMiniGameUserDataCommand.Parameters.AddWithValue("@HangoutAccountId", accountId);
					deleteMiniGameUserDataCommand.ExecuteNonQuery();
				}
			}
		}

	}
}
