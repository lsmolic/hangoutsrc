/**  --------------------------------------------------------  *
 *   ResourcesGuiImporter.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/10/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;

public class ResourcesGuiImporter
{
	public class GuiTextureImporter : CustomTextureImporter
	{
		public GuiTextureImporter(TextureImporter importer)
			: base(importer)
		{
		}

		public override void SetUnityImportSettings(TextureImporter textureImporter)
		{
			base.SetUnityImportSettings(textureImporter);

			textureImporter.textureFormat = TextureImporterFormat.ARGB32;
			textureImporter.isReadable = true;

			textureImporter.mipmapEnabled = false;
			textureImporter.npotScale = TextureImporterNPOTScale.None;
		}
	}

	private const uint mDefaultFontSize = 16;
	public static void ProcessFont(TrueTypeFontImporter importer)
	{
		//importer.fontTextureCase = FontTextureCase.ASCII;

		// This is a bit of a hack for now, but it handles the cases we need properly
		if(importer.assetPath.Contains("ASCII"))
		{
			importer.fontTextureCase = FontTextureCase.ASCII;
		}

		// If a font ends in an underscore and then an number (ex. _12), use that number as the font size
		uint fontSize = mDefaultFontSize;
		string assetPathWithoutExt = importer.assetPath.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
		string[] split = assetPathWithoutExt.Split(new char[1] { '_' }, StringSplitOptions.RemoveEmptyEntries);
		if (split.Length > 0)
		{
			uint loadedFontSize;
			if (uint.TryParse(split[split.Length - 1], out loadedFontSize))
			{
				fontSize = loadedFontSize;
			}
			
		}
		importer.fontSize = (int)fontSize;
	}
}
