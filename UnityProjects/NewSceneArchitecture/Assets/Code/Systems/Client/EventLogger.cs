using System;
using System.Collections.Generic;
using Hangout.Shared;
using UnityEngine;

namespace Hangout.Client
{
    public static class EventLogger
    {
        /// <summary>
        /// Log an event with no extra data.  Also sends to mixpanel
        /// </summary>
        /// <param name="categoryName">Category name from LogGlobals</param>
        /// <param name="eventName">Event name from LogGlobals</param>
        public static void Log(string categoryName, string eventName)
        {
            Log(categoryName, eventName, "", "");
        }

        /// <summary>
        /// Log an event categoryName, eventName and a string of arbitrary data.  Also sends to mixpanel
        /// </summary>
        /// <param name="categoryName">Category name from LogGlobals</param>
        /// <param name="eventName">Event name from LogGlobals</param>
        /// <param name="extraProps">Arbitrary data in JSON format</param>
        public static void Log(string categoryName, string eventName, string extraProps)
        {
			// Test to make sure extraProps is a valid Json string
			LitJson.JsonMapper.ToObject(extraProps);

            ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
            if (clientMessageProcessor != null)
            {
                List<object> messageData = new List<object>();
                messageData.Add(categoryName);
                messageData.Add(eventName);
                messageData.Add("props");
                messageData.Add(extraProps);
                Message message = new Message(MessageType.Event, messageData);
                clientMessageProcessor.SendMessageToReflector(message);

                // Send event to mix panel also
                JSDispatcher jsd = new JSDispatcher();

                string mpEvent = categoryName + "_" + eventName;
                jsd.LogMetricsEvent(mpEvent, extraProps, delegate(string s){} );
            }
            else
            {
                Console.WriteLine(String.Format("Warning: EventLogger not ready to log {0}, {1}, {2}, {3}", categoryName, eventName, extraProps));
            }
        }

        /// <summary>
        /// Log an event with one label/data pair.  Also sends to mixpanel
        /// </summary>
        /// <param name="categoryName">Category name from LogGlobals</param>
        /// <param name="eventName">Event name from LogGlobals</param>
        /// <param name="optLabel">Label of data being sent, like "price"</param>
        /// <param name="optData">Value of data being sent, like "100"</param>
        public static void Log(string categoryName, string eventName, string optLabel, string optData)
        {
            ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
            if (clientMessageProcessor != null)
            {
                List<object> messageData = new List<object>();
                messageData.Add(categoryName);
                messageData.Add(eventName);
                messageData.Add(optLabel);
                messageData.Add(optData);
                Message message = new Message(MessageType.Event, messageData);
                clientMessageProcessor.SendMessageToReflector(message);

                // Send event to mix panel also
                JSDispatcher jsd = new JSDispatcher();

                string mpEvent = categoryName+"_"+eventName;
                string properties = "";
                

                if (optLabel != "" && optData != "")
                {
                    properties = "{\"" + optLabel + "\":\"" + optData + "\"}";
                }
                jsd.LogMetricsEvent(mpEvent, properties, delegate(string s) { });
            }
            else 
            {
                Console.WriteLine(String.Format("Warning: EventLogger not ready to log {0}, {1}, {2}, {3}", categoryName, eventName, optLabel, optData));
            }
        }
		/// <summary>
		/// Log an event with one label/data pair. 
		/// </summary>
		/// <param name="categoryName">Category name from LogGlobals</param>
		/// <param name="eventName">Event name from LogGlobals</param>
		/// <param name="optLabel">Label of data being sent, like "price"</param>
		/// <param name="optData">Value of data being sent, like "100"</param>
		public static void LogNoMixPanel(string categoryName, string eventName, string optLabel, string optData)
		{
			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
			if (clientMessageProcessor != null)
			{
				List<object> messageData = new List<object>();
				messageData.Add(categoryName);
				messageData.Add(eventName);
				messageData.Add(optLabel);
				messageData.Add(optData);
				Message message = new Message(MessageType.Event, messageData);
				clientMessageProcessor.SendMessageToReflector(message);
			}
			else
			{
				Console.WriteLine(String.Format("Warning: EventLogger not ready to log {0}, {1}, {2}, {3}", categoryName, eventName, optLabel, optData));
			}
		}

    }
}
