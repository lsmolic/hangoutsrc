using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Client
{
    /// <summary>
    /// Translation class contains all the text that is displayed in the client.  
    /// Instead of hardcoding strings in your classes, put the string here and use the static variable in your code instead.
    /// 
    /// </summary>
    public class Translation
    {
        // Product 
        public static string PRODUCT_NAME = "Hangout";
        public static string PAY_CURRENCY_NAME = "Cash";
        public static string VIRTUAL_CURRENCY_NAME = "Coins";

        // General
        public static string OK = "Ok";
        public static string CANCEL = "Cancel";
        public static string RETRY = "Retry";
        public static string QUIT = "Quit";
        public static string LOADING_PROGRESS = "Loading...";

        // Connection
        public static string CONNECTING_TITLE = "Connecting";
        public static string CONNECTING = "Connecting to: ";
        public static string LOGIN_FAILED = "Login Failed";
        public static string LOGIN_RETRY_TEXT = "Try logging in again?";
		public static string CONNECT_FAILED = "Could not connect to server.";
		public static string CONNECTION_LOST = "Lost Connection to server.";
		public static string CONNECT_RETRY_TEXT = "Try to reconnect?";

        // Store purchase confirmation
        public static string PURCHASE = "Buy";
        public static string PURCHASE_CONFIRM_TITLE = PRODUCT_NAME + " Item Purchase";
        public static string PURCHASE_CONFIRM_TEXT = "Are you sure you want to buy this item?";
        public static string PURCHASE_CONFIRM_OK = "Buy it!";
        public static string PURCHASE_COMPLETING_TRANSACTION = "One moment, your purchase is being completed.";

        // Store purchase result
        public static string PURCHASE_ERROR_TITLE = "Purchase Not Completed";
        public static string PURCHASE_ERROR_GENERIC = "Something went wrong.";
        public static string PURCHASE_ERROR_INSUFFIENT_FUNDS = "You do not have enough cash or coins to complete the purchase.";
        public static string PURCHASE_ERROR_ITEM_EXPIRED = "Sorry, this item offer has expired.";
        public static string PURCHASE_ERROR_NO_INVENTORY = "Sorry, this item is now out of stock.";
        public static string PURCHASE_ERROR_ACCOUNT_NOT_FOUND = "Sorry, there was a problem finding the purchasing account.";
        public static string PURCHASE_ERROR_STORE_NOT_FOUND = "Sorry, there was a problem finding the store.";

        // Store categories
        public static string STORE_CLOSET = "Closet";
        public static string STORE_CLOTHES = "Clothes";
        public static string STORE_TOPS = "Tops";
        public static string STORE_PANTS = "Pants";
        public static string STORE_FACE = "Face";
        public static string STORE_MAKEUP = "Makeup";
        public static string STORE_HAIR = "Hair";
        public static string STORE_BODY = "Body";
        public static string STORE_ROOM = "Scenes";
        public static string STORE_ROOM_BG = "Scenery";
        public static string STORE_ROOM_ADDITION = "Add Scene";

        // Store inventory panel
        public static string STORE_EXPIRATION = "On Sale Until: {0}";
        public static string STORE_QUANTITY = "Inventory: {0}/{1}";
        public static string STORE_QUANTITY_UNLIMITED = "Inventory: Unlimited";

        // Rooms
        public static string APPLY_ROOM_BACKGROUND = "Apply to Room";
        public static string JOIN_ROOM = "GO";

        // Closet
        public static string WEARIT = "Wear it";
        public static string REMOVEIT = "Remove it";

        // Payment
        public static string GET_CASH = "GET CASH";
        public static string NEED_CASH_TEXT_COMING_SOON = "Coming Soon! Buy Hangout Cash to shop for premium items in the store.";
        public static string NEED_CASH_TEXT = "Do you want to buy or earn more Hangout Cash?";
        public static string NEED_CASH_BUTTON = "Get Cash";
		public static string NEED_CASH_TITLE = "Get Cash";
		public static string NEED_COIN_TEXT = "Earn coins by playing minigames. Use the coins to shop for cool items in the store.";
		public static string NEED_COIN_TITLE = "Earn Coins";
		public static string NOT_ENOUGH_COIN = "You don't have enough coin to purchase this item.";
		public static string NOT_ENOUGH_CASH = "You don't have enough Hangout Cash to purchase this item.";

		public static string PLAY_FASHION_CITY = "Play Fashion City";

        // Top Level Gui
        public static string ROOMS_BUTTON_TEXT = "Rooms";
        public static string SHOP_BUTTON_TEXT = "Shop";
        public static string MAP_BUTTON_TEXT = "Map";

		// Room picker GUI
		public static string ROOM_PICKER_MY_ROOMS = "My Rooms";
		public static string ROOM_PICKER_FRIENDS_ROOMS = "Friends' Rooms";
		public static string ROOM_PICKER_PUBLIC_ROOMS = "Public Rooms";

		//Escrow 
		public static string ESCROW_RECEIVED_ITEM = "Received Item";
		public static string ESCROW_FASHION_CITY_JOB_COINS_RECEIVED = "You have earned {0} coins from members of your entourage playing Fashion City!";

		//Friends / Entourage
		//public static string ENTOURAGE_MEMBER_COUNT = "Invite friends to your entourage to get coin bonuses. Current size: {0}";
		public static string ENTOURAGE_MEMBER_COUNT = "Each friend earns you a coin and experience point bonus in Fashion City. Current bonus: {0}%";
		public static string ENTOURAGE_INVITE_FRIENDS_BUTTON_TEXT = "  Invite Friends";

    }
}
