/**  --------------------------------------------------------  *
 *   StoreDisplayState.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/20/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Xml;
using System.Collections.Generic;

using UnityEngine;

using Hangout.Shared;
using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class StoreDisplayState : StoreGuiState
	{
		//private readonly Hangout.Shared.Action mBackToStoreSelectorCallback;

		protected XmlDocument mCurrentSearchResultsXml;
		private Hangout.Shared.Action mCloseInventoryGui;

		private readonly GridView mItemDisplayGrid;
		private readonly GuiFrame mItemDisplayFrame;

		// Item Info Detail Panel
		private readonly Window mOverlayWindow;

		private readonly GuiFrame mItemInfoFrame;
		private readonly Label mItemInfoTitleLabel;
		private readonly Label mItemInfoDescriptionLabel;
		private readonly Label mItemInfoNumAvailableLabel;
		private readonly Label mItemInfoExpirationLabel;

		private readonly Label mItemOverlayPrice;
		private Image mItemOverlayCoinIcon;
		private Image mItemOverlayCashIcon;
		private readonly Button mItemOverlayActionButton;
		//private readonly Image mItemOverlayOwnedImage;
		private Image mItemOverlayImage;
		private readonly Texture mRealMoneyIcon;
		private readonly Texture mGameCurrencyIcon;

		private readonly GuiFrame mCategorySideBar;
		private readonly GuiFrame mPaginationFrame;

		private readonly GuiFrame mItemGridButtonFrame;
		private readonly Button mNextPageButton;
		private readonly Button mPreviousPageButton;
		private readonly Button mCloseInventoryGuiButton;
		private readonly Button mFirstPageButton;
		private readonly Button mLastPageButton;
		private readonly Button mRemoveButton;

		private int mTotalPages = 0;

		private readonly Label mPaginationLabel;
		private int mCurrentStartIndex = 0;

		private int mGridWidth = 4;
		private int mGridHeight = 2;
		private int mBlockSize = 8;
		private string mCurrentItemType;
		private XmlElement mCurrentSelectedItemOffer;
		//private string mCurrentSelectedOfferCurrency = "";

		private List<KeyValuePair<GuiFrame, Button>> mGridItemButtons = new List<KeyValuePair<GuiFrame, Button>>();

		public override string Name
		{
			get { return this.GetType().Name; }
		}

		public StoreDisplayState(Window guiWindow, Window overlayWindow, Hangout.Shared.Action closeInventoryGui)
			: base(guiWindow)
		{

			mCloseInventoryGui = closeInventoryGui;


			mItemDisplayFrame = GuiWindow.SelectSingleElement<GuiFrame>("MainFrame/ItemDisplayFrame");

			// Item grid display
			mItemDisplayGrid = GuiWindow.SelectSingleElement<GridView>("MainFrame/ItemDisplayFrame/ItemDisplayGrid");
			mItemGridButtonFrame = GuiWindow.SelectSingleElement<GuiFrame>("MainFrame/ItemDisplayFrame/ItemDisplayGrid/GridButtonFrame");

			// Item Info Detail Panel
			mItemInfoFrame = GuiWindow.SelectSingleElement<GuiFrame>("MainFrame/ItemDisplayFrame/InfoFrame");
			mItemInfoTitleLabel = mItemInfoFrame.SelectSingleElement<Label>("DescriptionScrollFrame/TitleLabel");
			mItemInfoExpirationLabel = mItemInfoFrame.SelectSingleElement<Label>("ExpirationLabel");
			mItemInfoDescriptionLabel = mItemInfoFrame.SelectSingleElement<Label>("DescriptionScrollFrame/DescriptionLabel");
			mItemInfoNumAvailableLabel = mItemInfoFrame.SelectSingleElement<Label>("NumAvailableLabel");

			// Tab Sidebar 
			mCategorySideBar = GuiWindow.SelectSingleElement<GuiFrame>("MainFrame/CategorySideBarFrame");

			// Close button
			mCloseInventoryGuiButton = GuiWindow.SelectSingleElement<Button>("MainFrame/ItemDisplayFrame/CloseInventoryButton");
			mCloseInventoryGuiButton.AddOnPressedAction(delegate()
			{
				mCloseInventoryGui();
			});


			// Item Info Overlay 
			mOverlayWindow = overlayWindow;
			GuiFrame overlayFrame = mOverlayWindow.SelectSingleElement<GuiFrame>("MainFrame/OverlayFrame");
			mItemOverlayActionButton = overlayFrame.SelectSingleElement<Button>("GridButtonTopBar/Action");
			mItemOverlayPrice = overlayFrame.SelectSingleElement<Label>("GridButtonTopBar/PriceFrame/Price");
			mItemOverlayCoinIcon = overlayFrame.SelectSingleElement<Image>("GridButtonTopBar/PriceFrame/CoinIcon");
			mItemOverlayCashIcon = overlayFrame.SelectSingleElement<Image>("GridButtonTopBar/PriceFrame/CashIcon");
			//mItemOverlayOwnedImage = overlayFrame.SelectSingleElement<Image>("Owned");
			mItemOverlayImage = overlayFrame.SelectSingleElement<Image>("Image");
			mRemoveButton = overlayFrame.SelectSingleElement<Button>("RemoveButton");
			mRemoveButton.Text = Translation.REMOVEIT;

			mRealMoneyIcon = (Texture)Resources.Load(InventoryGlobals.CashIconPath);
			mGameCurrencyIcon = (Texture)Resources.Load(InventoryGlobals.CoinIconPath);

			mItemOverlayActionButton.Text = Translation.PURCHASE;

			mOverlayWindow.Showing = false;

			// Page forward/back panel
			mPaginationFrame = GuiWindow.SelectSingleElement<GuiFrame>("MainFrame/ItemDisplayFrame/PaginationFrame");
			mNextPageButton = GuiWindow.SelectSingleElement<Button>("MainFrame/ItemDisplayFrame/PaginationFrame/PaginationControlFrame/NextPageButton");
			mPreviousPageButton = GuiWindow.SelectSingleElement<Button>("MainFrame/ItemDisplayFrame/PaginationFrame/PaginationControlFrame/PreviousPageButton");
			mPaginationLabel = GuiWindow.SelectSingleElement<Label>("MainFrame/ItemDisplayFrame/PaginationFrame/PaginationControlFrame/PaginationLabel");
			mNextPageButton.AddOnPressedAction(delegate()
												{
													NextPage();
												});
			mPreviousPageButton.AddOnPressedAction(delegate()
												{
													PreviousPage();
												});
			mLastPageButton = GuiWindow.SelectSingleElement<Button>("MainFrame/ItemDisplayFrame/PaginationFrame/PaginationControlFrame/LastPageButton");
			mFirstPageButton = GuiWindow.SelectSingleElement<Button>("MainFrame/ItemDisplayFrame/PaginationFrame/PaginationControlFrame/FirstPageButton");
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

			// 			mBackToStoreSelectorButton = GuiWindow.SelectSingleElement<Button>("MainFrame/ItemDisplayFrame/PaginationFrame/BackToStoreSelectorButton");
			// 			mBackToStoreSelectorButton.AddOnPressedAction(delegate()
			// 															{
			// 																mBackToStoreSelectorCallback();
			// 															});
			mItemDisplayGrid.RemoveChildWidget(mItemGridButtonFrame);

		}

		/// <summary>
		/// Create empty buttons for the store.  The contents of the buttons will be filled in by HandleSearchResults
		/// </summary>
		private void CreateGridButtons()
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


		private void CleanupGridButtons()
		{
			for (int i = 0; i < mBlockSize; i++)
			{
				KeyValuePair<GuiFrame, Button> kvp = mGridItemButtons[i];
				kvp.Value.ClearOnPressedActions();
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


		private string FormatPrice(string price)
		{
			return ((int)Double.Parse(price)).ToString();
		}

		/// <summary>
		/// Update store buttons from xml data returned by server
		/// </summary>
		private void UpdateStoreButtons()
		{
			if (mCurrentSearchResultsXml != null)
			{
				XmlNodeList itemOfferList = mCurrentSearchResultsXml.SelectNodes("/Response/store/itemOffers/itemOffer");
				int i = 0;
				foreach (XmlElement itemOfferNode in itemOfferList)
				{
					//string description = itemOfferNode.GetAttribute("description");

					XmlElement itemNode = (XmlElement)itemOfferNode.SelectSingleNode("item");
					string thumbnailUrl = ConvertPaymentItemsUrl(itemNode.GetAttribute("smallImageUrl"));

					// Fill in details of button
					XmlElement itemOfferElement = itemOfferNode;
					Button currentButton = (Button)mGridItemButtons[i].Value;
					//currentButton.Text = itemName; 
					GuiFrame buttonFrame = mGridItemButtons[i].Key;
					Image thumbnail = buttonFrame.SelectSingleElement<Image>("Thumbnail");
					buttonFrame.Showing = true;
					Label price = buttonFrame.SelectSingleElement<Label>("PriceLabel");
					XmlNode moneyNode = itemOfferNode.SelectSingleNode("price/money");
					price.Text = FormatPrice(XmlUtility.GetStringAttribute(moneyNode, "amount"));

					Image priceIcon = buttonFrame.SelectSingleElement<Image>("PriceIcon");
					if (XmlUtility.GetStringAttribute(moneyNode, "currencyName") == "VCOIN")
					{
						priceIcon.Texture = mGameCurrencyIcon;
					}
					else
					{
						priceIcon.Texture = mRealMoneyIcon;
					}

					currentButton.ClearOnPressedActions();
					currentButton.Text = "Loading...";
					currentButton.Enabled = false;
					currentButton.AddOnPressedAction(delegate()
					{
						GridItemSelected(buttonFrame, itemOfferElement, itemNode);
					});

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
			Image priceIcon = frameToClear.SelectSingleElement<Image>("PriceIcon");
			priceIcon.Texture = null;
		}

		/// <summary>
		/// Update the page number based on the returned search results (i.e. Page 1/2)
		/// </summary>
		private void UpdatePageNumbers()
		{
			if (mCurrentSearchResultsXml != null)
			{
				// Update the pages
				XmlElement itemOffersNode = (XmlElement)mCurrentSearchResultsXml.SelectSingleNode("/Response/store/itemOffers");
				double startIndex = Double.Parse(itemOffersNode.GetAttribute("startIndex"));
				double blockSize = Double.Parse(itemOffersNode.GetAttribute("blockSize"));
				double totalItems = Double.Parse(itemOffersNode.GetAttribute("total"));
				double currentPage = (int)Math.Floor(startIndex / blockSize) + 1;
				mTotalPages = (int)Math.Ceiling(totalItems / blockSize);
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

				if (currentPage >= mTotalPages)
				{
					mNextPageButton.Disable();
				}
				else
				{
					mNextPageButton.Enable();
				}
				if (currentPage != 1)
				{
					mFirstPageButton.Enable();
				}
				else
				{
					mFirstPageButton.Disable();
				}
				if (currentPage != mTotalPages)
				{
					mLastPageButton.Enable();
				}
				else
				{
					mLastPageButton.Disable();
				}
				mPaginationLabel.Text = String.Format("Page {0}/{1}", currentPage.ToString(), mTotalPages.ToString());
			}
		}

		/// <summary>
		/// Show the item detail popup
		/// </summary>
		/// <param name="button"></param>
		/// <param name="index">Index of the item on the current page that this button corresponds to</param>
		private void GridItemSelected(IWidget widget, XmlElement itemOfferNode, XmlElement itemNode)
		{
			XmlElement priceNode = (XmlElement)itemOfferNode.SelectSingleNode("price/money");
			string description = itemOfferNode.GetAttribute("description");
			string itemName = itemOfferNode.GetAttribute("title");
			string thumbnailUrl = ConvertPaymentItemsUrl(itemNode.GetAttribute("smallImageUrl"));
			string uniqueId = itemNode.GetAttribute("name");
			string itemExpiration = itemOfferNode.GetAttribute("endDate");
			string itemQuantity = itemOfferNode.GetAttribute("numAvailable");
			string currency = priceNode.GetAttribute("currencyName");
			string price = priceNode.GetAttribute("amount");
			string itemType = itemNode.GetAttribute("itemTypeName");
			XmlNode assetsNode = itemNode.SelectSingleNode("Assets");

			mCurrentSelectedItemOffer = itemOfferNode;

			ShowItemInfoOverlay(widget, thumbnailUrl, price, currency);

			ShowItemInfoFrame(itemName, description, itemExpiration, itemQuantity);

			SetupOverlayActionButtons(uniqueId, itemType, assetsNode);

			// Log for metrics
			if (currency == "HOUTS")
			{
				EventLogger.Log(LogGlobals.CATEGORY_SHOPPING, LogGlobals.CLICKED_ON_CASH_ITEM,
					itemName, price);
				
				mItemOverlayActionButton.ClearOnPressedActions();
				mItemOverlayActionButton.AddOnPressedAction(delegate()
				{
					mOverlayWindow.Showing = false;
				});
				
				// Disable the Buy button if you don't have enough currency to purchase.
				if (GameFacade.Instance.RetrieveProxy<InventoryProxy>().Houts < Double.Parse(price))
				{
					mItemOverlayActionButton.AddOnPressedAction(delegate()
					{
						BuyCoinUtility.GoToBuyCashPage
						(
							Translation.NEED_CASH_TITLE,
							Translation.NOT_ENOUGH_COIN,
							delegate(string s) 
							{ 
								Console.WriteLine("Opening cash store: " + s);
							},
							delegate()
							{
								GameFacade.Instance.SendNotification(GameFacade.GET_CASH_GUI_CLOSED);
							}
						);
					});
				}
				else
				{
					mItemOverlayActionButton.AddOnPressedAction(delegate()
					{
						Purchase();
					});
				}
			}
			else
			{
				EventLogger.Log(LogGlobals.CATEGORY_SHOPPING, LogGlobals.CLICKED_ON_COIN_ITEM,
					itemName, price);
				// Disable the Buy button if you don't have enough currency to purchase.
				if (GameFacade.Instance.RetrieveProxy<InventoryProxy>().VCoin < Double.Parse(price))
				{
					mItemOverlayActionButton.AddOnPressedAction(delegate()
					{
						BuyCoinUtility.GoToBuyCashPage
						(
							Translation.NEED_CASH_TITLE,
							Translation.NOT_ENOUGH_CASH,
							delegate(string s)
							{
								Console.WriteLine("Opening cash store: " + s);
							},
							delegate()
							{
								GameFacade.Instance.SendNotification(GameFacade.GET_CASH_GUI_CLOSED);
							}
						);
					});
				}
				else
				{
					mItemOverlayActionButton.AddOnPressedAction(delegate()
					{
						Purchase();
					});
				}
			}
			mRemoveButton.ClearOnPressedActions();
			mRemoveButton.AddOnPressedAction(delegate()
			{
				List<AssetInfo> assetInfos = new List<AssetInfo>();
				foreach (XmlNode assetNode in assetsNode.SelectNodes("Asset"))
				{
					AssetInfo assetInfo = new ClientAssetInfo(assetNode);
					assetInfos.Add(assetInfo);
				}
				GameFacade.Instance.RetrieveMediator<AvatarMediator>().LocalAvatarEntity.RemoveTempAssetInfos(assetInfos);
				mOverlayWindow.Showing = false;
			});
		}

		private void ShowItemInfoFrame(string itemName, string description, string itemExpiration, string itemQuantity)
		{
			mItemInfoFrame.Showing = true;
			mItemInfoTitleLabel.Text = itemName;
			mItemInfoDescriptionLabel.Text = description;

			itemExpiration = itemExpiration.Replace("PST", "").Replace("PDT", "").Trim();

			DateTime dateTime = DateTime.ParseExact(itemExpiration, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
			string dateString = dateTime.ToString("MMM dd, yyyy");

			mItemInfoExpirationLabel.Text = String.Format(Translation.STORE_EXPIRATION, dateString);


			if (Int32.Parse(itemQuantity) > 0)
			{
				// TODO: maxAvailable should be passed with twofish response, but currently we only have num left, so make this max up
				string maxAvailable = "1000";
				mItemInfoNumAvailableLabel.Text = String.Format(Translation.STORE_QUANTITY, itemQuantity, maxAvailable);
			}
			else
			{
				mItemInfoNumAvailableLabel.Text = Translation.STORE_QUANTITY_UNLIMITED;
			}

			// Hide these until we have items that expire or are limited quant
			mItemInfoExpirationLabel.Showing = false;
			mItemInfoNumAvailableLabel.Showing = false;
		}

		private void ShowItemInfoOverlay(IWidget widget, string detailUrl, string price, string currencyName)
		{
			PositionWindowOverButton(mOverlayWindow, widget, mItemDisplayGrid);
			mItemOverlayPrice.Text = FormatPrice(price);
			foreach (KeyValuePair<GuiFrame, Button> kvp in mGridItemButtons)
			{
				if (kvp.Key == widget)
				{
					Image thumbnail = kvp.Key.SelectSingleElement<Image>("Thumbnail");
					mItemOverlayImage.Texture = thumbnail.Texture;
					break;
				}
			}

			if (currencyName == "VCOIN")
			{
				mItemOverlayCoinIcon.Showing = true;
				mItemOverlayCashIcon.Showing = false;
			}
			else
			{
				mItemOverlayCoinIcon.Showing = false;
				mItemOverlayCashIcon.Showing = true;
			}
		}

		private void SetupOverlayActionButtons(string itemId, string itemType, XmlNode assetsNode)
		{
			LocalAvatarEntity localAvatarEntity = GameFacade.Instance.RetrieveMediator<AvatarMediator>().LocalAvatarEntity;
			switch (itemType)
			{
				case ItemType.TOPS:
				case ItemType.PANTS:
				case ItemType.SKIRT:
				case ItemType.BAGS:
				case ItemType.SHOES:
				case ItemType.HAIR:
				case ItemType.BODY:
				case ItemType.MAKEUP:
				case ItemType.FACE:
				case ItemType.MOOD:
					// Call method to apply clothing to body
					localAvatarEntity.ApplyTempClothingToAvatar(assetsNode);
					break;
				case ItemType.EMOTE:
					string animationNameString = assetsNode.SelectSingleNode("Asset/AssetData/AnimationName").InnerText;
					animationNameString = animationNameString.Split(' ')[0];
					RigAnimationName animationName = (RigAnimationName)Enum.Parse(typeof(RigAnimationName), animationNameString);
					GameFacade.Instance.RetrieveProxy<AnimationProxy>().SetRigAnimationAssetInfo(animationName, new ClientAssetInfo(assetsNode.SelectSingleNode("Asset")));
					GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().GetAsset<RigAnimationAsset>(new ClientAssetInfo(assetsNode.SelectSingleNode("Asset")), delegate(RigAnimationAsset animationAsset)
					{
						GameFacade.Instance.SendNotification(GameFacade.PLAY_EMOTE, animationNameString as object);
					});
					break;
			}
		}

		/// <summary>
		/// Disable onPressed behavior on grid item buttons
		/// </summary>
		private void DisableItemButtons()
		{
			foreach (KeyValuePair<GuiFrame, Button> gridFrameButton in mGridItemButtons)
			{
				gridFrameButton.Value.Disable();
			}
		}

		/// <summary>
		/// Enable onPressed behavior on grid item buttons
		/// </summary>
		private void EnableItemButtons()
		{
			foreach (KeyValuePair<GuiFrame, Button> gridFrameButton in mGridItemButtons)
			{
				gridFrameButton.Value.Enabled = true;
			}
		}


		/// <summary>
		/// Search for next block of items
		/// </summary>
		private void NextPage()
		{
			HideInfoAndOverlayFrames();
			HideNextPrevButtons();
			ClearStoreButtons();

			// TODO: Do range checking
			mCurrentStartIndex += mBlockSize;

			mNextPageButton.Disable();

			Search(InventoryGlobals.HANGOUT_STORE, mCurrentItemType);
		}

		private void PreviousPage()
		{
			HideInfoAndOverlayFrames();
			HideNextPrevButtons();
			ClearStoreButtons();

			// TODO: Do range checking
			mCurrentStartIndex -= mBlockSize;

			mPreviousPageButton.Disable();

			Search(InventoryGlobals.HANGOUT_STORE, mCurrentItemType);
		}
		
		private void FirstPage()
		{
			HideInfoAndOverlayFrames();
			HideNextPrevButtons();
			ClearStoreButtons();

			// TODO: Do range checking
			mCurrentStartIndex = 0;

			mPreviousPageButton.Disable();
			mFirstPageButton.Disable();

			Search(InventoryGlobals.HANGOUT_STORE, mCurrentItemType);
		}
		
		private void LastPage()
		{
			HideInfoAndOverlayFrames();
			HideNextPrevButtons();
			ClearStoreButtons();

			// TODO: Do range checking
			mCurrentStartIndex = (mTotalPages - 1) * mBlockSize;

			mNextPageButton.Disable();
			mLastPageButton.Disable();

			Search(InventoryGlobals.HANGOUT_STORE, mCurrentItemType);
		}

		private void ShowNextPrevButtons()
		{
			mPaginationFrame.Showing = true;
			mNextPageButton.Showing = true;
			mPreviousPageButton.Showing = true;
		}
		private void HideNextPrevButtons()
		{
			mPaginationFrame.Showing = true;
		}

		private void HideInfoAndOverlayFrames()
		{
			mOverlayWindow.Showing = false;
			mItemInfoFrame.Showing = false;
			mCurrentSelectedItemOffer = null;
		}

		public override void EnterState()
		{
			CreateGridButtons();
			List<IWidget> gridFrames = new List<IWidget>();
			foreach (KeyValuePair<GuiFrame, Button> kvp in mGridItemButtons)
			{
				gridFrames.Add(kvp.Key);
			}
			mItemDisplayGrid.SetPositions(gridFrames, mGridHeight, mGridWidth, 0);
			mItemDisplayGrid.SetPagination(mBlockSize, 0);
			//mPaginationLabel.Text = String.Format("Page {0}/{1}", mItemDisplayGrid.CurrentPage.ToString(), mItemDisplayGrid.GetTotalPages().ToString());
			mOverlayWindow.Showing = false;
			mItemInfoFrame.Showing = false;
			mItemDisplayFrame.Showing = true;
			mCategorySideBar.Showing = true;
			if (mCurrentSearchResultsXml != null)
			{
				UpdateStoreButtons();
			}
			// Mix panel funnel metrics
			FunnelGlobals.Instance.LogFunnel(FunnelGlobals.FUNNEL_PURCHASE, FunnelGlobals.SHOP_OPENED, "");
		}

		public override void ExitState()
		{
			HideInfoAndOverlayFrames();
			mItemDisplayFrame.Showing = false;
			mCategorySideBar.Showing = false;
			CleanupGridButtons();
			GameFacade.Instance.RetrieveMediator<AvatarMediator>().LocalAvatarEntity.RevertDnaChanges();
		}
		#region purchase methods

		private void Purchase()
		{
			mOverlayWindow.Showing = false;

			Hangout.Shared.Action onPurchaseConfirm = delegate()
			{
				SendPurchaseNotification(mCurrentSelectedItemOffer);
			};
			Hangout.Shared.Action onPurchaseCancel = delegate()
			{
				HideInfoAndOverlayFrames();
			};

			List<object> args = new List<object>();
			args.Add(Translation.PURCHASE_CONFIRM_TITLE);
			args.Add(Translation.PURCHASE_CONFIRM_TEXT);
			args.Add(onPurchaseConfirm);
			args.Add(onPurchaseCancel);
			GameFacade.Instance.SendNotification(GameFacade.SHOW_CONFIRM, args);

			XmlElement priceNode = (XmlElement)mCurrentSelectedItemOffer.SelectSingleNode("price/money");
			string currency = priceNode.GetAttribute("currencyName");
			string itemName = mCurrentSelectedItemOffer.GetAttribute("title");
			string price = priceNode.GetAttribute("amount");

			string extraProps = "{\"item\":\"" + itemName + "\", \"price\":" + price + "}";
			// Log for metrics
			if (currency == "HOUTS")
			{
				EventLogger.Log(LogGlobals.CATEGORY_SHOPPING, LogGlobals.CLICKED_ON_CASH_ITEM_BUY_BUTTON, extraProps);
			}
			else if (currency == "VCOIN")
			{
				EventLogger.Log(LogGlobals.CATEGORY_SHOPPING, LogGlobals.CLICKED_ON_COIN_ITEM_BUY_BUTTON, extraProps);
			}
			// Mix panel funnel metrics
			string currencyProps = "{\"currency\":\"" + currency + "\"}";
			FunnelGlobals.Instance.LogFunnel(FunnelGlobals.FUNNEL_PURCHASE, FunnelGlobals.CLICKED_BUY, currencyProps);

		}

		/// <summary>
		/// Send REQUEST_ITEM_PURCHASE notification
		/// </summary>
		/// <param name="itemOffer">Item being purchased</param>
		private void SendPurchaseNotification(XmlElement itemOfferNode)
		{
			XmlElement priceNode = (XmlElement)itemOfferNode.SelectSingleNode("price/money");
			string itemOfferId = itemOfferNode.GetAttribute("id");
			string currency = priceNode.GetAttribute("currencyName");
			string itemName = itemOfferNode.GetAttribute("title");
			string price = priceNode.GetAttribute("amount");

			if (itemOfferId == "")
			{
				throw new ArgumentNullException("itemOfferId must be supplied with PurchaseRequest");
			}
			if (currency == "")
			{
				throw new ArgumentNullException("currency must be supplied with PurchaseRequest");
			}
			// Make purchase request
			InventoryProxy inventoryProxy = GameFacade.Instance.RetrieveProxy<InventoryProxy>();
			inventoryProxy.PurchaseRequest(itemOfferId, currency, inventoryProxy.HandlePurchaseResponse);

			// Log for metrics
			if (currency == "HOUTS")
			{
				EventLogger.Log(LogGlobals.CATEGORY_SHOPPING, LogGlobals.CLICKED_ON_CASH_ITEM_BUY_CONFIRM,
					itemName, price);
			}
			else if (currency == "VCOIN")
			{
				EventLogger.Log(LogGlobals.CATEGORY_SHOPPING, LogGlobals.CLICKED_ON_COIN_ITEM_BUY_CONFIRM,
					itemName, price);
			}

			// Mix panel funnel metrics
			string currencyProps = "{\"currency\":\"" + currency + "\"}";
			FunnelGlobals.Instance.LogFunnel(FunnelGlobals.FUNNEL_PURCHASE, FunnelGlobals.CLICKED_CONFIRM, currencyProps);
		}

		public void HandlePurchaseResults(XmlDocument xmlResponse)
		{
		}

		#endregion purchase methods

		#region search methods

		public void Search(string storeName, string itemTypes, int startingIndex)
		{
			mCurrentStartIndex = startingIndex;
			Search(storeName, itemTypes);
		}

		/// <summary>
		/// Used by the inventory controller to force a search so the correct items
		/// populate the display
		/// </summary>
		/// <param name="storeName"></param>
		/// <param name="itemType">Comma separated list of itemTypeNames</param>
		public void Search(string storeName, string itemTypes)
		{
			mCurrentSearchResultsXml = null;
			HideInfoAndOverlayFrames();
			ClearStoreButtons();
			DisableItemButtons();
			mCurrentItemType = itemTypes;

			InventoryProxy inventoryProxy = GameFacade.Instance.RetrieveProxy<InventoryProxy>();
			inventoryProxy.GetStoreInventory(storeName, itemTypes, mCurrentStartIndex, mBlockSize, inventoryProxy.HandleStoreInventoryResponse);
		}

		public override void HandleSearchResults(XmlDocument xmlResponse)
		{
			//Debug.Log("HandleSearchResults " + xmlResponse.OuterXml);
			XmlElement responseNode = (XmlElement)xmlResponse.SelectSingleNode("Response");
			string responseVerb = responseNode.GetAttribute("verb");
			foreach (XmlNode itemNode in xmlResponse.SelectNodes("item"))
			{
				string itemTypeName = XmlUtility.GetStringAttribute(itemNode, "itemTypeName");
				if (!mCurrentItemType.Contains(itemTypeName))
				{
					return;
				}
			}
			if (responseVerb == "GetStoreInventory")
			{
				// Save returned results
				mCurrentSearchResultsXml = xmlResponse;

				// Update the gui
				UpdatePageNumbers();
				UpdateStoreButtons();
			}
		}
		#endregion search
	}
}
