/*
	Created by Vilas Tewari on 2009-07-20.
   
	A CustomTextureImporter is used to modify Unity's texture import pipeline
	CustomTextureImporter is the base class for all other custom texture importers
*/


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class CustomTextureImporter
{
	public CustomTextureImporter(TextureImporter textureImporter)
	{
		SetUnityImportSettings(textureImporter);
	}

	/// <summary>
	/// Override this function to customize Import setting
	/// </summary>
	public virtual void SetUnityImportSettings(TextureImporter textureImporter)
	{
		// Generic Settings
		textureImporter.textureFormat = TextureImporterFormat.DXT1;
		
		string[] splitPath = textureImporter.assetPath.Split('_');
		if( splitPath.Length == 2 )
		{
			string[] splitLast = splitPath[1].Split('.');
			if( splitLast.Length == 2 )
			{
				try
				{
					textureImporter.textureFormat = (TextureImporterFormat)System.Enum.Parse(typeof(TextureImporterFormat), splitLast[0]);
				}
				catch (System.ArgumentException)
				{
					// If the string following _ isn't a TextureImporterFormat, just ignore it
				}
			}
		}

		if( textureImporter.textureFormat == TextureImporterFormat.Alpha8 ||
			textureImporter.textureFormat == TextureImporterFormat.ARGB16 ||
			textureImporter.textureFormat == TextureImporterFormat.ARGB32 ||
			textureImporter.textureFormat == TextureImporterFormat.RGB16 ||
			textureImporter.textureFormat == TextureImporterFormat.RGB24 )
		{
			textureImporter.isReadable = true;
		}

		// TODO: make this sort of thing not require a code change
		if( textureImporter.assetPath == "Assets/Resources/EmptyTexture.psd" )
		{
			textureImporter.textureFormat = TextureImporterFormat.Alpha8;
			textureImporter.isReadable = true;
		}

		textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;

		textureImporter.grayscaleToAlpha = false;
		textureImporter.maxTextureSize = 1024;

		// Bump Map
		textureImporter.convertToNormalmap = false;

		// Cube Map
		textureImporter.generateCubemap = TextureImporterGenerateCubemap.None;

		// Mipmap settings
		textureImporter.mipmapEnabled = true;
		textureImporter.borderMipmap = true;
	}

	/// <summary>
	/// Override this function to customize any Texture2D settings
	/// </summary>
	public virtual void CustomTexture2DSetup(Texture2D newTexture)
	{
		newTexture.wrapMode = TextureWrapMode.Clamp;
		newTexture.filterMode = FilterMode.Bilinear;
	}
}
