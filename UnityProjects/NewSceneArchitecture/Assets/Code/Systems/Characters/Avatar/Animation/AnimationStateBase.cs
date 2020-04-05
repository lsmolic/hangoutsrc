/**  --------------------------------------------------------  *
 *   AnimationStateBase.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/22/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace Hangout.Client
{
	public abstract class AnimationStateBase : State
	{
		/// Used by the AnimationStateMachine to automatically switch states when an animation is complete.
		public abstract bool DoneAnimating { get; }
	}	
}
