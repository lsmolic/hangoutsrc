/**  --------------------------------------------------------  *
 *   PlayerInventoryState.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/25/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Client.Gui;
using Hangout.Shared;

namespace Hangout.Client
{
	public class PlayerInventoryState : StoreGuiState
	{
		protected XmlDocument mCurrentSearchResultsXml;

		private Hangout.Shared.Action mCloseInventoryGui;
		//private readonly Hangout.Shared.Action mBackToStoreSelectorCallback;

		// Item Info Detail Panel
		private readonly Window mOverlayWindow;
		private readonly Image mItemOverlayImage;
		private readonly Button mItemOverlayCloseButton;
		
		//private Button mBackToStoreSelectorButton;

		private readonly GuiFrame mItemInfoFrame;
		private readonly Label mItemInfoTitleLabel;
		private readonly Label mItemInfoDescriptionLabel;
		//private readonly Label mItemInfoNumAvailableLabel;
		//private readonly Label mItemInfoExpirationLabel;

		//private readonly Texture mRealMoneyIcon;
		//private readonly Texture mGameCurrencyIcon;

		//Gui Elements
		private readonly GridView mItemDisplayGrid;

		private readonly GuiFrame mPlayerInventoryFrame;
		private readonly GuiFrame mPaginationFrame;
		private readonly GuiFrame mCategorySideBar;
		private GuiFrame mItemGridButtonFrame;

		private readonly Button mNextPageButton;
		private readonly Button mPreviousPageButton;
		private readonly Button mLastPageButton;
		private readonly Button mFirstPageButton;
		private readonly Button mCloseInventoryGuiButton;
		//private readonly Button mOverlayGiftButton;
		private readonly Button mOverlayRemoveButton;

		//private readonly Label mItemOverlayPrice;
		//private Image mItemOverlayCurrencyIcon;

		private readonly Label mPaginationLabel;
		//private readonly Label mTitleLabel;
		private int mCurrentStartIndex = 0;
		
		private int mGridWidth = 4;
		private int mGridHeight = 2;
		private int mBlockSize = 8;
		private int mTotalPages = 0;
		//private XmlElement mCurrentSelectedItem;

		private List<KeyValuePair<GuiFrame, Button>> mGridItemButtons = new List<KeyValuePair<GuiFrame, Button>>();

		public PlayerInventoryState(Window guiWindow, Window overlayWindow, Hangout.Shared.Action closeInventoryGui, Hangout.Shared.Action backToStoreSelectorCallback)
			: base(guiWindow)
		{
			mCloseInventoryGui = closeInventoryGui;
			//mBackToStoreSelectorCallback = backToStoreSelectorCallback;

			mPlayerInventoryFrame = GuiWindow.SelectSingleElement<GuiFrame>("MainFrame/PlayerInventoryFrame");
			mItemDisplayGrid = GuiWindow.SelectSingleElement<GridView>("MainFrame/PlayerInventoryFrame/ItemDisplayGrid");
			// Item Info Detail Panel
			mItemInfoFrame = GuiWindow.SelectSingleElement<GuiFrame>("MainFrame/PlayerInventoryFrame/InfoFrame");
			mItemInfoTitleLabel = mItemInfoFrame.SelectSingleElement<Label>("DescriptionScrollFrame/TitleLabel");
			//mItemInfoExpirationLabel = mItemInfoFrame.SelectSingleElement<Label>("ExpirationLabel");
			mItemInfoDescriptionLabel = mItemInfoFrame.SelectSingleElement<Label>("DescriptionScrollFrame/DescriptionLabel");
			//mItemInfoNumAvailableLabel = mItemInfoFrame.SelectSingleElement<Label>("NumAvailableLabel");
			
			mCategorySideBar = GuiWindow.SelectSingleElement<GuiFrame>("MainFrame/CategorySideBarFrame");

			mItemGridButtonFrame = GuiWindow.SelectSingleElement<GuiFrame>("MainFrame/PlayerInventoryFrame/ItemDisplayGrid/GridButtonFrame");
 			mCloseInventoryGuiButton = GuiWindow.SelectSingleElement<Button>("MainFrame/PlayerInventoryFrame/CloseInventoryGuiButton");
 			mCloseInventoryGuiButton.AddOnPressedAction(delegate()
 			{
 				mCloseInventoryGui();
 			});

			// Item Info Overlay 
			mOverlayWindow = overlayWindow;
			GuiFrame overlayFrame = mOverlayWindow.SelectSingleElement<GuiFrame>("MainFrame/OverlayFrame");
			mOverlayRemoveButton = overlayFrame.SelectSingleElement<Button>("Action");
			mItemOverlayImage = overlayFrame.SelectSingleElement<Image>("Image");
			mItemOverlayCloseButton = overlayFrame.SelectSingleElement<Button>("CloseButton");
			mItemOverlayCloseButton.AddOnPressedAction(delegate()
			{
				mOverlayWindow.Showing = false;
			});
			mPaginationFrame = GuiWindow.SelectSingleElement<GuiFrame>("MainFrame/PlayerInventoryFrame/PaginationFrame");
			mNextPageButton = GuiWindow.SelectSingleElement<Button>("MainFrame/PlayerInventoryFrame/PaginationFrame/PaginationControlFrame/NextPageButton");
			mPreviousPageButton = GuiWindow.SelectSingleElement<Button>("MainFrame/PlayerInventoryFrame/PaginationFrame/PaginationControlFrame/PreviousPageButton");
			mPaginationLabel = GuiWindow.SelectSingleElement<Label>("MainFrame/PlayerInventoryFrame/PaginationFrame/PaginationControlFrame/PaginationLabel");
			mNextPageButton.AddOnPressedAction(delegate()
			{
				NextPage();
			});
			mPreviousPageButton.AddOnPressedAction(delegate()
			{
				PreviousPage();
			});
			mLastPageButton = GuiWindow.SelectSingleElement<Button>("MainFrame/PlayerInventoryFrame/PaginationFrame/PaginationControlFrame/LastPageButton");
			mFirstPageButton = GuiWindow.SelectSingleElement<Button>("MainFrame/PlayerInventoryFrame/PaginationFrame/PaginationControlFrame/FirstPageButton");
			mLastPageButton.Disable();
			mLastPageButton.AddOnPressedAction(delegate()
			{
				LastPage();
			});
			mFirstPageButton.Disable();
			mFirstPageButton.AddOnPressedAction(delegate()
			{
				FirstPage();
			});
			mItemDisplayGrid.RemoveChildWidget(mItemGridButtonFrame);

			//mTitleLabel = mItemInfoFrame.SelectSingleElement<Label>("TitleLabel");

			mOverlayWindow.Showing = false;
			mItemInfoFrame.Showing = false;
		}

		/// <summary>
		/// Create empty buttons for the inventory page.  The contents of the buttons will be filled in by HandleSearchResults
		/// </summary>
		private void CreateInventoryButtons()
		{
			mGridItemButtons.Clear();
			for (int i = 0; i < mBlockSize; i++)
			{
				GuiFrame gridItemFrame = (GuiFrame)mItemGridButtonFrame.Clone();
				Button button = gridItemFrame.SelectSingleElement<Button>("GridButton");
				mItemDisplayGrid.AddChildWidget(gridItemFrame, new FixedPosition(0, 0));
				button.Text = "Loading...";
				button.Disable();
				mGridItemButtons.Add(new KeyValuePair<GuiFrame, Button>(gridItemFrame, button));
			}
		}

		/// <summary>
		/// Clear text and images from store buttons.   
		/// TODO: we may need to remove buttons from the itemGrid here
		/// </summary>
		private void ClearStoreButtons()
		{
			for (int i = 0; i < mBlockSize; i++)
			{
				((Button)mGridItemButtons[i].Value).Text = "Loading...";
				((Button)mGridItemButtons[i].Value).Image = null;
				((Button)mGridItemButtons[i].Value).Enabled = false;
				ClearGuiFrame((GuiFrame)(mGridItemButtons[i].Key));
			}
		}
		
		private void ClearGuiFrame(GuiFrame frameToClear)
		{
			Image frameImage = frameToClear.SelectSingleElement<Image>("Thumbnail");
			frameImage.Texture = null;
			Label price = frameToClear.SelectSingleElement<Label>("PriceLabel");
			price.Text = "";
		}
        private void CleanupInventoryButtons()
        {
            for (int i = 0; i < mBlockSize; i++)
            {
                KeyValuePair<GuiFrame, Button> kvp = mGridItemButtons[i];
                kvp.Value.ClearOnPressedActions();
                kvp.Value.Enabled = false;
                mItemDisplayGrid.RemoveChildWidget(kvp.Key);
            }
            mGridItemButtons.Clear();
        }
        
        private void ClearButton(Button button)
        {
			button.ClearOnPressedActions();
			button.Enabled = false;
			button.Text = "";
        }

		/// <summary>
		/// Populate the buttons with info received from the inventory search query
		/// </summary>
		private void UpdateInventoryButtons()
		{
			if (mCurrentSearchResultsXml != null)
			{
                XmlNodeList itemList = mCurrentSearchResultsXml.SelectNodes("/Response/itemInstances/itemInstance/item");
                int i = 0;
				foreach (XmlElement itemNode in itemList)
				{
                    string thumbnailUrl = ConvertPaymentItemsUrl(itemNode.GetAttribute("smallImageUrl"));

					// Fill in details of button
					Button currentButton = (Button)mGridItemButtons[i].Value;
					GuiFrame buttonFrame = mGridItemButtons[i].Key;
					buttonFrame.Showing = true;
					Image thumbnail = buttonFrame.SelectSingleElement<Image>("Thumbnail");
					//Label price = buttonFrame.SelectSingleElement<Label>("PriceLabel");

                    XmlElement itemNodeRef = itemNode;
					currentButton.ClearOnPressedActions();
					currentButton.Text = Translation.LOADING_PROGRESS;
					currentButton.AddOnPressedAction(delegate()
					{
						GridItemSelected(buttonFrame, itemNodeRef);
					});
					currentButton.Enabled = false;

					// If no thumbnail, don't load anything.
					// TODO: should we load a default image?
					if (thumbnailUrl != "")
					{
						ApplyImageToButton(thumbnailUrl, currentButton, thumbnail);
					}
					else
					{
						currentButton.Text = "No Image.";
					}

					i++;
                    if (i >= mBlockSize)
                    {
                        break;
                    }
				}
				
				for (int j = i; j < mBlockSize; ++j)
				{
					ClearButton(mGridItemButtons[j].Value);
				}
			}
			else
			{
				Console.LogError("no items found in store response");
			}
		}

		/// <summary>
		/// Update the page number based on the returned search results (i.e. Page 1/2)
		/// </summary>
		private void UpdatePageNumbers()
		{
			if (mCurrentSearchResultsXml != null)
			{
				// Update the pages
				XmlElement itemOffersNode = (XmlElement)mCurrentSearchResultsXml.SelectSingleNode("/Response/itemInstances");
				double startIndex = Double.Parse(itemOffersNode.GetAttribute("startIndex"));
				double blockSize = Double.Parse(itemOffersNode.GetAttribute("blockSize"));
				double totalItems = Double.Parse(itemOffersNode.GetAttribute("total"));

				int currentPage = (int)Math.Floor(startIndex / blockSize) + 1;
				int totalPages = (int)Math.Ceiling(totalItems / blockSize);
				mTotalPages = totalPages;
				// Hide next/prev buttons if we are at the edge of our range
				ShowNextPrevButtons();
				
				if (currentPage <= 1)
				{
					mPreviousPageButton.Disable();
				}
				else
				{
					mPreviousPageButton.Enable();
				}

				if (currentPage >= totalPages)
				{
					mNextPageButton.Disable();
				} 
				else
				{
					mNextPageButton.Enable();
				}
				// First/Last buttons
				if (currentPage != 1)
				{
					mFirstPageButton.Enable();
				}
				else
				{
					mFirstPageButton.Disable();
				}
				if (currentPage != totalPages)
				{
					mLastPageButton.Enable();
				}
				else
				{
					mLastPageButton.Disable();
				}

				mPaginationLabel.Text = String.Format("Page {0}/{1}", currentPage.ToString(), totalPages.ToString());
			}
		}

		/// <summary>
		/// Show the item detail popup
		/// </summary>
		/// <param name="button"></param>
		/// <param name="index">Index of the item on the current page that this button corresponds to</param>
		private void GridItemSelected(IWidget widget, XmlElement itemNode)
		{
            //Debug.Log("GridItemSelected " + itemNode.OuterXml);
			//XmlElement priceNode = (XmlElement)itemOfferNode.SelectSingleNode("price/money");
			//string description = itemOfferNode.GetAttribute("description");
			//string itemOfferId = itemOfferNode.GetAttribute("id");
			//string itemExpiration = itemOfferNode.GetAttribute("endDate");
			//string itemQuantity = itemOfferNode.GetAttribute("numAvailable");
			//string thumbnailUrl = itemNode.GetAttribute("smallImageUrl");
            string itemName = itemNode.GetAttribute("buttonName");
            string detailUrl = itemNode.GetAttribute("largeImageUrl");
			string uniqueId = itemNode.GetAttribute("name");
            string itemType = itemNode.GetAttribute("itemTypeName");
            string description = itemNode.GetAttribute("description");
            XmlNode assetsNode = itemNode.SelectSingleNode("Assets");

			//mTitleLabel.Text = button.Text;
			//TODO: this would be where the data in the info frame is updated.
			ShowItemInfoOverlay(widget, detailUrl);

			ShowItemInfoFrame(itemName, description);

            SetupOverlayActionButtons(uniqueId, itemType, assetsNode);
		}

		private void ShowItemInfoFrame(string itemName, string description)
		{
			mItemInfoFrame.Showing = true;
			mItemInfoTitleLabel.Text = itemName;
			mItemInfoDescriptionLabel.Text = description;

			//itemExpiration = itemExpiration.Replace("PST", "").Replace("PDT", "").Trim();

			//DateTime dateTime = DateTime.ParseExact(itemExpiration, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
			//string dateString = dateTime.ToString("MMM dd, yyyy");

			//mItemInfoExpirationLabel.Text = String.Format(Translation.STORE_EXPIRATION, dateString);
// 			if (Int32.Parse(itemQuantity) > 0)
// 			{
// 				// TODO: maxAvailable should be passed with twofish response, but currently we only have num left, so make this max up
// 				string maxAvailable = "1000";
// 				mItemInfoNumAvailableLabel.Text = String.Format(Translation.STORE_QUANTITY, itemQuantity, maxAvailable);
// 			}
// 			else
// 			{
// 				mItemInfoNumAvailableLabel.Text = Translation.STORE_QUANTITY_UNLIMITED;
// 			}
		}
		
		private void ShowItemInfoOverlay(IWidget widget, string detailUrl)
		{
			PositionWindowOverButton(mOverlayWindow, widget, mItemDisplayGrid);
			foreach (KeyValuePair<GuiFrame, Button> kvp in mGridItemButtons)
			{
				if (kvp.Key == widget)
				{
					Image thumbnail = kvp.Key.SelectSingleElement<Image>("Thumbnail");
					mItemOverlayImage.Texture = thumbnail.Texture;
					break;
				}
			}
		}

		private void SetupOverlayActionButtons(string itemId, string itemType, XmlNode assetsNode)
		{
			// Setup action buttons specific to the item type
			mOverlayRemoveButton.ClearOnPressedActions();

			LocalAvatarEntity localAvatarEntity = GameFacade.Instance.RetrieveMediator<AvatarMediator>().LocalAvatarEntity;

			switch (itemType)
			{
				case ItemType.ROOM_BACKDROP:
                    RoomManagerProxy roomManagerProxy = GameFacade.Instance.RetrieveProxy<RoomManagerProxy>();
                    if (roomManagerProxy.IsRoomOwner())
                    {
                        ApplyImageToRoom(itemId);
                    }
                    else
                    {
                        mOverlayRemoveButton.Disable();
                    }
					break;
				case ItemType.TOPS:
				case ItemType.PANTS:
                case ItemType.SKIRT:
                case ItemType.BAGS:
                case ItemType.SHOES:
				case ItemType.MAKEUP:				
					// Call method to apply clothing to body
                    localAvatarEntity.ApplyTempClothingToAvatar(assetsNode);

					mOverlayRemoveButton.Enable();
					mOverlayRemoveButton.Text = Translation.REMOVEIT;
					mOverlayRemoveButton.AddOnPressedAction(delegate()
					{
						//Remove clothing method.
						localAvatarEntity.RemoveClothingFromAvatar(assetsNode);
						mOverlayWindow.Showing = false;
						
					});
					break;
				case ItemType.FACE:
				case ItemType.HAIR:
				case ItemType.BODY:
					// Call method to apply clothing to body
					localAvatarEntity.ApplyTempClothingToAvatar(assetsNode);

					mOverlayRemoveButton.Disable();
					break;
			}
		}

		private void ApplyImageToRoom(string itemIdString)
		{
			ItemId itemId = new ItemId(UInt32.Parse(itemIdString));
			GameFacade.Instance.RetrieveProxy<RoomManagerProxy>().UpdateCurrentRoomBackground(itemId);
		}

		private void ShowNextPrevButtons()
		{
			mPaginationFrame.Showing = true;
			mNextPageButton.Showing = true;
			mPreviousPageButton.Showing = true;
		}

		private void DisableNextPrevButtons()
		{
			mNextPageButton.Disable();
			mPreviousPageButton.Disable();
		}

		/// <summary>
		/// Search for next block of items
		/// </summary>
		private void NextPage()
		{
			HideInfoAndOverlayFrames();
			DisableNextPrevButtons();
			ClearStoreButtons();

			// TODO: Do range checking
			mCurrentStartIndex += mBlockSize;

			InventoryProxy inventoryProxy = GameFacade.Instance.RetrieveProxy<InventoryProxy>();
            inventoryProxy.GetPlayerInventory(mCurrentStartIndex, mBlockSize, inventoryProxy.HandlePlayerInventoryResponse);
        }

		private void PreviousPage()
		{
			HideInfoAndOverlayFrames();
			DisableNextPrevButtons();
			ClearStoreButtons();

			// TODO: Do range checking
			mCurrentStartIndex -= mBlockSize;

			InventoryProxy inventoryProxy = GameFacade.Instance.RetrieveProxy<InventoryProxy>();
			inventoryProxy.GetPlayerInventory(mCurrentStartIndex, mBlockSize, inventoryProxy.HandlePlayerInventoryResponse);
		}

		private void FirstPage()
		{
			HideInfoAndOverlayFrames();
			DisableNextPrevButtons();
			ClearStoreButtons();

			// TODO: Do range checking
			mCurrentStartIndex = 0;

			mPreviousPageButton.Disable();
			mFirstPageButton.Disable();

			InventoryProxy inventoryProxy = GameFacade.Instance.RetrieveProxy<InventoryProxy>();
			inventoryProxy.GetPlayerInventory(mCurrentStartIndex, mBlockSize, inventoryProxy.HandlePlayerInventoryResponse);
		}

		private void LastPage()
		{
			HideInfoAndOverlayFrames();
			DisableNextPrevButtons();
			ClearStoreButtons();

			// TODO: Do range checking
			mCurrentStartIndex = (mTotalPages - 1) * mBlockSize;

			mNextPageButton.Disable();
			mLastPageButton.Disable();

			InventoryProxy inventoryProxy = GameFacade.Instance.RetrieveProxy<InventoryProxy>();
			inventoryProxy.GetPlayerInventory(mCurrentStartIndex, mBlockSize, inventoryProxy.HandlePlayerInventoryResponse);
		}

		private void HideInfoAndOverlayFrames()
		{
			mOverlayWindow.Showing = false;
			mItemInfoFrame.Showing = false;
		}

		private void Gift()
		{
			throw new NotImplementedException();
		}

		private void ThrowAway()
		{
			throw new NotImplementedException();
		}

		public override void EnterState()
		{
			CreateInventoryButtons();
			List<IWidget> gridFrames = new List<IWidget>();
			foreach (KeyValuePair<GuiFrame, Button> kvp in mGridItemButtons)
			{
				gridFrames.Add(kvp.Key);
			}
			mItemDisplayGrid.SetPositions(gridFrames, mGridHeight, mGridWidth, 0);
			mItemDisplayGrid.SetPagination(mBlockSize, 0);
			mPaginationLabel.Text = String.Format("Page {0}/{1}", mItemDisplayGrid.CurrentPage.ToString(), mItemDisplayGrid.GetTotalPages().ToString());
            mCurrentStartIndex = 0;
			DisableNextPrevButtons();
			InventoryProxy inventoryProxy = GameFacade.Instance.RetrieveProxy<InventoryProxy>();
			inventoryProxy.GetPlayerInventory(mCurrentStartIndex, mBlockSize, inventoryProxy.HandlePlayerInventoryResponse);
            mOverlayWindow.Showing = false;
            mItemInfoFrame.Showing = false;
			mPlayerInventoryFrame.Showing = true;
			mCategorySideBar.Showing = true;
		}

		public override void ExitState()
		{
			HideInfoAndOverlayFrames();
			mOverlayWindow.Showing = false;
            mPlayerInventoryFrame.Showing = false;
            mItemInfoFrame.Showing = false;
            mCategorySideBar.Showing = false;
            CleanupInventoryButtons();
			GameFacade.Instance.RetrieveMediator<AvatarMediator>().LocalAvatarEntity.SaveChangesToDna();
            GameFacade.Instance.RetrieveProxy<LocalAvatarProxy>().SaveDna();
        }

		public override void HandleSearchResults(XmlDocument xmlResponse)
		{
            try
            {
                XmlElement responseNode = (XmlElement)xmlResponse.SelectSingleNode("Response");
                string responseVerb = responseNode.GetAttribute("verb");
                if (responseVerb == "GetUserInventory")
                {
                    // Save returned results
                    mCurrentSearchResultsXml = xmlResponse;
                    
                    // Update inventory buttons
					UpdatePageNumbers();
                    UpdateInventoryButtons();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error in processing HandleSearchResults: " + ex.Message);
            }
		}
	}
}
