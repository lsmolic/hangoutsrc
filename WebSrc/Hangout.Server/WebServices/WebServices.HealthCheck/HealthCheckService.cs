using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Hangout.Server.WebServices
{
    /// <summary>
    /// HealthCheck Web Service to check the health of the Web Services and the databases
    /// </summary>
    public class HealthCheckService : ServiceBaseClass
    {
        /// <summary>
        /// The Dictionary array of databases to check 
        /// The key is the returned xml attribute name.
        /// the value is the Config string that references the connection string
        /// the table name to select a count from
        /// </summary>
        /// <returns>returns a dictionary of the databases to check</returns>
        private Dictionary<string, string> GetDataBasesToCheck()
        {
            Dictionary<string, string> DataBaseInfo = new Dictionary<string, string>();
            DataBaseInfo.Add("AccountsDB", "AccountsDBConnectionString, LocalAccountInfo");
            DataBaseInfo.Add("InventoryDB", "InventoryDBConnectionString, ItemsList");
            DataBaseInfo.Add("AvatarsDB", "AvatarsDBConnectionString, SystemAvatars");
            DataBaseInfo.Add("RoomsDB", "RoomsDBConnectionString, Rooms");
            DataBaseInfo.Add("MiniGameDB", "MiniGameDBConnectionString, MiniGames");
            DataBaseInfo.Add("LoggingDB", "LoggingDBConnectionString, NaughtyWords");

            return DataBaseInfo;
        }

        /// <summary>
        /// Checks the health of services and the databases
        /// If we return from this method without errors then by defualt services are working.
        /// For each database the attribute will be either 'OK' or 'FAILED'
        /// <HealthCheck><HealthCheck Services="OK" AccountsDB="OK" InventoryDB="OK" AvatarsDB="OK" RoomsDB="OK" MiniGameDB="OK" LoggingDB="OK" /></HealthCheck>
        /// <HealthCheck><HealthCheck Services="OK" AccountsDB="FAILED" InventoryDB="FAILED" AvatarsDB="FAILED" RoomsDB="FAILED" MiniGameDB="FAILED" LoggingDB="FAILED" /></HealthCheck>
        /// </summary>
        /// <returns>returns the health of services and the databases</returns>
        public SimpleResponse HealthCheck()
        {
            StringBuilder xmlBuilder = new StringBuilder("<HealthCheck ");
            xmlBuilder.Append("Services ='OK' ");

            Dictionary<string, string> DataBaseInfo = GetDataBasesToCheck();
            Type module = typeof(WebConfig);

            foreach (KeyValuePair<string, string> item in DataBaseInfo)
            {
                string[] itemValueArray = item.Value.Split(',');
        
                FieldInfo field = module.GetField(itemValueArray[0].Trim(), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                string value = (string)field.GetValue(module);
                xmlBuilder.Append(String.Format(" {0}='{1}'", item.Key, GetDatabaseStatus(value, itemValueArray[1].Trim())));
            }

            xmlBuilder.Append(" />");
            return new SimpleResponse("HealthCheck", xmlBuilder.ToString());
        }

        /// <summary>
        /// Helper function to GetThe Health of a datbase
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <param name="tableName">The name of a table to do select count(*) on</param>
        /// <returns>Either OK or FAILED</returns>
        private string GetDatabaseStatus(string connectionString, string tableName)
        {
            string status = "FAILED";
            try
            {
                using (MySqlConnection mysqlConnection = new MySqlConnection(connectionString))
                {
                    mysqlConnection.Open();
                    string getCountQuery =  String.Format("SELECT count(*) FROM {0}",tableName);
                    using (MySqlCommand getCountCommand = mysqlConnection.CreateCommand())
                    {
                        getCountCommand.CommandText = getCountQuery;
                        if (Convert.ToUInt32(getCountCommand.ExecuteScalar()) > 0)
                        {
                            status = "OK";
                        }
                    }
                }
            }
            catch { }
            return status;
        }
    }
}
