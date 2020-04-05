/* Pherg 10/25/09 */

using System;
using Hangout.Shared;
using UnityEngine;

namespace Hangout.Client
{
	public class TransformAnimationAsset : Asset
	{
		private AnimationClip mAnimationClip;
		public AnimationClip AnimationClip
		{
			get { return mAnimationClip; }
		}
		
		public TransformAnimationAsset(AssetSubType assetSubType, AnimationClip animationClip, string name, string path, string key)
			: base(assetSubType, name, path, key)
		{
			if (animationClip == null)
			{
				throw new ArgumentNullException("animationClip");
			}
			
			mAnimationClip = animationClip;
		}	
	}
}
