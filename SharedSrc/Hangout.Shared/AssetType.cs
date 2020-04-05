/*
   Created by Pherg on 2009-09-29.

	A Class that encapsulates AssetTypes on the client side.
*/
using System;

namespace Hangout.Shared
{
	public enum AssetType
	{
		//Color
		ColorAsset,
		//Mesh
		MeshAsset,
		SkinnedMeshAsset,
		FaceMeshAsset,
		//Texture (these are pixel sources)
		TextureAsset,
		//Animation
		ComplexUvAnimationAsset,
		UvAnimationAsset,
		MoodAnimationAsset,
        // Used for room backgrounds (these are Texture2D objects)
	    ImageAsset
	}
}