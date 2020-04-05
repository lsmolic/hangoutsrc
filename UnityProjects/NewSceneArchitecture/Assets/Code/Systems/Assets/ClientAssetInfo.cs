using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Hangout.Shared;
using UnityEngine;

namespace Hangout.Client
{
    public class ClientAssetInfo : AssetInfo
    {
        /// <summary>
        /// Example:
        /// foreach( AssetInfo ai in AssetInfo.Parse(anXmlDoc) ) { // do stuff }
        /// </summary>
        public static IEnumerable<AssetInfo> Parse(XmlNode xmlWithAssetNodes)
        {
            foreach (XmlNode assetNode in xmlWithAssetNodes.SelectNodes(".//Asset"))
            {
                yield return new ClientAssetInfo(assetNode);
            }
        }

        public ClientAssetInfo(XmlNode assetXml)
		{
			if (assetXml == null)
			{
				throw new ArgumentNullException("assetXml");
			}

			mAssetNode = assetXml;
			XmlElement xmlElement = (XmlElement)assetXml;
			mAssetId = new AssetId(uint.Parse(xmlElement.GetAttribute("AssetId")));
			mItemId = new ItemId(uint.Parse(xmlElement.GetAttribute("ItemId")));
			string AssetSubType = xmlElement.GetAttribute("AssetSubType");

			try
			{
				mAssetSubType = (AssetSubType)Enum.Parse(typeof(AssetSubType), AssetSubType);
			}
			catch (ArgumentException e)
			{
				throw new Exception("Trouble parsing string: " + AssetSubType + " to Enum." + e);
			}

			mPath = xmlElement.GetAttribute("AssetFilePath");

			mAssetData = assetXml.SelectSingleNode("AssetData");
			if (mAssetData == null)
			{
				throw new XmlException("AssetData could not found in Xml: " + assetXml.OuterXml);
			}

			string assetType = xmlElement.GetAttribute("AssetType");
			BuildAssetType(assetType, assetXml);
		}

		protected void BuildAssetType(string assetType, XmlNode assetXml)
		{
			if (assetType == null)
			{
				throw new XmlException("AssetCategory could not found in Xml");
			}
			try
			{
				mType = System.Type.GetType("Hangout.Client." + assetType);
			}
			catch (ArgumentNullException e)
			{
				CastingToTypeError(e, assetType);
			}
			catch (TargetInvocationException e)
			{
				CastingToTypeError(e, assetType);
			}
			catch (ArgumentException e)
			{
				CastingToTypeError(e, assetType);
			}
			catch (TypeLoadException e)
			{
				CastingToTypeError(e, assetType);
			}
			catch (FileLoadException e)
			{
				CastingToTypeError(e, assetType);
			}
			catch (BadImageFormatException e)
			{
				CastingToTypeError(e, assetType);
			}
			if (mType == null)
			{
				throw new Exception("Unable to create type from: " + assetType + " XML: " + assetXml.OuterXml);
			}
		}


        protected void CastingToTypeError(Exception e, string assetCategory)
        {
            throw new Exception("Unable to get Type from string: " + assetCategory, e);
        }

		public override string ToString()
		{
			string returnString = "Type: " + Type.ToString() + " AssetSubType: " + AssetSubType.ToString() + " AssetId: " + AssetId.Value + " ItemId: " + ItemId.Value + " Path: " + Path + " AssetData: " + AssetData.OuterXml;
			return returnString;
		}
    }
}
