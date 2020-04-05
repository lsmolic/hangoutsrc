/*
 *	Wroted up by the Pherg.
 *	9/24/9
 */

using System.Collections.Generic;
using System.Xml;
using System;
using System.IO;
using System.Reflection;

namespace Hangout.Shared
{
	public abstract class AssetInfo
	{
		protected System.Type mType;
		public System.Type Type
		{
			get { return mType; }
		}

        protected AssetSubType mAssetSubType;
		public AssetSubType AssetSubType
		{
			get { return mAssetSubType; }
		}

        protected AssetId mAssetId;
		public AssetId AssetId
		{
			get { return mAssetId; }
		}

        protected ItemId mItemId;
        public ItemId ItemId
        {
            get { return mItemId; }
        }

        protected string mPath;
		public string Path
		{
			get { return mPath; }
		}

        protected XmlNode mAssetData;
		public XmlNode AssetData
		{
			get { return mAssetData; }
		}
		
		protected XmlNode mAssetNode;
		public XmlNode AssetNode
		{
			get { return mAssetNode; }
		}
	}
}
