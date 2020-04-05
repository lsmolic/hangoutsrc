using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Client
{
    public static class FriendsCommands
    {
        public static void GetFriendsList(long facebookAccountId, string sessionKey)
        {
            ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
            Message requestFriendsMessage = new Message();
            requestFriendsMessage.Callback = (int)MessageSubType.ReceiveFriends;
            List<object> messageData = new List<object>();
            messageData.Add(facebookAccountId);
            messageData.Add(sessionKey);
            requestFriendsMessage.FriendsMessage(messageData);

            clientMessageProcessor.SendMessageToReflector(requestFriendsMessage); 
        }

		public static void GetEntourageList()
		{
			ClientMessageProcessor clientMessageProcessor = GameFacade.Instance.RetrieveProxy<ClientMessageProcessor>();
			Message requestFriendsMessage = new Message();
			requestFriendsMessage.Callback = (int)MessageSubType.ReceiveEntourage;
			requestFriendsMessage.FriendsMessage(new List<object>());

			clientMessageProcessor.SendMessageToReflector(requestFriendsMessage); 
		}
    }
}
