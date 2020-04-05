using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;
using System.IO;
using System.Collections;
using Hangout.Shared;
using System.Xml;
using log4net;
using Hangout.Server.StateServer;

namespace Hangout.Server
{
    public delegate string getUserIdDelegate(Guid sessionId);

    public class ServerMessageProcessor : IMessageProcessor
    {
        private ILog mLogger;
        private ServerStateMachine mServerStateMachine = null;
        private BinaryFormatter mBinaryFormatter = new BinaryFormatter(); 
        
        /// <summary>
        /// The reflector listening for messages from the unity client
        /// </summary>
        private Hangout.Server.ServerReflector mServerReflector = null;

        /// <summary>
        /// The reflector listening for messages from the admin client
        /// </summary>
        private Hangout.Server.ServerReflector mAdminReflector = null;

		public ServerReflector ServerReflector
		{
			get { return mServerReflector; }
		}

        public ServerMessageProcessor(ServerStateMachine serverStateMachine)
        {
            mLogger = LogManager.GetLogger("ServerMessageProcessor");

            mServerStateMachine = serverStateMachine;

			mServerReflector = new ServerReflector(mServerStateMachine.ConnectionHandler.Disconnect, ReceiveMessageFromReflector);
            mServerReflector.StartServerReflector(StateServerConfig.ServerIp, int.Parse(StateServerConfig.ServerPort));
            // Tell boss server that this server is online
            BossServerAPI.InitStateServer(StateServerConfig.ServerIp, StateServerConfig.ServerPort,
                delegate(XmlDocument xmlResponse) {
                    XmlNode stateServerNode = xmlResponse.SelectSingleNode("StateServerId");
                    if (stateServerNode != null)
                    {
                        string stateServerId = stateServerNode.InnerText;
                        serverStateMachine.StateServerId = stateServerId;
						mLogger.InfoFormat("Registered with Boss Server as stateserver id = {0}", stateServerId);
					}
                    else
                    {
                        mLogger.Error("Received an invalid response from BossServer.InitStateServer: " + xmlResponse.OuterXml);
                    }
                });

            //TODO: uncomment this to enable admin console
            //mAdminReflector = new ServerReflector(Disconnect, ReceiveMessageFromReflector);
            //mAdminReflector.StartServerReflector(9090);

            //CallToService getFacebookUsers = new CallToService(StateServerConfig.WebServicesBaseUrl + "Facebook/GetTestUsers");
            //getFacebookUsers.Send();
            //XmlDocument response = getFacebookUsers.ServiceResponse;

            //Console.WriteLine(getFacebookUsers.ServiceResponse.OuterXml);
        }
        
		#region Send_Message_To_Reflector
			/// <summary>
			/// Convenience function for sending a message to a single sessionId.
			/// </summary>
			/// <param name="message"></param>
			/// <param name="sessionId"></param>
			public void SendMessageToReflector(Message message, Guid sessionId)
			{
				List<Guid> sessionIds = new List<Guid>();
				sessionIds.Add(sessionId);
				SendMessageToReflector(message, sessionIds);
			}

			public void SendMessageToReflector(Message message, List<Guid> sessionIds)
			{
        		//UNCOMMENT THIS TO SEE THE TOSTRING VERSION OF THE DATA YOU'RE SERIALIZING IN THE MESSAGE
				//Console.WriteLine("print message data types:");
				//foreach (object messageDataElement in message.Data)
				//{
				//    Console.WriteLine("message data type: " + messageDataElement.GetType().Name + " value: " + messageDataElement.ToString());
				//}
				
				MemoryStream ms = new MemoryStream();
				mBinaryFormatter.Serialize(ms, message);
				ms.Position = 0;

				//UNCOMMENT THIS TO SEE THE HEX VALUES OF THE MESSAGE YOU'RE GOING TO SEND TO THE CLIENT
				//Console.WriteLine("outgoing memory stream size: " + ms.Length);
				//byte[] byteArray = ms.ToArray();
				//Console.WriteLine("binary to hex: " + BitConverter.ToString(byteArray));
				//Console.Write("\n");
				//Console.WriteLine("-- BEGIN OUTGOING SERIALIZED --");
				//Console.WriteLine(System.Text.Encoding.UTF8.GetString(ms.GetBuffer()));
				//Console.WriteLine("-- END OUTGOING SERIALIZED --");
				mServerReflector.SendMessageToClient(ms, sessionIds);
			}

			public void SendMessageToAdminReflector(Message message, List<Guid> sessionIds)
			{
				//UNCOMMENT THIS TO SEE THE TOSTRING VERSION OF THE DATA YOU'RE SERIALIZING IN THE MESSAGE
				//Console.WriteLine("print message data types:");
				//foreach(object messageDataElement in message.Data)
				//{
				//	Console.WriteLine("message data type: " + messageDataElement.GetType().Name + " value: " + messageDataElement.ToString());
				//}

				MemoryStream ms = new MemoryStream();
				mBinaryFormatter.Serialize(ms, message);
				ms.Position = 0;

				//UNCOMMENT THIS TO SEE THE HEX VALUES OF THE MESSAGE YOU'RE GOING TO SEND TO THE CLIENT
				//Console.WriteLine("memory stream size: " + ms.Length);
				//byte[] byteArray = ms.ToArray();
				//Console.WriteLine("binary to hex: " + BitConverter.ToString( byteArray ));
				//Console.Write("\n");
				mAdminReflector.SendMessageToClient(ms, sessionIds);
			}
		#endregion

		#region Receive_Message_From_Reflector
			public void ReceiveMessageFromReflector(ConnectionEventArgs connectionEventArgsMessage)
			{
				connectionEventArgsMessage.MessagePacket.Position = 0;
                Message message = (Message)mBinaryFormatter.Deserialize(connectionEventArgsMessage.MessagePacket);
				Guid senderId = connectionEventArgsMessage.SenderId;
				List<Guid> recipients = connectionEventArgsMessage.Recipients;

				ProcessMessage(message, senderId, recipients);
			}
		#endregion

		//sender Id is assigned at the socketListener and is the message sender's sessionId
        protected void ProcessMessage(Message receivedMessage, Guid senderId, List<Guid> recipients)
        {
            switch (receivedMessage.MessageType)
            {
                case MessageType.Connect:
                    mServerStateMachine.ConnectionHandler.ReceiveRequest(receivedMessage, senderId);
                    break;
                case MessageType.Event:
                    ServerAccount account = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(senderId);
                    string accountId = "";
                    if (account != null)
                    {
                        accountId = account.AccountId.ToString();
                    }
                    Metrics.Log(receivedMessage, accountId);
                    break;
                case MessageType.Create:
                    break;
                case MessageType.Update:
					mServerStateMachine.ServerObjectRepository.ReceiveRequest(receivedMessage, senderId);
                    break;
                case MessageType.Room:
					mServerStateMachine.RoomManager.ReceiveRequest(receivedMessage, senderId);
                    break;
                case MessageType.Friends:
					mServerStateMachine.FriendsManager.ReceiveRequest(receivedMessage, senderId);
                    break;
                case MessageType.Admin:
					mServerStateMachine.AdminManager.ReceiveRequest(receivedMessage, senderId);
                    break;
                case MessageType.PaymentItems:
					mServerStateMachine.PaymentItemsManager.ReceiveRequest(receivedMessage, senderId);
					break;
				case MessageType.FashionMinigame:
					mServerStateMachine.FashionMinigameServer.ReceiveRequest(receivedMessage, senderId);
					break;
				case MessageType.Account:
					mServerStateMachine.UsersManager.ReceiveRequest(receivedMessage, senderId);
					break;
                case MessageType.AssetRepository:
					mServerStateMachine.ServerAssetRepository.ReceiveRequest(receivedMessage, senderId);
                    break;
				case MessageType.Heartbeat:
					//answer the heartbeat call
					ProcessHeartbeatMessage(receivedMessage, senderId);
					break;
				case MessageType.Escrow:
					mServerStateMachine.EscrowManager.ReceiveRequest(receivedMessage, senderId);
					break;
				default:
					mLogger.Error("Unable to process a message with unexpected MessageType (" + receivedMessage.MessageType + ")");
					break;
			}
			//foreach (object o in receivedMessage.Data)
			//{
			//    Console.WriteLine(o.ToString() + " : " + o.GetType().ToString());
			//}

		}

		private int mNumber = 0;
		private void FloodTheClient()
		{
			List<object> messageData = new List<object>();
			messageData.Add(mNumber);
			mNumber++;
			Message testMessage = new Message(MessageType.Connect, MessageSubType.RequestLogin, messageData);
            SendMessageToReflector(testMessage, new List<Guid>());
			FloodTheClient();
		}

		public void Dispose() { }

		private void ProcessHeartbeatMessage(Message receivedMessage, Guid senderId)
		{
			Message heartbeatMessage = new Message(MessageType.Heartbeat, receivedMessage.Data);

			SendMessageToReflector(heartbeatMessage, senderId);
		}
		
    }
}
