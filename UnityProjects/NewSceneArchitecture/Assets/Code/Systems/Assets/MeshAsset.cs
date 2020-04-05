/*
   Created by Vilas Tewari on 2009-08-11.

	An Asset that encapsulates a MeshObject with information like
	a displayName, Id, etc..
*/


using System;
using Hangout.Shared;
using UnityEngine;

namespace Hangout.Client
{
	public class MeshAsset : Asset
	{
		private readonly Mesh mMesh;

		public Mesh Mesh
		{
			get { return mMesh; }
		}

		public MeshAsset(AssetSubType type, Mesh mesh, string displayName, string path, string key)
			: base(type, displayName, path, key)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			mMesh = mesh;
		}
	}
}