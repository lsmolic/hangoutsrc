/**  --------------------------------------------------------  *
 *   IStateMachine.cs  
 *
 *   Author: Matt, Hangout Industries
 *   Date: 07/08/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;

namespace Hangout.Client
{
	public interface IStateMachine
	{
		IState TransitionToState(string stateName);
		void EnterInitialState(IState initialState);
	}	
}
