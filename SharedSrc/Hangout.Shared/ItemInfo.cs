/*
 *	Wroted up by the Mortoc.
 *	12/8/9
 */

using System.Collections.Generic;
using System.Xml;
using System;
using System.IO;
using System.Reflection;

namespace Hangout.Shared
{
	public class ItemInfo
	{
        private ItemId mItemId;
        public ItemId ItemId
        {
            get { return mItemId; }
        }

		private IEnumerable<AssetId> mAssets;
		public IEnumerable<AssetId> Assets
		{
			get { return mAssets; }
		}

		private readonly string mItemTypeString;
		public string ItemTypeString
		{
			get { return mItemTypeString; }
		}

		private readonly string mThumbnailUrl;
		public string ThumbnailUrl
		{
			get { return mThumbnailUrl; }
		}

        public ItemInfo(ItemId id, IEnumerable<AssetId> assets, string itemTypeString, string thumbnailUrl)
        {
			mItemId = id;
			mAssets = assets;
			if( !ItemType.ValidItemTypeString(itemTypeString) )
			{
				throw new ArgumentException("itemTypeString(" + itemTypeString + ") is not valid. See ItemType.cs for a list of valid item types.");
			}
			mItemTypeString = itemTypeString;
			mThumbnailUrl = thumbnailUrl;
        }
	}
}
