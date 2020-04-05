/**  --------------------------------------------------------  *
 *   StoreStateMachine.cs  
 *
 *   Author: Pherg, Hangout Industries
 *   Date: 08/19/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

using Hangout.Client.Gui;

namespace Hangout.Client
{

	public class StoreStateMachine : StateMachine
	{
		public new StoreGuiState CurrentState
		{
			get { return GetCurrentStoreGuiState(); }
		}
	
		private StoreGuiState GetCurrentStoreGuiState()
		{
			StoreGuiState currentState = mCurrentState as StoreGuiState;
			if (currentState == null)
			{
				throw new NullReferenceException("currentState cannot be cast to StoreGuiState.");
			}
			return currentState;
		}
	}
}
