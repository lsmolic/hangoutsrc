/*=============================================================
(c) all rights reserved
================================================================*/

using System;
using PureMVC.Patterns;
using PureMVC.Interfaces;

using System.Collections;
using System.Collections.Generic;
using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class ChatMediator : Mediator
	{
		private ChatWindow mChatWindow = null;
        public ChatWindow ChatWindow
        {
            set { mChatWindow = value; } 
        }

		public ChatMediator()
			: base()
		{
		}

		public override void OnRemove()
		{
			base.OnRemove();

			if(mChatWindow != null)
			{
				mChatWindow.Showing = false;
				mChatWindow = null;
			}
		}

		public override IList<string> ListNotificationInterests()
		{
			List<string> interestList = new List<string>();

			interestList.Add(GameFacade.SEND_CHAT);
			interestList.Add(GameFacade.RECV_CHAT);
			interestList.Add(GameFacade.SEND_EMOTICON);
			interestList.Add(GameFacade.RECV_EMOTICON);
			interestList.Add(GameFacade.APPLICATION_EXIT);

			return interestList;
		}

		public override void HandleNotification(INotification notification)
		{
			// TODO:  fill in   
			switch (notification.Name)
			{
				case GameFacade.SEND_CHAT:
					break;
				case GameFacade.RECV_CHAT:
					break;
				case GameFacade.RECV_EMOTICON:
					break;
				case GameFacade.SEND_EMOTICON:
					break;
				case GameFacade.APPLICATION_EXIT:
					break;

			}
		}
		public void AddChatText(string chatText)
		{
			mChatWindow.AddChatText(chatText);
		}
	}
}
