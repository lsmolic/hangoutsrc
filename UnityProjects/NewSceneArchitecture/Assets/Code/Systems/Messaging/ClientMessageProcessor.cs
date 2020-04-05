using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Net;
using System.Timers;
using System.Xml;

using PureMVC.Interfaces;
using PureMVC.Patterns;

using Hangout.Shared;
using Hangout.Client.Gui;
using UnityEngine;

namespace Hangout.Client
{
    public class ClientMessageProcessor : Proxy, IMessageProcessor
    {

        private Hangout.Client.ClientReflector mClientReflector = null;
        private BinaryFormatter mBinaryFormatter = new BinaryFormatter();
        private IMessageRouter mClientObjectRepository = null;
        private Guid mLocalSessionId = Guid.Empty;
		private AccountId mLocalAccountId = null;
		private IScheduler mScheduler = null;
		private ITask mHeartbeatTask = null;

		public AccountId LocalAccountId
		{
			get
			{
				if(mLocalAccountId == null)
				{
					throw new ArgumentNullException("mLocalAccountId");
				}
				return mLocalAccountId;
			}
		}

        public Guid LocalSessionId
        {
            get
            {
                if (mLocalSessionId == Guid.Empty)
                {
                    throw new ArgumentException("LocalSessionId not set.");
                }
                return mLocalSessionId;
            }
        }

        public ClientMessageProcessor()
        {
			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			mClientReflector = new Hangout.Client.ClientReflector(mScheduler, delegate()
			{
				GameFacade.Instance.SendNotification(GameFacade.DISCONNECTED);
				ShowLostConnectionPopup();
			});

			mHeartbeatTask = mScheduler.StartCoroutine(HeartbeatCoroutine());
        }


		private ITask mRequestMessageFromReflectorTask = null;
        public bool Connect(string address, int port)
        {
			bool isConnected = mClientReflector.Connect(address, port);
			if(isConnected)
			{
				SendNotification(GameFacade.CONNECTED);
				mRequestMessageFromReflectorTask = mScheduler.StartCoroutine(RequestMessageFromReflector());
			}
			else
			{
				GameFacade.Instance.SendNotification(GameFacade.DISCONNECTED);
				ShowConnectFailedPopup(address, port);
			}
			return isConnected;
        }

		private void ShowLostConnectionPopup()
		{
			List<object> args = new List<object>();
			args.Add(Translation.CONNECTION_LOST);
			args.Add(Translation.CONNECT_RETRY_TEXT);
			Hangout.Shared.Action okcb = delegate()
			{
				GameFacade.Instance.SendNotification(GameFacade.REQUEST_CONNECT);
			};
			args.Add(okcb);
			//Hangout.Shared.Action cancelcb = delegate() { };
			//args.Add(cancelcb);
			//args.Add(Translation.RETRY); // optional. "Ok" is default
			GameFacade.Instance.SendNotification(GameFacade.SHOW_DIALOG, args);
		}

		private void ShowConnectFailedPopup(string address, int port)
		{
			List<object> args = new List<object>();
			args.Add(Translation.CONNECT_FAILED);
			args.Add(Translation.CONNECT_RETRY_TEXT);
			Hangout.Shared.Action okcb = delegate() 
			{
				GameFacade.Instance.SendNotification(GameFacade.REQUEST_CONNECT);
			};
			args.Add(okcb);
			//Hangout.Shared.Action cancelcb = delegate() { };
			//args.Add(cancelcb);
			//args.Add(Translation.RETRY); // optional. "Ok" is default
			GameFacade.Instance.SendNotification(GameFacade.SHOW_DIALOG, args);
		}

		private IEnumerator<IYieldInstruction> HeartbeatCoroutine()
		{
			Message heartbeatMessage = new Message(MessageType.Heartbeat, new List<object>());

			//send a heartbeat message to the server every 10 seconds?  this is subject to change
			int heartBeatCounter = 0;//we only want to send a timestamp every 10 seconds
			while (true)
			{
				heartbeatMessage.Data.Clear();
				++heartBeatCounter;
				if(heartBeatCounter == 10)
				{
					heartBeatCounter = 0;
					heartbeatMessage.Data.Add(DateTime.Now);
				}
				SendMessageToReflector(heartbeatMessage);
				yield return new YieldForSeconds(10.0f);
			}
		}

        public void Dispose()
        {
			mHeartbeatTask.Exit();
			if (mRequestMessageFromReflectorTask != null)
			{
				mRequestMessageFromReflectorTask.Exit();
			}
			
			if(mClientReflector != null)
			{
				mClientReflector.Dispose();
				mClientReflector = null;
			}
        }

		/// <summary>
		/// destroys all the cached local distributed objects
		/// </summary>
		public void DeleteClientCache()
		{
			if (mClientObjectRepository != null)
			{
				((ClientObjectRepository)mClientObjectRepository).DestroyAllDistributedObjects();
			}
		}

        public void SendMessageToReflector(Message message)
        {
            MemoryStream ms = new MemoryStream();
            mBinaryFormatter.Serialize(ms, message);
            ms.Position = 0;

            mClientReflector.SendMessage(ms);
        }

        private IEnumerator<IYieldInstruction> RequestMessageFromReflector()
        {
            while (true)
            {
                try
                {
                    MemoryStream nextMessage = mClientReflector.GetNextMessage();
                    while (nextMessage != null)
                    {
                        //UNCOMMENT THIS SO PRINT THE HEX VALUES OF THE MESSAGE WE JUST RECEIVED FROM THE REFLECTOR
						//UnityEngine.Debug.Log("memorystream length: " + nextMessage.Length);
						//byte[] byteArray = nextMessage.ToArray();
						//UnityEngine.Debug.Log("message hex value: " + BitConverter.ToString( byteArray ));

                        Message myReceivedMessage = null;
                        try
                        {
                            myReceivedMessage = (Message)mBinaryFormatter.Deserialize(nextMessage);
                        }
                        catch (SerializationException e)
                        {
							Console.WriteLine(e.ToString(), LogLevel.Error);
							continue;
                        }
                        ProcessMessage(myReceivedMessage);

                        nextMessage = mClientReflector.GetNextMessage();
                    }
                }
                catch (System.Runtime.Serialization.SerializationException e)
                {
					Hangout.Client.Console.LogError("SerializationException:" + e);
                }

                yield return new YieldForSeconds(0.05f);
            }
        }

        public void ProcessMessage(Message receivedMessage)
        {
            if (receivedMessage == null)
            {
                throw new ArgumentException("Error the received message is null");
            }

            switch (receivedMessage.MessageType)
            {
                case MessageType.Connect:
					if(receivedMessage.Callback == (int)MessageSubType.SuccessfulLogin)
					{
						HandleConnectionToStateServer(receivedMessage);
					}
					GameFacade.Instance.RetrieveProxy<ConnectionProxy>().ReceiveMessage(receivedMessage);

                    break;
                case MessageType.Create:
                    if (mClientObjectRepository == null)
                    {
                        throw new ArgumentException("Error. Client Object Repository is null, in handler for Create, message=" + receivedMessage.ToString());
                    }
                    mClientObjectRepository.ReceiveMessage(receivedMessage);
                    break;
                case MessageType.Update:
                    if (mClientObjectRepository == null)
                    {
                        throw new Exception("Error. Client Object Repository is null, in handler for Update");
                    }
                    mClientObjectRepository.ReceiveMessage(receivedMessage);
                    break;
                case MessageType.Delete:
                    if (mClientObjectRepository == null)
                    {
                        throw new Exception("Error. Client Object Repository is null, in handler for Delete");
                    }
                    mClientObjectRepository.ReceiveMessage(receivedMessage);
                    break;
                case MessageType.Loading:
                    GameFacade.Instance.RetrieveProxy<RoomManagerProxy>().ReceiveMessage(receivedMessage);
                    break;
                case MessageType.Room:
                    GameFacade.Instance.RetrieveMediator<RoomAPIMediator>().ReceiveMessage(receivedMessage);
                    break;
                case MessageType.Friends:
                    GameFacade.Instance.RetrieveMediator<FriendsMediator>().ReceiveMessage(receivedMessage);
                    break;
                case MessageType.PaymentItems:
					GameFacade.Instance.RetrieveProxy<InventoryProxy>().ReceiveMessage(receivedMessage);
                    break;
				case MessageType.FashionMinigame:
					FashionGame.FashionGameCommands.ProcessMessage(receivedMessage);
					break;
				case MessageType.AssetRepository:
					GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().ReceiveMessage(receivedMessage);
					break;
				case MessageType.Heartbeat:
					//check if we've included a timestamp and if we have, log the delta between the time now and the time the message was sent
					if(receivedMessage.Data.Count > 0)
					{
						ILogger logger = GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger;
						TimeSpan timeSpan = (TimeSpan)(DateTime.Now - CheckType.TryAssignType<DateTime>(receivedMessage.Data[0]));
						string logString = "Ping for client: " + mLocalSessionId + " : " + timeSpan.Duration().ToString();
						logger.LogLine(logString);
						Console.Log(logString);
					}
					break;
				case MessageType.Error:
					GameFacade.Instance.RetrieveMediator<ErrorHandlerMediator>().ReceiveMessage(receivedMessage);
					break;
				case MessageType.Escrow:
					GameFacade.Instance.RetrieveMediator<EscrowMediator>().ReceiveMessage(receivedMessage);
					break;
            }
        }

		/// <summary>
		/// Client has successfully connected to the server.  Prompt
        /// the user to login
		/// </summary>
		/// <param name="receivedMessage"></param>
        private void HandleConnectionToStateServer(Message receivedMessage)
        {
            mLocalSessionId = (Guid)receivedMessage.Data[0];
			mLocalAccountId = (AccountId)receivedMessage.Data[2];
            mClientObjectRepository = new ClientObjectRepository(mLocalSessionId);
            mClientObjectRepository.RegisterSendMessageCallback(SendMessageToReflector);
        }

		/// <summary>
		/// Make a call to web services
		/// </summary>
		/// <param name="path">Fully resolved path for the current stage</param>
		/// <param name="callback">Callback that accepts an XmlDocument</param>
		public void CallToService(string path, Action<XmlDocument> callback)
		{
			GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(GetServiceResult(path, callback));
		}

		/// <summary>
		/// Downloads text from a URL without caching it in Unity (browser will still cache normally)
		/// </summary>
		private IEnumerator<IYieldInstruction> GetServiceResult(string url, Action<XmlDocument> wwwDataResult)
		{
			SpooledWWW assetWWW = new SpooledWWW(url);
			yield return new YieldForSpooledWWW(assetWWW);
			if (!String.IsNullOrEmpty(assetWWW.result.error))
			{
				Console.WriteLine("Error while trying to download text: " + assetWWW.result.error + " From path: " + url);
			}
			XmlDocument wwwDoc = new XmlDocument();
            wwwDoc.LoadXml(assetWWW.result.data);
			GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger.Log("GetServiceResult xml = " + wwwDoc.OuterXml, LogLevel.Info);

			wwwDataResult(wwwDoc);
		}

    }
}
