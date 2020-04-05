/*
   Created by Vilas Tewari on 2009-08-11.

	An Asset that encapsulates a PixelSource with information like
	a displayName, Id, etc..
*/

using Hangout.Shared;

namespace Hangout.Client
{
	public class TextureAsset : Asset
	{
		private PixelSource mPixelSource;

		public PixelSource PixelSource
		{
			get { return mPixelSource; }
		}

		public TextureAsset(AssetSubType type, PixelSource tex, string displayName, string path, string key)
			: base(type, displayName, path, key)
		{
			mPixelSource = tex;
		}
	}
}