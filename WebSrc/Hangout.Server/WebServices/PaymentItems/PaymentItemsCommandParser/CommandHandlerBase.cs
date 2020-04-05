using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Reflection;
using System.Xml;
using Hangout.Server;
using Hangout.Shared;

//Base Class for the CommandHandler. Derived from the CommandParser.
//Support methods (including logging), used for processing Hangout, Twofish and PayPal commands. 

namespace Hangout.Server.WebServices
{
    public class CommandHandlerBase : CommandParser
    {

        public CommandHandlerBase()
        {
            SetLogger("CommandHandler");
        }

        public CommandHandlerBase(string loggerName)
        {
            SetLogger(loggerName);
        }

        public CommandHandlerBase(Type loggerType)
        {
            SetLogger(loggerType);
        }
        

        /// <summary>
        /// Retreive the ConfigurationAppSetting from the app config file
        /// </summary>
        /// <param name="key">The key to find</param>
        /// <param name="defaultValue">The default value if the key is not found or an error occurs</param>
        /// <returns>Returns the App Config Setting string or if the key is not found or an error occurs returns the default value</returns>
        protected string GetConfigurationAppSetting(string key, string defaultValue)
        {
            string value = defaultValue;

            try
            {
                value = ConfigurationManager.AppSettings[key];
                if (value == null)
                {
                    value = defaultValue;
                }
            }
            catch { }
            return value;
        }

         /// <summary>
        ///  Write to the debug log key value pairs, encrptying the the values in the itemsToEncrypt array
         /// </summary>
        /// <param name="service">The name of the service being called</param>
        /// <param name="keyValues">Dictionary containing the key value pairs to write to the log.</param>
         /// <param name="itemsToEncrypt">The string array of key items to encrypt</param>
         protected void DebugLogEncryptList(string service, Dictionary<string, string> keyValues, string[] itemsToEncrypt)
         {
            Dictionary<string, string> debugKeyValues = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kvp in keyValues)
            {
                if (IsKeyEncrypt(kvp.Key, itemsToEncrypt))
                {
                    SimpleCrypto crypt = new SimpleCrypto();
                    debugKeyValues[kvp.Key] = crypt.TDesEncrypt(kvp.Value); 
                }
                else
                {
                    debugKeyValues[kvp.Key] = kvp.Value;

                }
            }
            DebugLog(service, debugKeyValues);
         }

         /// <summary>
         /// Check if the key is in the itemsToEncrypt array
         /// </summary>
         /// <param name="key">The key to check against</param>
         /// <param name="itemsToEncrypt">The string array of key items to encrypt</param>
         /// <returns>true if the key matches an item in the itemsToEncrypt array else false</returns>
         private bool IsKeyEncrypt(string key, string[] itemsToEncrypt)
         {
             bool encrypt = false;
             foreach (string item in itemsToEncrypt)
             {
                 if (item == key)
                 {
                     encrypt = true;
                     break;
                 }
             }
             return encrypt;
         }

        /// <summary>
        ///  Write to the debug log key value pairs.
        /// </summary>
        /// <param name="service">The name of the service being called</param>
        /// <param name="keyValues">Dictionary containing the key value pairs to write to the log.</param>
        protected void DebugLog(string service, Dictionary<string, string> keyValues)
        {
            try
            {
                string keyValueString = "";
                string delim = "";
                foreach (KeyValuePair<string, string> kvp in keyValues)
                {
                    keyValueString += delim + String.Format(" Key:{0}  Value:{1} ", kvp.Key, kvp.Value);
                    delim = ",";
                }
                logDebug("Service", String.Format("{0}, KeyValue: {1}", service, keyValueString));
            }

            catch (Exception ex)
            {
                logError("Error DebugLog", ex);
            }
        }
    }
}
