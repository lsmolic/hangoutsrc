/**  --------------------------------------------------------  *
 *   DisconnectedState.cs  
 *
 *   Author: Matt, Hangout Industries
 *   Date: 07/08/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client
{
	public class DisconnectedState : State
	{
		public override string Name
		{
			get { return this.GetType().Name; }
		}

		public DisconnectedState()
		{
		}

		public override void EnterState()
		{
		}

		public override void ExitState()
		{
		}
	}	
}
