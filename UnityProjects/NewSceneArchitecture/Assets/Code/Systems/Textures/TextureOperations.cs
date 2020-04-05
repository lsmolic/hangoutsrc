/*
   Created by Vilas Tewari.

   A Utility Class for Texture Operations
*/


using UnityEngine;
using System.Collections;

public static class TextureOperations
{

	// Textures that are in compressed formats ( DXT compressed ) cannot be edited ( cannot get and set pixels on them )
	public static bool IsReadableWritable(Texture2D tex)
	{
		return (tex.format == TextureFormat.RGB24 || tex.format == TextureFormat.ARGB32 || tex.format == TextureFormat.Alpha8);
	}

	public static Texture2D CreateTextureWithColor(int width, int height, Color fillColor)
	{

		Texture2D result = new Texture2D(width, height);
		FillTextureWithColor(result, fillColor);
		return result;
	}

	public static void FillTextureWithColor(Texture2D texture, Color fillColor)
	{
		if (IsReadableWritable(texture))
		{
			Color[] pixels = texture.GetPixels(0);
			for (int i = 0; i < pixels.Length; ++i)
			{
				pixels[i] = fillColor;
			}
			texture.SetPixels(pixels, 0);
			texture.Apply();
		}
		else
		{
			Hangout.Client.Console.LogError("Cannot Fill Texture: It is a DXT compressed texture");
		}
	}

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
			Hangout.Client.Console.LogError("Cannot Resize Texture: It might be a DXT compressed texture");
		}
		return result;
	}

	public static Texture2D ColorArrayToTexture2D(int width, int height, Color[] pixels)
	{
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pixels, 0);
		result.Apply();
		return result;
	}
}
