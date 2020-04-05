/*
   Created by Vilas Tewari on 2009-08-18.

	This class handles merging, Rigging and skinning of the Avatar's meshes ( cuts )
*/

using UnityEngine;
using System;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client
{
	public class AvatarBodyMesh : IDisposable
	{
		private Transform mBodyTransform;
		private SkinnedMeshRenderer mSkinnedMeshRenderer;

		// We use this to get the atlas space for UVs
		private AvatarTexture mBodyTexture;

		// Mapping of AssetSubType to SkinnedMeshAsset
		private IDictionary<AssetSubType, SkinnedMeshRenderer> mCurrentlyWearing;
		private List<SkinnedMeshRenderer> mMeshes;

		// Avatar Rig
		private IDictionary<string, int> mBoneNameToIndex;
		private IDictionary<int, Rect> mAtlasSpaces;

		// Is the current mesh out of date
		private bool mIsDirty;

		// Used for efficient mesh combining
		private int[] mTriangles;
		private Vector2[] mUvs;
		private Vector3[] mNormals;
		private Vector3[] mVerts;
		private BoneWeight[] mBoneWeights;
		private Matrix4x4[] mBindPoses;
		private int mTotalVertCount;
		private int mTotalTriCount;

		public bool IsDirty
		{
			get { return mIsDirty; }
		}

		public AvatarBodyMesh(AvatarTexture bodyTexture, Transform bodyTransform, SkinnedMeshRenderer smk)
		{
			mBodyTransform = bodyTransform;
			mSkinnedMeshRenderer = smk;
			mBoneNameToIndex = new Dictionary<string, int>();
			mCurrentlyWearing = new Dictionary<AssetSubType, SkinnedMeshRenderer>();
			mMeshes = new List<SkinnedMeshRenderer>();

			mBodyTexture = bodyTexture;
			mIsDirty = false;
			mAtlasSpaces = new Dictionary<int, Rect>();
			SetupAvatarRig();
		}

		/// <summary>
		/// Create a new mesh to reflect any changes made
		/// </summary>
		public void CreateMesh()
		{
			// For visual debugging aid
			// MeshFilter meshFilter = gameObject.AddComponent( typeof( MeshFilter) ) as MeshFilter;

			if (mIsDirty)
			{
				Mesh newMesh = new Mesh();

				mUvs = new Vector2[mTotalVertCount];
				mNormals = new Vector3[mTotalVertCount];
				mVerts = new Vector3[mTotalVertCount];
				mTriangles = new int[mTotalTriCount];
				mBoneWeights = new BoneWeight[mTotalVertCount];

				int processedVertCount = 0;
				int processedTriCount = 0;

				// Map bone weights to new bone Array
				for (int x = 0; x < mMeshes.Count; ++x)
				{
					Mesh assetMesh = mMeshes[x].sharedMesh;
					int[] sourceTriangles = assetMesh.triangles;
					Vector3[] sourceVertices = assetMesh.vertices;
					Vector3[] sourceNormals = assetMesh.normals;
					Vector2[] sourceUvs = assetMesh.uv;
					BoneWeight[] sourceBoneWeights = assetMesh.boneWeights;
					Transform[] sourceBones = mMeshes[x].bones;

					/*
						Add Triangles to triangles list.
						Make sure to add vert count to the triangle indices since the verts now have different indices
					*/
					for (int t = 0; t < sourceTriangles.Length; ++t)
					{
						mTriangles[processedTriCount] = processedVertCount + sourceTriangles[t];
						++processedTriCount;
					}

					/* Process Uvs to atlas them correctly */
					Rect atlasSpace = mAtlasSpaces[x];
					Vector2 atlasDimensions = new Vector2(atlasSpace.width, atlasSpace.height);
					Vector2 atlasPosition = new Vector2(atlasSpace.x, atlasSpace.y);
					for (int v = 0; v < sourceVertices.Length; ++v)
					{
						Vector2 atlasedUv = Vector2.Scale(sourceUvs[v], atlasDimensions);
						atlasedUv += atlasPosition;
						mUvs[processedVertCount] = atlasedUv;
						mVerts[processedVertCount] = sourceVertices[v];
						mNormals[processedVertCount] = sourceNormals[v];

						/* Setup BoneIndicies and BoneWeights */
						int boneIndex = 0;

						boneIndex = GetBoneIndex(sourceBones[sourceBoneWeights[v].boneIndex0].name);
						mBoneWeights[processedVertCount].boneIndex0 = boneIndex;

						boneIndex = GetBoneIndex(sourceBones[sourceBoneWeights[v].boneIndex1].name);
						mBoneWeights[processedVertCount].boneIndex1 = boneIndex;

						boneIndex = GetBoneIndex(sourceBones[sourceBoneWeights[v].boneIndex2].name);
						mBoneWeights[processedVertCount].boneIndex2 = boneIndex;

						boneIndex = GetBoneIndex(sourceBones[sourceBoneWeights[v].boneIndex3].name);
						mBoneWeights[processedVertCount].boneIndex3 = boneIndex;

						mBoneWeights[processedVertCount].weight0 = sourceBoneWeights[v].weight0;
						mBoneWeights[processedVertCount].weight1 = sourceBoneWeights[v].weight1;
						mBoneWeights[processedVertCount].weight2 = sourceBoneWeights[v].weight2;
						mBoneWeights[processedVertCount].weight3 = sourceBoneWeights[v].weight3;

						++processedVertCount;
					}
				}

				// Build vertices, UVs & skin weights by Copying info from meshes
				newMesh.vertices = mVerts;
				newMesh.uv = mUvs;
				newMesh.triangles = mTriangles;
				newMesh.normals = mNormals;
				newMesh.boneWeights = mBoneWeights;
				newMesh.bindposes = mBindPoses;
				newMesh.RecalculateBounds();

				// Setup SkinnedMeshRenderer Component
				Mesh.Destroy(mSkinnedMeshRenderer.sharedMesh);
				mSkinnedMeshRenderer.sharedMesh = newMesh;
			}
			mIsDirty = false;
		}

		public void Dispose()
		{
			mBodyTexture.Dispose();
			Mesh.Destroy(mSkinnedMeshRenderer.sharedMesh);
		}

		public bool IsWearing(AssetSubType type)
		{
			return mCurrentlyWearing.ContainsKey(type);
		}

		/// <summary>
		/// Set a Mesh asset on an Avatar i.e change what it's wearing
		/// </summary>
		public void SetMesh(AssetSubType type, SkinnedMeshRenderer skinnedMesh)
		{
			if (mCurrentlyWearing.ContainsKey(type))
				mMeshes.Remove(mCurrentlyWearing[type]);

			mCurrentlyWearing[type] = skinnedMesh;
			mMeshes.Add(skinnedMesh);

			// Update Tri & Vert totals. Also update UvSpaces
			mTotalVertCount = 0;
			mTotalTriCount = 0;
			mAtlasSpaces.Clear();
			foreach (KeyValuePair<AssetSubType, SkinnedMeshRenderer> kvp in mCurrentlyWearing)
			{
				Mesh assetMesh = kvp.Value.sharedMesh;

				mTotalVertCount += assetMesh.vertices.Length;
				mTotalTriCount += assetMesh.triangles.Length;
				mAtlasSpaces.Add(mMeshes.IndexOf(kvp.Value), mBodyTexture.GetTextureZoneUvArea(kvp.Key));
			}

			mIsDirty = true;
		}

		/// <summary>
		/// Get the index of a bone based on it's transform name. This is used to map an asset from one rig to another
		/// </summary>
		private int GetBoneIndex(string boneName)
		{
			return mBoneNameToIndex[boneName];
		}

		/// <summary>
		/// Get Avatar Rig Joints and store the bind poses
		/// </summary>
		private void SetupAvatarRig()
		{
			//	Find Root node.
			//	Assumes the Rig's root node has "ROOT" in it's name!
			//	Assume the Root is child of mBodyTransform	
			Transform root = null;
			foreach (Transform child in mBodyTransform)
			{
				if (child.name.ToLower().Contains("root"))
				{
					root = child;
					break;
				}
			}

			// If the root is found get rest of the joints. I Assume all joints are uniquely named!
			if (root != null)
			{
				List<Transform> avatarBones = new List<Transform>();
				GetJointTransforms(avatarBones, root);
				mBindPoses = new Matrix4x4[avatarBones.Count];

				// create a mapping of bone name to transform
				for (int x = 0; x < avatarBones.Count; ++x)
				{
					mBoneNameToIndex.Add(avatarBones[x].name, x);

					//	Set Bind Poses. NOTE: This Assumes that the rig is in the bind pose.
					//	TODO: We should think of a way to store the bind pose data in Unity, on the rig
					//	this way we don't have to have the rig physically be in bind pose
					mBindPoses[x] = avatarBones[x].worldToLocalMatrix * mBodyTransform.localToWorldMatrix;
				}
				// Setup bones on SkinnedMeshRenderer
				mSkinnedMeshRenderer.bones = avatarBones.ToArray();
			}
			else
			{
				throw new Exception("Unable to find root node");
			}
		}

		/// <summary>
		/// Recursively get all the transforms under topNode and put them into jointList
		/// </summary>
		private void GetJointTransforms(List<Transform> jointList, Transform topNode)
		{
			jointList.Add(topNode);
			foreach (Transform child in topNode)
			{
				GetJointTransforms(jointList, child);
			}
		}
	}
}