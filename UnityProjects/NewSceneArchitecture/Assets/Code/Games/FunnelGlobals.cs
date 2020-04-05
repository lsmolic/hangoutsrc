using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Client
{
    public class FunnelGlobals
    {
        public const string FASHION_MINIGAME = "FashionMinigame";
        public const string PUBLIC_LOBBY = "PublicLobby";
        
        public const string FUNNEL_PURCHASE = "purchase";
        public const string SHOP_OPENED = "shop_opened";
        public const string CLICKED_BUY = "clicked_buy";
        public const string CLICKED_CONFIRM = "clicked_confirm";
        public const string PURCHASE_COMPLETE = "purchase_succeeded";

        public const string FUNNEL_FASHION_GAME = "fashion_game";
        public const string FUNNEL_FRIEND_HIRE = "friend_hire";
        public const string HIRE_FRIEND_POPUP = "hire_friend_popup";
        public const string CLICKED_HIRE = "clicked_hire";
        public const string CLICKED_PLAY = "clicked_play";

        public Dictionary<string, uint> mPurchaseFunnel;
        public Dictionary<string, uint> PurchaseFunnel
        {
            get { return mPurchaseFunnel; }
        }

        public Dictionary<string, uint> mFriendHireFunnel;
        public Dictionary<string, uint> FriendHireFunnel
        {
            get { return mFriendHireFunnel; }
        }

        private Dictionary<string, Dictionary<string, uint>> mFunnelNameToFunnel;

        private static FunnelGlobals mInstance;
        public static FunnelGlobals Instance
        {
            get 
            { 
                if (mInstance == null)
                {
                    mInstance = new FunnelGlobals();
                }
                return mInstance; 
            }
        }

        // Shopping funnel
        private FunnelGlobals()
        {
            // Item Purchase Funnel
            mPurchaseFunnel = new Dictionary<string, uint>();
            mPurchaseFunnel[SHOP_OPENED]        = 1;          
            mPurchaseFunnel[CLICKED_BUY]        = 2;         // add currency as property
            mPurchaseFunnel[CLICKED_CONFIRM]    = 3;         // add currency as property
            mPurchaseFunnel[PURCHASE_COMPLETE]  = 4;         // add currency as property

            // Friend hire funnel for fashion minigame
            mFriendHireFunnel = new Dictionary<string, uint>();
            mFriendHireFunnel[HIRE_FRIEND_POPUP]    = 1;     // add level as property
            mFriendHireFunnel[CLICKED_HIRE]         = 2;
            mFriendHireFunnel[CLICKED_PLAY]         = 3;

            mFunnelNameToFunnel = new Dictionary<string, Dictionary<string, uint>>();
            mFunnelNameToFunnel[FUNNEL_FRIEND_HIRE] = mFriendHireFunnel;
            mFunnelNameToFunnel[FUNNEL_PURCHASE] = mPurchaseFunnel;

         }

        public void LogFunnel(string funnelName, string stepName, string props)
        {

            JSDispatcher jsd = new JSDispatcher();
            jsd.LogMetricsFunnelStep(funnelName, mFunnelNameToFunnel[funnelName][stepName], stepName, props, delegate(string s) { });
        }

        public void LogFunnel(string funnelName, uint stepNumber, string stepName, string props)
        {
            JSDispatcher jsd = new JSDispatcher();
            jsd.LogMetricsFunnelStep(funnelName, stepNumber, stepName, props, delegate(string s) { });
        }
    };
}
