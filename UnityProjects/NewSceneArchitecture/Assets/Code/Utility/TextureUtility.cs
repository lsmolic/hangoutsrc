/**  --------------------------------------------------------  *
 *   TextureUtility.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 09/14/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

namespace Hangout.Client
{
	public static class TextureUtility
	{
		public static Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
		{
			Texture2D result = new Texture2D(newWidth, newHeight);

			if (IsReadableWritable(source))
			{
				Color[] pixels = result.GetPixels(0);
				for (int i = 0; i < pixels.Length; ++i)
				{
					float u = (1f / (newWidth - 1)) * (i % newWidth);
					float v = (1f / (newHeight - 1)) * Mathf.Floor(i / newWidth);
					pixels[i] = source.GetPixelBilinear(u, v);
				}
				result.SetPixels(pixels, 0);
				result.Apply();
			}
			else
			{
				throw new Exception("Unable to resize Texture (" + source.name + "). Are you using a compressed texture format?");
			}
			return result;
		}

		public static Texture2D CopyTexture(Texture2D source)
		{
			Texture2D result = new Texture2D(source.width, source.height);
			result.SetPixels(source.GetPixels());
			return result;
		}

		public static bool IsReadableWritable(Texture2D tex)
		{
			return (tex.format == TextureFormat.RGB24 || tex.format == TextureFormat.ARGB32 || tex.format == TextureFormat.Alpha8);
		}
	}
}