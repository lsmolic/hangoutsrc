/*
   Created by Vilas Tewari on 2009-08-16.
*/

using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	public class BodyController : IDisposable
	{
		private AvatarBodyMesh mBodyMesh;

		// Face Texture System
		private AvatarBodyTexture mBodyTexture;
		private GameObject mAvatarGameObject;
		public GameObject AvatarGameObject
		{
			get { return mAvatarGameObject; }
		}

		public void Dispose()
		{
			mBodyMesh.Dispose();
			mBodyTexture.Dispose();
		}
		
		private Hangout.Shared.Action mBodyControllerLoadedCallback;
		
		private List<string> mAssetsBeingLoaded = new List<string>();

		public AvatarBodyTexture BodyTexture
		{
			get { return mBodyTexture; }
		}
		
		public BodyController(GameObject bodyGameObject)
		{
			if (bodyGameObject == null)
			{
				throw new ArgumentNullException("avatarGameObject");
			}
			mAvatarGameObject = bodyGameObject;

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
		}
		
		public void SetAssets(IEnumerable<Asset> assetsToSet)
		{
			foreach( Asset asset in assetsToSet )
			{
				SetSingleAsset(asset);
			}
		}

		public void AssetRetrievedFromRepository(Asset asset)
		{
			if (asset == null)
			{
				throw new Exception("asset coming back from ClientRepo is null.");
			}
			SetSingleAsset(asset);

			//This code makes the applying of textures more efficient.  Since it takes time to 
			//apply each one we only do it once all the requested assets have been set.

			//After we set the asset we try to remove it from the loadingAssets list.
			if (mAssetsBeingLoaded.Contains(asset.UniqueKey))
			{
				mAssetsBeingLoaded.Remove(asset.UniqueKey);
			}
			else
			{
				throw new Exception("Asset being set that was not in mAssetsBeingLoaded.  Most likely an asset was being added through a function in this class which did not add it to the list.");
			}

			//If there are no more assets being loaded then apply the changes and compress.
			if (mAssetsBeingLoaded.Count == 0)
			{
				BodyTexture.ApplyChanges();
				//We check to see if there is a callback waiting to be executed when the head controller is finished.
				//TODO: Robustify this.  Right now the avatar entity is not given to anything until it is finished 
				//loading.  To finish loading it needs to load this head controller fully.  The only place this callback
				//is set is in one of the constructors.  This means no system should be able to touch the avatarEntity 
				//which contains this controller until it's all set up.  Which it will set the callback to null so it
				//never gets executed twice, or executed before the first initial items are set on the head controller.
				//If something could get access to the avatarEntity before this head controller is set up it could set
				//an asset which could prematurely execute this callback. 
				if (mBodyControllerLoadedCallback != null)
				{
					mBodyControllerLoadedCallback();
					mBodyControllerLoadedCallback = null;
				}
			}
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

		public void SetSingleAsset(Asset asset)
		{
			switch (asset.Type)
			{
				case AssetSubType.HairTexture:
				case AssetSubType.HandsTexture:
				case AssetSubType.TopTexture:
				case AssetSubType.BottomTexture:
				case AssetSubType.FootwearTexture:
				case AssetSubType.BagTexture:
					SetBodyTexture(asset);
					break;
				case AssetSubType.HairColor:
				case AssetSubType.SkinColor:
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
			}
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
		
		public void UpdateAssets()
		{
			ApplyMeshChanges();
			BodyTexture.ApplyChanges();
		}

		public void UpdateAssetsOverFrames(int frames)
		{
			BodyTexture.ApplyChangesOverFrames(frames, ApplyMeshChanges);
		}

		public void UpdateAssetsOverFramesFirstTime(int frames, Hangout.Shared.Action onFinished)
		{
			BodyTexture.ApplyChangesOverFrames(frames, delegate()
			{
				ApplyMeshChangesFirstTime(onFinished);
			});
		}
	}
}