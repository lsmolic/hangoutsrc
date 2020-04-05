/*
	Created by Vilas Tewari on 2009-07-20.
   
	An AvatarFbxImporter is a CustomFbxImporter that deals with Avatar .fbx files
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Xml;
using Hangout.Client;

public class AvatarFbxImporter : CustomFbxImporter 
{

	public AvatarFbxImporter( ModelImporter fbxModelImporter, string assetPath ) 
	: base ( fbxModelImporter, assetPath ) 
	{
	}
	
	
	/*
		Override this function for other Import setting setups
	*/
	public override void SetUnityImportSettings( ModelImporter fbxModelImporter ) 
	{
		
		base.SetUnityImportSettings( fbxModelImporter );
		
		fbxModelImporter.generateMaterials = ModelImporterGenerateMaterials.PerSourceMaterial;
		fbxModelImporter.generateAnimations = ModelImporterGenerateAnimations.InRoot;
		fbxModelImporter.splitAnimations = true;
		fbxModelImporter.clipAnimations = SetupAnimationClips();
	}
	
	/*
		Override this function to customize material assignment
	*/
	public override Material CustomMaterialSetup( Material newMaterial, Renderer meshRenderer ) {
		//vilas's fault.. no really, its all him.. talk to him about this change.
		return null;
	}
	
	/*
		Look for a .animationInfo XML file with the same name as this prefab
		to determine if this prefab should have any animation clips defined
	*/
	private ModelImporterClipAnimation[] SetupAnimationClips() 
	{
		FileInfo assetPath = new FileInfo(m_assetPath);
		DirectoryInfo assetFolder = assetPath.Directory;
		
		string searchPattern = "*" + assetPath.Name.Replace( assetPath.Extension, "" ) + "_animationInfo.xml";
		FileInfo[] animationInfoFiles = assetFolder.GetFiles( searchPattern, SearchOption.TopDirectoryOnly );
		
		ModelImporterClipAnimation[] animations;
		
		/* If we find a _animationInfo.xml file */
		if( animationInfoFiles.Length == 1 )
		{
			/* Create XMLDoc from file */
			XmlDocument animationInfo = new XmlDocument();
			XmlNode rootNode;
			
			animationInfo.Load( animationInfoFiles[0].ToString() );
			rootNode = animationInfo.SelectNodes("//Animations")[0];
			
			XmlNodeList animationClipNodes = rootNode.SelectNodes("descendant::AnimationClip");
			animations = new ModelImporterClipAnimation[animationClipNodes.Count];
			
			/* foreach AnimationClip node create a clip */
			int count = 0;
			foreach( XmlNode animationClipNode in animationClipNodes )
			{
				animations[count] = new ModelImporterClipAnimation();
				animations[count].name = XmlUtility.GetStringAttribute(animationClipNode, "name");
				animations[count].firstFrame = XmlUtility.GetIntAttribute(animationClipNode, "startFrame");
				animations[count].lastFrame = XmlUtility.GetIntAttribute(animationClipNode, "endFrame");
				animations[count].loop = XmlUtility.GetBoolAttribute(animationClipNode, "loop");
				
				++count;
			}
			return animations;
		}
		return new ModelImporterClipAnimation[0];
	}
}
