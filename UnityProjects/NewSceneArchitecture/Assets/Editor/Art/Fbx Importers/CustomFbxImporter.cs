/*
	Created by Vilas Tewari on 2009-07-20.
   
	A CustomFbxImporter is used to add / modify Unity's asset Import pipeline
	
	CustomFbxImporter is the base class for all Customs Fbx Importer scripts
	It sets up basic import settings such as materials, animation setting etc.
*/


using UnityEngine;
using UnityEditor;
using System.Collections;

public class CustomFbxImporter : System.Object {
	
	protected string m_assetPath;
	
	/*
		Constructor
	*/
	public CustomFbxImporter( ModelImporter fbxModelImporter, string assetPath ) {
		
		m_assetPath = assetPath;
		SetUnityImportSettings( fbxModelImporter );
	}
	
	/*
		Override this function for other Import setting setups
	*/
	public virtual void SetUnityImportSettings( ModelImporter fbxModelImporter ) {
		
		fbxModelImporter.globalScale = 0.01f;
		fbxModelImporter.recalculateNormals = false;
		fbxModelImporter.generateMaterials = ModelImporterGenerateMaterials.None;
		
		fbxModelImporter.generateAnimations = ModelImporterGenerateAnimations.None;
		fbxModelImporter.splitAnimations = false;
		
		fbxModelImporter.addCollider = false;
		fbxModelImporter.swapUVChannels = false;
	}
	
	/*
		Override this function to customize the way the prefab is setup
	*/
	public virtual void CustomPrefabSetup( GameObject fbxPrefab ) {}
	
	/*
		Override this function to customize material assignment
	*/
	public virtual Material CustomMaterialSetup( Material newMaterial, Renderer meshRenderer ) {
		// Returning null lets Unity use its default material assignement system
		return null;
	}
}
