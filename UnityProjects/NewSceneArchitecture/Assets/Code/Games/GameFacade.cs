/*=============================================================
(c) all rights reserved
================================================================*/
using System;
using System.Collections.Generic;

using PureMVC.Patterns;
using PureMVC.Interfaces;

using Hangout.Shared;
using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class GameFacade : Facade, IFacade, IDisposable
	{
		// Notification constants. The Facade is the ideal
		// location for these constants, since any part
		// of the application participating in PureMVC 
		// Observer Notification will know the Facade.

        /// <summary>
        ///  APPLICATION_INIT is sent once all initial data is setup.  
        ///  Mediators should handle this if they need to invoke any init calls to complete setup.
        ///  (The init calls should not be called by the constructor)
        /// </summary>
        public const string APPLICATION_INIT = "applicationInit";
        public const string APPLICATION_EXIT = "applicationExit";
    

        public const string ENTER_STATE = "enterState";
		public const string EXIT_STATE = "exitState";

        public const string REQUEST_CONNECT = "requestConnect";
		public const string CONNECTED = "connected";
		public const string LOGIN_REQUEST = "loginRequest";
		public const string LOGIN_SUCCESS = "loginSuccess";
		public const string LOGIN_FAILED = "loginFailed";
		public const string DISCONNECTED = "disconnected";
		
		public const string CLIENT_ASSET_REPOSITORY_LOADED = "clientAssetRepositoryLoaded";
		public const string STARTUP_COMMAND = "startup";
		
		// Loading finished notifs.
		public const string LOCAL_AVATAR_LOADED = "localAvatarLoaded";
		public const string SOUND_PROXY_LOADED = "soundProxyLoaded";
		
        // Popups
        public const string SHOW_DIALOG = "showDialog";    // Ok button
        public const string SHOW_CONFIRM = "showConfirm";  // Ok and cancel buttons
        public const string HIDE_POPUP = "hidePopup"; 
        public const string SHOW_SHOPPING_POPUP = "showShoppingPopup";
        public const string HIDE_SHOPPING_POPUP = "hideShoppingPopup";
		
		// General
        public const string START_LOADING = "startLoading";
        public const string END_LOADING = "endLoading";
        public const string ANIMATION_PROXY_LOADED = "animationProxyLoaded";

        // Navigation Bar
		public const string CLOSE_ALL_WINDOWS = "closeAllWindows";
        public const string HOME_BUTTON_CLICKED = "homeButtonClicked";
        public const string FRIEND_BUTTON_CLICKED = "friendButtonClicked";
        public const string CLOSET_BUTTON_CLICKED = "closetButtonClicked";
        public const string SHOP_BUTTON_CLICKED = "shopButtonClicked";
        public const string MAP_BUTTON_CLICKED = "mapButtonClicked";
        public const string SETTINGS_BUTTON_CLICKED = "settingsButtonClicked";
        public const string LOCATION_BUTTON_CLICKED = "locationButtonClicked";

        // Chat/Emote
		public const string RECV_CHAT = "recvChat";
		public const string SEND_CHAT = "sendChat";
        public const string PLAY_EMOTE = "playEmote";
        public const string SEND_EMOTICON = "sendEmoticon";
		public const string RECV_EMOTICON = "recvEmoticon";

        // Game state
        public const string ENTER_ROOM_STATE = "enterRoomState";
        public const string ROOM_LOADING_STARTED = "roomLoadingStarted";
        public const string ROOM_LOADING_FINISHED = "roomLoadingFinished";
        public const string SHOW_SERVER_ROOM_API = "showServerRoomAPI";
		public const string TOGGLE_ROOM_PICKER_GUI = "toggleRoomPickerGui";
        public const string SHOW_ROOM_PICKER_GUI = "showRoomPickerGui";
        public const string SWITCHING_TO_GREEN_SCREEN_ROOM = "switchingToGreenScreenRoom";
		public const string SWITCHING_TO_FASHION_MINI_GAME = "fashionFrenzyMiniGame";
		public const string ENTER_RUNWAY_SEQUENCE = "enterRunwaySequence";
		
		// Map
		public const string MAP_GUI_OPENED = "mapGuiOpened";
		public const string MAP_GUI_CLOSED = "mapGuiClosed";
		
		// Get Cash
		public const string GET_CASH_GUI_OPENED = "getCashGuiOpened";
		public const string GET_CASH_GUI_CLOSED = "getCashGuiClosed";
		
		// Green Screen Room State
		public const string GREEN_SCREEN_ROOM_LOADED = "greenScreenRoomLoaded";
		public const string ENTERED_GREEN_SCREEN_ROOM_DEFAULT_STATE = "enteredGreenScreenRoomDefaultState";

        
        // Room creator
		public const string ENTER_ROOM_CREATOR = "enterRoomCreator";
		
        // Fashion Game
        public const string ENTER_FASHION_MINIGAME = "enterFashionMinigame";
        public const string REMOVE_FASHION_MINIGAME_LOADING_SCREEN = "removeFashionMinigameLoadingScreen";

        // Inventory
        public const string REQUEST_INVENTORY = "requestStoreInventory";
        public const string RECV_STORE_INVENTORY = "recvStoreInventory";
        public const string RECV_PLAYER_INVENTORY = "recvStoreInventory";
        public const string RECV_PLAYER_ANIMATIONS = "recvPlayerAnimations";
        public const string RECV_ITEM_PURCHASE = "recvItemPurchase";
        public const string RECV_USER_BALANCE = "recvUserBalance";
        public const string RECV_PAYMENT_ID = "recvPaymentId";
		public const string REQUEST_STORE_INVENTORY = "requestStoreInventory";
		public const string REQUEST_PLAYER_INVENTORY = "requestPlayerInventory";
		public const string REQUEST_PLAYER_ANIMATIONS = "requestPlayerAnimations";
		public const string REQUEST_ITEM_PURCHASE = "requestItemPurchase";
		public const string REQUEST_USER_BALANCE = "requestUserBalance";
		public const string REQUEST_DELETE_INVENTORY_ITEM = "requestDeleteInventoryItem";
		public const string REQUEST_PAYMENT_ID = "requestPaymentId";
        
        // Shop
        public const string SHOP_OPENED = "shopOpened";
        public const string SHOP_CLOSED = "shopClosed";
        public const string SHOP_HEAD_VIEW = "headView";
		public const string SHOP_BODY_VIEW = "bodyView";

        public const string SHOW_FRIENDS = "showFriends";
		public const string SHOW_ENTOURAGE = "showEntourage";
        
        // Sounds
		public const string PLAY_SOUND_BUTTON_PRESS = "playSoundButtonPress";
        public const string PLAY_SOUND_ERROR = "playSoundError";
        public const string PLAY_SOUND_CLOSE = "playSoundClose";
        public const string PLAY_SOUND_SWAPPING_ITEMS = "playSoundSwappingItems";
        public const string PLAY_SOUND_POPUP_APPEAR_A = "playSoundPopUpAppearA";
        public const string PLAY_SOUND_POPUP_APPEAR_B = "playSoundPopupAppearB";
        public const string PLAY_SOUND_APPLY_ROOM_BACKGROUND = "playSoundApplyRoomBackground";
        public const string PLAY_SOUND_LEVEL_UP = "playSoundLevelUp";
        public const string PLAY_SOUND_MAP_OPEN = "playSoundMapOpen";

		//Escrow
		public const string REQUEST_ESCROW = "requestEscrow";

		private static readonly object mInstanceLock = new object();
		private static GameFacade mInstance;
		
		//TODO: Layers Hack.  Need to figure out where to put layers for input fun.
		public static readonly int MODEL_LAYER = 31;
		public static readonly int UNSELECTABLE_MODEL_LAYER = 30;
		public static readonly int STATION_LAYER = 29;
		public static readonly int CLOTHING_LAYER = 28;
		public static readonly int GROUND_LAYER = 27;
		public static readonly int WALL_LAYER = 26;
		public static readonly int IGNORE_RAYCAST = UnityEngine.LayerMask.NameToLayer("Ignore Raycast");
		
		/// <summary>
		/// Use this instance instead of Facade.Instance
		/// </summary>
		public new static GameFacade Instance
		{
			get
			{
				if( mInstance == null )
				{
					lock (mInstanceLock)
					{
						if( mInstance == null )
						{
							mInstance = new GameFacade();
						}
					}
				}
				return mInstance;
			}
		}

		protected override void InitializeController()
		{
			base.InitializeController();

			// Register commands here
			RegisterCommand(STARTUP_COMMAND, typeof(StartupCommand));
            RegisterCommand(REQUEST_CONNECT, typeof(RequestConnectionCommand));
			RegisterCommand(LOGIN_REQUEST, typeof(LoginRequestCommand));
		} 

		public void Dispose()
		{
			SendNotification(APPLICATION_EXIT);
            Instance.RetrieveProxy<ClientMessageProcessor>().Dispose();
		}
	}

}
