/**  --------------------------------------------------------  *
 *   YieldTask.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/26/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	public class YieldTask : YieldWhile
	{
		public YieldTask(ITask task)
			: base(delegate()
		{
			return task.IsRunning;
		})
		{
			if( task == null )
			{
				throw new ArgumentNullException("task");
			}
		}
	}	
}
