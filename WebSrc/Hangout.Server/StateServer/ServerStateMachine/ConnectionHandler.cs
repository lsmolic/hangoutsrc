using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using System.Collections;
using log4net;
using System.Xml;
using System.Diagnostics;

namespace Hangout.Server
{
	//TODO: rename this to ServerConnectionManager
    public class ConnectionHandler : AbstractExtension
    {
        private static ILog mLogger = LogManager.GetLogger("ConnectionHandler");

        public ConnectionHandler(ServerStateMachine serverStateMachine) : base(serverStateMachine)
        {
			base.mMessageActions.Add(MessageSubType.Disconnect, HandleDisconnect);
			base.mMessageActions.Add(MessageSubType.RequestLogin, HandleLoginRequest);
		}

		private void HandleDisconnect(Message receivedMessage, Guid senderId)
		{
			Disconnect(senderId);
		}

		public void Disconnect(Guid sessionId)
		{
			mServerStateMachine.ProcessDisconnectSession(sessionId);
		}

        /// <summary>
        /// Handle login request from client
        /// </summary>
        /// <param name="loginMessage"></param>
		private void HandleLoginRequest(Message loginMessage, Guid senderId)
        {
			Guid sessionId = senderId; 
            string fbAccountId = CheckType.TryAssignType<string>(loginMessage.Data[0]);
            string fbSessionKey = CheckType.TryAssignType<string>(loginMessage.Data[1]);
            string nickName = CheckType.TryAssignType<string>(loginMessage.Data[2]);
			string firstName = CheckType.TryAssignType<string>(loginMessage.Data[3]);
			string lastName = CheckType.TryAssignType<string>(loginMessage.Data[4]);
			string campaignId = CheckType.TryAssignType<string>(loginMessage.Data[5]);
			string referrerId = CheckType.TryAssignType<string>(loginMessage.Data[6]);
            AvatarId avatarId = CheckType.TryAssignType<AvatarId>(loginMessage.Data[7]);

			Stopwatch loginTimer = new Stopwatch();
			loginTimer.Elapsed.Add(loginMessage.TimeSinceMessageWasCreated);
			loginTimer.Start();

			// TODO: Move the ipaddress handling within the extension
			string userIpAddress = mServerStateMachine.ServerMessageProcessor.ServerReflector.GetClientIPAddress(senderId);

			mLogger.Info(String.Format("HandleLoginRequest | sessionId={0} | fbAcctId={1} | fbSessionKey={2} | nickname={3} | firstname={4} | lastname={5} | avatarId={6} | ip={7}", 
                sessionId, fbAccountId, fbSessionKey, nickName, firstName, lastName, avatarId.ToString(), userIpAddress));

            Action<ServerAccount> getAccountForUserCallback = delegate(ServerAccount serverAccount)
            {
                mLogger.Debug("GetAccountForUserCallback");
                
                try
                {
	                if (serverAccount != null)
	                {
	                    mServerStateMachine.ProcessUserLogin(sessionId, serverAccount, fbSessionKey, avatarId, loginTimer);
	                }
	                else
	                {
                        mLogger.Warn("LoginFailed|sessionId=" + sessionId);
						Message loginFailMessage = new Message(MessageType.Connect, MessageSubType.InvalidLogin, new List<object>());
                        mServerStateMachine.SendMessageToReflector(loginFailMessage, sessionId);
	                }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.StackTrace currentStackTrace = new System.Diagnostics.StackTrace();                    
                    mLogger.Error("Got an exception in getAccountForUserCallback", ex);
                }
            };

			mServerStateMachine.UsersManager.GetAccountForUser(fbAccountId, fbSessionKey, nickName, firstName, lastName, userIpAddress, campaignId, referrerId, getAccountForUserCallback);
        }
    }
}
