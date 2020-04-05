/**  --------------------------------------------------------  *
 *   StoreSelectionState.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/19/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class StoreSelectionState : StoreGuiState
	{	
		List<IWidget> mMockButtons = new List<IWidget>();
		private Action<string> mOpenStore;
		//private Hangout.Shared.Action mCloseInventoryGui;

		//private Window mInventoryWindow; 
		
		private readonly GuiFrame mStoreSelectorFrame;
		private readonly GuiFrame mCategorySideBar;
		private readonly GridView mStoreSelectorGrid;
		
		private readonly Button mStoreButton;
		private readonly Button mCloseInventoryGuiButton;
		
		private const string mResourcePath = "resources://GUI/Inventory/Shopping.gui";
		
		public override string Name
		{
			get { return this.GetType().Name; }
		}

		public StoreSelectionState(Window guiWindow, Action<string> openStore, Hangout.Shared.Action closeInventoryGui)
		: base(guiWindow)
		{
			mOpenStore = openStore;

			mStoreSelectorFrame = guiWindow.SelectSingleElement<GuiFrame>("MainFrame/StoreSelector");
			mCategorySideBar = GuiWindow.SelectSingleElement<GuiFrame>("MainFrame/CategorySideBarFrame");
			mStoreSelectorGrid = guiWindow.SelectSingleElement<GridView>("MainFrame/StoreSelector/StoreSelectorGrid");
			//mShopLabel = guiWindow.SelectSingleElement<Label>("MainFrame/StoreSelector/ShopLabel");
			mStoreButton = GuiWindow.SelectSingleElement<Button>("MainFrame/StoreSelector/StoreSelectorGrid/StoreButton");
			mStoreSelectorGrid.RemoveChildWidget(mStoreButton);
			mCloseInventoryGuiButton = guiWindow.SelectSingleElement<Button>("MainFrame/StoreSelector/CancelButton");
			//mCloseInventoryGui = closeInventoryGui;
			mCloseInventoryGuiButton.AddOnPressedAction(closeInventoryGui);
		}

		//This is where the logic will go to grab the actual items to display from the DB.
		private void GetStoreButtons()
		{
			for (int i = 0; i < 5; ++i)
			{
				Button button = (Button)mStoreButton.Clone();
				if (i % 2 == 0)
				{
					button.Text = "Shoes";
					button.AddOnPressedAction(delegate()
											{
												mOpenStore("Shoes");
											});
					mMockButtons.Add(button);
				}
				else
				{
					button.Text = "Shirts";
					button.AddOnPressedAction(delegate()
											{
												mOpenStore("Shirts");
											});
					mMockButtons.Add(button);
				}
			}
		}
		
		//When we d/l the data for the button this function will be used to update the 
		//button from the loading button to the real Gui button.
		private void UpdateButtons()
		{
		
		}
		//This function is in charge of building temp  buttons to indicate loading.
		private void BuildLoadingButtons()
		{
			
		}
		
		public override void EnterState()
		{
			mMockButtons.Clear();
			//This is where the logic will go to grab the actual items to display from the DB.
			GetStoreButtons();
			mStoreSelectorGrid.SetPositions(mMockButtons, mMockButtons.Count, 1, 0);
			mStoreSelectorFrame.Showing = true;
			mCategorySideBar.Showing = true;
		}
		
		public override void ExitState()
		{	
			mStoreSelectorFrame.Showing = false;
			mCategorySideBar.Showing = false;
		}

		public override void HandleSearchResults(XmlDocument xmlDocument)
		{
			throw new NotImplementedException();
		}
	}
}
