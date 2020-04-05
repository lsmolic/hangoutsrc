using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Configuration;
using Hangout.Shared;

using log4net;
using log4net.Layout;
using log4net.Core;
using log4net.Util;


namespace Hangout.Server
{
    public class MoneyPaymentsInfoAppender : log4net.Appender.AppenderSkeleton
    {
        private string mWebServicesBaseUrl = "";
        private string mKey = ""; 

         public string Key
         {
             get { return mKey; }
             set { mKey = value; }
         }

        public MoneyPaymentsInfoAppender()
        {
            mWebServicesBaseUrl = ConfigurationSettings.AppSettings["WebServicesBaseUrl"];
			if (String.IsNullOrEmpty(mWebServicesBaseUrl))
			{
				throw new Exception("App/Web config does not contain a definition for 'WebServicesBaseUrl'");
			}
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                XmlDocument eventDoc = (XmlDocument)loggingEvent.MessageObject;

                LoggingEventData eventData = loggingEvent.GetLoggingEventData();
                DateTime timeStamp = eventData.TimeStamp.ToLocalTime();

                LogEvent(eventDoc, timeStamp);
            }

            catch (Exception ex)
            {
                ErrorHandler.Error("Error MoneyPaymentsInfoAppender", ex);
            }
        }

        public void LogEvent( XmlDocument eventDoc, DateTime timeStamp)
        {
            try
            {
                string timeStampString = timeStamp.ToString();

                Action<XmlDocument> logServiceCallback = delegate(XmlDocument receivedXmlResult)
                {
                    MoneyPaymentsLogServiceResults(receivedXmlResult);
                };

                MoneyPaymentsLogService(timeStampString, eventDoc.InnerXml, logServiceCallback);
            }

            catch (Exception ex)
            {
                ErrorHandler.Error("Error MoneyPayments LogEvent", ex);
            }
        }

        private void MoneyPaymentsLogService(string timeStamp, string xmlData, Action<XmlDocument> logServiceCallback)
        {
            WebServiceRequest moneyPaymentsLogService = new WebServiceRequest(mWebServicesBaseUrl, "MoneyPaymentsLog", "WritePaymentsLog");
            moneyPaymentsLogService.Method = FormMethod.POST;
            moneyPaymentsLogService.AddParam("key", mKey);
            moneyPaymentsLogService.AddParam("timeStamp", timeStamp);
            moneyPaymentsLogService.AddParam("xmlData", xmlData);
            moneyPaymentsLogService.GetWebResponseAsync(logServiceCallback);
        }

        private void MoneyPaymentsLogServiceResults(XmlDocument response)
        {
            try
            {
                    
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Error MoneyPaymentsLogServiceResults", ex);
            }

        }
    }
}



