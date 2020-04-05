/**  --------------------------------------------------------  *
 *   LoadingState.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/21/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace Hangout.Client
{
	public class LoadingState : State
	{
		public override string Name
		{
			get { return "Loading"; }
		}
		
		public override void EnterState()
		{
            //Console.WriteLine("LoadingState.EnterState");
		}
		
		public override void ExitState()
		{
            //Console.WriteLine("LoadingState.ExitState");
		}
		
		public override void Dispose()
		{
			
		}
	}	
}
