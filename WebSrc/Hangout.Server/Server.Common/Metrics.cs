using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using Hangout.Shared;

namespace Hangout.Server
{
	public static class Metrics
	{
		public static ILog mMetricsLog = LogManager.GetLogger("Metrics");

		public static void Log(string categoryName, string eventName, string optLabel, string optData, string accountId)
		{
            string message = "||" + categoryName + "&" + eventName + "&" + optLabel + "&" + optData + "&" + accountId + "&";
			mMetricsLog.Info(message);
		}

        public static void Log(string categoryName, string eventName, string optLabel, string accountId)
        {
            Log(categoryName, eventName, optLabel, " ", accountId);
        }

        public static void Log(string categoryName, string eventName, string accountId)
        {
            Log(categoryName, eventName, " ", " ", accountId);
        }

        /// <summary>
        /// Handle an event log message from the client
        /// </summary>
        /// <param name="message"></param>
        public static void Log(Message receivedMessage, string accountId)
        {
            string categoryName = (string)receivedMessage.Data[0];
            string eventName = (string)receivedMessage.Data[1];
            string subEvent = (string)receivedMessage.Data[2];
            string eventData = (string)receivedMessage.Data[3];
            Metrics.Log(categoryName, eventName, subEvent, eventData, accountId);
        }
	}
}
