/**  --------------------------------------------------------  *
 *   AvatarEntity.cs
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 07/24/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	public class AvatarEntity : IEntity
	{		
		private readonly int FRAMES_TO_UPDATE_ASSETS = 16;

		// These are used on construction of the avatar animation state machine.
		// It's the most convenient way to get the idle and walk cycle the first time.
		public RigAnimationName IdleRigAnimationName
		{
			get { return AvatarAssetController.RigAnimationController.IdleAnimationName; }
		}
		public RigAnimationName WalkRigAnimationName
		{
			get { return AvatarAssetController.RigAnimationController.WalkAnimationName; }
		}
		
		// TODO: Hard coded values
		public static Material AvatarMaterial
		{
			get
			{
				Material avatarMaterial = new Material(Shader.Find("Avatar/Saturation Shader"));
				avatarMaterial.SetFloat("_Falloff", 1.65f);
				avatarMaterial.SetFloat("_Cuttoff", 0.7f);
				return avatarMaterial;
			}
		}

		private float mMaxWalkSpeed = 1.0f;
		public float MaxWalkSpeed
		{
			get { return mMaxWalkSpeed; }
			set { mMaxWalkSpeed = value; }
		}

		private GameObject mAvatarBodyGameObject;
		public GameObject UnityGameObject
		{
			get { return mAvatarBodyGameObject; }
		}

		private Transform mHeadTransform;
		public Transform HeadTransform
		{
			get { return mHeadTransform; }
		}

		private bool mDisposed = false;
		public virtual void Dispose()
		{
			if (!mDisposed)
			{
				mDisposed = true;

				//the name tag needs to be disposed before the avatarBodyGameObject because it has a reference to the avatar body transform
				mNametag.Dispose();
				mNametag = null;

				GameObject.Destroy(mAvatarBodyGameObject);
				mAvatarBodyGameObject = null;

				GameObject.Destroy(mHeadTransform.gameObject);
				mHeadTransform = null;

				mAvatarAssetController.Dispose();
			}
		}

		private readonly AvatarAssetController mAvatarAssetController;
		public AvatarAssetController AvatarAssetController
		{
			get { return mAvatarAssetController; }
		}

		private NameTag3D mNametag;
        public Hangout.Client.NameTag3D Nametag
        {
            get { return mNametag; }
        }
        
        public string AvatarName
        {
            get { return mNametag.DisplayString; }
			set { mNametag.DisplayString = value; }
        }

		protected ClientAssetRepository mClientAssetRepository;

        public AvatarEntity(GameObject avatarBodyGameObject, GameObject headGameObject)
		{
			if (avatarBodyGameObject == null)
			{
				throw new ArgumentNullException("avatarBodyGameObject");
			}
			mAvatarBodyGameObject = avatarBodyGameObject;

			if (headGameObject == null)
			{
				throw new ArgumentNullException("headGameObject");
			}
			mHeadTransform = headGameObject.transform;

			mAvatarAssetController = new AvatarAssetController(headGameObject, mAvatarBodyGameObject);

			//This next call is based on no-downloading of mesh.  All being instantiated off of resources.
			mAvatarAssetController.ApplyMeshChanges();

			// TODO: Hard coded values
			//	Setup 3D NameTag system for avatar
			IScheduler scheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
			Font nameTagFont = Resources.Load("Fonts/HelveticaNeueCondensedBold_12") as Font;
			mNametag = new NameTag3D("", new Color(0.1f, 0.1f, 0.1f, 0.4f), nameTagFont, this.HeadTransform, scheduler);
			// The nametag was not meant to be parented to the avatar. It just tracks the avatar.
			// mNametag.SetGameObjectsParents(avatarBodyGameObject);
			mNametag.Offset = new Vector3(0, 0.31f, 0);
            mNametag.Visible = true;

			mClientAssetRepository = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
		}

		/// <summary>
		/// Setting assets on AvatarEntity.
		/// </summary>
		/// <param name="assetInfos"></param>
		public virtual void UpdateAssets(IEnumerable<AssetInfo> assetInfos)
		{
			mClientAssetRepository.GetAssets<Asset>(assetInfos, delegate(IEnumerable<Asset> assets)
			{
				ApplyAssets(assetInfos, assets);
			});
		}

		public void SetAssetsWithCallback(IEnumerable<AssetInfo> assetInfos, Action<AvatarEntity> assetsSet)
		{
			mClientAssetRepository.GetAssets<Asset>(assetInfos, delegate(IEnumerable<Asset> assets)
			{
				this.SetAssetsOnAvatarWithCallback(assets, assetsSet);
			});
		}
		
		protected void ApplyAssets(IEnumerable<AssetInfo> assetInfos, IEnumerable<Asset> assets)
		{
			this.SetAssetsOverFrames(assets);
		}
		
		public void SetAssetsOverFrames(IEnumerable<Asset> assetsToSet)
		{
			mAvatarAssetController.SetAssetsAndUpdateOverFrames(assetsToSet, FRAMES_TO_UPDATE_ASSETS);
		}

		private void SetAssetsOnAvatarWithCallback(IEnumerable<Asset> assetsToSet, Action<AvatarEntity> setAssets)
		{
			mAvatarAssetController.SetAssetsAndUpdateOverFramesWithCallback(assetsToSet, FRAMES_TO_UPDATE_ASSETS, delegate()
			{
				setAssets(this);
			});
		}
		
		public void SetAssets(IEnumerable<Asset> assetsToSet)
		{
			mAvatarAssetController.SetAssets(assetsToSet);
		}
		
		public void SetAsset(Asset assetToSet)
		{
			mAvatarAssetController.SetAsset(assetToSet);
		}
		
		public void RegisterForWalkAnimationChange(Hangout.Shared.Action animationChangeCallback)
		{
			mAvatarAssetController.RigAnimationController.RegisterForWalkAnimationChange(animationChangeCallback);
		}

		public void RegisterForIdleAnimationChange(Hangout.Shared.Action animationChangeCallback)
		{
			mAvatarAssetController.RigAnimationController.RegisterForIdleAnimationChange(animationChangeCallback);
		}
	}
}
