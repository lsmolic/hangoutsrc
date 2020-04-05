using System;
using Hangout.Client.Gui;
using Hangout.Shared;

namespace Hangout.Client
{
	public class NavigationBar : GuiController, IDisposable
	{
		private const string mResourcePath = "resources://GUI/TopLevel/NavigationBar.gui";
		public Window MainWindow
		{
			get { return mMainWindow; }
		}

		private readonly Button mEmoteMenuButton;
		public Button EmoteMenuButton
		{
			get { return mEmoteMenuButton; }
		}

		private ChatWindow mChatWindow;
		public ChatWindow ChatWindow
		{
			get { return mChatWindow; }
		}

		private readonly Window mMainWindow;
		private readonly Button mMapButton;
		public Button MapButton
		{
			get { return mMapButton; }
		}

		private readonly Button mEntourageButton;
		public Button EntourageButton
		{
			get { return mEntourageButton; }
		}

		private readonly Button mFriendButton;
		public Button FriendButton
		{
			get { return mFriendButton; }
		}

		private readonly Button mRoomButton;
		public Button RoomButton
		{
			get { return mRoomButton; }
		}

		private readonly Button mClosetButton;
		public Button ClosetButton
		{
			get
			{ return mClosetButton; }
		}
		private readonly Button mShopButton;
		public Button ShopButton
		{
			get { return mShopButton; }
		}

		private readonly Button mSettingsButton;
		public Button SettingsButton
		{
			get { return mSettingsButton; }
		}

		private readonly Label mCurrentLocationLabel;

		// Store the notification name for the current open window
		private string mCurrentOpenWindow;

		public bool Showing
		{
			set
			{
				mMainWindow.Showing = value;
				mChatWindow.Showing = value;
			}
		}

		public NavigationBar(IGuiManager guiManager)
			: base(guiManager, mResourcePath)
		{
			mMainWindow = (Window)this.MainGui;
			mMapButton = mMainWindow.SelectSingleElement<Button>("MainFrame/Map");
			mFriendButton = mMainWindow.SelectSingleElement<Button>("MainFrame/Friend");
			mRoomButton = mMainWindow.SelectSingleElement<Button>("MainFrame/Room");
			mClosetButton = mMainWindow.SelectSingleElement<Button>("MainFrame/Closet");
			mShopButton = mMainWindow.SelectSingleElement<Button>("MainFrame/Shop");
			mSettingsButton = mMainWindow.SelectSingleElement<Button>("MainFrame/Settings");
			mCurrentLocationLabel = mMainWindow.SelectSingleElement<Label>("MainFrame/CurrentLocationLabel");
			mEmoteMenuButton = mMainWindow.SelectSingleElement<Button>("MainFrame/EmoteMenu");
			mEntourageButton = mMainWindow.SelectSingleElement<Button>("MainFrame/Entourage");
			EmoteGuiController emoteController = new EmoteGuiController(GameFacade.Instance.RetrieveMediator<RuntimeGuiManager>());

			// Hide these until they are implemented
			mFriendButton.Showing = false;
			mSettingsButton.Showing = false;

			mEntourageButton.AddOnPressedAction(delegate()
			{
				ToggleWindow(GameFacade.SHOW_ENTOURAGE);
				EventLogger.Log(LogGlobals.CATEGORY_GUI, LogGlobals.ENTOURAGE_BUTTON);
			});

			mRoomButton.AddOnPressedAction(delegate()
			{
				ToggleWindow(GameFacade.TOGGLE_ROOM_PICKER_GUI, MessageSubType.PublicRooms);
				EventLogger.Log(LogGlobals.CATEGORY_GUI, LogGlobals.ROOM_BUTTON);
			});

			mFriendButton.AddOnPressedAction(delegate()
			{
				ToggleWindow(GameFacade.FRIEND_BUTTON_CLICKED);
				EventLogger.Log(LogGlobals.CATEGORY_GUI, LogGlobals.FRIEND_BUTTON);
			});

			mClosetButton.AddOnPressedAction(delegate()
			{
				ToggleWindow(GameFacade.CLOSET_BUTTON_CLICKED);
				EventLogger.Log(LogGlobals.CATEGORY_GUI, LogGlobals.CLOSET_BUTTON);
			});

			mShopButton.AddOnPressedAction(delegate()
			{
				ToggleWindow(GameFacade.SHOP_BUTTON_CLICKED);
				EventLogger.Log(LogGlobals.CATEGORY_GUI, LogGlobals.SHOP_BUTTON);
			});

			mSettingsButton.AddOnPressedAction(delegate()
			{
				ToggleWindow(GameFacade.SETTINGS_BUTTON_CLICKED);
				EventLogger.Log(LogGlobals.CATEGORY_GUI, LogGlobals.SETTINGS_BUTTON);
			});
			mCurrentLocationLabel.Text = string.Empty;

			mMapButton.AddOnPressedAction(delegate()
			{
				ToggleWindow(GameFacade.MAP_BUTTON_CLICKED);
				EventLogger.Log(LogGlobals.CATEGORY_GUI, LogGlobals.MAP_BUTTON);
			});

			mEmoteMenuButton.AddOnPressedAction(delegate()
			{
				emoteController.ToggleOpen();
			});

			// Load chat bar
			IInputManager inputManager = GameFacade.Instance.RetrieveMediator<InputManagerMediator>();
			ChatMediator chatMediator = GameFacade.Instance.RetrieveMediator<ChatMediator>();
			mChatWindow = new ChatWindow(inputManager, mMainWindow.SelectSingleElement<IGuiFrame>("MainFrame/ChatBar"));
			chatMediator.ChatWindow = mChatWindow;
		}

		public void ToggleWindow(string notificationName)
		{
			// Close all other windows 
			CloseAllOtherWindows(notificationName);
			GameFacade.Instance.SendNotification(notificationName);
		}

		public void ToggleWindow(string notificationName, object body)
		{
			// Close all other windows 
			CloseAllOtherWindows(notificationName);
			GameFacade.Instance.SendNotification(notificationName, body);
		}

		public void CloseAllWindows()
		{
			mCurrentOpenWindow = null;
			GameFacade.Instance.SendNotification(GameFacade.CLOSE_ALL_WINDOWS);
		}

		private void CloseAllOtherWindows(string notificationName)
		{
			// Only close windows other than the one that is being toggled, so we
			// don't get reopen a window we meant to close
			if (mCurrentOpenWindow != notificationName)
			{
				mCurrentOpenWindow = notificationName;
				GameFacade.Instance.SendNotification(GameFacade.CLOSE_ALL_WINDOWS);
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			mChatWindow.Dispose();
		}

		public void UpdateCurrentLocationLabel(string currentLocationName)
		{
			mCurrentLocationLabel.Text = currentLocationName;
		}

		public void DisableButtons()
		{
			mMapButton.Disable();
			mFriendButton.Disable();
			mRoomButton.Disable();
			mClosetButton.Disable();
			mShopButton.Disable();
			mSettingsButton.Disable();
			mEmoteMenuButton.Disable();
			mEntourageButton.Disable();
		}

		public void EnableButtons()
		{
			mMapButton.Enable();
			mFriendButton.Enable();
			mRoomButton.Enable();
			mClosetButton.Enable();
			mShopButton.Enable();
			mSettingsButton.Enable();
			mEmoteMenuButton.Enable();
			mEntourageButton.Enable();
		}

	}

}