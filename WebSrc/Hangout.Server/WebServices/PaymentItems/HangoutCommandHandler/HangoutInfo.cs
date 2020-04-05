using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    public class HangoutInfo : HangoutCommandBase 
    {
        public HangoutInfo() : base(System.Reflection.MethodBase.GetCurrentMethod()) { }

        /// <summary>
        /// HealthCheck call to PaymentItems to check if the system is operational
        /// </summary>
        /// <returns>XmlDocument document containing the HealthCheck information</returns>
        public XmlDocument HealthCheck()
        {
            XmlDocument response = null;
            try
            {
                PaymentCommand cmd = new PaymentCommand();
                cmd.Noun = "Widget";
                cmd.Verb = "HealthCheck";

                response = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());
                AddAttribute(response, "/Response/HealthCheck", "PaymentItems", "OK");

                string buildInfo = GetTwoFishBuildInfo();
                AddAttribute(response, "/Response/HealthCheck", "BuildInfo", buildInfo);
           }

            catch (Exception ex)
            {
                logError("HealthCheck", ex);
                response.Load("<Response noun='HangoutInfo' verb='HealthCheck'><HealthCheck PaymentItems='Error' Twofish='Unknown' /></Response>");
           }

            return response;
        }

        private string GetTwoFishBuildInfo()
        {
            string buildInfo = "Unknown";

            try
            {
                PaymentCommand cmd = new PaymentCommand();
                cmd.Noun = "Widget";
                cmd.Verb = "SystemInfo";

                XmlDocument response = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());
                buildInfo = response.SelectSingleNode("/Response/SystemInfo/@Build").InnerText;
            }

            catch (Exception ex)
            {
                logError("GetTwoFishBuildInfo", ex);
            }

            return buildInfo;
        }

    }
}
