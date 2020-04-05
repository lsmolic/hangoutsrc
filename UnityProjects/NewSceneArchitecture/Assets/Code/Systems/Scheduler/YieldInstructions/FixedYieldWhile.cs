/**  --------------------------------------------------------  *
 *   YieldFixedYieldWhileWhile.cs
 *
 *   Author: Pherg
 *   Date: 10/07/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	public class FixedYieldWhile : IYieldInstruction
	{
		private readonly Hangout.Shared.Func<bool> mCondition;

		public FixedYieldWhile(Hangout.Shared.Func<bool> condition)
		{
			if( condition == null )
			{
				throw new ArgumentNullException("condition");
			}
			mCondition = condition;
		}
		
		public bool Ready
		{
			get { return !mCondition(); }
		}
	}	
}
