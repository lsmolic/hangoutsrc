/**  --------------------------------------------------------  *
 *   LoginState.cs  
 *
 *   Author: Matt, Hangout Industries
 *   Date: 07/08/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Hangout.Client.Gui;

namespace Hangout.Client
{
	public class LoginState : State
	{
		public override string Name
		{
			get { return this.GetType().Name; }
		}

		public override void EnterState()
		{
        }

		public override void ExitState()
		{

            GameFacade.Instance.SendNotification(GameFacade.HIDE_POPUP, null);
        }
	}
	
}

