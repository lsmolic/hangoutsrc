/**  --------------------------------------------------------  *
 *   YieldUntilNextFrame.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/15/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections;
using Hangout.Shared;

namespace Hangout.Client
{
	public class YieldUntilNextFrame : IYieldInstruction
	{
		private int mStartFrame = Time.frameCount;
				
		public bool Ready
		{
			get
			{
				return mStartFrame != Time.frameCount;
			}
		}
	}	
}
