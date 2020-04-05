using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using Hangout.Shared.Messages;
using System.Xml;

namespace Hangout.Server
{
    public class FriendsManager : AbstractExtension
    {
        public delegate void FacebookFriendsReceivedEvent(ServerAccount serverAccount, Guid sessionId);
        public event FacebookFriendsReceivedEvent mFacebookFriendsReceivedEvent;

        public FriendsManager(ServerStateMachine serverStateMachine) : base (serverStateMachine)
        {
            mMessageActions.Add(MessageSubType.ReceiveFriends, SendFriendsListToClient);
			mMessageActions.Add(MessageSubType.ReceiveEntourage, SendEntourageListToClient);
        }

        private void SendFriendsListToClient(Message receivedMessage, Guid senderId)
        {
            //TODO: eventually we should rework this so the guid isn't even sent in the message
			//Guid sessionId = CheckType.TryAssignType<Guid>(receivedMessage.Data[0]);
            long facebookAccountId = CheckType.TryAssignType<long>(receivedMessage.Data[1]);
            string sessionKey = CheckType.TryAssignType<string>(receivedMessage.Data[2]);

            GetAllFacebookFriendsForUser(senderId, facebookAccountId, sessionKey, delegate(List<FacebookFriendInfo>friendInfos)
                {
                    List<object> friendsListMessageData = new List<object>();
                    foreach(FacebookFriendInfo friendInfo in friendInfos)
                    {
                        friendsListMessageData.Add(friendInfo.FirstName + " " + friendInfo.LastName);
                    }

                    Message sendFriendsListToClientMessage = new Message();
                    sendFriendsListToClientMessage.Callback = (int)MessageSubType.ReceiveFriends;
                    sendFriendsListToClientMessage.FriendsMessage(friendsListMessageData);
					SendMessageToClient(sendFriendsListToClientMessage, senderId);
                }
            );
        }

		private void SendEntourageListToClient(Message receivedMessage, Guid senderId)
		{
			ServerAccount serverAccount = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(senderId);
			
		    List<object> entourageListMessageData = new List<object>();
			foreach (FacebookFriendInfo friendInfo in serverAccount.HangoutFacebookFriends)
            {
                entourageListMessageData.Add(new Pair<string, string>(friendInfo.FirstName + " " + friendInfo.LastName, friendInfo.ImageUrl));
            }

            Message sendEntourageListToClientMessage = new Message();
            sendEntourageListToClientMessage.Callback = (int)MessageSubType.ReceiveEntourage;
            sendEntourageListToClientMessage.FriendsMessage(entourageListMessageData);
			SendMessageToClient(sendEntourageListToClientMessage, senderId);
		}

        public void GetAllFacebookFriendsForUser(Guid senderId, long facebookAccountId, string facebookSessionKey, Action<List<FacebookFriendInfo>> facebookFriendsCallback)
        {
            System.Action<XmlDocument> getFacebookFriendsServiceFinished = delegate(XmlDocument xmlResponse)
            {
                List<FacebookFriendInfo> friendInfos = null;
                if(FriendsXmlUtil.GetFriendsInfoFromXml(xmlResponse, out friendInfos))
                {
					facebookFriendsCallback(friendInfos);

					ServerAccount serverAccount = mServerStateMachine.SessionManager.GetServerAccountFromSessionId(senderId);
					//if the serverAccount is null, we don't do anything.. this can happen for example if the user logs out before the service call has time to return
					if (serverAccount != null)
					{
						mFacebookFriendsReceivedEvent(serverAccount, senderId);
					}
                }
                else
                {
					StateServerAssert.Assert(new Exception("There was a problem parsing the friends info out of the service returned xml: " + xmlResponse.OuterXml));
                }
            };

			FriendsManagerServiceAPI.GetAllFacebookFriends(facebookAccountId, facebookSessionKey, getFacebookFriendsServiceFinished);
        }
    }
}
