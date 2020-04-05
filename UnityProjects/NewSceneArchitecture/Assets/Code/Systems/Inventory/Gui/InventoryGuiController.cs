/**  --------------------------------------------------------  *
 *   InventoryGuiController.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/12/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System.Xml;
using Hangout.Shared;

namespace Hangout.Client.Gui
{	
	public class InventoryGuiController : GuiController
	{		
		//State Machine Members
		private ClosedInventoryState mClosedInventoryState;
		private StoreSelectionState mStoreSelectionState;
		private StoreDisplayState mStoreDisplayState;
		private StoreStateMachine mStoreStateMachine;
		private PlayerInventoryState mPlayerInventoryState;

		//SideBarButtons
		private Button mCloset;
		private Button mClothing;
		private Button mAccessories;
		private Button mMakeUp;
		private Button mHair;
		private Button mEmote;
		private Button mBodyShape;
		private Button mRoomImage;

        private readonly Window mInventoryWindow;
        private readonly Window mInventoryOverlayWindow;
        private readonly Window mStoreOverlayWindow;
        private readonly Window mPurchaseModalWindow;
        
        private readonly GuiFrame mItemDisplayFrame;
		private readonly GuiFrame mPlayerInventoryFrame;
		private readonly GuiFrame mStoreSelector;
		private readonly GuiFrame mCategorySideBar;

		private Button mLastTabSelected;

		private const string mResourcePath = "resources://GUI/Inventory/Shopping.gui";

		public InventoryGuiController(IGuiManager guiManager)
			: base(guiManager, mResourcePath)
		{
			mInventoryWindow = (Window)this.MainGui;
			mInventoryWindow.OnShowing(OnShowingCallback);

			mItemDisplayFrame = mInventoryWindow.SelectSingleElement<GuiFrame>("MainFrame/ItemDisplayFrame");
			mStoreSelector = mInventoryWindow.SelectSingleElement<GuiFrame>("MainFrame/StoreSelector");
			mPlayerInventoryFrame = mInventoryWindow.SelectSingleElement<GuiFrame>("MainFrame/PlayerInventoryFrame");
			mCategorySideBar = mInventoryWindow.SelectSingleElement<GuiFrame>("MainFrame/CategorySideBarFrame");
            foreach (ITopLevel topLevel in AllGuis)
            {
                if (topLevel.Name == "StoreOverlayWindow")
                {
                    mStoreOverlayWindow = (Window)topLevel;
                }
                if (topLevel.Name == "InventoryOverlayWindow")
                {
                    mInventoryOverlayWindow = (Window)topLevel;
                }
                if (topLevel.Name == "PurchaseModalWindow")
                {
					mPurchaseModalWindow = (Window)topLevel;
                    Label modalLabel = mPurchaseModalWindow.SelectSingleElement<Label>("MainFrame/PurchaseModalFrame/PurchasingLabel");
                    modalLabel.Text = Translation.PURCHASE_COMPLETING_TRANSACTION;

                }
            }
            SetUpSideBar();
			
			//Hide EEERRRRRRYTHANG
			mInventoryWindow.Showing = false;
			mItemDisplayFrame.Showing = false;
			mStoreSelector.Showing = false;
			mPlayerInventoryFrame.Showing = false;
			mCategorySideBar.Showing = false;
			
			mPurchaseModalWindow.Showing = false;
			
			ConstructStateMachine();
		}
		
		private void OnShowingCallback(bool showing)
		{
			if (!showing)
			{
				GameFacade.Instance.SendNotification(GameFacade.PLAY_SOUND_CLOSE);
			}
		}
		
		//Side bar category buttons.  The buttons call search with a specific query.  This assumes
		//the gui is in the StoreDisplayState.  Which should be true since we only show this when that
		//state is opened.
		private void SetUpSideBar()
		{
			mCloset = mInventoryWindow.SelectSingleElement<Button>("MainFrame/CategorySideBarFrame/ClosetButton");
			mCloset.AddOnPressedAction(delegate(){
					mStoreStateMachine.TransitionToState(mPlayerInventoryState);
					GameFacade.Instance.SendNotification(GameFacade.SHOP_BODY_VIEW);
					SelectActiveTab(mCloset);
			});
			
			mClothing = mInventoryWindow.SelectSingleElement<Button>("MainFrame/CategorySideBarFrame/ClothesButton");
			mClothing.AddOnPressedAction(delegate()
			{
					mStoreStateMachine.TransitionToState(mStoreDisplayState);
                    mStoreDisplayState.Search(InventoryGlobals.HANGOUT_STORE, ItemType.TOPS + "," + ItemType.PANTS + "," + ItemType.SKIRT, 0);
					GameFacade.Instance.SendNotification(GameFacade.SHOP_BODY_VIEW);
					SelectActiveTab(mClothing);
			});

			mAccessories = mInventoryWindow.SelectSingleElement<Button>("MainFrame/CategorySideBarFrame/AccessoriesButton");
			mAccessories.AddOnPressedAction(delegate()
			{
				mStoreStateMachine.TransitionToState(mStoreDisplayState);
				mStoreDisplayState.Search(InventoryGlobals.HANGOUT_STORE, ItemType.SHOES + "," + ItemType.BAGS, 0);
				GameFacade.Instance.SendNotification(GameFacade.SHOP_BODY_VIEW);
				SelectActiveTab(mAccessories);
			});

			mMakeUp = mInventoryWindow.SelectSingleElement<Button>("MainFrame/CategorySideBarFrame/MakeupButton");
			mMakeUp.AddOnPressedAction(delegate()
			{
					mStoreStateMachine.TransitionToState(mStoreDisplayState);
					mStoreDisplayState.Search(InventoryGlobals.HANGOUT_STORE, ItemType.MAKEUP, 0);
					GameFacade.Instance.SendNotification(GameFacade.SHOP_HEAD_VIEW);
					SelectActiveTab(mMakeUp);
			});
			mHair = mInventoryWindow.SelectSingleElement<Button>("MainFrame/CategorySideBarFrame/HairButton");
			mHair.AddOnPressedAction(delegate()
			{
					mStoreStateMachine.TransitionToState(mStoreDisplayState);
					mStoreDisplayState.Search(InventoryGlobals.HANGOUT_STORE, ItemType.HAIR, 0);
					GameFacade.Instance.SendNotification(GameFacade.SHOP_HEAD_VIEW);
					SelectActiveTab(mHair);
			});

			mEmote = mInventoryWindow.SelectSingleElement<Button>("MainFrame/CategorySideBarFrame/EmoteButton");
			mEmote.AddOnPressedAction(delegate()
			{
			        mStoreStateMachine.TransitionToState(mStoreDisplayState);
					mStoreDisplayState.Search(InventoryGlobals.HANGOUT_STORE, ItemType.MOOD + "," + ItemType.EMOTE + "," + ItemType.EMOTICON, 0);
			        GameFacade.Instance.SendNotification(GameFacade.SHOP_BODY_VIEW);
			        SelectActiveTab(mEmote);
			});

			mBodyShape = mInventoryWindow.SelectSingleElement<Button>("MainFrame/CategorySideBarFrame/BodyButton");
			mBodyShape.AddOnPressedAction(delegate()
			{
					mStoreStateMachine.TransitionToState(mStoreDisplayState);
					mStoreDisplayState.Search(InventoryGlobals.HANGOUT_STORE, ItemType.BODY, 0);
					GameFacade.Instance.SendNotification(GameFacade.SHOP_BODY_VIEW);
					SelectActiveTab(mBodyShape);
			});
			mRoomImage = mInventoryWindow.SelectSingleElement<Button>("MainFrame/CategorySideBarFrame/RoomImageButton");
			mRoomImage.AddOnPressedAction(delegate()
			{
					mStoreStateMachine.TransitionToState(mStoreDisplayState);
					mStoreDisplayState.Search(InventoryGlobals.HANGOUT_STORE, ItemType.ROOM + "," + ItemType.ROOM_BACKDROP, 0);
					GameFacade.Instance.SendNotification(GameFacade.SHOP_BODY_VIEW);
					SelectActiveTab(mRoomImage);
			});
		}

		private void SelectActiveTab(Button activeTabButton)
		{
			// First enable them all
			mCloset.Enabled = true;
			mClothing.Enabled = true;
			mAccessories.Enabled = true;
			mMakeUp.Enabled = true;
			mHair.Enabled = true;
			mEmote.Enabled = true;
			mBodyShape.Enabled = true;
			mRoomImage.Enabled = true;

			mLastTabSelected = activeTabButton;
			activeTabButton.Enabled = false;
		}

		private void ConstructStateMachine()
		{
			mStoreStateMachine = new StoreStateMachine();

			mClosedInventoryState = new ClosedInventoryState(mInventoryWindow);
			mStoreSelectionState = new StoreSelectionState(mInventoryWindow, OpenStore, Close);
            mStoreDisplayState = new StoreDisplayState(mInventoryWindow, mStoreOverlayWindow, Close);
			mPlayerInventoryState = new PlayerInventoryState(mInventoryWindow, mInventoryOverlayWindow, Close, OpenStoreSelector);

			mClosedInventoryState.AddTransition(mStoreSelectionState);
			mClosedInventoryState.AddTransition(mStoreDisplayState);
			mClosedInventoryState.AddTransition(mPlayerInventoryState);
			//mClosedInventoryState.AddTransition(mClosedInventoryState);

			mPlayerInventoryState.AddTransition(mClosedInventoryState);
			mPlayerInventoryState.AddTransition(mPlayerInventoryState);
			mPlayerInventoryState.AddTransition(mStoreSelectionState);
			mPlayerInventoryState.AddTransition(mStoreDisplayState);
			mPlayerInventoryState.AddTransition(mPlayerInventoryState);
			
// 			mStoreSelectionState.AddTransition(mClosedInventoryState);
// 			mStoreSelectionState.AddTransition(mStoreDisplayState);
// 			mStoreSelectionState.AddTransition(mPlayerInventoryState);
			
			mStoreDisplayState.AddTransition(mStoreSelectionState);
			mStoreDisplayState.AddTransition(mPlayerInventoryState);
			mStoreDisplayState.AddTransition(mClosedInventoryState);
			mStoreDisplayState.AddTransition(mStoreDisplayState);
			
			mStoreStateMachine.EnterInitialState(mClosedInventoryState);
		}

		/// <summary>
		/// Passes the xml response to the current state to parse out the search results
		/// </summary>
		/// <param name="xmlResponse"></param>
        public void HandlePlayerInventoryResult(XmlDocument xmlResponse)
		{
            if (mStoreStateMachine.CurrentState == mPlayerInventoryState) 
            {
                mStoreStateMachine.CurrentState.HandleSearchResults(xmlResponse);
            }
		}

        /// <summary>
        /// Passes the xml response to the current state to parse out the search results
        /// </summary>
        /// <param name="xmlResponse"></param>
        public void HandleStoreInventoryResult(XmlDocument xmlResponse)
        {
            if (mStoreStateMachine.CurrentState == mStoreDisplayState)
            {
                mStoreStateMachine.CurrentState.HandleSearchResults(xmlResponse);
            }
        }

		private void OpenStore(string storeName)
		{
			// NOTE: we are ignoring the store name for now since we have only one store
			mStoreStateMachine.TransitionToState(mStoreDisplayState);
			SelectActiveTab(mClothing);
		}
		
		private void OpenStoreSelector()
		{
			mCategorySideBar.Showing = false;
			mStoreStateMachine.TransitionToState(mStoreSelectionState);
			SelectActiveTab(mClothing);
		}

		
		public void Close()
		{
			GameFacade.Instance.SendNotification(GameFacade.SHOP_CLOSED);
		}
		
		public void BeginShopping()
		{
			mCategorySideBar.Showing = false;
			mStoreStateMachine.TransitionToState(mStoreDisplayState);
			// Then set the active one
			// Setting the button to disabled shows the active state (see the xml style)
			if (mLastTabSelected == null)
			{
				// First time to open the shopping gui, lets select clothing
				mLastTabSelected = mClothing;
				mStoreDisplayState.Search(InventoryGlobals.HANGOUT_STORE, ItemType.TOPS + "," + ItemType.PANTS + "," + ItemType.SKIRT, 0);
			}
			
			SelectActiveTab(mLastTabSelected);
		}
		
		public void TogglePlayerInventory()
		{
			if (mStoreStateMachine.CurrentState == mPlayerInventoryState)
			{
				ClosePlayerInventory();
			}
			else
			{
				OpenPlayerInventory();
			}
		}
		
		public void OpenPlayerInventory()
		{
			mCategorySideBar.Showing = true;
			mStoreStateMachine.TransitionToState(mPlayerInventoryState);
			SelectActiveTab(mCloset);
		}

        public void ClosePlayerInventory()
        {
            mCategorySideBar.Showing = false;
            if (mStoreStateMachine.CurrentState != mClosedInventoryState)
            {
				mStoreStateMachine.TransitionToState(mClosedInventoryState);
            }
        }
        
        public void ShowPurchaseModal()
        {
			mPurchaseModalWindow.Showing = true;
			mPurchaseModalWindow.InFront = true;
        }
        
        public void HidePurchaseModal()
        {
			mPurchaseModalWindow.Showing = false;
        }
    }
}