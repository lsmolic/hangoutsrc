using System.Collections.Generic;
using System;

using Hangout.Client.Gui;
using Hangout.Client.FashionGame;

namespace Hangout.Client
{
	public class TopBar : GuiController
	{
		private const string mResourcePath = "resources://GUI/TopLevel/TopBar.gui";
        public Window MainWindow
        {
            get { return mMainWindow; }
        }
        private readonly Window mMainWindow;
        private readonly Button mCashIcon;
        private readonly Button mCoinsIcon;
        private readonly Label mCashAmountLabel;
        private readonly Label mCoinsAmountLabel;
        private readonly Button mBuyCashButton;
		private readonly Button mFashionCityButton;

		public bool Showing
		{
			set
			{
				foreach (ITopLevel gui in this.AllGuis)
				{
					gui.Showing = value;
				}
			}
		}

		public TopBar(IGuiManager guiManager)
			: base(guiManager, mResourcePath)
		{
            mMainWindow = (Window)this.MainGui;
            
			// Commented out until we get karma working again...
			// mKarmaIcon = mMainWindow.SelectSingleElement<Button>("MainFrame/KarmaMeter/Icon");
			// mKarmaAmountLabel = mMainWindow.SelectSingleElement<Label>("MainFrame/KarmaMeter/Amount");
			// Todo: replace this with real texture
			// mKarmaIcon.Image = (Texture)Resources.Load("GUI/Chat/Window/Images/ChatBorderMaster");

			mCoinsIcon = mMainWindow.SelectSingleElement<Button>("MainFrame/CoinsMeter/CoinIcon");
			mCoinsAmountLabel = mMainWindow.SelectSingleElement<Label>("MainFrame/CoinsMeter/Amount");
			mCoinsIcon.AddOnPressedAction(HandleCoinsPressed);

			mCashIcon = mMainWindow.SelectSingleElement<Button>("MainFrame/CashMeter/CashIcon");
			mCashAmountLabel = mMainWindow.SelectSingleElement<Label>("MainFrame/CashMeter/Amount");
			mCashIcon.AddOnPressedAction(BuyCash);

			mBuyCashButton = mMainWindow.SelectSingleElement<Button>("MainFrame/BuyCash");
            mBuyCashButton.Text = Translation.GET_CASH;
			mBuyCashButton.AddOnPressedAction(BuyCash);

			mFashionCityButton = mMainWindow.SelectSingleElement<Button>("MainFrame/FashionCity");
			mFashionCityButton.Text = Translation.PLAY_FASHION_CITY;
			mFashionCityButton.AddOnPressedAction(HandleFashionCityPressed);
		}

		private void BuyCash()
		{
			BuyCoinUtility.GoToBuyCashPage
			(
				Translation.NEED_CASH_TITLE,
				Translation.NEED_CASH_TEXT,
				delegate(string s) 
				{ 
					Console.WriteLine("Opening cash store: " + s);
				},
				delegate()
				{
					GameFacade.Instance.SendNotification(GameFacade.GET_CASH_GUI_CLOSED);
				}
			);
		}

        public void SetCashAmount(string balance)
        {
            mCashAmountLabel.Text = balance;
        }

        public void SetCoinsAmount(string balance)
        {
            mCoinsAmountLabel.Text = balance;
        }

        public void SetKarmaAmount(string balance)
        {
            // mKarmaAmountLabel.Text = balance;
        }

		private void HandleCoinsPressed()
		{
			// Popup confirm
			List<object> args = new List<object>();
			args.Add(Translation.NEED_COIN_TITLE);
			args.Add(Translation.NEED_COIN_TEXT);
			GameFacade.Instance.SendNotification(GameFacade.SHOW_DIALOG, args);
		}

		// If this gets much more complicated, replace with a state machine
		public enum TopBarMode
		{
			Hangout,
			FashionMinigame,
			LoggingIn
		}

		private TopBarMode mMode;
		public TopBarMode Mode
		{
			get { return mMode; }
			set
			{
				mMode = value;
				switch(mMode)
				{
					case TopBarMode.Hangout:
						mFashionCityButton.Text = Translation.PLAY_FASHION_CITY;
						mFashionCityButton.ClearOnPressedActions();
						mFashionCityButton.AddOnPressedAction(delegate()
						{
							GameFacade.Instance.SendNotification(GameFacade.SWITCHING_TO_FASHION_MINI_GAME);
						});

						EnableBuyCashButtons();
						mFashionCityButton.Enable();
						break;

					case TopBarMode.FashionMinigame:
						mFashionCityButton.Text = FashionGameTranslation.GO_TO_SHOW;
						mFashionCityButton.ClearOnPressedActions();
						mFashionCityButton.AddOnPressedAction(delegate()
						{
							FashionMinigame.GoToRunway();
						});

						mFashionCityButton.Enable();
						DisableBuyCashButtons();
						break;

					case TopBarMode.LoggingIn:
						mFashionCityButton.Disable();
						DisableBuyCashButtons();
						break;

					default:
						throw new NotImplementedException();
				}
			}
		}

		private void EnableBuyCashButtons()
		{
			mBuyCashButton.Enable();
			mCashIcon.Enable();
		}

		private void DisableBuyCashButtons()
		{
			mBuyCashButton.Disable();
			mCashIcon.Disable();
		}

		private void HandleFashionCityPressed()
		{
			GameFacade.Instance.SendNotification(GameFacade.SWITCHING_TO_FASHION_MINI_GAME);
		}
	}
}
