/**  --------------------------------------------------------  *
 *   Emote.cs
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
	public class Emote
	{
		private readonly RigAnimationName mName;
		public RigAnimationName Name
		{
			get { return mName; }
		}

		private readonly AnimationSequence mSequence;
		public AnimationSequence Sequence 
		{
			get 
			{
				return mSequence;
			}
		}
		
		public Emote(RigAnimationName name, AnimationSequence sequence)
		{
			mName = name;
			
			if( sequence == null )
			{
				throw new ArgumentNullException("sequence");
			}
			mSequence = sequence;
		}
	}	
}
