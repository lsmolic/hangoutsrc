/**  --------------------------------------------------------  *
 *   HangoutPipeline.cs  
 *
 *   Author: Mortoc
 *   Date: 5/14/2009
 *
 *   --------------------------------------------------------  *
 */

using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Text;

using Hangout.Client.Gui;

public class HangoutPipeline : AssetPostprocessor
{
	// The CustomFbxImporter class preforms mesh setup
	private CustomFbxImporter mCustomFbxImporter = null;

	// The CustomTextureImporter class preforms texture setup
	private CustomTextureImporter mCustomTextureImporter = null; 

	/// <summary>
	/// Takes a path unity gives to OnPostprocessAllAssets or from AssetImporter.assetPath and turns it into an absolute FileInfo
	/// </summary>
	public static FileInfo UnityPathToFileInfo(string unityPath)
	{
		// Really Unity? Both Application.dataPath and the importedAsset have the Assets directory in there?
		return new FileInfo(Application.dataPath + unityPath.Substring("Assets".Length));
	}

	public static DirectoryInfo UnityPathToDirectoryInfo(string unityPath)
	{
		return new DirectoryInfo(Application.dataPath + unityPath.Substring("Assets".Length));
	}

	/// <summary>
	/// On Preprocess identify the texture's type based on some convention
	/// Assign a CustomTextureImporter to the asset and setup import settings
	/// </summary>
	public void OnPreprocessTexture()
	{
		TextureImporter textureImporter = assetImporter as TextureImporter;

		// Process Generic Texture file
		if ( !assetPath.Contains("Resources/GUI") )
		{
			mCustomTextureImporter = new CustomTextureImporter(textureImporter);
		}
		else
		{
			mCustomTextureImporter = new ResourcesGuiImporter.GuiTextureImporter(textureImporter);
		}
	}

	/// <summary>
	/// On Postprocess, set attributes on the created Texture2D object
	/// </summary>
	public void OnPostprocessTexture(Texture2D newTexture)
	{
		mCustomTextureImporter.CustomTexture2DSetup(newTexture);
	}

	/// <summary>
	/// On Preprocessing identify the assets' type based on some convention
	/// Assign an FbxImporter to the asset and setup import settings
	/// </summary>
	public void OnPreprocessModel()
	{

		/*
			AssetPostprocessor class gives us access to this AssetImporter Object.
			Changing the import settings on this object will change the way Unity
			imports the assets as a prefab
		*/
		ModelImporter fbxModelImporter = assetImporter as ModelImporter;

		//	IMPORT CONDITIONS
		if (assetPath.Contains("Resources"))
		{

			// Process Avatar
			if (assetPath.Contains("Avatar"))
			{
				mCustomFbxImporter = new AvatarFbxImporter(fbxModelImporter, assetPath);
				return;
			}

			// Process Room Backdrop
			if (assetPath.Contains("Room Image Backdrop"))
			{
				mCustomFbxImporter = new RoomBackdropFbxImporter(fbxModelImporter, assetPath);
				return;
			}
		}

		// Process Generic FBX file
		mCustomFbxImporter = new CustomFbxImporter(fbxModelImporter, assetPath);
	}

	/// <summary>
	/// Run CustomPrefabSetup() of the CustomFbxImporter. This will setup components
	/// GameObject hierarchy etc.
	/// </summary>
	void OnPostprocessModel(GameObject g)
	{
		if (mCustomFbxImporter != null)
		{
			mCustomFbxImporter.CustomPrefabSetup(g);
		}
	}

	/// <summary>
	/// On AssignMaterialModel we link the generated materials to
	///	existing Materials in the Materials Folder on the same level as the asset
	///	OR
	/// we generate a new Material in the Materials folder
	/// </summary>
	Material OnAssignMaterialModel(Material newMaterial, Renderer meshRenderer)
	{
		return mCustomFbxImporter.CustomMaterialSetup(newMaterial, meshRenderer);
	}

	private static void OnPostprocessAllAssets( string[] importedAssets,
												string[] deletedAssets,
												string[] movedAssets,
												string[] movedFromPath )
	{
		try
		{
			foreach (string importedAsset in importedAssets)
			{
				FileInfo fi = UnityPathToFileInfo(importedAsset);
				if (fi.Name == "NewBehaviourScript.cs")
				{
					NewScriptProcessor.Import(fi);
				}

				AssetImporter importer = AssetImporter.GetAtPath(importedAsset);
				if (importer is TrueTypeFontImporter)
				{
					ResourcesGuiImporter.ProcessFont((TrueTypeFontImporter)importer);
				}
			}

			// We removed the delete part of the delete command from our unity source, so we re-create it here
			foreach (string deletedAsset in deletedAssets)
			{
				FileInfo toDelete = UnityPathToFileInfo(deletedAsset);
				DirectoryInfo toDeleteDir = UnityPathToDirectoryInfo(deletedAsset);
				if (toDelete.Exists)
				{
					toDelete.Delete();
				}
				else if (toDeleteDir.Exists)
				{
					toDeleteDir.Delete(true);
				}
			}

			AssetDatabase.Refresh();
		}
		catch (System.Exception e)
		{
			// Unity doesn't handle exceptions thrown here well at all, this allows us to get a little data out of the error.
			Debug.LogError(e);
		}
	}
}