/*
   Created by Vilas Tewari on 2009-08-11.

	A MeshAsset that has skinning and rigging data
*/

using Hangout.Shared;
using UnityEngine;

namespace Hangout.Client
{
	public class SkinnedMeshAsset : MeshAsset
	{
		protected SkinnedMeshRenderer mSkinnedMeshRenderer;

		public SkinnedMeshRenderer SkinnedMeshRenderer
		{
			get { return mSkinnedMeshRenderer; }
		}

		public SkinnedMeshAsset(AssetSubType type, SkinnedMeshRenderer smk, string displayName, string path, string key)
			: base(type, smk.sharedMesh, displayName, path, key)
		{
			mSkinnedMeshRenderer = smk;
		}
	}
}