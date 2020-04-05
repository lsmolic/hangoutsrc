using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

// TwoFish command class to call Widget methods. Get the WidgetConfig information.
namespace Hangout.Server.WebServices
{
  
    internal class Widget : TwoFishCommandBase
    {
        public Widget() : base(System.Reflection.MethodBase.GetCurrentMethod()) { }


        /// <summary>
        /// HealthCheck call to TwoFish to check if the system is operational
        /// </summary>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XmlDocument document containing the HealthCheck information</returns>
        public XmlDocument HealthCheck(BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue.Replace("/v1", ""));

            Dictionary<string, string> keyValues = new Dictionary<string, string>();

            DebugLog("HealthCheck", keyValues);

            StringBuilder responseString = new StringBuilder("<response>");

            string response = rest.GetStringRequest("admin/healthCheck", keyValues);

            response = response.Replace("healthCheckResponse", "HealthCheck");
      
            if (response.Contains("Error"))
            {
                response = "<HealthCheck Twofish='Error' />";
            }

            responseString.Append(response);
            responseString.Append("</response>");
            
            XmlDocument responseXml = new XmlDocument();
            responseXml.LoadXml(responseString.ToString());

            return (responseXml);

        }

        /// <summary>
        /// SystemInfo call to TwoFish to check if the system is operational
        /// </summary>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XmlDocument document containing the SystemInfo information</returns>
        public XmlDocument SystemInfo(BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue.Replace("/v1", ""));

            Dictionary<string, string> keyValues = new Dictionary<string, string>();

            DebugLog("SystemInfo", keyValues);

            StringBuilder responseString = new StringBuilder("<response>");

            string response = rest.GetStringRequest("admin/getSystemInfo", keyValues);

            string appBuild = FindStringBetween(response, "app.build</td><td>", "</td>");

            if (appBuild.Length == 0)
            {
                response = "<SystemInfo Twofish='Error' />";
            }
            else
            {
                response = String.Format("<SystemInfo Build='{0}' />", appBuild);
            }

            responseString.Append(response);
            responseString.Append("</response>");

            XmlDocument responseXml = new XmlDocument();
            responseXml.LoadXml(responseString.ToString());

            return (responseXml);
        }



        /// <summary>
        /// Implements call to Twofish method widgetConfig
        /// </summary>
        /// <param name="commmonKeyValue">Twofish REST service common values</param>
        /// <param name="baseAddress">Twofish REST service base address</param>
        /// <returns>XmlDocument document containing the WidgetConfig information</returns>
        public XmlDocument WidgetConfig(CommmonKeyValues commmonKeyValue, BaseAddress baseAddress)
        {
            ServiceHandler rest = new ServiceHandler(baseAddress.BaseAddressValue);

            Dictionary<string, string> keyValues = commmonKeyValue.GetCommmonInfo(false);

            DebugLog("WidgetConfig", keyValues);

            return (rest.GetXMLRequest("widgetConfig", keyValues));
        } 


        /// <summary>
        /// Find a string in the response between 2 strings
        /// </summary>
        /// <param name="response">The string to search</param>
        /// <param name="begFindString">The beginning string to find</param>
        /// <param name="endFindString">The ending string to find</param>
        /// <returns>The found string or if nothing found then a blank string</returns>
        private string FindStringBetween(string response, string begFindString, string endFindString)
        {
            string returnString = "";

            try
            {
                int begIndex = response.IndexOf(begFindString);
                if (begIndex > 0)
                {
                    begIndex += begFindString.Length;
                    int endIndex = response.IndexOf(endFindString, begIndex);

                    if (endIndex > begIndex)
                    {
                        returnString =  response.Substring(begIndex, endIndex - begIndex);
                    }
                }
            }

            catch (Exception ex)
            {
                logError("FindStringBetween", ex);
            }

            return returnString;
        }
    }
}