/**  --------------------------------------------------------  *
 *   TaskCollection.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 11/3/2009
 *	 
 *   --------------------------------------------------------  *
 */


using System;
using System.Collections.Generic;
using Hangout.Shared;

namespace Hangout.Client
{
	/// <summary>
	/// List of tasks that automatically removes completed tasks and can safely dispose any active tasks
	/// </summary>
	public class TaskCollection : ICollection<ITask>, IDisposable
	{
		private readonly List<ITask> mTasks = new List<ITask>();

		public void Dispose()
		{
			while(mTasks.Count != 0)
			{
				mTasks[0].Exit();
			}
		}

		public void Add(ITask item)
		{
			mTasks.Add(item);
			item.AddOnExitAction(delegate()
			{
				mTasks.Remove(item);
			});
		}

		public void Clear()
		{
			mTasks.Clear();
		}

		public bool Contains(ITask item)
		{
			return mTasks.Contains(item);
		}

		public void CopyTo(ITask[] array, int arrayIndex)
		{
			mTasks.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return mTasks.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(ITask item)
		{
			return mTasks.Remove(item);
		}

		public IEnumerator<ITask> GetEnumerator()
		{
			return mTasks.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return mTasks.GetEnumerator();
		}
	}
}
