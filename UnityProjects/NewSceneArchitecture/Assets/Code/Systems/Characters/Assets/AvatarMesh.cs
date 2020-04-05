/*
   Created by Vilas Tewari on 2009-08-28.
   
	A Mesh that is managed by an avatar.
	Avatar meshes always use AvatarTextures to atlas thier UVs
*/


using UnityEngine;
using System.Collections;
using System;

namespace Hangout.Client
{
	public abstract class AvatarMesh : IDisposable
	{
		/* We use this to get the atlas space for UVs */
		protected AvatarTexture mTexturePalette;
		
		protected Transform mMeshTransform;
		
		public AvatarMesh( AvatarTexture texturePalette, Transform meshTransform )
		{
			mTexturePalette = texturePalette;
			mMeshTransform = meshTransform;
		}

		public virtual void Dispose()
		{
			mTexturePalette.Dispose();
		}
	}
}