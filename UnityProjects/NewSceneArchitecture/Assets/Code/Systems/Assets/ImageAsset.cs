/*
   Created by Vilas Tewari on 2009-08-11.

	An Asset that encapsulates a Texture2D for things like room backgrounds with information like
	a displayName, Id, etc... Notably, this does not use a PixelSource.
*/

using Hangout.Shared;
using UnityEngine;

namespace Hangout.Client
{
    public class ImageAsset : Asset
    {
        private Texture2D mTexture2D;
        public Texture2D Texture2D
        {
            get { return mTexture2D; }
        }

		public static string UniqueKeyNamespace
		{
			get { return "Path:"; }
		}

        public ImageAsset(AssetSubType type, Texture2D tex, string displayName, string path, string key)
            : base(type, displayName, path, key)
        {
            mTexture2D = tex;
        }

		public ImageAsset(Texture2D tex, string path)
			: base(AssetSubType.NotSet, tex.name, path, UniqueKeyNamespace + path)
		{
			mTexture2D = tex;
		}
    }
}