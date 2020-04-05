/*
	Created by Vilas Tewari on 2009-07-20.
   
	A RoomBackdropFbxImporter is a CustomFbxImporter that deals with image backdrop
	planes for rooms
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class RoomBackdropFbxImporter : CustomFbxImporter {

	/*
		Constructor
	*/
	public RoomBackdropFbxImporter( ModelImporter fbxModelImporter, string assetPath ) : base ( fbxModelImporter, assetPath ) {}
	
	/*
		Override this function for other Import setting setups
	*/
	public override void SetUnityImportSettings( ModelImporter fbxModelImporter ) {
		
		base.SetUnityImportSettings( fbxModelImporter );
		
		fbxModelImporter.generateMaterials = ModelImporterGenerateMaterials.PerSourceMaterial;
	}

    public override Material CustomMaterialSetup(Material newMaterial, Renderer meshRenderer)
    {
        // Get Material Path
        FileInfo m_assetFile = new FileInfo(m_assetPath);
        string materialPath = m_assetPath.Replace(m_assetFile.Name, "Materials/") + newMaterial.name + ".mat";

        // If Material does not Exist
        if (!AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)))
        {
            AssetDatabase.CreateAsset(newMaterial, materialPath);
        }

        Material roomMaterial = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;

        roomMaterial.shader = Shader.Find("Flat Shader");

        return roomMaterial;
    }
}
