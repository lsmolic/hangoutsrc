/*
 *	Wroted up by the Pherg.
 *	9/24/9
 */	

using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Server
{
	public class ServerAssetInfo : AssetInfo
	{
		/// <summary>
		/// Example:
		/// foreach( AssetInfo ai in AssetInfo.Parse(anXmlDoc) ) { // do stuff }
		/// </summary>
        public static IEnumerable<AssetInfo> Parse(XmlNode xmlWithAssetNodes)
		{
            foreach (XmlElement assetNode in xmlWithAssetNodes.SelectNodes("Items/Item/Assets/Asset"))
			{
                ItemId itemId = new ItemId(uint.Parse(assetNode.GetAttribute("ItemId")));
				yield return new ServerAssetInfo(itemId, assetNode);
			}
		}

        public ServerAssetInfo(ItemId itemId, XmlNode assetXml)
            : this(assetXml)
        {
            mItemId = itemId;    
        }

		public ServerAssetInfo(XmlNode assetXml)
		{
			if (assetXml == null)
			{
				throw new ArgumentNullException("assetXml");
			}
            XmlElement xmlElement = (XmlElement)assetXml;
            
			mAssetId = new AssetId(uint.Parse(xmlElement.GetAttribute("AssetId")));
            //mItemId = new ItemId(uint.Parse(xmlElement.GetAttribute("ItemId")));
            string AssetSubType = xmlElement.GetAttribute("AssetSubType");
			
			try
			{
				mAssetSubType = (AssetSubType)Enum.Parse(typeof(AssetSubType), AssetSubType);
			}
			catch (ArgumentException e)
			{
				throw new Exception("Trouble parsing string: " + AssetSubType + " to Enum." + e);
			}
			
			string assetType = xmlElement.GetAttribute("AssetType");
		}
	}
}
