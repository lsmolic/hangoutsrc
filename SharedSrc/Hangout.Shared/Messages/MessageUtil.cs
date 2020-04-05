using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Hangout.Shared.Messages
{
    public static class MessageUtil
   {
		public static void ReceiveMessage<T>(Message receivedMessage, IDictionary<T, Action<Message, Guid>> messageActionDictionary, Guid senderId)
		{
			int actionIndex = receivedMessage.Callback;
			if (!Enum.IsDefined(typeof(T), actionIndex))
			{
				throw new ArgumentException("Invalid cast from callback index " + typeof(T).Name + " to message action callback " + actionIndex);
			}
			T actionType = (T)Enum.ToObject(typeof(T), actionIndex);

			Action<Message, Guid> callbackResponse = null;
			if (!messageActionDictionary.TryGetValue(actionType, out callbackResponse))
			{
				//throw new Exception("Callback index [" + actionIndex + "] not found in supplied dictionary");
				// This isn't fatal.  We may have cancelled the callback on the client before hearing back from the server. 
				//  Log it as a warning in case this is actually unexpected
				Console.WriteLine("Warning: Callback index [" + actionIndex + "] not found in supplied dictionary");
				return;
			}

			// Call the Action stored for this message
			callbackResponse(receivedMessage, senderId);
		}

        /// <summary>
        /// This will automatically call the callback registered in the dictionary that is passed in.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="receivedMessage"></param>
        /// <param name="messageActionDictionary"></param>
        public static void ReceiveMessage<T>(Message receivedMessage, IDictionary<T, Action<Message>> messageActionDictionary)
        {
            int actionIndex = receivedMessage.Callback;
            if (!Enum.IsDefined(typeof(T), actionIndex))
            {
                throw new ArgumentException("Invalid cast from callback index " + typeof(T).Name + " to message action callback " + actionIndex);
            }
            T actionType = (T)Enum.ToObject(typeof(T), actionIndex);

            Action<Message> callbackResponse = null;
            if (!messageActionDictionary.TryGetValue(actionType, out callbackResponse))
            {
                //throw new Exception("Callback index [" + actionIndex + "] not found in supplied dictionary");
				// This isn't fatal.  We may have cancelled the callback on the client before hearing back from the server. 
				//  Log it as a warning in case this is actually unexpected
				Console.WriteLine("Warning: Callback index [" + actionIndex + "] not found in supplied dictionary");
				return;
			}

            // Call the Action stored for this message
            callbackResponse(receivedMessage);
        }

        
        /// <summary>
		/// Gets all the elements in message.Data that can be parsed to an XmlDocument
		/// </summary>
		public static IEnumerable<XmlDocument> GetXmlDocumentsFromMessage(Message m)
		{
			return GetXmlDocumentsFromMessageData(m.Data);
		}

		public static IEnumerable<XmlDocument> GetXmlDocumentsFromMessageData(List<object> messageData)
		{
			List<XmlDocument> result = new List<XmlDocument>();
			foreach (object obj in messageData)
			{
				if (obj is string)
				{
					string possibleXml = (string)obj;

					XmlDocument resultDoc = new XmlDocument();
					bool isXml = false;
					try
					{
						resultDoc.LoadXml(possibleXml);
						isXml = true;
					}
					catch (XmlException)
					{
						// guess it wasn't XML.
						// If there's an XML TryParse function somewhere, replace this silly try catch with it.
					}

					if (isXml)
					{
						result.Add(resultDoc);
					}
				}
			}
			return result;
		}
    }
}
