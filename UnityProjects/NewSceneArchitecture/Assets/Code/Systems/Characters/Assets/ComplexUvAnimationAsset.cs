/*
   Created by Vilas Tewari on 2009-08-31.
   
	
*/

using UnityEngine;
using System.Collections;
using System.Xml;

using Hangout.Shared;

namespace Hangout.Client
{
	public class ComplexUvAnimationAsset : Asset
	{
		private readonly ComplexUvAnimation mAnimation;
		
		/*
			Properties
		*/
		public ComplexUvAnimation Animation
		{
			get{ return mAnimation; }
		}

		public ComplexUvAnimationAsset(AssetSubType type, ComplexUvAnimation animation, string displayName, string path, string key)
			: base(type, displayName, path, key)
		{
			mAnimation = animation;
		}
	}
}