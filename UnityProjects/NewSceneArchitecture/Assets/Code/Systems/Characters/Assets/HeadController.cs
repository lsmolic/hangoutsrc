/*
   Created by Vilas Tewari on 2009-08-16.

	A Component that manages various systems that control
	an avatar head. This Component must be on the head transform
	It implements head & face animation / behavior and can
	override animation on the head transform
*/

using UnityEngine;

using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	/// <summary>
	/// Controls everything to do with the face animation and texture (including mesh and all that)
	/// </summary>
	public class HeadController : IDisposable
	{

		private AvatarFaceTexture mFaceTexture;
		private Renderer mFaceRenderer;

		private AvatarFaceMesh mAvatarFaceMesh;
		private FaceAnimator mFaceAnimator;

		private readonly ITask mUpdateTask;

		public bool Visible
		{
			get { return mFaceRenderer.enabled; }
			set { mFaceRenderer.enabled = value; }
		}

		public HeadController(GameObject headGameObject)
		{
			if (headGameObject == null)
			{
				throw new ArgumentNullException("headGameObject");
			}

			//headGameObject.AddComponent(typeof(MeshFilter));
			mFaceRenderer = headGameObject.GetComponent(typeof(MeshRenderer)) as Renderer;
			//mFaceRenderer.enabled = false;

			// Create the Avatar's Face LayeredPalette
			mFaceTexture = new AvatarFaceTexture(mFaceRenderer);

			// FaceMesh will setup mesh and UvShells
			mAvatarFaceMesh = new AvatarFaceMesh(mFaceTexture, headGameObject.transform);
			mFaceAnimator = new FaceAnimator(mAvatarFaceMesh);

			mUpdateTask = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(Update());;
		}

		public void Dispose()
		{
			mUpdateTask.Exit();

			mAvatarFaceMesh.Dispose();
			mFaceTexture.Dispose();
			mFaceAnimator.Dispose();
		}

		public void SetAssets(IEnumerable<Asset> assetsToSet)
		{
			foreach (Asset asset in assetsToSet)
			{
				SetSingleAsset(asset);
			}
		}

		public void SetSingleAsset(Asset asset)
		{
			switch (asset.Type)
			{
				// Mesh
				case AssetSubType.FaceMesh:
					mAvatarFaceMesh.SetMesh(asset);
					break;
				// Textures
				case AssetSubType.EyeShadowTexture:
				case AssetSubType.EyeLinerTexture:
				case AssetSubType.EyeTexture:
				case AssetSubType.EyebrowTexture:
				case AssetSubType.NoseTexture:
				case AssetSubType.EarTexture:
				case AssetSubType.FaceMarkTexture:
				case AssetSubType.FaceBlushTexture:
				case AssetSubType.MouthTexture:
					SetFaceTexture(asset);
					break;
				// Colors
				case AssetSubType.SkinColor:
				case AssetSubType.EyeColor:
				case AssetSubType.EyebrowColor:
				case AssetSubType.MouthColor:
					SetFaceTextureColor(asset);
					break;
				case AssetSubType.EyeShadowColor:
				case AssetSubType.FaceBlushColor:
					SetFaceTextureColorOnly(asset);
					break;
				case AssetSubType.EyeShadowAlpha:
				case AssetSubType.FaceBlushAlpha:
					SetFaceTextureAlpha(asset);
					break;
				case AssetSubType.FaceAnimation:
					PlayAnimation(asset);
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Change a texture
		/// </summary>
		private void SetFaceTexture(Asset asset)
		{
			TextureAsset textureAsset = asset as TextureAsset;
			if (textureAsset.PixelSource == null)
			{
				throw new ArgumentNullException("asset");
			}
			mFaceTexture.SetLayerSource(textureAsset.Type, textureAsset.PixelSource);
		}

		/// <summary>
		/// Change a texture color
		/// </summary>
		private void SetFaceTextureColor(Asset asset)
		{
			ColorAsset colorAsset = asset as ColorAsset;
			if (colorAsset == null)
			{
				throw new ArgumentNullException("asset");
			}
			mFaceTexture.SetLayerFilterColor(colorAsset.Type, colorAsset.Color);
		}

		// Change a texture color
		private void SetFaceTextureColorOnly(Asset asset)
		{
			ColorAsset colorAsset = asset as ColorAsset;
			if (colorAsset == null)
			{
				throw new ArgumentNullException("asset");
			}
			mFaceTexture.SetLayerFilterColorOnly(asset.Type, colorAsset.Color);
		}

		// Change a texture color
		private void SetFaceTextureAlpha(Asset asset)
		{
			ColorAsset colorAsset = asset as ColorAsset;
			if (colorAsset == null)
			{
				throw new ArgumentNullException("asset");
			}
			mFaceTexture.SetLayerFilterAlpha(asset.Type, colorAsset.Color);
		}

		private void PlayAnimation(Asset asset)
		{
			FaceAnimationAsset faceAnimationAsset = asset as FaceAnimationAsset;
			if (faceAnimationAsset == null)
			{
				throw new ArgumentNullException("asset");
			}
			mFaceAnimator.FaceAnimation = faceAnimationAsset.FaceAnimation;
		}


		/* NEW FUNCS TO INTEGRATE */
		public int GetUvShellIndex(string shellName)
		{
			return mAvatarFaceMesh.GetUvShellIndex(shellName);
		}
		public Vector2 GetUvShellOffset(int index)
		{
			return mAvatarFaceMesh.GetUvShellOffset(index);
		}
		public float GetUvShellRotation(int index)
		{
			return mAvatarFaceMesh.GetUvShellRotation(index);
		}
		public Vector2 GetUvShellScale(int index)
		{
			return mAvatarFaceMesh.GetUvShellScale(index);
		}

		public void SetUvShellOffset(Vector2 offset, int index)
		{
			mAvatarFaceMesh.SetUvShellOffset(offset, index);
		}
		public void SetUvShellRotation(float rotation, int index)
		{
			mAvatarFaceMesh.SetUvShellRotation(rotation, index);
		}
		public void SetUvShellScale(Vector2 scale, int index)
		{
			mAvatarFaceMesh.SetUvShellScale(scale, index);
		}

		private IEnumerator<IYieldInstruction> Update()
		{
			while (true)
			{
				mAvatarFaceMesh.UpdateUvs();
				yield return new YieldUntilNextFrame();
			}
		}

		public void UpdateAssets()
		{
			mFaceTexture.ApplyChanges();
		}

		public void UpdateAssetsOverFrames(int frames)
		{
			mFaceTexture.ApplyChangesOverFrames(frames, delegate()
			{
				// The face renderer is set to false during construction so when
				// we set the face and finish applying the face to the face we do this.
				mFaceRenderer.enabled = true;
			});
		}
	}
}