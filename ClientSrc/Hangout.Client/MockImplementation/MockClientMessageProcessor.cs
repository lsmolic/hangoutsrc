using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Timers;
using System.Xml;

using PureMVC.Interfaces;
using PureMVC.Patterns;

using Hangout.Shared;

namespace Hangout.Client
{
    public class MockClientMessageProcessor : Mediator,  IMessageProcessor
    {
        private PaymentItemsCommand mPaymentItemCommand = null;
        private BinaryFormatter mBinaryFormatter = new BinaryFormatter();
        private Hangout.Client.ClientReflector mClientReflector = null;
        private Guid mLocalSessionId = Guid.Empty;
        private string mLocalAccountId = "";
        private IScheduler mScheduler = null;
        private string mStateServerAddress = "";
        private int mStateServerPort = -1;
        private Object lockObject = new Object();


        public MockClientMessageProcessor(string stateServerAddress, int stateServerPort)
        {
            mStateServerAddress = stateServerAddress;
            mStateServerPort = stateServerPort;
        }
        

        // Maps message names to Action deleagtes to call when receiving message
        private Dictionary<MessageType, Action<Message>> mMessageTypes = new Dictionary<MessageType, Action<Message>>();

        internal void RegisterMessageType(MessageType messageType, Action<Message> callback)
        {
            lock (lockObject)
            {
                mMessageTypes[messageType] = callback;
            }
        }

        internal void UnRegisterMessageType(MessageType messageType)
        {
            lock (lockObject)
            {
                mMessageTypes.Remove(messageType);
            }
        }

        internal void UnRegisterAllMessages()
        {
            mMessageTypes.Clear();
        }

        public string SessionId
        {
            get { return mLocalSessionId.ToString(); }
        }

        public void SetSessionId(Guid sessionId)
        {
            if ((mLocalSessionId == Guid.Empty) || (mLocalSessionId != sessionId))
            {
                mLocalSessionId = sessionId;
            }
        }

        public void StartReflector(GameFacade mInstance)
        {
            if (mClientReflector == null)
	      	{
                string stateServerAddress = mStateServerAddress; //"127.0.0.1";  //"64.106.173.25";
				int stateServerPort = mStateServerPort;  //8000;

				Console.WriteLine(stateServerAddress + ":" + stateServerPort);

                mScheduler = mInstance.RetrieveMediator<SchedulerMediator>().Scheduler;

                mClientReflector = new Hangout.Client.ClientReflector(mScheduler, delegate()
                {
                    
                });

                mClientReflector.Connect(stateServerAddress, stateServerPort);

                mPaymentItemCommand = new PaymentItemsCommand(GetMyIpAddress());
	       	}               
		}

        public void DisconnectReflector()
        {
            //mClientReflector.Disconnect();
            mClientReflector.Dispose();
            mClientReflector = null;
        }

        public void ProcessPaymentItemCommand(string command,  Dictionary<string, string> commandArgs)
        {
            string paymentItemsCommand = mPaymentItemCommand.CreatePaymentCommand(command, commandArgs);   

            Message PaymentItemsMessage = new Message();
            List<object> dataObject = new List<object>();
            dataObject.Add(paymentItemsCommand  );
            PaymentItemsMessage.PaymentItemsMessage(dataObject);

            SendMessageToReflector(PaymentItemsMessage);
        }

        public string CreatePaymentCommand(string command, Dictionary<string, string> commandArgs)
        {
            return ( mPaymentItemCommand.CreatePaymentCommand(command, commandArgs));
        }
        

        public void LaunchMoneyAccountFunding ()
        {
            mPaymentItemCommand.LaunchMoneyAccountFunding();
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
        
        public void RequestMessageFromReflector()
        {
            if (mClientReflector != null)
            {
                MemoryStream nextMessage = mClientReflector.GetNextMessage();
                if (nextMessage != null)
                {
                    Message myReceivedMessage = (Message)mBinaryFormatter.Deserialize(nextMessage);
                    ProcessMessage(myReceivedMessage);
                    RequestMessageFromReflector();
                }
            }
            }

        protected void ProcessMessage(Message receivedMessage)
        {
            MessageType messageType = receivedMessage.MessageType;

            Action<Message> callback = null;
            lock (lockObject)
            {
                if (mMessageTypes.TryGetValue(messageType, out callback))
                {
                    UnRegisterMessageType(messageType);
                // Call the Action stored for this message
                    callback(receivedMessage);
                }
                else
                {
                    ProcessNonregisteredMessage(receivedMessage);
                }
            }
        }

        private void ProcessNonregisteredMessage(Message receivedMessage)
        {
             switch (receivedMessage.MessageType)
            {
                case MessageType.Create:
                    Console.WriteLine("Create object with doId: " + receivedMessage.DistributedObjectId);
                    break;
                case MessageType.Connect:
					Console.Write("Received Login Message from server...");
					if(receivedMessage.Data[0] != null)
					{
                        mLocalSessionId = (Guid)receivedMessage.Data[0];
                        Console.Write(receivedMessage.Data[0].ToString() + "\n\n");
                        switch ((MessageSubType)receivedMessage.Callback)
                        { 
                            case MessageSubType.SuccessfulLogin:
                                mLocalAccountId = (string)receivedMessage.Data[2].ToString();
                                ProcessPaymentItemCommand("SecurePaymentInfo", new Dictionary<string, string>());
                                break;
                         }
					}
                    break;  

                case MessageType.PaymentItems:
                    if (receivedMessage.Data[0] != null)
                    {
                        mPaymentItemCommand.ResponseHandler((string)receivedMessage.Data[0]);
                    }
                    break;


            }
        }

        public void LoginUser()
        {
            List<object> dataObject = new List<object>();
            dataObject.Add(DeveloperSettings.Instance().MockFacebookAccountId);
            dataObject.Add(DeveloperSettings.Instance().MockFacebookSessionKey);
            dataObject.Add(DeveloperSettings.Instance().MockFacebookNickName);
			dataObject.Add(DeveloperSettings.Instance().MockFirstName);
			dataObject.Add(DeveloperSettings.Instance().MockLastName);
            dataObject.Add(DeveloperSettings.Instance().CampaignId);
            dataObject.Add(DeveloperSettings.Instance().ReferrerId);

            dataObject.Add(DeveloperSettings.Instance().MockDefaultAvatarId);

            Message loginMsg = new Message(MessageType.Connect, MessageSubType.RequestLogin, dataObject);
            SendMessageToReflector(loginMsg);
        }

        public string GetMyIpAddress()
        {
            string ipAddress = "";
            try
            {
                ipAddress = mClientReflector.ClientIpAddress;
            }
            catch { }

            return ipAddress;
        }
    }

    public class SchedulerMediator : Mediator
    {
        private readonly IScheduler mScheduler;
        public IScheduler Scheduler
        {
            get { return mScheduler; }
        }

        public SchedulerMediator(IScheduler scheduler)
        {
            mScheduler = scheduler;
        }

    }

    public class DeveloperSettings
    {
        private const string SETTINGS_PATH = "..\\..\\..\\..\\..\\UnityProjects\\NewSceneArchitecture\\Assets\\resources\\DeveloperSettings.tweaks.xml";

        private long mMockFacebookAccountId;

        private string mMockFacebookSessionKey;

        private string mMockFacebookNickName;

		private string mMockFirstName;

		private string mMockLastName;

		private string mCampaignId;

        private string mReferrerId;

        private AvatarId mMockDefaultAvatarId;

        private static DeveloperSettings mInstance = null;

        public string MockFacebookNickName
        {
            get { return mMockFacebookNickName; }
        }

		public string MockFirstName
		{
			get { return mMockFirstName; }
		}

		public string MockLastName
		{
			get { return mMockLastName; }
		}

		public string CampaignId
		{
			get { return mCampaignId; }
		}

        public string ReferrerId
        {
            get { return mReferrerId; }
        }

        public string MockFacebookSessionKey
        {
            get { return mMockFacebookSessionKey; }
        }

        public long MockFacebookAccountId
        {
            get { return mMockFacebookAccountId; }
        }

        
        public AvatarId MockDefaultAvatarId
        {
            get { return mMockDefaultAvatarId; }
        }

        public DeveloperSettings()
        {
            try
            {
                XmlDocument tweaks = new XmlDocument();
                tweaks.Load(SETTINGS_PATH);
                string mockFacebookAccountId = tweaks.SelectSingleNode("/Test/Tweakable[@name='Mock Facebook AccountId']").Attributes["value"].InnerText;
                mMockFacebookSessionKey = tweaks.SelectSingleNode("/Test/Tweakable[@name='Mock Facebook SessionKey']").Attributes["value"].InnerText;
                mMockFacebookNickName = tweaks.SelectSingleNode("/Test/Tweakable[@name='Mock Facebook NickName']").Attributes["value"].InnerText;
				mMockFirstName = tweaks.SelectSingleNode("/Test/Tweakable[@name='Mock FirstName']").Attributes["value"].InnerText;
				mMockLastName = tweaks.SelectSingleNode("/Test/Tweakable[@name='Mock LastName']").Attributes["value"].InnerText;
                mCampaignId = tweaks.SelectSingleNode("/Test/Tweakable[@name='CampaignId']").Attributes["value"].InnerText;
                mReferrerId = tweaks.SelectSingleNode("/Test/Tweakable[@name='ReferrerId']").Attributes["value"].InnerText;

                mMockFacebookAccountId = Convert.ToInt64(mockFacebookAccountId);
                mMockDefaultAvatarId = new AvatarId(1);
            }
            catch { }
       }

        public static DeveloperSettings Instance()
        {
            if (mInstance == null)
            {
                mInstance = new DeveloperSettings();
            }
            return mInstance;
        }

    }
 }
