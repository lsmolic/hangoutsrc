using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Reflection;
using Hangout.Server;

//Set of common classes used by the command parser.
//BaseAddress is the base url address for the twofish calls REST call.
//CommonKeyValues is the common values used across all twofish REST calls.
//Simple Response is a simple XML response (a single Key-Value pair).

namespace Hangout.Server.WebServices
{
    /// <summary>
    /// Set of common classes used by the command parser.
    /// BaseAddress is the base url address for the twofish calls REST call.
    /// CommonKeyValues is the common values used across all twofish REST calls.
    /// Simple Response is a simple XML response (a single Key-Value pair).
    /// </summary>
    public class BaseAddress
    {
        string baseAddress = ConfigurationManager.AppSettings["BaseAddress"];
        /// <summary>
        /// Returns the BaseAddress from the App config file
        /// </summary>
        public string BaseAddressValue
        {
            get { return baseAddress; }
        }

    }

    public class CommmonKeyValues
    {
        string appFamilyId = ConfigurationManager.AppSettings["AppFamilyId"];
        string appName = ConfigurationManager.AppSettings["AppName"];
        string partnerId = ConfigurationManager.AppSettings["PartnerId"];
        string accessKey = ConfigurationManager.AppSettings["AccessKey"];
        string namespaceId = ConfigurationManager.AppSettings["NamespaceId"];


        /// <summary>
        /// Returns the common key values for twoFish from the app config file
        /// </summary>
        /// <param name="addAppName"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetCommmonInfo(bool addAppName)
        {
            Dictionary<string, string> commmonKeyValue = new Dictionary<string, string>();

            commmonKeyValue.Add("format", "xml");
            commmonKeyValue.Add("auth.partnerId", partnerId);
            commmonKeyValue.Add("auth.accessKey", accessKey);
            commmonKeyValue.Add("auth.appFamilyId", appFamilyId);
            commmonKeyValue.Add("auth.namespaceId", namespaceId);
            if (addAppName)
            {
                commmonKeyValue.Add("appName", appName);
            }

            return commmonKeyValue;
        }

        /// <summary>
        /// GetCommmonInfo
        /// </summary>
        /// <param name="addAppName"></param>
        /// <param name="paymentGatewayConfigIdFlag"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetCommmonInfo(bool addAppName, string paymentGatewayName)
        {
            Dictionary<string, string> commmonKeyValue = GetCommmonInfo(addAppName);

            if (!String.IsNullOrEmpty (paymentGatewayName))
            {
                string paymentGatewayConfigId = ConfigurationManager.AppSettings[paymentGatewayName];
                commmonKeyValue.Add("paymentGatewayConfigId", paymentGatewayConfigId);
            }

            return commmonKeyValue;
        }
    }
}
