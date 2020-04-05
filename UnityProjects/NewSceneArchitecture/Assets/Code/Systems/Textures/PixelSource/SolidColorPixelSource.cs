/*
	Created by Vilas Tewari

	TexturePixelSource Is a PixelSource that gets it's data from a Texture2D
	TexturePixelSource color info is stored as a Color Array
*/

using UnityEngine;
using System.Collections;


namespace Hangout.Client
{
	public class SolidColorPixelSource : PixelSource
	{

		// All pixels are this color
		private HColor mColor;
		private bool mIsWhite;

		/*
			Constructor
		*/
		public SolidColorPixelSource(int width, int height, Color color)
			: base(width, height)
		{
			mColor = new HColor(color);
			mIsWhite = (color == Color.white);
		}

		public override bool IsPixelClear(int x, int y)
		{
			return mColor.a == 0;
		}

		/*
			Get Solid Color
		*/
		protected override HColor GetPixel(int x, int y)
		{
			return mColor;
		}
		public override bool IsPixelWhite(int x, int y)
		{
			return mIsWhite;
		}

		/*
			Utility
		*/
		public static SolidColorPixelSource Clear(int width, int height)
		{
			return new SolidColorPixelSource(width, height, Color.clear);
		}
	}
}