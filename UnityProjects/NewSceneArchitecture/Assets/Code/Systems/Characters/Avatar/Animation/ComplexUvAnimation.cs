/*
   Created by Vilas Tewari on 2009-08-27.

	A ComplexUvAnimation is an Array of complexFrames
	A ComplexFrame contains UvAnimations and other parameters
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using Hangout.Shared;

namespace Hangout.Client
{
	public class ComplexUvAnimation : UvAnimation
	{
		private readonly ComplexFrame[] mFrames;
		
		/*
			Properties
		*/
		public int ComplexFrameCount
		{
			get{ return mFrames.Length; }
		}
	 	public ComplexFrame[] Frames
	 	{
	 		get{ return mFrames; }
	 	}
		
		public ComplexUvAnimation( string name, ComplexFrame[] frames )
		: base(name)
		{
			mFrames = frames;
		}
	}
}