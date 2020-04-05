using System;
using System.Text;

namespace Hangout.Shared
{
    /// <summary>
    /// For any noteworthy game or user event, add a static string here so it's easy to look up the meanings later.   
    /// 
    /// The event log takes a categoryName, eventName and subEvent. 
    /// public static void Log(string categoryName, string eventName, string subEvent, string eventData, string accountId)
    /// 
    /// categoryName -  These should be major categories like Chat, Shopping, Login, etc.   Try and keep new categoryName's to a minimum.  
    /// eventName -  These are major events you want to track, like when a user opens the shopping interface
    /// optLabel  -  (Optional) These key/value data that you want to track within an event.  
    /// optValue  -  Arbitrary data for the event
    /// </summary>
    public static class LogGlobals
    {
        // General Events
        public static string EVENT_WINDOW_OPENED = "WindowOpened";
        public static string EVENT_WINDOW_CLOSED = "WindowClosed";
        public static string EVENT_TAB_OPENED = "TabOpened";
        public static string EVENT_BUTTON_CLICKED = "ButtonClicked";

        //////////////////////////////////////////////////////////////////////////
        // Server Stats
        //////////////////////////////////////////////////////////////////////////
        public static string CATEGORY_SERVER_STATS = "ServerStats";
        // Events
        public static string CONCURRENCY = "Concurrency";
        

        //////////////////////////////////////////////////////////////////////////
        // Connection
        //////////////////////////////////////////////////////////////////////////
        public static string CATEGORY_CONNECTION = "Connection";
        // Events
        public static string EVENT_CONNECTED = "Connected";
        public static string EVENT_LOGIN = "Login";
        public static string EVENT_LOGOUT = "Logout";
        public static string EVENT_DISCONNECTED = "Disconnected";
        // Labels
        public static string LOGIN_SUCCESS = "LoginSuccess";
        public static string LOGIN_FAILED = "LoginFailed";
        public static string SESSION_ID = "SessionId";


		//////////////////////////////////////////////////////////////////////////
		// Loading
		//////////////////////////////////////////////////////////////////////////
		public static string CATEGORY_LOADING = "Loading";
		// Events
		public static string TIME_TO_PLAY = "TimeToPlay";
		

        //////////////////////////////////////////////////////////////////////////
        // Client Stats
        //////////////////////////////////////////////////////////////////////////
        public static string CATEGORY_CLIENT_STATS = "ClientStats";
        // Events
        public static string EVENT_STAT = "Stat";

        //////////////////////////////////////////////////////////////////////////
        // Toolbar 
        //////////////////////////////////////////////////////////////////////////
        public static string CATEGORY_GUI = "Gui";
        // Events
		public static string GET_CASH_BUTTON = "GetCashClicked";
		public static string FASHION_GAME_BUTTON = "FCGameClicked";
        public static string MAP_BUTTON = "MapClicked";
		public static string ROOM_BUTTON = "RoomsClicked";
		public static string FRIEND_BUTTON = "FriendsClicked";
		public static string ENTOURAGE_BUTTON = "EntourageClicked";
		public static string CLOSET_BUTTON = "ClosetClicked";
		public static string SHOP_BUTTON = "ShopClicked";
		public static string SETTINGS_BUTTON = "SettingsClicked";
		public static string EVENT_EMOTE_CLICKED = "EmoteClicked";
		public static string EVENT_MOOD_CLICKED = "MoodClicked";
		public static string EVENT_EMOTICON_CLICKED = "EmoticonClicked";

        //////////////////////////////////////////////////////////////////////////
        // Chat
        //////////////////////////////////////////////////////////////////////////
        public static string CATEGORY_CHAT = "Chat";
        // Events
        public static string EVENT_CHAT_SENT = "Sent";
        public static string EVENT_CHAT_FILTERED = "Filtered";

        //////////////////////////////////////////////////////////////////////////
        // Shopping 
        //////////////////////////////////////////////////////////////////////////
        public static string CATEGORY_SHOPPING = "Shop";
        // Events
        public static string BUY = "Purchase";
        public static string DONATE = "Donate";
        public static string PURCHASE_RESULT = "PurchaseResult";
        public static string CLICKED_ON_CASH_ITEM = "CashItemClicked";
		public static string CLICKED_ON_COIN_ITEM = "CoinItemClicked";
		public static string CLICKED_ON_CASH_ITEM_BUY_BUTTON = "CashItemBuyClicked";
		public static string CLICKED_ON_COIN_ITEM_BUY_BUTTON = "CoinItemBuyClicked";
		public static string CLICKED_ON_CASH_ITEM_BUY_CONFIRM = "CashItemBuyConfirmClicked";
		public static string CLICKED_ON_COIN_ITEM_BUY_CONFIRM = "CoinItemBuyConfirmClicked";
        public static string COIN_PURCHASE_SUCCESS = "CoinItemPurchased";
        public static string CASH_PURCHASE_SUCCESS = "CashItemPurchased";
        public static string PURCHASE_FAILED = "PurchaseFailed";
        // SHOP_OPENED: Insert shop names in the shopping state machine


        //////////////////////////////////////////////////////////////////////////
        // Minigames (each minigame has it's own category)
        //////////////////////////////////////////////////////////////////////////
        public static string CATEGORY_FASHION_MINIGAME = "FCGame";

        public static string EVENT_MINIGAME_ENTERED = "SessionStart";
        public static string EVENT_MINIGAME_EXITED = "SessionExit";
		public static string LEVEL_STARTED = "LevelStarted";
        public static string LEVEL_COMPLETE = "LevelComplete";
		public static string MODEL_COMPLETE = "ModelComplete";
		public static string GAMEPLAY_BEHAVIOR = "Behavior";
        public static string FRIEND_HIRE_PROMPTED = "FriendHiredPrompted";
        public static string FRIEND_HIRED = "FriendHired";
		public static string OUT_OF_ENERGY = "OutOfEnergy";
		
		public static string LEVELED_UP_EVENT = "LevelUp";

		public static string PROGRESSION_GUI_IMPRESSION = "ProgressGuiImpression";
		public static string PROGRESSION_GUI_BUTTON_CLICK = "ClickProgressGui";

		public static string BUY_ENERGY_RESULT = "BuyEnergyResult";

		// Labels
		public static string MINIGAME_LEVEL_SCORE = "LevelScore";
		public static string MINIGAME_TOTAL_XP = "MinigameTotalXP";
		public static string MINIGAME_COINS = "MinigameCoins";
		public static string MISSED_MODELS_IN_LEVEL = "MissedModelsInLevel";
		public static string TIME_TO_COMPLETE_LEVEL = "TimeToCompleteLevel";
		public static string PLAY_SESSION_TIME = "PlaySessionTime";
		public static string CLOTHING_MISSED = "ClothingMissed";
		public static string STATIONS_MISSED = "StationsMissed";
		public static string STACKED_CLOTHING = "StackedClothing";
		public static string HOLDING_STATION = "HoldingStation";
		public static string UNFIXED_CLOTHING = "UnfixedClothing";
		public static string USELESS_CLICK = "UselessClick";


        //////////////////////////////////////////////////////////////////////////
        // Rooms
        //////////////////////////////////////////////////////////////////////////
        public static string CATEGORY_ROOMS = "Room";
        // Events
        public static string ROOM_ENTERED = "Entered";
        public static string ROOM_EXITED = "Exited";
        public static string GUI_OPENED = "GuiOpened";
        public static string GUI_CLOSED = "GuiClosed";
        public static string PUBLIC_TAB_OPENED = "PublicTab";
        public static string FRIEND_TAB_OPENED = "FriendTab";
        public static string MY_ROOMS_TAB_OPENED = "MyRoomsTab";
        // Label
        public static string ROOM_LABEL = "Room"; // Fill in opt_value in room join code 

        //////////////////////////////////////////////////////////////////////////
        // Map
        //////////////////////////////////////////////////////////////////////////
        public static string CATEGORY_MAP = "Map";
        // Events
		public static string PUBLIC_HANGOUT_CLICKED = "PublicHOClicked";
		public static string FRIEND_HANGOUT_CLICKED = "FriendHOClicked";
		public static string MY_HANGOUT_CLICKED = "MyHangoutClicked";
		public static string SHOP_CLICKED = "ShopClicked";
		public static string FASHION_CITY_CLICKED = "FCClicked";

        //////////////////////////////////////////////////////////////////////////
        // Account
        //////////////////////////////////////////////////////////////////////////
        public static string CATEGORY_ACCOUNT = "Account";
        // Events
        public static string EVENT_ACCOUNT_CREATED = "AccountCreated";
        public static string EVENT_PAYMENT_ACCOUNT_CREATED = "PaymentAccountCreated";
        public static string EVENT_AVATAR_CREATED = "AvatarCreated";
        public static string EVENT_AVATAR_DELETED = "AvatarDeleted";
        // Label
        public static string AVATAR_ID_LABEL = "AvatarId";
        public static string ACCOUNT_ID_LABEL = "AccountId";
        public static string PAYMENT_ACCOUNT_ID_LABEL = "PIAccountId";

		//////////////////////////////////////////////////////////////////////////
		// Facebook Feeds
		//////////////////////////////////////////////////////////////////////////
		public static string CATEGORY_FACEBOOK = "FB";
		public static string FACEBOOK_FEED_POST = "FeedPostShown";
		public static string FACEBOOK_INVITE = "InviteShown";

    }

}