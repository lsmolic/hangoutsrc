/**  --------------------------------------------------------  *
 *   StateMachine.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/20/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

using PureMVC.Patterns;
using PureMVC.Interfaces;

namespace Hangout.Client
{
	public class StateMachine : Mediator, IStateMachine
	{
		protected IState mCurrentState = null;
		public IState CurrentState 
		{ 
			get { return mCurrentState; }
		}
		
		public void EnterInitialState(IState initialState)
		{
			mCurrentState = initialState;
			mCurrentState.StateMachine = this;
			this.CurrentState.EnterState();
		}

		public IState TransitionToState(IState state)
		{
			return TransitionToState(state.Name);
		}
		
		public virtual IState TransitionToState(string stateName)
		{
			IState requestedState = null;

			if( mCurrentState == null )
			{
				throw new InvalidOperationException("Cannot TransitionToState on a StateMachine that hasn't entered it's initial state.");
			}
			
			if( !mCurrentState.CanGoToState(stateName, out requestedState) )
			{
				throw new Exception("State (" + mCurrentState.Name + ") does not contain a transition to State (" + stateName + ")");	
			}
			
			// Exit current state
			SendNotification(GameFacade.EXIT_STATE, mCurrentState.Name);
			mCurrentState.ExitState();
			mCurrentState.StateMachine = null;
			
			// Enter new state
			mCurrentState = requestedState;
			mCurrentState.StateMachine = this;
			mCurrentState.EnterState();
			SendNotification(GameFacade.ENTER_STATE, mCurrentState.Name);
			
			return mCurrentState;
		}
	}	
}
