using System.Collections.Generic;
using Hangout.Client.Gui;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace Hangout.Client
{
	public class ToolbarMediator : Mediator
	{
		private NavigationBar mNavigationBar;
		public NavigationBar NavigationBar
		{
			get { return mNavigationBar; }
		}

		private TopBar mTopBar;
		public TopBar TopBar
		{
			get { return mTopBar; }
		}

		public ToolbarMediator()
		{
			RuntimeGuiManager guiManager = GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>();
			mNavigationBar = new NavigationBar(guiManager);
			mTopBar = new TopBar(guiManager);
			mNavigationBar.DisableButtons();
			mTopBar.Mode = TopBar.TopBarMode.LoggingIn;
		}

		public override void OnRemove()
		{
			base.OnRemove();
			mNavigationBar.Dispose();
			mTopBar.Dispose();
		}

		public void Init()
		{
			GameFacade.Instance.RetrieveProxy<InventoryProxy>().GetUserBalance();
			mNavigationBar.EnableButtons();
			mTopBar.Mode = TopBar.TopBarMode.Hangout;
		}

		public void SetCurrentLocation(string location)
		{
			mNavigationBar.UpdateCurrentLocationLabel(location);
		}

		public override IList<string> ListNotificationInterests()
		{
			return new string[] 
            {
                GameFacade.APPLICATION_EXIT,
                GameFacade.RECV_USER_BALANCE
			};
		}

		public override void HandleNotification(INotification notification)
		{
			base.HandleNotification(notification);
			switch (notification.Name)
			{
				case GameFacade.RECV_USER_BALANCE:
					UpdateBalance(notification);
					break;
			}
		}

		private void UpdateBalance(INotification notification)
		{
			string vcoin = (notification.Body as string[])[0];
			string houts = (notification.Body as string[])[1];
			// Real money currency
			if (houts != "")
			{
				mTopBar.SetCashAmount(houts);
			}
			// Game currency
			if (vcoin != "")
			{
				mTopBar.SetCoinsAmount(vcoin);
			}
		}
	}
}