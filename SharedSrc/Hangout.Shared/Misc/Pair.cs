/**  --------------------------------------------------------  *
 *   Pair.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/22/2009
 *	 
 *   --------------------------------------------------------  *
 */


using System;
using System.Collections.Generic;

namespace Hangout.Shared
{
	/// <summary>
	/// This is a 2-tuple. Similar in usage to a KeyValuePair, without any special meaning to the first or second values.
	/// </summary>
	/// <typeparam name="T">Pair.First type</typeparam>
	/// <typeparam name="U">Pair.Second type</typeparam>
	[Serializable]
	public class Pair<T, U>
	{
		private T mFirst;
		private U mSecond;

		public T First
		{
			get { return mFirst; }
			set { mFirst = value; }
		}
		public U Second
		{
			get { return mSecond; }
			set { mSecond = value; }
		}

		public Pair(T first, U second)
		{
			mFirst = first;
			mSecond = second;
		}

		public Pair()
		{
			mFirst = default(T);
			mSecond = default(U);
		}

		public static implicit operator KeyValuePair<T, U>(Pair<T, U> pair)
		{
			return new KeyValuePair<T, U>(pair.First, pair.Second);
		}
	}


	/// <summary>
	/// This is a 2-tuple. Similar in usage to a KeyValuePair, without any special meaning to the first or second values.
	/// </summary>
	/// <typeparam name="T">Pair.First and Pair.Second type</typeparam>
	[Serializable]
	public class Pair<T> : Pair<T, T>
	{
		public Pair()
			: base()
		{
		}
		public Pair(T first, T second)
			: base(first, second)
		{
		}
	}
}