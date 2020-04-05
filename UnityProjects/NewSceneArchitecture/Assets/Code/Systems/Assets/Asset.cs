/*
   Created by Vilas Tewari on 2009-08-11.

	A Class that encapsulates an asset with information like
	a displayName, Id, etc..
*/

using System;

using Hangout.Shared;

namespace Hangout.Client
{
	public abstract class Asset : IDisposable
	{
		private readonly string mDisplayName;
		private readonly string mUniqueKey;
		private readonly ClientAssetRepository mClientAssetRepository;
		private AssetSubType mAssetType;

		public AssetSubType Type
		{
			get { return mAssetType; }
			set { mAssetType = value; }
		}
		public string DisplayName
		{
			get { return mDisplayName; }
		}
		public string UniqueKey
		{
			get { return mUniqueKey; }
		}
		private string mPath;
		public string Path
		{
			get { return mPath; }
		}

		/// <summary>
		/// The ClientAssetRepository reference counts Assets, Dispose releases reference on this asset
		/// </summary>
		public void Dispose()
		{
			//mClientAssetRepository.ReleaseReference(this);
		}

		public virtual void DestroyUnityResource()
		{
			// Any subclasses that have Unity References need to implement cleanup code in this overrides of this function
		}

		public Asset(AssetSubType type, string displayName, string path, string key)
		{
			mAssetType = type;
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}
			mDisplayName = displayName;
			mPath = path;
			mUniqueKey = key;
			mClientAssetRepository = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			if (mClientAssetRepository == null)
			{
				throw new Exception("Cannot create assets without an AssetRepository proxy registered in GameFacade");
			}
		}
		public Asset(AssetSubType type, string displayName, string key)
			: this(type, displayName, null, key)
		{
		}

		public Asset(string displayName, string key)
			: this(AssetSubType.NotSet, displayName, null, key)
		{
			UnityEngine.Debug.Log("SOMEONE IS USING THIS!!!!");
		}
		
		public override string ToString()
		{
			return (mDisplayName + " type: " + this.GetType().ToString() + " : " + mUniqueKey);
		}
	}
}