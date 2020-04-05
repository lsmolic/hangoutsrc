/* Pherg 11/17/09
 * Takes in RigAnimationAssets to apply to the rig.  Handles swapping
 * walk and idle cycles as well as other emotes based on mood.
 */

using System;
using System.Collections.Generic;
using System.Text;

using Hangout.Shared;

using UnityEngine;

namespace Hangout.Client
{
	public class RigAnimationController
	{
		private GameObject mRig;
		private Animation mAnimationComponent;
		
		private Hangout.Shared.Action mWalkAnimationChangedCallback;
		private Hangout.Shared.Action mIdleAnimationChangedCallback;

		private RigAnimationAsset mRigIdleAnimationAsset;
		public RigAnimationName IdleAnimationName
		{
			get 
			{ 
				if (mRigIdleAnimationAsset == null)
				{
					return RigAnimationName.None;
				}
				else
				{
					return mRigIdleAnimationAsset.AnimationName;
				}
			}
		}

		private RigAnimationAsset mRigWalkAnimationAsset;
		public RigAnimationName WalkAnimationName
		{
			get 
			{ 
				if (mRigWalkAnimationAsset == null)
				{
					return RigAnimationName.None;
				}
				else
				{
					return mRigWalkAnimationAsset.AnimationName;
				}
			}
		}
		
		public RigAnimationController(GameObject rig)
		{
			mRig = rig;
			mAnimationComponent = mRig.GetComponent(typeof(Animation)) as Animation;
		}
		
		public void SetAnimation(Asset asset)
		{
			RigAnimationAsset rigAnimationAsset = asset as RigAnimationAsset;
			if (rigAnimationAsset == null)
			{
				Console.LogError("Could not convert asset to RigAnimationAsset.");
				return;
			}
			
			if (rigAnimationAsset.Type == AssetSubType.RigWalkAnimation)
			{
				if (mRigWalkAnimationAsset != null)
				{
					mAnimationComponent.RemoveClip(mRigWalkAnimationAsset.AnimationClip);
				}
				mRigWalkAnimationAsset = rigAnimationAsset;

				// Notify that the walk has changed.
				if (mWalkAnimationChangedCallback != null)
				{
					mWalkAnimationChangedCallback();
				}
			}
			else if (rigAnimationAsset.Type == AssetSubType.RigIdleAnimation)
			{
				if (mRigIdleAnimationAsset != null)
				{
					mAnimationComponent.RemoveClip(mRigIdleAnimationAsset.AnimationClip);
				}
				mRigIdleAnimationAsset = rigAnimationAsset;
				
				// Notify that the idle has changed.
				if (mIdleAnimationChangedCallback != null)
				{
					mIdleAnimationChangedCallback();
				}
			}

			mAnimationComponent.AddClip(rigAnimationAsset.AnimationClip, rigAnimationAsset.AnimationClip.name);
		}

		public void RegisterForWalkAnimationChange(Hangout.Shared.Action animationChangeCallback)
		{
			mWalkAnimationChangedCallback = animationChangeCallback;
		}

		public void RegisterForIdleAnimationChange(Hangout.Shared.Action animationChangeCallback)
		{
			mIdleAnimationChangedCallback = animationChangeCallback;
		}
	}
}
