/* Pherg 11/17/09
 * Holds the Animation for rig animations of walk cycles, emotes, etc.
 */

using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using Hangout.Shared;

namespace Hangout.Client
{
	public class RigAnimationAsset : Asset
	{
		private readonly AnimationClip mAnimationClip;
		public AnimationClip AnimationClip
		{
			get { return mAnimationClip; }
		}
		
		private RigAnimationName mAnimationName;
		public RigAnimationName AnimationName
		{
			get { return mAnimationName; }
		}
		
		public RigAnimationAsset(AssetSubType type, AnimationClip rigAnimationClip, string displayName, string path, string key, RigAnimationName animationName)
			: base(type, displayName, path, key)
		{
			if (rigAnimationClip == null)
			{
				throw new ArgumentNullException("rigAnimationClip");
			}
			mAnimationClip = rigAnimationClip;
			
			mAnimationName = animationName;
		}
	}
}
