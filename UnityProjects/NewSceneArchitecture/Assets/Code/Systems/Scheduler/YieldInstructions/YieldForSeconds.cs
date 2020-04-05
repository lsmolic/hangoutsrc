/**  --------------------------------------------------------  *
 *   YieldForSeconds.cs
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
	public class YieldForSeconds : IYieldInstruction
	{
		private readonly float mTimeComplete;
		public YieldForSeconds(float seconds)
		{
			mTimeComplete = Time.time + seconds;
		}
		
		public bool Ready
		{
			get 
			{
				return Time.time >= mTimeComplete;
			}
		}
	}	
}
