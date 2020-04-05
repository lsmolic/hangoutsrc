using System;
using System.Collections.Generic;
using System.Xml;
using Hangout.Server.StateServer;
using Hangout.Shared;
using log4net;
using System.Threading;
using System.Diagnostics;

namespace Hangout.Server
{
    public class ServerStateMachine
    {
        private ILog mLogger;

        private string mStateServerId;
        public string StateServerId
        {
            get { return mStateServerId; }
            set { mStateServerId = value; }
        }

		private ConnectionHandler mConnectionHandler = null;
		public ConnectionHandler ConnectionHandler
		{
			get { return mConnectionHandler; }
		}

		private ZoneIdManager mZoneManager = null;
        public Hangout.Server.ZoneIdManager ZoneManager
        {
            get { return mZoneManager; }
        }

        
        private AvatarManager mAvatarManager = null;
        public AvatarManager AvatarManager
        {
            get { return mAvatarManager; }
        }

        private UsersManager mUsersManager = null;
        public UsersManager UsersManager
        {
            get { return mUsersManager; }
        }
        
        private PaymentItemsManager mPaymentItemsManager = null;
        public PaymentItemsManager PaymentItemsManager
        {
            get { return mPaymentItemsManager; }
        }

        private FashionMinigameServer mFashionMinigameServer = null;
		public FashionMinigameServer FashionMinigameServer
		{
			get { return mFashionMinigameServer; }
		}

        private ServerMessageProcessor mServerMessageProcessor = null;
        public ServerMessageProcessor ServerMessageProcessor
        {
            get { return mServerMessageProcessor; }
        }
        
        private ServerObjectRepository mServerObjectRepository = null;
        public ServerObjectRepository ServerObjectRepository
        {
            get { return mServerObjectRepository; }
        }

        private DistributedObjectIdManager mDistributedObjectIdManager = null;
        public DistributedObjectIdManager DistributedObjectIdManager
        {
            get { return mDistributedObjectIdManager; }
        }

        private ServerEngine mServerEngine = null;
        public ServerEngine ServerEngine
        {
            get { return mServerEngine; }
        }

        private ServerAssetRepository mServerAssetRepository = null;
        public ServerAssetRepository ServerAssetRepository
        {
            get { return mServerAssetRepository; }
        }

        private RoomManager mRoomManager = null;
        public RoomManager RoomManager
        {
            get { return mRoomManager; }
        }

        private FriendsManager mFriendsManager = null;
        public FriendsManager FriendsManager
        {
            get { return mFriendsManager; }
        }

		private EscrowManager mEscrowManager = null;
		public EscrowManager EscrowManager
		{
			get { return mEscrowManager; }
		}

        private SessionManager mSessionManager = null;
        private AdminManager mAdminManager = null;
        public AdminManager AdminManager
        {
            get { return mAdminManager; }
        }

        public SessionManager SessionManager
        {
            get { return mSessionManager; }
        }

		private ServerProcessingLoop mServerProcessingLoop = null;

        private ConfigManagerStateServer mConfigManager = null;
        public ConfigManagerStateServer ConfigManager
        {
            get { return mConfigManager; }
        }

        public ServerStateMachine()
        {
			//at this point, we should still be running on the main thread
			mServerProcessingLoop = new ServerProcessingLoop();
			mServerProcessingLoop.AddServerLoopWorker(WebServiceRequest.WebServiceRequestManager);
            mLogger = LogManager.GetLogger("ServerStateMachine");
		}

		public virtual void RunForever()
		{
			mConfigManager = new ConfigManagerStateServer();
			mServerAssetRepository = new ServerAssetRepository(this, delegate()
				{
					ContinueStateMachineSetup();
				});

			mServerProcessingLoop.StartLoop();
        }

		private void ContinueStateMachineSetup()
		{
			mZoneManager = new ZoneIdManager();
			mSessionManager = new SessionManager();
			mUsersManager = new UsersManager(this);
			mServerObjectRepository = new ServerObjectRepository(this);
			mDistributedObjectIdManager = new DistributedObjectIdManager();
			mServerEngine = new ServerEngine(mSessionManager, mServerObjectRepository, SendMessageToReflector);
			mConnectionHandler = new ConnectionHandler(this);

			// Extensions
			mRoomManager = new RoomManager(this);
            mFashionMinigameServer = new FashionMinigameServer(this);
			mFriendsManager = new FriendsManager(this);
            mFriendsManager.mFacebookFriendsReceivedEvent += mFashionMinigameServer.FacebookFriendsReady;
			mAvatarManager = new AvatarManager(this);
			mPaymentItemsManager = new PaymentItemsManager(this);
			mEscrowManager = new EscrowManager(this);
			mServerProcessingLoop.AddServerLoopWorker(RemoteCallToService.CallToServiceCallbackManager);
            mAdminManager = new AdminManager(this);

            // This needs to be called to instantiate the instance of TextFilter
			TextFilter tf = TextFilter.Instance;

			//this should be the last thing we create in our startup sequence!!  the server message processor kicks off a loop to process incoming messages
			mServerMessageProcessor = new ServerMessageProcessor(this);
			mServerProcessingLoop.AddServerLoopWorker(mServerMessageProcessor.ServerReflector);
        }

        /// <summary>
        /// Called by ConnectionHandler on successful login
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="userAccount"></param>
        /// <param name="facebookSessionKey"></param>
        /// <param name="defaultAvatarId"></param>
        public void ProcessUserLogin(Guid sessionId, ServerAccount userAccount, string facebookSessionKey, AvatarId defaultAvatarId, Stopwatch loginTimer)
        {
            mSessionManager.AddSession(sessionId, userAccount);
            BossServerAPI.RegisterNewSession(userAccount.AccountId, sessionId.ToString(), "1", ZoneId.LimboZone.ToString(), mStateServerId, delegate(XmlDocument xmlDocument) { });
            BossServerAPI.UpdateStateServer(mStateServerId, mServerMessageProcessor.ServerReflector.GetNumConnections(), "1", delegate(XmlDocument xmlDocument) { });

            mAvatarManager.GetAvatar(sessionId, ZoneId.LimboZone, userAccount, defaultAvatarId, delegate(bool successfullyGotAvatar)
            {
                //send an error message to the client if we can't get or create an avatar
                if (successfullyGotAvatar == false)
                {
                    LoginError(sessionId, ErrorIndex.CannotGetOrCreateAvatar, MessageSubType.UserLoginGetOrCreateAvatarError);
                }
                else 
                {
                    // We got all the required data for the avatar.   Send a login success message.   
                    // Check for the room and get the facebook friends in the background, as these calls take too long and aren't required to let the user continue.
					LoginSuccess(sessionId, loginTimer);

                    // Check if a user has a default room, and create a room for the user if it doesn't exist
                    mRoomManager.GetOrCreateDefaultRoomForUser(sessionId, userAccount, delegate(IServerDistributedRoom room)
                    {
                        if (room == null)
                        {
                            //send an error message to the client if we can't get or create a room
                            LoginError(sessionId, ErrorIndex.CannotGetOrCreateRoom, MessageSubType.UserLoginGetOrCreateRoomError);
                        }
                        else
                        {
                            // Get facebook friends, and cache it on the account.  We have to do this now before the session id for facebook expires
                            mFriendsManager.GetAllFacebookFriendsForUser(sessionId, userAccount.FacebookAccountId, facebookSessionKey, delegate(List<FacebookFriendInfo> friendInfos)
                            {
                                userAccount.AddFacebookFriends(friendInfos);
                            });
                        }
                    });
                }
            });
        }

		private void LoginSuccess(Guid sessionId, Stopwatch loginTimer)
        {
            List<object> loginSuccessMessageData = new List<object>();
            ServerAccount serverAccount = mSessionManager.GetServerAccountFromSessionId(sessionId);
			if (serverAccount != null)
			{
				loginSuccessMessageData.Add(sessionId);
				loginSuccessMessageData.Add(serverAccount.UserProperties);
				loginSuccessMessageData.Add(serverAccount.AccountId);

				Message loginSuccessMessage = new Message(MessageType.Connect, MessageSubType.SuccessfulLogin, loginSuccessMessageData);

				SendMessageToReflector(loginSuccessMessage, sessionId);
                Metrics.Log(LogGlobals.CATEGORY_CONNECTION, LogGlobals.EVENT_LOGIN, LogGlobals.LOGIN_SUCCESS, serverAccount.AccountId.ToString());
				loginTimer.Stop();
				mLogger.InfoFormat("LoginSuccess |sessionId={0} | accountId={1} | Time Taken={2}", sessionId, serverAccount.AccountId.ToString(), loginTimer.Elapsed.Duration());
			}
			else
			{
				LoginError(sessionId, ErrorIndex.CannotGetAccountFromSessionManager, MessageSubType.UserLoginCannotGetAccountFromSessionManagerError);
			}
        }

		private void LoginError(Guid sessionId, ErrorIndex errorIndex, MessageSubType errorActionType)
		{
            ServerAccount serverAccount = mSessionManager.GetServerAccountFromSessionId(sessionId);
            string accountId = "unknown account";
            if (serverAccount != null) 
            {
                accountId = serverAccount.AccountId.ToString();
            }
            mLogger.Warn(String.Format("LoginError | sessionId={0} | accountId={1}", sessionId, accountId));
            Metrics.Log(LogGlobals.CATEGORY_CONNECTION, LogGlobals.EVENT_LOGIN, LogGlobals.LOGIN_FAILED, accountId);
            Message loginErrorMessage = StateServerError.ErrorToUser(errorIndex, errorActionType);
			SendMessageToReflector(loginErrorMessage, sessionId);
			DisconnectUser(sessionId);
		}

        public void SendMessageToReflector(Message message, Guid sessionId)
        {
            List<Guid> sessionIds = new List<Guid>();
            sessionIds.Add(sessionId);
            mServerMessageProcessor.SendMessageToReflector(message, sessionIds);
        }

        public void SendMessageToReflector(Message message, List<Guid> sessionIds)
        {
            mServerMessageProcessor.SendMessageToReflector(message, sessionIds);
        }

        public void SendMessageToAdminReflector(Message message, Guid sessionId)
        {
            List<Guid> sessionIds = new List<Guid>();
            sessionIds.Add(sessionId);
            mServerMessageProcessor.SendMessageToAdminReflector(message, sessionIds);
        }

        public void SendMessageToAdminReflector(Message message, List<Guid> sessionIds)
        {
            mServerMessageProcessor.SendMessageToAdminReflector(message, sessionIds);
        }

		/// <summary>
		/// this function is meant to be a server side trigger to disconnect a client (e.g. booted from a room)
		/// </summary>
		private void DisconnectUser(Guid sessionToDisconnect)
		{
			mServerMessageProcessor.ServerReflector.ForceClientDisconnect(sessionToDisconnect);
		}

        public void ProcessDisconnectSession(Guid sessionIdToClose)
        {
            IServerDistributedRoom roomToLeave = mRoomManager.FindRoomForUser(sessionIdToClose);
            if (roomToLeave != null)
            {
                mRoomManager.LeaveRoom(sessionIdToClose, roomToLeave);
            }
            ServerAccount serverAccount = mSessionManager.GetServerAccountFromSessionId(sessionIdToClose);
            if (serverAccount != null)
            {
                BossServerAPI.RemoveSession(serverAccount.AccountId, sessionIdToClose.ToString(), delegate(XmlDocument xmlDocument) { });
            }
            BossServerAPI.UpdateStateServer(mStateServerId, mServerMessageProcessor.ServerReflector.GetNumConnections(), "1", delegate(XmlDocument xmlDocument) { });

            mServerEngine.ProcessDisconnectSession(sessionIdToClose);
        }

		private void StartLoading(Guid sessionId, RoomId roomId)
		{
            Message message = mRoomManager.BeginLoadingRoomMessage(roomId);
			SendMessageToReflector(message, sessionId);
		}
    }

	// For unit testing
	public class TestServerStateMachine : ServerStateMachine
	{
		public TestServerStateMachine()
		{
		}

		public void SendMessageToReflector(Message message, Guid sessionId)
		{
		}

		public void SendMessageToReflector(Message message, List<Guid> sessionIds)
		{
		}
	}
}
