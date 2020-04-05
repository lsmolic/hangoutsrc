/* Pherg 11/10/09 */

using System;
using System.Collections.Generic;
using System.Text;

using PureMVC.Patterns;

using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class FashionMinigameLoadingMediator : Mediator
	{
		private const string mLoadingScreenGuiPath = "resources://GUI/Minigames/Fashion/LoadingScreen.gui";
		private GuiController mLoadingGui;
		private IGuiManager mGuiManager;

		public override System.Collections.Generic.IList<string> ListNotificationInterests()
		{
			List<string> interests = new List<string>(base.ListNotificationInterests());
			interests.Add(GameFacade.REMOVE_FASHION_MINIGAME_LOADING_SCREEN);
			return interests;
		}

		public override void HandleNotification(PureMVC.Interfaces.INotification notification)
		{
			base.HandleNotification(notification);

			switch (notification.Name)
			{
				case GameFacade.REMOVE_FASHION_MINIGAME_LOADING_SCREEN:
					RemoveLoadingScreen();
					break;
				default:
					throw new Exception("Unexpected notification (" + notification.Name + ")");
			}
		}

		private void RemoveLoadingScreen()
		{
			mLoadingGui.Dispose();
		}

		public override void OnRegister()
		{
			base.OnRegister();
			mGuiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
			mLoadingGui = new GuiController(mGuiManager, mLoadingScreenGuiPath);
			Window mainWindow = (Window)mLoadingGui.MainGui;
			mainWindow.InFront = true;
		}

		public override void OnRemove()
		{
			mLoadingGui.Dispose();
			base.OnRemove();
		}
	}
}
