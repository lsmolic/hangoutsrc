/**  --------------------------------------------------------  *
 *   AnimationLoopCount.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/20/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace Hangout.Client
{
	public class AnimationLoopCount
	{
		private uint mMin;
		private uint mMax;
		private bool mInfinite;

		public uint Min
		{
			get { return mMin; }
			set { mMin = value; }
		}

		public uint Max
		{
			get { return mMax; }
			set { mMax = value; }
		}

		public bool Infinite
		{
			get { return mInfinite; }
			set { mInfinite = value; }
		}

		public AnimationLoopCount(uint loopCount)
		: this(loopCount, loopCount) {}
		
		public AnimationLoopCount(uint min, uint max)
		{
			mMin = min;
			mMax = max;
			mInfinite = false;
		}
		
		public AnimationLoopCount()
		{
			mInfinite = true;
			mMin = 0;
			mMax = 0;
		}
	}	
}
