/**  --------------------------------------------------------  *
 *   IState.cs  
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
	public interface IState : IDisposable
	{
		string Name { get; }
		
		/**
		 * If this state is the CurrentState of a state machine, it will be set in this variable. 
		 * The IStateMachine is responsible for settings this, all the StateMachines that derive 
		 *  from StateMachine automatically do this.
		 */
		IStateMachine StateMachine { get; set; }
		
		bool AddTransition(IState stateToTransitionTo);
		
	 	bool CanGoToState(string stateName, out IState requestedState);
	 	bool CanGoToState(string stateName);
		
		void EnterState();
		
		void ExitState();
	}	
}
