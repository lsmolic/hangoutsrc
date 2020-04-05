/**  --------------------------------------------------------  *
 *   TaskCollectionTest.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 11/03/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;
using System.Collections.Generic;

using Hangout.Client;

namespace Hangout.Shared.UnitTest
{
	internal class MockYieldInstruction : IYieldInstruction
	{
		private bool mStep = false;
		public void Step()
		{
			mStep = true;
		}
		public bool Ready
		{
			get
			{
				bool result = mStep;
				mStep = false;
				return result;
			}
		}
	}

	internal class MockFixedYieldInstruction : MockYieldInstruction, IFixedYieldInstruction
	{
	}

	[TestFixture]
	public class SchedulerTest
	{
		private Scheduler mTestScheduler = new Scheduler();
		private MockYieldInstruction mMockYieldInstruction = new MockYieldInstruction();
		private MockFixedYieldInstruction mMockFixedYieldInstruction = new MockFixedYieldInstruction();

		private IEnumerator<IYieldInstruction> MockCoroutine(IYieldInstruction yieldInstruction, Hangout.Shared.Action onStep)
		{
			while(true)
			{
				yield return yieldInstruction;
				onStep();
			}
		}

		private IEnumerator<IYieldInstruction> MockCoroutineOptionalYield(IYieldInstruction yieldInstruction, Func<bool> onStep)
		{
			while( onStep() )
			{
				yield return yieldInstruction;
			}
		}

		[Test]
		public void TasksGetCalledOnUpdate()
		{
			int calls = 0;
			mTestScheduler.StartCoroutine(MockCoroutine(mMockYieldInstruction, delegate()
			{
				calls++;
			}));

			mMockYieldInstruction.Step();
			mTestScheduler.Update();
			Assert.AreEqual(1, calls);

			calls = 0;
			int iterations = 20;
			for(int i = 0; i < iterations; ++i)
			{
				mMockYieldInstruction.Step();
				mTestScheduler.Update();
			}
			Assert.AreEqual(iterations, calls);
		}


		[Test]
		public void TasksGetCalledOnFixedUpdate()
		{
			int calls = 0;
			mTestScheduler.StartCoroutine(MockCoroutine(mMockFixedYieldInstruction, delegate()
			{
				calls++;
			}));

			mMockFixedYieldInstruction.Step();
			mTestScheduler.FixedUpdate();
			Assert.AreEqual(1, calls);

			calls = 0;
			int iterations = 20;
			for (int i = 0; i < iterations; ++i)
			{
				mMockFixedYieldInstruction.Step();
				mTestScheduler.FixedUpdate();
			}
			Assert.AreEqual(iterations, calls);
		}

		[Test]
		public void TasksThatDontYieldVerification()
		{
			int calls = 0;
			mTestScheduler.StartCoroutine(MockCoroutineOptionalYield(mMockYieldInstruction, delegate()
			{
				calls++;
				return false;
			}));

			mTestScheduler.Update();
			mTestScheduler.Update();

			Assert.AreEqual(1, calls);
		}
	}	
}
