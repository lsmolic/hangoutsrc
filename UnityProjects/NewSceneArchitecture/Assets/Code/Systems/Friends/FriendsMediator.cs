using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using Hangout.Client.Gui;
using Hangout.Shared;
using Hangout.Shared.Messages;

namespace Hangout.Client
{
    public class FriendsMediator : Mediator
    {
        private FriendsGui mFriendsGui = null;
		private EntourageGui mEntourageGui = null;
        private Dictionary<MessageSubType, Action<Message>> mMessageActions = new Dictionary<MessageSubType, Action<Message>>();


        public FriendsMediator()
        {
            IGuiManager guiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
            mFriendsGui = new FriendsGui(guiManager);
            mFriendsGui.Showing = false;

			mEntourageGui = new EntourageGui(guiManager);
			mEntourageGui.Showing = false;

            mMessageActions.Add(MessageSubType.ReceiveFriends, ReceiveFriends);
			mMessageActions.Add(MessageSubType.ReceiveEntourage, ReceiveEntourage);
        }

        public override IList<string> ListNotificationInterests()
        {
            List<string> interestList = new List<string>();

            interestList.Add(GameFacade.SHOW_FRIENDS);
			interestList.Add(GameFacade.SHOW_ENTOURAGE);
			interestList.Add(GameFacade.CLOSE_ALL_WINDOWS);

            return interestList;
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
				case GameFacade.CLOSE_ALL_WINDOWS:
					mEntourageGui.Showing = false;
					break;
                case GameFacade.SHOW_FRIENDS:
                    FriendsCommands.GetFriendsList(1520031799L, "48217ab261f65b06468459a7-1520031799");
                    mFriendsGui.Showing = true;
                    break;
				case GameFacade.SHOW_ENTOURAGE:
					FriendsCommands.GetEntourageList();
					mEntourageGui.Showing = !mEntourageGui.Showing;
					break;
            }
        }

        public void ReceiveMessage(Message receivedMessage)
        {
            MessageUtil.ReceiveMessage<MessageSubType>(receivedMessage, mMessageActions);
        }

        private void ReceiveFriends(Message receivedMessage)
        {
            List<string> friends = GetFriendsFromMessageData<string>(receivedMessage.Data);
            if (mFriendsGui.Showing)
            {
                mFriendsGui.ListFriends(friends);
            }
        }

		private void ReceiveEntourage(Message receivedMessage)
		{
			List<Pair<string, string>> friends = GetFriendsFromMessageData<Pair<string, string>>(receivedMessage.Data);
			if (mEntourageGui.Showing)
			{
				mEntourageGui.ListEntourage(friends);
			}
		}

        private List<T> GetFriendsFromMessageData<T>(List<object> messageDataObjects)
        {
            List<T> friends = new List<T>();
            foreach(object messageDataObject in messageDataObjects)
            {
                T friendObject = (T)messageDataObject;
				friends.Add(friendObject);
            }

            return friends;
        }

    }
}
