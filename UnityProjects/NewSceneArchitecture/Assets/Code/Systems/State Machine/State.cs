/*  --------------------------------------------------------  *
 *   State
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 05/21/09
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;
using System;
using System.Collections.Generic;

namespace Hangout.Client
{
	public abstract class State : IState
	{ 
		private readonly IDictionary<string, IState> mTransitions = new Dictionary<string, IState>(); 
		private IStateMachine mStateMachine = null;
		public IStateMachine StateMachine
		{
			get { return mStateMachine; }
			set { mStateMachine = value; }
		}

		public bool AddTransition(IState toState)
		{
			IState existingState;
			bool result = false;
			if( !mTransitions.TryGetValue(toState.Name, out existingState) )
			{
				result = true;
				mTransitions.Add(toState.Name, toState);
			}

			return result;
		}

		public bool RemoveTransition(IState toState)
		{
			return mTransitions.Remove(toState.Name);
		}

		public bool CanGoToState(string stateName, out IState requestedState)
		{
			bool canGo = this.CanGoToState(stateName);
			if( canGo )
			{
				requestedState = mTransitions[stateName];
			}
			else
			{
				requestedState = null;
			}
			
			return canGo;
		}
		
		public bool CanGoToState(string stateName)
		{
			return mTransitions.ContainsKey(stateName);
		}

		public virtual string Name
		{
			get { return this.GetType().Name; }
		}

		public abstract void EnterState();
		public abstract void ExitState();

		public virtual void Dispose()
		{
		}
	}	
}
