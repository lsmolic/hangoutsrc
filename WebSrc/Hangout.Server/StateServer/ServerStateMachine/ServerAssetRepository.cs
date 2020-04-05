using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using Hangout.Shared.Messages;
using System.Xml;
using Hangout.Server.StateServer;
using log4net;

namespace Hangout.Server
{
	public class ServerAssetRepository : AbstractExtension, IServerAssetRepository
	{
		private readonly IDictionary<ItemId, ItemInfo> mItems = new Dictionary<ItemId, ItemInfo>();
		private readonly IDictionary<AssetId, XmlNode> mAssetIdsToAssetXml = new Dictionary<AssetId, XmlNode>();
		private Hangout.Shared.Action mItemsAndAssetsServiceDoneCallback = null;
		protected static ILog mLogger = LogManager.GetLogger("ServerAssetRepository");

		public ServerAssetRepository(ServerStateMachine serverStateMachine, Hangout.Shared.Action itemsAndAssetsServiceDoneCallback) : base (serverStateMachine)
		{
			mItemsAndAssetsServiceDoneCallback = itemsAndAssetsServiceDoneCallback;

			mMessageActions.Add(MessageSubType.GetItemsById, GetItemsById);

			CallServices();
		}

		protected virtual void CallServices()
		{
			ServerAssetRepositoryServiceAPI.GetItemsService(delegate(XmlDocument itemsXmlResponse)
			{
				GetItems(itemsXmlResponse);
				ServerAssetRepositoryServiceAPI.GetAssetsService(delegate(XmlDocument assetsXmlResponse)
				{
					GetAssetXml(assetsXmlResponse);
					mItemsAndAssetsServiceDoneCallback();
				});
			});
		}

		protected void GetItems(XmlDocument responseXmlDoc)
		{
			XmlNodeList items = responseXmlDoc.GetElementsByTagName("Item");
			foreach (XmlNode item in items)
			{
				uint itemIdUnsignedInt = 0;
				if (uint.TryParse(item.SelectSingleNode("ItemId").InnerText, out itemIdUnsignedInt))
				{
					ItemId newItemId = new ItemId(itemIdUnsignedInt);
					if (!mItems.ContainsKey(newItemId))
					{
						List<AssetId> assetIds = new List<AssetId>();
						foreach (XmlNode assetIdXml in item.SelectNodes("AssetIdList/AssetId"))
						{
							uint assetIdUnsignedInt = 0;
							if (uint.TryParse(assetIdXml.InnerText, out assetIdUnsignedInt))
							{
								assetIds.Add(new AssetId(assetIdUnsignedInt));
							}
						}

						// TODO: Get ThumbnailUrl
						mItems.Add(newItemId, new ItemInfo(newItemId, assetIds, item.Attributes["ItemType"].InnerText, item.Attributes["SmallImageURL"].InnerText));
					}
					else
					{
						mLogger.Warn("Duplicate Item listings are being loaded into the server asset repo. Ignoring an instance of ItemID (" + itemIdUnsignedInt + ")");
					}
				}
			}
		}

		protected void GetAssetXml(XmlDocument responseXmlDoc)
		{
			XmlNodeList assets = responseXmlDoc.GetElementsByTagName("Asset");
			foreach (XmlNode asset in assets)
			{
				uint assetIdUnsignedInt = 0;
				if (uint.TryParse(asset.Attributes["AssetId"].Value, out assetIdUnsignedInt))
				{
					AssetId assetId = new AssetId(assetIdUnsignedInt);
					try
					{
						mAssetIdsToAssetXml.Add(assetId, asset);
					}
					catch (System.Exception ex)
					{
						StateServerAssert.Assert(ex);
						continue;
					}
				}
			}
		}

		public XmlDocument GetXmlDna(ItemId itemId)
		{
			return GetXmlDna(new ItemId[] { itemId });
		}

		/// <summary>
		/// the expected output of this xml document looks like this:
		/// <Items>
		///     <Item>
		///         <Id>1</Id>
		///         <Assets>
		///             <Asset ItemId="" AssetId="">
		///                 <!-- asset xml data from web service -->
		///             </Asset>
		///             <Asset ItemId="" AssetId="">
		///                 <!-- asset xml data from web service -->
		///             </Asset>
		///         </Assets>
		///     </Item>
		///     <Item>
		///         <Id></Id>
		///         <Assets><Asset ItemId="" AssetId=""></Asset></Assets>
		///     </Item>
		/// </Items>
		/// </summary>
		/// <param name="itemIdsForObject"></param>
		/// <returns></returns>
		public XmlDocument GetXmlDna(IEnumerable<ItemId> itemIdsForObject)
		{
			Dictionary<ItemId, List<XmlNode>> itemIdsToAssetXmls = GetAssetXmlFromItemList(itemIdsForObject);
			//build an xml structure out of the Dictionary<ItemId, List<XmlNode>>
			StringBuilder xmlBuilder = new StringBuilder();
			xmlBuilder.Append("<Items>");
			foreach (KeyValuePair<ItemId, List<XmlNode>> itemIdToAssetXmls in itemIdsToAssetXmls)
			{
				ItemInfo itemInfo;
                if (mItems.TryGetValue(itemIdToAssetXmls.Key, out itemInfo))
                {
				    xmlBuilder.AppendFormat
				    (
					    "<Item type=\"{0}\" thumbnailUrl=\"{1}\" >", 
					    itemInfo.ItemTypeString,
					    itemInfo.ThumbnailUrl
				    );
				    xmlBuilder.AppendFormat("<Id>{0}</Id>", itemIdToAssetXmls.Key.ToString());
				    xmlBuilder.Append("<Assets>");
				    foreach (XmlNode assetXml in itemIdToAssetXmls.Value)
				    {
					    string assetXmlString = assetXml.OuterXml;

					    int indexOfAssetIdAttribute = assetXmlString.IndexOf(" AssetId");
					    if (indexOfAssetIdAttribute > 0 && indexOfAssetIdAttribute < assetXmlString.Length)
					    {
						    //add the ItemId attribute
						    assetXmlString = assetXmlString.Insert(indexOfAssetIdAttribute, " ItemId=\"" + itemIdToAssetXmls.Key.ToString() + "\"");
					    }
					    else
					    {
						    StateServerAssert.Assert(new System.Exception("indexOfAssetIdAttribute out of bounds.  will be unable to instert ItemId into xml node"));
					    }

					    xmlBuilder.Append(assetXmlString);
				    }
				    xmlBuilder.Append("</Assets>");
				    xmlBuilder.Append("</Item>");
                }
			}
			xmlBuilder.Append("</Items>");

			XmlDocument newdoc = new XmlDocument();
			newdoc.LoadXml(xmlBuilder.ToString());
			return newdoc;
		}

		/// <summary>
		/// Get the Dna object that represents a list of ItemId's 
		/// </summary>
		/// <param name="itemIdsForObject"></param>
		/// <returns></returns>
		public Dna GetDna(IEnumerable<ItemId> itemIdsForObject)
		{
			Dna dna = new Dna();
			Dictionary<ItemId, List<XmlNode>> itemIdsToAssetXmls = GetAssetXmlFromItemList(itemIdsForObject);

			foreach (ItemId itemId in itemIdsToAssetXmls.Keys)
			{
				List<XmlNode> assetXmlList = itemIdsToAssetXmls[itemId];
				foreach (XmlNode assetXml in assetXmlList)
				{
					ServerAssetInfo assetInfo = new ServerAssetInfo(itemId, assetXml);
					dna.ApplyInfoToDna(assetInfo);
				}
			}
			return dna;
		}

		private Dictionary<ItemId, List<XmlNode>> GetAssetXmlFromItemList(IEnumerable<ItemId> itemIdsForObject)
		{
			//transform the List<ItemId> to a Dictionary<ItemId, List<XmlNode>>
			Dictionary<ItemId, List<XmlNode>> itemIdsToAssetXmls = new Dictionary<ItemId, List<XmlNode>>();
			foreach (ItemId itemId in itemIdsForObject)
			{
				if (!itemIdsToAssetXmls.ContainsKey(itemId))
				{
					itemIdsToAssetXmls.Add(itemId, new List<XmlNode>());
				}
				// look up the item ids to asset ids
				ItemInfo itemInfo;
				if (mItems.TryGetValue(itemId, out itemInfo))
				{
					foreach (AssetId assetId in itemInfo.Assets)
					{
						XmlNode assetXml = null;
						// look up the asset ids to asset xml 
						if (mAssetIdsToAssetXml.TryGetValue(assetId, out assetXml))
						{
							// store the item ids to list of asset xmls
							itemIdsToAssetXmls[itemId].Add(assetXml);
						}
					}
				}
			}
			return itemIdsToAssetXmls;
		}

		public void GetItemsById(Message message, Guid senderId)
		{
			List<ItemId> itemIds = CheckType.TryAssignType<List<ItemId>>(message.Data[0]);
			XmlDocument xmlResponse = GetXmlDna(itemIds);

			List<object> data = new List<object>();
			data.Add(xmlResponse.OuterXml);
			Message responseMessage = new Message(MessageType.AssetRepository, data);
			responseMessage.Callback = message.Callback;
			mServerStateMachine.SendMessageToReflector(responseMessage, senderId);
		}
	}
}
