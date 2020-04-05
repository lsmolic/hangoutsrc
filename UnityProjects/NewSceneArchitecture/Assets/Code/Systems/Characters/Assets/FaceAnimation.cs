/*
   Created by Vilas Tewari on 2009-08-31.

	
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using Hangout.Shared;

namespace Hangout.Client
{
	public class FaceAnimation
	{
		private readonly string mName;
		
		private readonly UvAnimation mInitialAnimation;
		private List<UvAnimation> mAnimationSet = new List<UvAnimation>();
		
		public string Name
		{
			get{ return mName; }
		}
		public UvAnimation InitialAnimation
		{
			get{ return mInitialAnimation; }
		}
		public UvAnimation RandomAnimation
		{
			get{ return GetRandomAnimation(); }
		}

		public FaceAnimation(string name, UvAnimation initialAnimation, List<UvAnimation> animationSet)
		{
			mName = name;
			mInitialAnimation = initialAnimation;
			mAnimationSet = animationSet;
		}


		private UvAnimation GetRandomAnimation()
		{
			return mAnimationSet[ Random.Range( 0, mAnimationSet.Count )];
		}
	}
}