/*=============================================================
(c) all rights reserved
================================================================*/
using UnityEngine;

using PureMVC.Interfaces;
using PureMVC.Patterns;

using System;
using System.Collections;
using System.Collections.Generic;

using Hangout.Client.Gui;
using Hangout.Shared;

namespace Hangout.Client
{
	public class LoginMediator : Mediator, IMediator
	{
        private GuiController mLoadingGui;

        private const string mTopLoginWindowName = "LoginWindow";
		private const string mUsernameTextboxName = "LoginUsernameTextbox";
		private const string mPasswordTextboxName = "LoginPasswordTextbox";
		private const string mSendButtonName = "SendButton";

		private const string mResourcePath = "resources://GUI/Login/Login.gui";
		private const string mLoadingScreenGuiPath = "resources://GUI/Minigames/Fashion/LoadingScreen.gui";

        private Window mLoginWindow = null;

		private Textbox mUsernameTextbox = null;
		private Textbox mPasswordTextbox = null;
		private Button mSendButton = null;

		// Action<string, string> is used as delegate(string login, string password)
		//private List<Hangout.Shared.Action<string, string>> mSendLoginCallbacks = new List<Hangout.Shared.Action<string, string>>();

		public override IList<string> ListNotificationInterests()
		{
			List<string> interestList = new List<string>();
            interestList.Add(GameFacade.APPLICATION_INIT);
            interestList.Add(GameFacade.CONNECTED);
			interestList.Add(GameFacade.LOGIN_SUCCESS);
			interestList.Add(GameFacade.LOGIN_FAILED);
			return interestList;
		}

		private float mInitTime;
		public override void HandleNotification(INotification notification)
		{
			switch (notification.Name)
			{
                case GameFacade.APPLICATION_INIT:
					mInitTime = Time.realtimeSinceStartup;
                    ShowSplashScreen();
                    break;

				case GameFacade.CONNECTED:
                    SendLoginCredentials();
                    EventLogger.Log(LogGlobals.CATEGORY_CONNECTION, LogGlobals.EVENT_CONNECTED);
					//ShowLoginWindow();
					break;

				case GameFacade.LOGIN_SUCCESS:
                    Console.WriteLine("Login Success (" + (Time.realtimeSinceStartup - mInitTime).ToString("f1") + "s)");
					HideSplashScreen();
					break;

				case GameFacade.LOGIN_FAILED:
					ShowLoginFailedPopup();
					break;
			}
		}

        private void ShowSplashScreen()
        {
			IGuiManager guiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
			mLoadingGui = new GuiController(guiManager, mLoadingScreenGuiPath);
            Window mainWindow = (Window)mLoadingGui.MainGui;
            mainWindow.InFront = true;
		}

        private void HideSplashScreen()
        {
            if (mLoadingGui != null)
            {
				mLoadingGui.Dispose();
				mLoadingGui = null;
            }
        }

		private void ConstructGui(IGuiManager guiManager)
		{
			XmlGuiFactory factory = new XmlGuiFactory(mResourcePath, guiManager);
			IEnumerable allXmlGuiElements = factory.ConstructAllElements();
			foreach (IGuiElement element in allXmlGuiElements)
			{
				if (element is Window && element.Name == mTopLoginWindowName)
				{
					mLoginWindow = (Window)element;
					break;
				}
			}

			if (mLoginWindow == null)
			{
				throw new ArgumentNullException("No top level Login Window named: " + mTopLoginWindowName + " in the gui xml: " + mResourcePath);
			}
			mLoginWindow.Showing = false;


			mUsernameTextbox = mLoginWindow.SelectSingleElement<IGuiElement>("**/" + mUsernameTextboxName) as Textbox;
			if (mUsernameTextbox == null)
			{
				throw new ArgumentNullException("No textbox named: " + mUsernameTextboxName);
			}

			mPasswordTextbox = mLoginWindow.SelectSingleElement<IGuiElement>("**/" + mPasswordTextboxName) as Textbox;
			if (mPasswordTextbox == null)
			{
				throw new ArgumentNullException("No textbox named: " + mPasswordTextboxName);
			}

			mSendButton = mLoginWindow.SelectSingleElement<IGuiElement>("**/" + mSendButtonName) as Button;
			if (mSendButton == null)
			{
				throw new ArgumentNullException("No textbox named: " + mSendButtonName);
			}

			mSendButton.AddOnPressedAction(SendLoginCredentials);
		}

		private void SendLoginCredentials()
		{
            ConnectionProxy connectionProxy = GameFacade.Instance.RetrieveProxy<ConnectionProxy>();
            List<object> data = new List<object>();
            data.Add(connectionProxy.FacebookAccountId);
            data.Add(connectionProxy.SessionKey);
            data.Add(connectionProxy.NickName); //nickname
			data.Add(connectionProxy.FirstName); //first name
			data.Add(connectionProxy.LastName); //last name
			data.Add(connectionProxy.CampaignId); //campaign
			data.Add(connectionProxy.ReferrerId); //referrerId
            data.Add(connectionProxy.SelectedAvatar);

            SendNotification(GameFacade.LOGIN_REQUEST, data);
			// Hide login gui so we can't keep pressing login
			HideLoginWindow();
		}

        private void ShowLoginFailedPopup()
        {
            List<object> args = new List<object>();
            args.Add(Translation.LOGIN_FAILED);
            args.Add(Translation.LOGIN_RETRY_TEXT);
            Hangout.Shared.Action okcb = AttemptToReconnectToServer;
            Hangout.Shared.Action cancelcb = delegate() { };
            args.Add(okcb);
            args.Add(cancelcb);
            args.Add(Translation.RETRY); // optional. "Ok" is default
            args.Add(Translation.QUIT); // optional. "Cancel" is default
            GameFacade.Instance.SendNotification(GameFacade.SHOW_CONFIRM, args);
        }

		private void AttemptToReconnectToServer()
		{
			GameFacade.Instance.SendNotification(GameFacade.REQUEST_CONNECT);
		}

		private void ShowLoginWindow()
		{
            IGuiManager guiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
            ConstructGui(guiManager);
            mLoginWindow.Showing = true;
		}

		private void HideLoginWindow()
		{
			if (mLoginWindow != null)
			{
				mLoginWindow.Close();
			}
			mUsernameTextbox = null;
            mPasswordTextbox = null;
			mSendButton = null;
		}
	}
}
