/* Pherg 10/26/09 */

using System;
using System.Collections.Generic;
using Hangout.Client.Gui;
using Hangout.Shared;
using PureMVC.Patterns;
using UnityEngine;

namespace Hangout.Client
{
	public class GreenScreenRoomLoadingMediator : Mediator
	{
		private Hangout.Shared.Action mFinishedLoadingCallback;

		private const string mLoadingScreenGuiPath = "resources://GUI/Minigames/Fashion/LoadingScreen.gui";
		private GuiController mLoadingGui;
		private IGuiManager mGuiManager;
		
		private bool mAvatarLoaded = false;
		private bool mClientAssetRepoLoaded = false;
	
		public GreenScreenRoomLoadingMediator(Hangout.Shared.Action finishedLoadingCallback)
		{
			mAvatarLoaded = false;
			mClientAssetRepoLoaded = false;
			if (finishedLoadingCallback == null)
			{
				throw new ArgumentNullException("finishedLoadingCallback");
			}
			
			if (GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().Loaded)
			{
				mClientAssetRepoLoaded = true;
				CheckLoadingFinished();
			}
			mFinishedLoadingCallback = finishedLoadingCallback;
		}

		public override System.Collections.Generic.IList<string> ListNotificationInterests()
		{
			return new string[]
			{ 
				GameFacade.LOCAL_AVATAR_LOADED,
				GameFacade.ANIMATION_PROXY_LOADED,
			};
		}

		public override void HandleNotification(PureMVC.Interfaces.INotification notification)
		{
            Console.WriteLine("GreenScreenRoomLoadingMediator.HandleNotification");
            base.HandleNotification(notification);

			switch (notification.Name)
			{
				case GameFacade.LOCAL_AVATAR_LOADED:
                    Console.WriteLine("LOCAL_AVATAR_LOADED");
					mAvatarLoaded = true;
					CheckLoadingFinished();
					break;
				case GameFacade.ANIMATION_PROXY_LOADED:
                    Console.WriteLine("ANIMATION_PROXY_LOADED");
					mClientAssetRepoLoaded = true;
					CheckLoadingFinished();
					break;
				default:
                    Console.WriteLine("UNEXPECTED NOTIFICATION");
					throw new Exception("Unexpected notification (" + notification.Name + ")");
			}
		}
		
		private void CheckLoadingFinished()
		{
			if (mAvatarLoaded && mClientAssetRepoLoaded)
			{
                Console.WriteLine("LOADING FINISHED");
				mFinishedLoadingCallback();
			}
		}
		
		public override void OnRegister()
		{
			base.OnRegister();
			mGuiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();

			mLoadingGui = new GuiController(mGuiManager, mLoadingScreenGuiPath);
			GameFacade.Instance.RetrieveMediator<ToolbarMediator>().TopBar.Mode = TopBar.TopBarMode.LoggingIn;

			CheckLoadingFinished();
		}

		public override void OnRemove()
		{
			mLoadingGui.Dispose();
			base.OnRemove();
		}
	}
}
