/*
 * Pherg wrote this on 9/21/09
 */

using UnityEngine; 

using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using PureMVC.Patterns;

using Hangout.Shared;

namespace Hangout.Client
{
	/// <summary>
	/// This class has 2 responsibilities; being a central repository to access the active avatars (local and foreign) and also to build them.
	/// We may eventually want to split these 2 responsibilities into different classes
	/// </summary>
	public class AvatarMediator : CharacterMediator
	{
		private bool mHideForeignAvatars = false;
		private readonly IDictionary<DistributedObjectId, AvatarEntity> mForeignAvatarEntities = new Dictionary<DistributedObjectId, AvatarEntity>();
		public IDictionary<DistributedObjectId, AvatarEntity> ForeignAvatarEntities
		{
			get { return mForeignAvatarEntities; }
		}

		private Pair<DistributedObjectId, LocalAvatarEntity> mLocalAvatarEntity = null; 

		/// <summary>
		/// LocalAvatarEntity can be recreated, so this object can change. You probably don't want to cache this reference.
		/// </summary>
		public LocalAvatarEntity LocalAvatarEntity
		{
			get {	
					if (mLocalAvatarEntity != null)
					{
						return mLocalAvatarEntity.Second;
					}
					else
					{
						return null;
					} 
				}
		}
		
		public void AddLocalAvatarEntity(LocalAvatarEntity localAvatar, DistributedObjectId doId)
		{
			if (mLocalAvatarEntity != null)
			{
				Console.LogError("The local avatar should only be built once.  if we are seeing this message then this function be being used incorrectly.  this system is flimsy and the assumption that mLocalAvatarEntity has to be null might be incorrect.  if you see this message and it's still not crunch month look into this.");
			}
			if (localAvatar == null)
			{
				throw new ArgumentNullException("localAvatar");
			}
			if (doId == null)
			{
				throw new ArgumentNullException("doId");
			}
			mLocalAvatarEntity = new Pair<DistributedObjectId, LocalAvatarEntity>(doId, localAvatar);
		}
		
		public void RegisterForeignAvatar(AvatarEntity foreignAvatar, DistributedObjectId doId)
		{
			mForeignAvatarEntities.Add(new KeyValuePair<DistributedObjectId,AvatarEntity>(doId, foreignAvatar));
			if (mHideForeignAvatars)
			{
				HideForeignAvatars();
			}
		}

		private void SetupDownloadedAvatar(AvatarEntity avatarEntity, IEnumerable<Asset> loadedAssets, Action<AvatarEntity> avatarEntityLoadedCallback)
		{
			avatarEntity.AvatarAssetController.SetAssets(loadedAssets);
		
			// TODO: Make these paths less hard coded.
			CharacterMediator.AddAnimationClipToRig(avatarEntity.UnityGameObject, "Avatar/F_walk_normal_loop");
			CharacterMediator.AddAnimationClipToRig(avatarEntity.UnityGameObject, "Avatar/F_idle_normal_loop");
			
			avatarEntityLoadedCallback(avatarEntity);
		}

		public void RemoveForeignAvatarEntity(DistributedObjectId foreignAvatarDistributedObjectId)
		{
			AvatarEntity foreignAvatarEntity = null;
			List<AvatarEntity> foreignAvatarsToRemove = new List<AvatarEntity>();
			if (mForeignAvatarEntities.TryGetValue(foreignAvatarDistributedObjectId, out foreignAvatarEntity))
			{
				foreignAvatarsToRemove.Add(foreignAvatarEntity);
				foreignAvatarEntity.Dispose();
			}

			mForeignAvatarEntities.Remove(foreignAvatarDistributedObjectId);
			
			foreignAvatarsToRemove.Clear();
		}

		public void RemoveLocalAvatarEntity(DistributedObjectId localAvatarDistributedObjectId)
		{
			if (mLocalAvatarEntity.First == localAvatarDistributedObjectId)
			{
				mLocalAvatarEntity.Second.Dispose();
				mLocalAvatarEntity = null;
			}
			else
			{
				throw new System.Exception("Error: trying to remove an avatar that is not our current local avatar.");
			}
		}
		
		// TODO: Make a state machine so that avatars which enter the scene while shopping change to their correct state.
		public void HideForeignAvatars()
		{
			mHideForeignAvatars = true;
			foreach (AvatarEntity foreignAvatar in mForeignAvatarEntities.Values)
			{
				foreach(Renderer renderer in foreignAvatar.UnityGameObject.GetComponentsInChildren(typeof(Renderer)))
				{
					renderer.enabled = false;
				}
				foreignAvatar.Nametag.Visible = false;
			}
		}

		public void ShowForeignAvatars()
		{
			mHideForeignAvatars = false;
			foreach (AvatarEntity foreignAvatar in mForeignAvatarEntities.Values)
			{
				foreach (Renderer renderer in foreignAvatar.UnityGameObject.GetComponentsInChildren(typeof(Renderer)))
				{
					renderer.enabled = true;
				}
				foreignAvatar.Nametag.Visible = true;
			}
		}

		public override void OnRemove()
		{
			mForeignAvatarEntities.Clear();
		}
	}
}
