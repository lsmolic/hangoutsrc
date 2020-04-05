using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hangout.Server;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Xml.Linq;

namespace Hangout.Server.WebServices
{
	public class Escrow : ServiceBaseClass
	{
		private ServicesLog mServiceLog = new ServicesLog("Escrow");

		public SimpleResponse GetEscrowTransactions(string toFacebookAccountId, string fromAccountId, string transactionType, string timeStamp, string deleteTransactions)
		{

			mServiceLog.Log.InfoFormat("GetEscrowTransactions: toFacebookAccountId={0}, fromAccountId={1}, transactionType={2}, timeStamp={3}",
					toFacebookAccountId, fromAccountId, transactionType, timeStamp);
			StringBuilder xmlBuilder = new StringBuilder();
			try
			{
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.MiniGameDBConnectionString))
				{
					mysqlConnection.Open();

					string getTransactionsQuery = "SELECT * FROM EscrowTransactions ";
					StringBuilder sqlOptions = new StringBuilder();							
					using (MySqlCommand getTransactionsCommand = mysqlConnection.CreateCommand())
					{
						if (!String.IsNullOrEmpty(toFacebookAccountId))
						{
							sqlOptions.AppendFormat(" AND ToAccountId=@toFacebookAccountId ");
							getTransactionsCommand.Parameters.AddWithValue("@toFacebookAccountId", toFacebookAccountId);
						}
						if (!String.IsNullOrEmpty(fromAccountId))
						{
							sqlOptions.AppendFormat(" AND FromAccountId=@fromAccountId ");
							getTransactionsCommand.Parameters.AddWithValue("@fromAccountId", fromAccountId);
						}
						if (!String.IsNullOrEmpty(transactionType))
						{
							sqlOptions.AppendFormat(" AND TransactionType=@transactionType ");
							getTransactionsCommand.Parameters.AddWithValue("@transactionType", transactionType);
						}
						if (!String.IsNullOrEmpty(timeStamp))
						{
							sqlOptions.AppendFormat(" AND TransactionTimeStamp=@timeStamp ");
							getTransactionsCommand.Parameters.AddWithValue("@timeStamp", timeStamp);
						}
						getTransactionsQuery += " WHERE 1 " + sqlOptions;
						getTransactionsQuery += " ORDER BY TransactionTimeStamp ASC;";
						
						getTransactionsCommand.CommandText = getTransactionsQuery;
						using (MySqlDataReader getTransactionsReader = getTransactionsCommand.ExecuteReader())
						{
							while (getTransactionsReader.Read())
							{
								XElement escrowTransaction = new XElement("Transaction");
								escrowTransaction.SetAttributeValue("FromAccountId", getTransactionsReader["FromAccountId"].ToString());
								escrowTransaction.SetAttributeValue("ToFacebookAccountId", getTransactionsReader["ToAccountId"].ToString());
								escrowTransaction.SetAttributeValue("TransactionType", getTransactionsReader["TransactionType"].ToString());
								escrowTransaction.SetAttributeValue("Value", getTransactionsReader["Value"].ToString());
								escrowTransaction.SetAttributeValue("TransactionTimeStamp", getTransactionsReader["TransactionTimeStamp"].ToString());

								xmlBuilder.Append(escrowTransaction.ToString());
							}
						}
						if (deleteTransactions == "1")
						{
							getTransactionsCommand.CommandText = " DELETE FROM EscrowTransactions WHERE 1 " + sqlOptions;
							getTransactionsCommand.ExecuteNonQuery();
						}
					}
				}
				return new SimpleResponse("EscrowTransactions", xmlBuilder.ToString());
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

		public SimpleResponse CreateMultipleEscrowTransactions(string toFacebookAccountIdCsvList, string fromAccountId, string transactionType, string value)
		{
			if (!String.IsNullOrEmpty(toFacebookAccountIdCsvList))
			{

				string[] accountIds = toFacebookAccountIdCsvList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string s in accountIds)
				{
					long sLong = Convert.ToInt64(s);
					CreateEscrowTransaction(sLong.ToString(), fromAccountId, transactionType, value);
				}
			}
			return new SimpleResponse("success", "true");
		}

        public SimpleResponse CreateEscrowTransaction(string toFacebookAccountId, string fromAccountId, string transactionType, string value)
        {
            mServiceLog.Log.DebugFormat("CreateEscrowTransaction {0} {1} {2} {3}", toFacebookAccountId, fromAccountId, transactionType, value);

            try
            {
                using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.MiniGameDBConnectionString))
                {
                    mysqlConnection.Open();

                    int oldValue;
                    string insertOrUpdateTransactionsQuery = "INSERT INTO EscrowTransactions (FromAccountId, ToAccountId, TransactionType, Value, TransactionTimeStamp) " +
                        " VALUES (@FromAccountId, @ToAccountId, @TransactionType, @Value, NOW()) " +
                        " ON DUPLICATE KEY UPDATE Value=Value+@Value;";
                    using (MySqlCommand insertOrUpdateTransactionsCommand = mysqlConnection.CreateCommand())
                    {
                        insertOrUpdateTransactionsCommand.Parameters.AddWithValue("@ToAccountId", toFacebookAccountId);
                        insertOrUpdateTransactionsCommand.Parameters.AddWithValue("@FromAccountId", fromAccountId);
                        insertOrUpdateTransactionsCommand.Parameters.AddWithValue("@TransactionType", transactionType);
                        insertOrUpdateTransactionsCommand.Parameters.AddWithValue("@Value", value);

                        insertOrUpdateTransactionsCommand.CommandText = insertOrUpdateTransactionsQuery;
                        object o = insertOrUpdateTransactionsCommand.ExecuteScalar();
                    }
                }
                mServiceLog.Log.DebugFormat("CreateEscrowTransaction success {0} {1} {2} {3}", toFacebookAccountId, fromAccountId, transactionType, value);
                return new SimpleResponse("CreateTransaction", "success");
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

		public SimpleResponse ClearEscrowTransactions(string toFacebookAccountId, string fromAccountId, string transactionType, string timeStamp)
		{
			try
			{
				using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.MiniGameDBConnectionString))
				{
					mysqlConnection.Open();
					

					StringBuilder sqlOptions = new StringBuilder();
					using (MySqlCommand getTransactionsCommand = mysqlConnection.CreateCommand())
					{
						string getTransactionsQuery = "DELETE FROM EscrowTransactions ";

						//TODO: set logical rules for what can be removed. This could be abused in it's current state... especially by 
						//someone who is careless. If you are not specific enough 

						if (!String.IsNullOrEmpty(toFacebookAccountId))
						{
							sqlOptions.AppendFormat(" ToAccountId=@toFacebookAccountId ");
							getTransactionsCommand.Parameters.AddWithValue("@toFacebookAccountId", toFacebookAccountId);
						}
						else
						{
							throw new ArgumentException(" toFacebookAccountId must be defined. ");
						}
						if (!String.IsNullOrEmpty(fromAccountId))
						{
							sqlOptions.AppendFormat(" AND FromAccountId=@fromAccountId ");
							getTransactionsCommand.Parameters.AddWithValue("@fromAccountId", fromAccountId);
						}
						if (!String.IsNullOrEmpty(transactionType))
						{
							sqlOptions.AppendFormat(" AND TransactionType=@transactionType ");
							getTransactionsCommand.Parameters.AddWithValue("@transactionType", transactionType);
						}
						if (!String.IsNullOrEmpty(timeStamp))
						{
							sqlOptions.AppendFormat(" AND TransactionTimeStamp <= @timeStamp ");
							getTransactionsCommand.Parameters.AddWithValue("@timeStamp", timeStamp);
						}
						
						getTransactionsQuery += " WHERE 0 OR ( " + sqlOptions + " );";
						getTransactionsCommand.CommandText = getTransactionsQuery;
						getTransactionsCommand.ExecuteNonQuery();
					}
				}
				return new SimpleResponse("ClearTransaction", "success");
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
