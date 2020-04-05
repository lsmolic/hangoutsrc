using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Timers;
using System.Xml;
using Hangout.Client;
using Hangout.Shared;
using ServerAdminTool;

namespace ServerAdminTool
{
    public class AdminClientMessageProcessor : IMessageProcessor
    {
        private Action<string> mUpdateTreeCallback;
        private ClientReflector mClientReflector = null;
        private BinaryFormatter mBinaryFormatter = new BinaryFormatter();
        private Timer getMoreMessagesTimer = new Timer(200);
        private string sessionId = "";

        public string SessionId
        {
            get {return sessionId;}
        }
		
		public AdminClientMessageProcessor()
	    {
        }

        public void SetUpdateTreeCallback(Action<string> updateTreeCallback)
        {
            mUpdateTreeCallback = updateTreeCallback;
        }

        public void Connect(string stateServerAddress)
        {
            // Kill any connection that already exists
            Dispose();

            if (mClientReflector == null)
	      	{
				int adminPort = 9090;

				//Console.WriteLine(stateServerAddress + ":" + stateServerPort);

                mClientReflector = new Hangout.Client.ClientReflector(new Logger());
                mClientReflector.ConnectBlocking(stateServerAddress, adminPort);

	           getMoreMessagesTimer.Interval = 200;
	           getMoreMessagesTimer.Elapsed += new ElapsedEventHandler(ReceiveMessagesFromReflector);
	           getMoreMessagesTimer.Start();
	       	}            
		}

        public void Dispose()
        {
            if (mClientReflector != null)
            {
                mClientReflector.Dispose();
            }
        }

        public void SendMessageToReflector(Message message)
        {
            MemoryStream ms = new MemoryStream();
            mBinaryFormatter.Serialize(ms, message);
            ms.Position = 0;

            mClientReflector.SendMessage(ms);
        }


        /// at this point we would hand the stream over to the queue
        /// 

        //RECEIVE MESSAGE FROM SERVER
		//TODO: this is demo code only!  remove this when we actually get messages being kicked around by the server

		public void SendMockMessageForTesting(Message receivedMessage)
		{
			ProcessMessage(receivedMessage);
		}


        public void ReceiveMessagesFromReflector(object source, EventArgs e )
        {
            RequestMessageFromReflector();    
        }

        private void RequestMessageFromReflector()
        {
            MemoryStream nextMessage = mClientReflector.GetNextMessage();
            if (nextMessage != null)
            {
                Message myReceivedMessage = (Message)mBinaryFormatter.Deserialize(nextMessage);
                ProcessMessage(myReceivedMessage);
                RequestMessageFromReflector();
            } 
        }

        protected void ProcessMessage(Message receivedMessage)
        {
            switch (receivedMessage.MessageType)
            {
                case MessageType.Admin:
                    Console.WriteLine("Received admin message");
                    try
                    {
                        string xmlString = (string)receivedMessage.Data[0];
                        SetTreeData(xmlString);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        throw e;
                    }
                    break;
            }
        }


		private void ReceivedUpdateFromDistributedObject(uint doId, string message)
		{
			throw new NotImplementedException();
			//server.sendmessage(doId, message);
		}
		
		/************************************/
		// IClientMessageProcessor Funcs.
		/************************************/
		
		public void SendMessageToServer( string message ) 
		{
			throw new NotImplementedException();
		}

        public void RequestUpdate()
        {
            // Send connect to server
            Hangout.Shared.Message adminMessage = new Hangout.Shared.Message();
            List<object> messageData = new List<object>();
            messageData.Add("GetObjects");
            adminMessage.AdminDataMessage(messageData);

            SendMessageToReflector(adminMessage);
        }

        public void SetTreeData(string xmlString)
        {
            mUpdateTreeCallback(xmlString);
        }

    
    }
}
