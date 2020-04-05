/*
   Created by Vilas Tewari on 2009-08-31.
   
	
*/

using UnityEngine;
using System.Collections;
using System.Xml;

using Hangout.Shared;

namespace Hangout.Client
{
	public class FaceAnimationAsset : Asset
	{
		private readonly FaceAnimation mFaceAnimation;
		public FaceAnimation FaceAnimation
		{
			get{ return mFaceAnimation; }
		}
		
		private FaceAnimationName mFaceAnimationName;
		public FaceAnimationName FaceAnimationName
		{
			get { return mFaceAnimationName; }
		}

		public FaceAnimationAsset(AssetSubType type, FaceAnimation faceAnimation, string displayName, string path, string key, FaceAnimationName faceAnimationName)
			: base(type, displayName, path, key)
		{
			mFaceAnimation = faceAnimation;
			mFaceAnimationName = faceAnimationName;
		}
	}
}