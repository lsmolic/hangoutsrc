/*
	Created by Vilas Tewari.
	
	A Utility class for Pixel Operations like blending.
*/

using UnityEngine;
using System.Collections;

namespace Hangout.Client
{
	public static class Pixel
	{
		public enum PixelBlendMode { Additive, Multiply, Layer, Screen, Overlay, Color, HueShift };

		public delegate HColor BlendPixels(HColor pixel1, HColor pixel2);


		// ** SINGLE PIXEL BLEND FUNCTIONS ** //
		/*
			Pixel2 is the 'top' pixel, Pixel1 is the bottom
		*/

		static public HColor BlendPixelsLayer(HColor pixel1, HColor pixel2)
		{
			float a = pixel2.a / 255f;
			float aComplement = 1 - (pixel2.a / 255f);
			byte r = (byte)((pixel1.r * aComplement) + (pixel2.r * a));
			byte g = (byte)((pixel1.g * aComplement) + (pixel2.g * a));
			byte b = (byte)((pixel1.b * aComplement) + (pixel2.b * a));
			return new HColor(r, g, b, pixel2.a);
		}

		/*
			Get Appropriate BlendPixels function
		*/
		public static BlendPixels GetBlendFunction(PixelBlendMode blendMode)
		{
			switch (blendMode)
			{
				case PixelBlendMode.Additive:
					return HColor.Add;
				case PixelBlendMode.Multiply:
					return HColor.Multiply;
				case PixelBlendMode.Layer:
					return Pixel.BlendPixelsLayer;
				default:
					return Pixel.BlendPixelsLayer;
			}
		}
	}
}