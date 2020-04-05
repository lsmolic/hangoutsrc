/*
   Created by Vilas Tewari on 2009-08-11.

	An Asset that encapsulates a Color value with information like
	a displayName, Id, etc..
*/


using UnityEngine;

using System.Collections;

using Hangout.Shared;

namespace Hangout.Client
{
	public class ColorAsset : Asset
	{
		Color mColor;

		public Color Color
		{
			get { return mColor; }
		}

		public ColorAsset(AssetSubType type, Color color, string displayName, string path, string key)
			: base(type, displayName, path, key)
		{
			mColor = color;
		}
	}
}
