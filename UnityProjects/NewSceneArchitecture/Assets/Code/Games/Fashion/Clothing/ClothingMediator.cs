/**  --------------------------------------------------------  *
 *   ClothingMediator.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/14/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using PureMVC.Patterns;

using Hangout.Shared;
using Hangout.Client.Gui;

namespace Hangout.Client.FashionGame
{
	/// <summary>
	/// This class is the factory for ClothingItems as well as the repository for the ClothingItems' generated assets
	/// </summary>
	public class ClothingMediator : Mediator
	{
		private const string NEEDS_FIXIN_ICON_PATH = "GUI/Minigames/Fashion/sewingIcon";
		private const float NEEDS_FIXIN_ICON_SCALE = 2.0f;
		private const int THUMBNAIL_SIZE = 30;

		private System.Random mRandom = new System.Random((int)DateTime.UtcNow.Ticks * typeof(ClothingMediator).GetHashCode());
		
		private bool mClothingLoaded = false;
		public bool ClothingLoaded
		{
			get { return mClothingLoaded; }
		}

		private IDictionary<ItemId, ClothingItem> mItemIdsToClothingItems = new Dictionary<ItemId, ClothingItem>();
		private IDictionary<string, IList<ClothingItem>> mItemTypeToClothingItem = new Dictionary<string, IList<ClothingItem>>();
		public IEnumerable<string> ClothingItemTypes
		{
			get { return mItemTypeToClothingItem.Keys; }
		}

		public IEnumerable<ClothingItem> ClothingForType(string clothingType)
		{
			if( !ItemType.ValidItemTypeString(clothingType) )
			{
				throw new ArgumentException("clothingType(" + clothingType + ") is not a valid ItemType");
			}

			IList<ClothingItem> clothes = new List<ClothingItem>();
			mItemTypeToClothingItem.TryGetValue(clothingType, out clothes);

			return clothes;
		}

		public Texture2D GetThumbnail(ItemId clothingId)
		{
			Texture2D result;
			ClothingItem prototype;
			if (mItemIdsToClothingItems.TryGetValue(clothingId, out prototype))
			{
				result = prototype.Thumbnail;
			}
			else
			{
				throw new Exception("Unexpected clothing of itemId (" + clothingId + ")");
			}
			return result;
		}

		public IGuiStyle GetThumbStyle(ItemId clothingId)
		{
			IGuiStyle result;
			ClothingItem prototype;
			if (mItemIdsToClothingItems.TryGetValue(clothingId, out prototype))
			{
				FashionGameGui gui = GameFacade.Instance.RetrieveMediator<FashionGameGui>();
				result = new GuiStyle(gui.GetNamedStyle(prototype.StyleName), "ThumbnailStyle");
				result.InternalMargins = result.InternalMargins * 0.5f;
			}
			else
			{
				throw new Exception("Unexpected clothing of itemId (" + clothingId + ")");
			}
			return result;
		}

		// Used to determine if we've downloaded all the assets for the level.
		private int mItemsToLoad = 0;

		public void LoadClothing(IEnumerable<ItemId> itemIds, string levelPath)
		{
			mItemIdsToClothingItems.Clear();
			mItemTypeToClothingItem.Clear();
			mClothingLoaded = false;
			
			GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().GetItemToAssetsXmlDoc(itemIds, delegate(XmlDocument itemsDoc)
			{
				XmlNodeList itemNodes = itemsDoc.SelectNodes("Items/Item");
				mItemsToLoad = itemNodes.Count;
				
				GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(WaitForAssetsToDownload());

				XmlDocument fashionGui = XmlUtility.LoadXmlDocument(FashionGameGui.FASHION_GUI_PATH);
				List<string> clothingButtonStyles = Functionals.Reduce<List<string>>
				(
					delegate(List<string> accumulator, object xmlNode)
					{
						accumulator.Add(((XmlNode)xmlNode).InnerText);
						return accumulator;
					},
					fashionGui.SelectNodes("GUI/ClothingButtonStyles/Style/@name")
				);
				int styles = clothingButtonStyles.Count;

				foreach (XmlNode itemNode in itemNodes)
				{
					List<AssetInfo> assetInfos = new List<AssetInfo>();
					foreach (XmlNode assetNode in itemNode.SelectNodes("Assets/Asset"))
					{
						assetInfos.Add(new ClientAssetInfo(assetNode));
					}

					uint idOfItem = uint.Parse(itemNode.SelectSingleNode("Id").InnerText);
					ItemId itemId = new ItemId(idOfItem);
					string thumbnailUrl = itemNode.Attributes["thumbnailUrl"].InnerText;
					string itemType = itemNode.Attributes["type"].InnerText;

					GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().GetAssets<Asset>(assetInfos, delegate(IEnumerable<Asset> assets)
					{
						if( clothingButtonStyles.Count == 0 )
						{
							throw new Exception("Not enough styles(" + styles + ") in " + FashionGameGui.FASHION_GUI_PATH + " 'GUI/ClothingButtonStyles/' for all the clothing items in this level(" + levelPath + ")");
						}

						string styleName = clothingButtonStyles[mRandom.Next(0, clothingButtonStyles.Count)];
						clothingButtonStyles.Remove(styleName);

						RetrieveItemAssetsFromRepository(itemId, itemType, thumbnailUrl, styleName, assets);
					});
				}
			});
		}
 

		private static readonly Texture2D mNeedsFixinIconTexture;
		static ClothingMediator()
		{
			Texture2D icon = (Texture2D)Resources.Load(NEEDS_FIXIN_ICON_PATH);
			mNeedsFixinIconTexture = TextureUtility.ResizeTexture
			(
				icon,
				(int)((float)icon.width * NEEDS_FIXIN_ICON_SCALE),
				(int)((float)icon.height * NEEDS_FIXIN_ICON_SCALE)
			);
		}

		private void RetrieveItemAssetsFromRepository
		(
			ItemId itemId, 
			string itemType, 
			string thumbnailUrl, 
			string styleName, 
			IEnumerable<Asset> assets
		)
		{
			GameFacade.Instance.RetrieveProxy<ClientAssetRepository>().LoadAssetFromPath<ImageAsset>
			(
				thumbnailUrl, 
				delegate(ImageAsset imageAsset)
				{
					Texture2D fullImage = imageAsset.Texture2D;
					Texture2D thumbnail = TextureUtility.ResizeTexture(fullImage, THUMBNAIL_SIZE, THUMBNAIL_SIZE);
					
					TexturePixelSource fullImageSource = new TexturePixelSource(fullImage);
					LayeredTexture textureBuilder = new LayeredTexture(fullImageSource.Width, fullImageSource.Height, true);
					
					PixelSource fixinIcon = new TexturePixelSource(mNeedsFixinIconTexture);

					// normal and normalFixin
					textureBuilder.AddLayer("Main", fullImageSource);
					textureBuilder.FlattenLayers();
					
					Texture2D normal = TextureUtility.CopyTexture(textureBuilder.Texture2D);
					textureBuilder.AddLayer("FixinIcon", fixinIcon);
					textureBuilder.MoveLayerRelative("FixinIcon", 10, fullImageSource.Height - 18 - THUMBNAIL_SIZE);
					textureBuilder.FlattenLayers();
					Texture2D normalFixin = TextureUtility.CopyTexture(textureBuilder.Texture2D);
					
					normal.Apply();
					normalFixin.Apply();
					Pair<Texture2D> fullImagePair = new Pair<Texture2D>(fullImage, normalFixin);

					ClothingItem clothingItem = new ClothingItem(itemId, itemType, fullImagePair, thumbnail, styleName, assets);
					mItemIdsToClothingItems.Add(itemId, clothingItem);

					IList<ClothingItem> clothingItemsInType; 
					if( mItemTypeToClothingItem.TryGetValue(itemType, out clothingItemsInType) )
					{
						clothingItemsInType.Add(clothingItem);
					}
					else
					{
						mItemTypeToClothingItem.Add(itemType, new List<ClothingItem>());
						mItemTypeToClothingItem[itemType].Add(clothingItem);
					}
					
					textureBuilder.Dispose();
				}
			);
		}
		
		private IEnumerator<IYieldInstruction> WaitForAssetsToDownload()
		{
			yield return new YieldWhile(delegate()
			{
				return mItemIdsToClothingItems.Count < mItemsToLoad;
			});
			mClothingLoaded = true;
		}
		
		public ClothingItem BuildClothingItem(ItemId itemId)
		{
			ClothingItem result;
			ClothingItem prototype;
			if( mItemIdsToClothingItems.TryGetValue(itemId, out prototype))
			{
				result = new ClothingItem(prototype);
			}
			else
			{
				throw new Exception("Unexpected ClothingItem itemId: " + itemId.ToString());
			}
			return result;
		}

		private static TexturePixelSource TexturePixelSourceFromNode(XmlNode node, int resultSize)
		{
			TexturePixelSource result;
			XmlAttribute pathAttrib = node.Attributes["path"];
			if (pathAttrib != null)
			{
				Texture2D loadedTexture = (Texture2D)Resources.Load(pathAttrib.InnerText);
				if (loadedTexture == null)
				{
					throw new Exception("Unable to find Texture at Path (" + pathAttrib.InnerText + ")");
				}

				loadedTexture = TextureUtility.ResizeTexture(loadedTexture, resultSize, resultSize);
				result = new TexturePixelSource(loadedTexture);
			}
			else
			{
				throw new Exception("Unable to find path attribute in XmlNode (" + node + ")");
			}
			return result;
		}

		private static string NameFromNode(XmlNode node)
		{
			XmlAttribute nameAttrib = node.Attributes["name"];
			if (nameAttrib == null)
			{
				throw new Exception("Unable to find name attribute in XmlNode(" + node + ")");
			}
			return nameAttrib.InnerText;
		}
	}
}
