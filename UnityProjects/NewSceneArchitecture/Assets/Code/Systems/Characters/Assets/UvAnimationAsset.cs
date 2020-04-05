/*
   Created by Vilas Tewari on 2009-08-31.
   
	
*/

using UnityEngine;
using System.Collections;
using System.Xml;

using Hangout.Shared;

namespace Hangout.Client
{
	public class UvAnimationAsset : Asset
	{
		private readonly SimpleUvAnimation mAnimation;
		
		/*
			Properties
		*/
		public SimpleUvAnimation Animation
		{
			get{ return mAnimation; }
		}

		public UvAnimationAsset(AssetSubType type, SimpleUvAnimation animation, string displayName, string path, string key)
			: base(type, displayName, path, key)
		{
			mAnimation = animation;
		}
	}
}