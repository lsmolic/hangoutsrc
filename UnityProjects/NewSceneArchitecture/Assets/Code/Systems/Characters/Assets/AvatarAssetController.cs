/* Pherg 11/11/09 
 * This is an encapsulation of the head and body controllers. */

using UnityEngine;

using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	public class AvatarAssetController : IDisposable
	{
		private ITask mUpdatingBodyTask;
		private ITask mUpdatingFaceTask;
		
		private Dictionary<AssetSubType, Asset> mAssetsBeingUpdatedOverFrames = new Dictionary<AssetSubType,Asset>();
		
		private Hangout.Shared.Action mFinishedUpdatingCallback;
		
		private AvatarFaceTexture mFaceTexture;
		private Renderer mFaceRenderer;

		private AvatarFaceMesh mAvatarFaceMesh;
		private FaceAnimator mFaceAnimator;
		
		private RigAnimationController mRigAnimationController;
		public RigAnimationController RigAnimationController
		{
			get { return mRigAnimationController; }
		}

		private readonly ITask mUpdateTask;

		public bool Visible
		{
			get { return mFaceRenderer.enabled; }
			set { mFaceRenderer.enabled = value; }
		}

		private AvatarBodyMesh mBodyMesh;

		public AvatarBodyTexture BodyTexture
		{
			get { return mBodyTexture; }
		}

		// Face Texture System
		private AvatarBodyTexture mBodyTexture;
		private GameObject mAvatarGameObject;
		public GameObject AvatarGameObject
		{
			get { return mAvatarGameObject; }
		}

		public AvatarAssetController(GameObject headGameObject, GameObject bodyGameObject)
		{
			if (bodyGameObject == null)
			{
				throw new ArgumentNullException("avatarGameObject");
			}
			mAvatarGameObject = bodyGameObject;
			
			mRigAnimationController = new RigAnimationController(mAvatarGameObject);

			// Add skinned mesh renderer component
			Renderer mBodyRenderer = mAvatarGameObject.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
			if (mBodyRenderer == null)
			{
				mBodyRenderer = mAvatarGameObject.AddComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
			}
			mBodyRenderer.material = AvatarEntity.AvatarMaterial;

			// Create the Avatar's Body LayeredPalette
			mBodyTexture = new AvatarBodyTexture(mBodyRenderer);

			// Setup the Face UvShellAnimator
			mBodyMesh = new AvatarBodyMesh(mBodyTexture, bodyGameObject.transform, (SkinnedMeshRenderer)mBodyRenderer);
			
			if (headGameObject == null)
			{
				throw new ArgumentNullException("headGameObject");
			}

			//headGameObject.AddComponent(typeof(MeshFilter));
			mFaceRenderer = headGameObject.GetComponent(typeof(MeshRenderer)) as Renderer;
			mFaceRenderer.enabled = false;

			// Create the Avatar's Face LayeredPalette
			mFaceTexture = new AvatarFaceTexture(mFaceRenderer);

			// FaceMesh will setup mesh and UvShells
			mAvatarFaceMesh = new AvatarFaceMesh(mFaceTexture, headGameObject.transform);
			mFaceAnimator = new FaceAnimator(mAvatarFaceMesh);

			mUpdateTask = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(Update());;
		}

		public void SetAssets(IEnumerable<Asset> assetsToSet)
		{
			foreach (Asset asset in assetsToSet)
			{
				SetSingleAsset(asset);
			}
			UpdateAssets();
		}
		
		public void SetAsset(Asset asset)
		{
			SetSingleAsset(asset);
		}

		private void SetSingleAsset(Asset asset)
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
					SetBodyTextureFilterColor(asset);
					SetFaceTextureColor(asset);
					break;
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
				case AssetSubType.HairTexture:
				case AssetSubType.HandsTexture:
				case AssetSubType.TopTexture:
				case AssetSubType.BottomTexture:
				case AssetSubType.FootwearTexture:
				case AssetSubType.BagTexture:
					SetBodyTexture(asset);
					break;
				case AssetSubType.HairColor:
					SetBodyTextureFilterColor(asset);
					break;
				case AssetSubType.TopSkinnedMesh:
				case AssetSubType.HairSkinnedMesh:
				case AssetSubType.BottomSkinnedMesh:
				case AssetSubType.FootwearSkinnedMesh:
				case AssetSubType.HandsSkinnedMesh:
				case AssetSubType.BagSkinnedMesh:
					SetBodyMesh(asset);
					break;
				case AssetSubType.TopColor:
				case AssetSubType.BottomColor:
				case AssetSubType.FootwearColor:
					SetBodyTextureSolidColor(asset);
					break;
				case AssetSubType.RigWalkAnimation:
				case AssetSubType.RigIdleAnimation:
				case AssetSubType.RigAnimation:
					mRigAnimationController.SetAnimation(asset);
					break;
				case AssetSubType.FaceAnimation:
					PlayAnimation(asset);
					break;
				default:
					Console.LogError("Avatar Asset Controller does not know how to handle AssetSubType: " + asset.Type.ToString() +".  Asset Path: " + asset.Path + " Asset Name: " + asset.DisplayName);
					break;
			}
		}

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

		public void PlayAnimation(Asset asset)
		{
			FaceAnimationAsset faceAniamtionAsset = asset as FaceAnimationAsset;

			mFaceAnimator.FaceAnimation = faceAniamtionAsset.FaceAnimation;
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

		private void SetBodyTextureSolidColor(Asset asset)
		{
			ColorAsset colorAsset = (ColorAsset)asset;
			SolidColorPixelSource pixelSource = new SolidColorPixelSource(1024, 1024, colorAsset.Color);
			mBodyTexture.SetLayerSource(asset.Type, pixelSource);
		}

		// Change a texture, doesn't apply that texture
		private void SetBodyTexture(Asset asset)
		{
			TextureAsset textureAsset = (TextureAsset)asset;
			if (textureAsset.PixelSource == null)
			{
				throw new ArgumentNullException("textureAsset.PixelSource");
			}
			mBodyTexture.SetLayerSource(textureAsset.Type, textureAsset.PixelSource);
		}

		// Change a texture color
		private void SetBodyTextureFilterColor(Asset asset)
		{
			ColorAsset colorAsset = (ColorAsset)asset;
			mBodyTexture.SetLayerFilterColor(colorAsset.Type, colorAsset.Color);
		}

		private void SetBodyMesh(Asset asset)
		{
			SkinnedMeshAsset skinnedMeshAsset = (SkinnedMeshAsset)asset;
			if (skinnedMeshAsset.SkinnedMeshRenderer == null)
			{
				throw new ArgumentNullException("skinnedMeshAsset.SkinnedMeshRenderer");
			}
			mBodyMesh.SetMesh(skinnedMeshAsset.Type, skinnedMeshAsset.SkinnedMeshRenderer);
		}

		public void ApplyMeshChanges()
		{
			mBodyMesh.CreateMesh();
		}

		public void ApplyMeshChangesFirstTime(Hangout.Shared.Action onFinished)
		{
			mBodyMesh.CreateMesh();
			onFinished();
		}

		private IEnumerator<IYieldInstruction> Update()
		{
			while (true)
			{
				mAvatarFaceMesh.UpdateUvs();
				yield return new YieldUntilNextFrame();
			}
		}
		
		private void UpdateAssets()
		{
			ApplyMeshChanges();
			mFaceTexture.ApplyChanges();
			BodyTexture.ApplyChanges();
		}

		public void SetAssetsAndUpdateOverFrames(IEnumerable<Asset> assets, int frames)
		{
			if (mAssetsBeingUpdatedOverFrames.Count > 0)
			{
				mUpdatingBodyTask.ForceExit();
				mUpdatingFaceTask.ForceExit();
				foreach (Asset asset in assets)
				{
					if (mAssetsBeingUpdatedOverFrames.ContainsKey(asset.Type))
					{
						mAssetsBeingUpdatedOverFrames[asset.Type] = asset;
					}
					else
					{
						mAssetsBeingUpdatedOverFrames.Add(asset.Type, asset);
					}
					SetSingleAsset(asset);
				}
			}
			else
			{
				foreach (Asset asset in assets)
				{
					SetSingleAsset(asset);
					if (mAssetsBeingUpdatedOverFrames.ContainsKey(asset.Type))
					{
						mAssetsBeingUpdatedOverFrames[asset.Type] = asset;
					}
					else
					{
						mAssetsBeingUpdatedOverFrames.Add(asset.Type, asset);
					}
				}
			}
			UpdateAssetsOverFrames(frames);
		}

		private void UpdateAssetsOverFrames(int frames)
		{
			mUpdatingBodyTask = BodyTexture.ApplyChangesOverFrames(frames, ApplyMeshChanges);
			mUpdatingBodyTask.AddOnExitAction(delegate()
			{
				mFaceRenderer.enabled = true;
				mAssetsBeingUpdatedOverFrames.Clear();
				if (mFinishedUpdatingCallback != null)
				{
					mFinishedUpdatingCallback();
					mFinishedUpdatingCallback = null;
				}
			});
			mUpdatingFaceTask = mFaceTexture.ApplyChangesOverFrames(frames, delegate()
			{
			});
		}

		public void SetAssetsAndUpdateOverFramesWithCallback(IEnumerable<Asset> assets, int frames, Hangout.Shared.Action onFinished)
		{
			SetAssetsAndUpdateOverFrames(assets, frames);
			mFinishedUpdatingCallback += onFinished;
		}

		public void Dispose()
		{
			mBodyMesh.Dispose();
			mBodyTexture.Dispose();

			mUpdateTask.Exit();

			mAvatarFaceMesh.Dispose();
			mFaceTexture.Dispose();
			mFaceAnimator.Dispose();
		}
	}
}
