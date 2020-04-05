using System;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;


namespace Hangout.Server.WebServices
{
    public class MoneyPaymentsLogService : ServiceBaseClass
    {
        private string mTransactionKey = "";

        public MoneyPaymentsLogService()
        {
			mTransactionKey = WebConfig.TransactionLogKey;
        }

        /// <summary>
        /// Get the Transaction log elements dictionary
        /// containing a list of all Transaction log xml elements and the corresponding
        /// Transaction log database column
        /// </summary>
        /// <returns>The Transaction log elements dictionary</returns>
        private Dictionary<string, string> GetTransactionLogElements()
        {
            Dictionary<string, string> logElements = new Dictionary<string, string>();
            logElements.Add("PaymentItemsUserId", "Transaction/piUserId");
            logElements.Add("ExternalTransactionId", "Transaction/extTransactId");
            logElements.Add("TransactionReference", "Transaction/transactionRef");
            logElements.Add("ErrorDescription", "Transaction/errorDescription");
            logElements.Add("HangoutUserId", "Transaction/hangoutUserId");
            logElements.Add("SessionGuid", "Transaction/sessionGuid");
            logElements.Add("OfferId", "Transaction/offerId");
            logElements.Add("Amount", "Transaction/amount");
            logElements.Add("PayerId", "Transaction/payerId");
            logElements.Add("IPAddress", "Transaction/ipAddress");
            logElements.Add("SecureKey", "Transaction/secureKey");
            logElements.Add("MoneyType", "Transaction/moneyType");
            logElements.Add("FirstName", "Transaction/firstName");
            logElements.Add("LastName", "Transaction/lastName");
            logElements.Add("Address", "Transaction/address");
            logElements.Add("City", "Transaction/city");
            logElements.Add("State", "Transaction/state");
            logElements.Add("ZipCode", "Transaction/zipCode");
            logElements.Add("CountryCode", "Transaction/countryCode");
            logElements.Add("PhoneNumber", "Transaction/phoneNumber");
            logElements.Add("Email", "Transaction/email");
            logElements.Add("BillingDescription", "Transaction/billingDesc");
            logElements.Add("StartBillDate", "Transaction/startDate");
            logElements.Add("NumPayments", "Transaction/numPayments");
            logElements.Add("PayFrequency", "Transaction/payFrequncy");
            logElements.Add("TransactionDateTime", "Transaction/transactDate");
            logElements.Add("TransactionStatus", "Transaction/transactionStatus");
            return logElements;
        }

        /// <summary>
        /// Get the State log elements dictionary
        /// containing a list of all State log xml elements and the corresponding 
        /// State log database column
        /// The PaymentItemsUserId and ExternalTransactionId are added for Create and not for Update 
        /// </summary>
        /// <param name="type">True if Create (Insert) and false for Update</param>
        /// <returns>The State log elements dictionary</returns>
        private Dictionary<string, string> GetStateLogElements(bool type)
        {
            Dictionary<string, string> logElements = new Dictionary<string, string>();

            //We need to add PaymentItemsUserId and ExternalTransactionId for Create and not for Update 
            if (type)
            {
                logElements.Add("PaymentItemsUserId", "Transaction/piUserId");
                logElements.Add("ExternalTransactionId", "Transaction/extTransactId");
            }

            logElements.Add("State", "State/toTransactState");
            logElements.Add("TransactionStatus", "Transaction/transactionStatus");

            logElements.Add("OfferId", "Transaction/offerId");
            logElements.Add("Token", "Transaction/token");
            logElements.Add("PayerId", "Transaction/payerId");
            logElements.Add("IPAddress", "Transaction/ipAddress");
            logElements.Add("TransactionDateTime", "Transaction/transactDate");

            return logElements;
        } 

        /// <summary>
        /// WritePaymentsLog service to write if the transaction log and
        /// if specified write the State log
        /// </summary>
        /// <param name="key">a key identifying that the call has permission to call the service</param>
        /// <param name="timeStamp"></param>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public SimpleResponse WritePaymentsLog(string key, string timeStamp, string xmlData)
        {
            bool success = false;

            if (key != mTransactionKey)
            {
                throw new Exception(String.Format ("WriteMoneyPayment TransactionLogKey did not match Key: {0}", key));
            }

            XmlDocument logDoc = new XmlDocument();
            logDoc.LoadXml(xmlData);

            XmlNode moneyPaymentNode = logDoc.SelectSingleNode("/MoneyPayment");

            string timeStampConverted = ConvertToMySqlTimeStamp(timeStamp);

            AddUpdateNodeValue(logDoc, "/MoneyPayment/Transaction/transactDate", timeStampConverted);
            
            success = WriteMoneyPaymentTransactionLog(moneyPaymentNode);

            CreateUpdateMoneyPaymentStateLogEntry(moneyPaymentNode);

            return new SimpleResponse("MoneyPaymentsLog", "Success");
        }

        public XmlDocument GetTransactionsToNotify(string key, int state)
        {
            XmlDocument xmlResponse = new XmlDocument();
            xmlResponse.LoadXml ("<Transactions></Transactions>");

            using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.LoggingDBConnectionString))
            {
                mysqlConnection.Open();

                using (MySqlCommand transactionsCommand = mysqlConnection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append(" SELECT tstate.ExternalTransactionId, tstate.TransactionStatus, tstate.OfferId, ");
                    sql.Append(" tlog.HangoutUserId, tlog.SessionGuid, tlog.ErrorDescription, tlog.TransactionStatus as LogTransactionStatus, tlog.MoneyType ");
                    sql.Append(" FROM MoneyPaymentTransactionState tstate ");
                    sql.Append(" INNER JOIN MoneyPaymentTransactionLog tlog ON tstate.ExternalTransactionId = tlog.ExternalTransactionId ");
                    if (state == -1)
                    {
                        sql.Append(" WHERE tstate.State > 2 AND tstate.State < 20 ");
                    }
                    else
                    {
                        sql.Append(" WHERE tstate.State = @State ");
                        transactionsCommand.Parameters.AddWithValue("@State", state.ToString());
                    }

                    sql.Append(" ORDER BY tstate.TransactionDateTime ");

                    transactionsCommand.CommandText = sql.ToString();
                    using (MySqlDataAdapter transactionAdapter = new MySqlDataAdapter())
                    {
                        transactionAdapter.SelectCommand = transactionsCommand;
                        DataSet dsTransaction = new DataSet("Transactions");
                        transactionAdapter.Fill(dsTransaction);

                        if (dsTransaction.Tables.Count > 0)
                        {
                            dsTransaction.Tables[0].TableName = "Transaction";
                        }
                        xmlResponse.LoadXml(dsTransaction.GetXml());
                    }
                }
            }
            return xmlResponse;
        }

        private bool WriteMoneyPaymentTransactionLog(XmlNode moneyPaymentNode)
        {
            bool success = false;
            uint transactionLogId = 0;

            string piUserId = GetXmlValueString(moneyPaymentNode, "Transaction/piUserId");
            string extTransactId = GetXmlValueString(moneyPaymentNode, "Transaction/extTransactId");

            if (extTransactId.Length > 0) //&& (piUserId.Length > 0))
            {
                Dictionary<string, string> logElements = GetTransactionLogElements();
                string createNewLogEntryQuery = CreateInsertTransactionLogQuery(logElements, "MoneyPaymentTransactionLog");

                using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.LoggingDBConnectionString))
                {
                    mysqlConnection.Open();

                    using (MySqlCommand createLogCommand = mysqlConnection.CreateCommand())
                    {
                        createLogCommand.CommandText = createNewLogEntryQuery;
                        AddSqlParamaterValues(createLogCommand.Parameters, moneyPaymentNode, logElements, false);

                        transactionLogId = Convert.ToUInt32(createLogCommand.ExecuteScalar());
                        if (transactionLogId == 0)
                        {
                            throw new Exception("WriteMoneyPaymentTransactionLog returned and Index of  0");
                        }
                    }

                    success = true;
                }
            }
            return success;
        }


        private bool CreateUpdateMoneyPaymentStateLogEntry(XmlNode moneyPaymentNode)
        {
            bool success = false;
            uint stateLogId = 0;
            Dictionary<string, string> logElements = null;

            string toTransactState = GetXmlValueString(moneyPaymentNode, "State/toTransactState");
            string extTransactId = GetXmlValueString(moneyPaymentNode, "Transaction/extTransactId");

            if ((toTransactState.Length > 0) && (extTransactId.Length > 0))
            {
                using (MySqlConnection mysqlConnection = new MySqlConnection(WebConfig.LoggingDBConnectionString))
                {
                    mysqlConnection.Open();

                    using (MySqlCommand commandStateLog = mysqlConnection.CreateCommand())
                    {
                        commandStateLog.CommandText = "SELECT COUNT(*) FROM MoneyPaymentTransactionState WHERE ExternalTransactionId = @ExternalTransactionId ";
                        commandStateLog.Parameters.AddWithValue("@ExternalTransactionId", extTransactId);

                        uint returnValue = Convert.ToUInt32(commandStateLog.ExecuteScalar());

                        commandStateLog.Parameters.Clear();
                   

                        if (returnValue == 1)
                        {
                            logElements = GetStateLogElements(false);
                            commandStateLog.CommandText = CreateUpdateTransactionLogQuery(logElements, moneyPaymentNode, "MoneyPaymentTransactionState");
                            commandStateLog.Parameters.AddWithValue("@ExternalTransactionId", extTransactId);
                            AddSqlParamaterValues(commandStateLog.Parameters, moneyPaymentNode, logElements, true);
                        }
                        else
                        {
                            logElements = GetStateLogElements(true);
                            commandStateLog.CommandText = CreateInsertTransactionLogQuery(logElements, "MoneyPaymentTransactionState");
                            AddSqlParamaterValues(commandStateLog.Parameters, moneyPaymentNode, logElements, false);
                        }
                      
                        stateLogId = Convert.ToUInt32(commandStateLog.ExecuteScalar());
                        if (stateLogId == 0)
                        {
                            throw new Exception("CreateUpdateMoneyPaymentStateLogEntry returned and Index of  0");
                        }  
                        success = true;
                    }
                }
            }
            return success;
        }

        private void AddUpdateNodeValue(XmlDocument xmlDoc, string xPath, string value)
        {
            if (value.Trim().Length > 0)
            {
                if (xmlDoc.SelectSingleNode(xPath).Attributes["value"] == null)
                {
                    XmlNode xmlDocNode = xmlDoc.SelectSingleNode(xPath);
                    XmlAttribute attribute = xmlDoc.CreateAttribute("value");
                    attribute.Value = value;
                    xmlDocNode.Attributes.Append(attribute);
                }
                else
                {
                    xmlDoc.SelectSingleNode(xPath).Attributes["value"].InnerText = value;
                }
            }
        }

       
        private string CreateInsertTransactionLogQuery(Dictionary<string, string> logElements, string tableName)
        {
            StringBuilder queryColumns = new StringBuilder();
            StringBuilder queryValues = new StringBuilder();
           
            string delim = "";
            foreach (KeyValuePair<string, string> kvp in logElements)
            {
                queryColumns.Append(delim + kvp.Key);
                queryValues.Append(delim + "@" + kvp.Key);
                delim = ", ";
            }

            string query = String.Format("INSERT INTO {0} ({1}) VALUES ({2}); SELECT LAST_INSERT_ID();", tableName, queryColumns.ToString(), queryValues.ToString());

            return query;
        }

        private string CreateUpdateTransactionLogQuery(Dictionary<string, string> logElements, XmlNode moneyPaymentNode, string tableName)
        {
            StringBuilder setExpresssion = new StringBuilder();

            string delim = "";
            foreach (KeyValuePair<string, string> kvp in logElements)
            {
                //no value then do not add
                if (GetXmlValueString(moneyPaymentNode, kvp.Value).Trim().Length > 0)
                {
                    setExpresssion.Append(delim + kvp.Key + " = " + "@" + kvp.Key);
                    delim = ", ";
                }
            }

            string whereClause = "WHERE ExternalTransactionId = @ExternalTransactionId";
            string selectExpression = "SELECT MoneyPaymentStateIndex FROM MoneyPaymentTransactionState WHERE ExternalTransactionId = @ExternalTransactionId";
            string query = String.Format("UPDATE MoneyPaymentTransactionState SET {0} {1} ; {2};", setExpresssion.ToString(), whereClause, selectExpression);

            return query;

        }

        private void AddSqlParamaterValues(MySqlParameterCollection parameters, XmlNode moneyPaymentNode, Dictionary<string, string> logElements, bool updateMode)
        {
            foreach (KeyValuePair<string, string> kvp in logElements)
            {
                string parameterName = String.Format("@{0}", kvp.Key);
                string parameterValue = GetXmlValueString(moneyPaymentNode, kvp.Value);

                //no value then do not add if in update mode
                if ((updateMode == false) || (parameterValue.Length > 0))
                {
                    parameters.AddWithValue(parameterName, parameterValue);
                }
            }
        }


        private string ConvertToMySqlTimeStamp(string timeStamp)
        {
            string timeStampConverted = "";

            try
            {
                if (timeStamp.Trim().Length > 0)
                {
                    //Need to convert timeStamp to format yyyy-MM-dd hh:mm:ss
                    DateTime dateTime = DateTime.ParseExact(timeStamp, "MM/d/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                    timeStampConverted = dateTime.ToString("yyyy-MM-dd hh:mm:ss");
                }
            }

            catch {}

            return (timeStampConverted);
        }


        private string GetXmlValueString(XmlNode node, string element)
        {
            string value = "";

            try
            {
                value = node.SelectSingleNode(element).Attributes["value"].InnerText;
            }
            catch { }

            return value;
        }

    }
}

   