/*
   Created by Vilas Tewari on 2009-08-11.

	A Mesh Asset that represents an Avatars Head & Face
	It contains additional info about the UVShell index and name
*/

using UnityEngine;
using System.Collections;
using System.Xml;
using System;

using Hangout.Shared;

namespace Hangout.Client
{
	public class FaceMeshAsset : MeshAsset
	{
		private readonly int mUvShellCount;
		private readonly string[] mUvShellNames;
		private readonly Vector2[] mUvGridDimensions;
		private readonly string[] mTextureZoneNames;
		
		public int UvShellCount
		{
			get{ return mUvShellCount; }
		}
		
		
		public FaceMeshAsset( AssetSubType type, Mesh mesh, string displayName, string path, string key, XmlNode meshInfo )
		: base( type, mesh, displayName, path, key )
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			
			XmlNode faceMeshInfoNode = meshInfo;
				
			XmlNodeList uvShellNodes = faceMeshInfoNode.SelectNodes("descendant::UvShell");
			mUvShellCount = uvShellNodes.Count;
			if( mUvShellCount == 0 )
			{
				Console.LogError("FaceMeshAsset(): FaceMesh " + DisplayName + " does not have any UvShell info");
				return;
			}
			mUvShellNames = new string[mUvShellCount];
			mUvGridDimensions = new Vector2[mUvShellCount];
			mTextureZoneNames = new string[mUvShellCount];
			
			/* foreach UvShell get its data */
			int count = 0;
			foreach( XmlNode uvShellNode in uvShellNodes )
			{
				int subMeshIndex = XmlUtility.GetIntAttribute(uvShellNode, "subMeshIndex");
				mUvShellNames[subMeshIndex] = XmlUtility.GetStringAttribute(uvShellNode, "name");
				mTextureZoneNames[subMeshIndex] = XmlUtility.GetStringAttribute(uvShellNode, "textureZone");

				int gridWidth = XmlUtility.GetIntAttribute(uvShellNode, "gridWidth");
				int gridHeight = XmlUtility.GetIntAttribute(uvShellNode, "gridHeight");
				
				mUvGridDimensions[subMeshIndex] = new Vector2( gridWidth, gridHeight );
				++count;
			}
		}
		
		public string GetUvShellName( int shellIndex )
		{
			if( shellIndex < mUvShellCount)
				return mUvShellNames[shellIndex];
			else
				Console.LogError( "FaceMeshAsset GetUvShellName: Invalid shell index");
			return "";
		}
		
		public Vector2 GetUvGridDimensions( int shellIndex )
		{
			if( shellIndex < mUvShellCount)
				return mUvGridDimensions[shellIndex];
			else
				Console.LogError( "FaceMeshAsset GetUvGridDimensions: Invalid shell index");
			return Vector2.zero;
		}
		
		public string GetUvShellTextureZoneName( int shellIndex )
		{
			if( shellIndex < mUvShellCount)
				return mTextureZoneNames[shellIndex];
			else
				Console.LogError( "FaceMeshAsset GetUvShellTextureZoneName: Invalid shell index");
			return "";
		}
	}
}