/**  --------------------------------------------------------  *
 *   StateMachineTest.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 07/20/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

using Hangout.Client;

namespace Hangout.Shared.UnitTest
{
	[TestFixture]
	public class StateMachineTest
	{
		private class TestState : State
		{
			private uint mEnterCount = 0;
			private uint mExitCount = 0;
			private readonly string mName;

			public uint EnterCount { get { return mEnterCount; } }
			public uint ExitCount { get { return mExitCount; } }

			public override string Name { get { return mName; } }
			public override void EnterState()
			{
				mEnterCount++;
			}
			public override void ExitState()
			{
				mExitCount++;
			}
			
			public TestState(string name)
			{
				mName = name;
			}
			
			public override void Dispose()
			{

			}
		}
		private TestStateMachine mTestStateMachine = new TestStateMachine(); 
		
		[Test]
		public void SuccessfulStateTransitionsSetCurrentState()
		{
			TestState defaultState = new TestState("DefaultTestState");
			mTestStateMachine.EnterInitialState(defaultState);
			TestState testState = new TestState("TestState01"); 
			
			defaultState.AddTransition(testState);
			testState.AddTransition(defaultState);
			
			mTestStateMachine.TransitionToState(testState.Name);
			mTestStateMachine.TransitionToState(defaultState.Name);
			mTestStateMachine.TransitionToState(testState.Name);
			
			Assert.AreEqual(2u, defaultState.EnterCount);
			Assert.AreEqual(2u, defaultState.ExitCount);

			Assert.AreEqual(2u, testState.EnterCount);
			Assert.AreEqual(1u, testState.ExitCount);
		}
	}	
}
