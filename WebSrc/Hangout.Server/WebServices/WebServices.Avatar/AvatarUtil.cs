using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Hangout.Server.WebServices
{
	public class AvatarUtil
	{
		private ServicesLog mServiceLog = new ServicesLog("AvatarsUtil");

		public void DeleteAvatar(string avatarId)
		{
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AvatarsDBConnectionString))
			{
				mysqlConnection.Open();

				string table = "Avatars";
				int numAvatarId = Convert.ToInt32(avatarId);
				if (numAvatarId < 1000001)
				{
					table = "SystemAvatars";
				}
				string deleteAvatarQuery = "DELETE av, ac FROM " + table + " AS av,AccountToAvatarMapping AS ac WHERE av.AvatarId=ac.AvatarId AND av.AvatarId=@AvatarId;";

				using (MySqlCommand deleteAvatarCommand = mysqlConnection.CreateCommand())
				{
					try
					{
						deleteAvatarCommand.CommandText = deleteAvatarQuery;
						deleteAvatarCommand.Parameters.AddWithValue("@AvatarId", avatarId);
						deleteAvatarCommand.ExecuteNonQuery();
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

		public void DisableAvatar(string avatarId)
		{
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AvatarsDBConnectionString))
			{
				mysqlConnection.Open();

				string table = "Avatars";
				int numAvatarId = Convert.ToInt32(avatarId);
				if (numAvatarId < 1000001)
				{
					table = "SystemAvatars";
				}
				string disableAvatarQuery = "UPDATE " + table + " SET IsEnabled = '0' WHERE AvatarId=@AvatarId;" +
											"UPDATE AccountToAvatarMapping SET IsDefault ='0' WHERE AvatarId=@AvatarId;";

				using (MySqlCommand disableAvatarCommand = mysqlConnection.CreateCommand())
				{
					try
					{
						disableAvatarCommand.CommandText = disableAvatarQuery;
						disableAvatarCommand.Parameters.AddWithValue("@AvatarId", avatarId);
						disableAvatarCommand.ExecuteNonQuery();
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

		public void EnableAvatar(string avatarId)
		{
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AvatarsDBConnectionString))
			{
				mysqlConnection.Open();

				string table = "Avatars";
				int numAvatarId = Convert.ToInt32(avatarId);
				if (numAvatarId < 1000001)
				{
					table = "SystemAvatars";
				}
				string enableAvatarQuery = "UPDATE " + table + " SET IsEnabled = '1' WHERE AvatarId=@AvatarId;";

				using (MySqlCommand enableAvatarCommand = mysqlConnection.CreateCommand())
				{
					try
					{
						enableAvatarCommand.CommandText = enableAvatarQuery;
						enableAvatarCommand.Parameters.AddWithValue("@AvatarId", avatarId);
						enableAvatarCommand.ExecuteNonQuery();
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

		public void DisableAllAvatarsForUser(string accountId)
		{
			List<string> avatars = new List<string>();
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AvatarsDBConnectionString))
			{
				mysqlConnection.Open();
				string getAllAvatarsForAccountQuery = "SELECT AvatarId, IsEnabled FROM AccountToAvatarMapping WHERE HangoutAccountId=@HangoutAccountId;";

				using (MySqlCommand getAllAvatarsForAccountCommand = mysqlConnection.CreateCommand())
				{
					getAllAvatarsForAccountCommand.Parameters.AddWithValue("@HangoutAccountId", accountId);
					getAllAvatarsForAccountCommand.CommandText = getAllAvatarsForAccountQuery;
					using (MySqlDataReader getAllAvatarsReader = getAllAvatarsForAccountCommand.ExecuteReader())
					{
						while (getAllAvatarsReader.Read())
						{
							if (getAllAvatarsReader["IsEnabled"].ToString() == "1")
							{
								avatars.Add(getAllAvatarsReader["AvatarId"].ToString());
							}
						}
					}
				}
			}
			if (avatars.Count > 0)
			{
				foreach (string avatarId in avatars)
				{
					DisableAvatar(avatarId);
				}
			}
		}

		public void SetAllUserAvatarsToNotBeDefault(string accountId)
		{
			using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.AvatarsDBConnectionString))
			{
				mysqlConnection.Open();
				string disableQuery = "UPDATE AccountToAvatarMapping SET IsDefault = '0' WHERE HangoutAccountId=@HangoutAccountId;";

				using (MySqlCommand disableAvatarCommand = mysqlConnection.CreateCommand())
				{
					try
					{
						disableAvatarCommand.CommandText = disableQuery;
						disableAvatarCommand.Parameters.AddWithValue("@HangoutAccountId", accountId);
						disableAvatarCommand.ExecuteNonQuery();
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
