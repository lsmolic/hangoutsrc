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
	[TestFixture]
	public class TaskCollectionTest
	{
		private class MockTask : ITask
		{
			private bool mRunning = true;
			private Hangout.Shared.Action mOnExit;
			public bool IsRunning
			{
				get { return mRunning; }
			}

			public void AddOnExitAction(Hangout.Shared.Action onExit)
			{
				mOnExit += onExit;
			}

			public void Exit()
			{
				if (mRunning)
				{
					mRunning = false;
					if( mOnExit != null )
					{
						mOnExit();
					}
				}
			}
			
			public void ForceExit()
			{
				
			}
		}
		
		[Test]
		public void TaskCollectionCleansCompletedTasksAutomatically()
		{
			TaskCollection testCollection = new TaskCollection();
			ITask testTask = new MockTask();
			testCollection.Add(testTask);

			Assert.AreEqual(1, testCollection.Count);
			testTask.Exit();
			Assert.AreEqual(0, testCollection.Count);
		}

		[Test]
		public void TaskCollectionCleansCompletedTasksAutomaticallyMultipleEntries()
		{
			TaskCollection testCollection = new TaskCollection();
			ITask testTask = new MockTask();
			testCollection.Add(testTask);
			testCollection.Add(testTask);
			testCollection.Add(testTask);

			Assert.AreEqual(3, testCollection.Count);
			testTask.Exit();
			Assert.AreEqual(0, testCollection.Count);
		}

		[Test]
		public void TaskCollectionDisposeVerification()
		{
			TaskCollection testCollection = new TaskCollection();
			
			testCollection.Add(new MockTask());
			testCollection.Add(new MockTask());
			testCollection.Add(new MockTask());
			testCollection.Add(new MockTask());
			testCollection.Add(new MockTask());

			Assert.AreEqual(5, testCollection.Count);
			testCollection.Dispose();

			Assert.AreEqual(0, testCollection.Count);
		}
	}	
}
