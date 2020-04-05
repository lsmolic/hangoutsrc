/**  --------------------------------------------------------  *
 *   YieldWhile.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/22/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Shared
{
	public class YieldWhile : IYieldInstruction
	{
		private readonly Hangout.Shared.Func<bool> mCondition;
		
		public YieldWhile(Hangout.Shared.Func<bool> condition)
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
