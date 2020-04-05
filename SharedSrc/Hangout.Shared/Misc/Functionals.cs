/*
 * Map(func, [a, b, c, d]) -> [func(a), func(b), func(c), func(d)]
 * 
 * Filter(func, [a, b, c, d]) -> [a, c]  (if func(a) == true and func(c) == true)
 * 
 * Reduce(func, [a, b, c, d]) -> func(func(func(a,b),c),d)
 * 
 * 
 * Map - Lazy (Generator).  Does not evaluate func(item) until the item is retrieved from the IEnumerable result.
 * MapImmediate - Allocates and returns an IList result.  func(item) is evaluated immediately for all items.
 * 
 * Filter - Lazy (Generator).  Does not evaluate func(item) until the item is retrieved from the IEnumerable result.
 * FilterImmediate - Allocates and returns an IList result.  func(item) is evaluated immediately for all items.
 * 
 * Reduce - No lazy form, only works with immediate evaluation.
 * 
 */

using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hangout.Shared
{
    public static class Functionals
    {
		/// <summary>
		/// Non-generic form of Map
		/// </summary>
		public static IEnumerable<U> Map<U>(Func<U, object> applyFunc, IEnumerable items)
		{
			foreach (object item in items)
			{
				yield return applyFunc(item);
			}
		}

        public static IEnumerable<U> Map<T, U> (Func<U, T> applyFunc, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                yield return applyFunc(item);
            }
        }

		/// <summary>
		/// Non-generic form of MapImmediate
		/// </summary>
		public static IList<U> MapImmediate<U>(Func<U, object> applyFunc, IEnumerable items)
		{
			List<U> newItems = new List<U>();
			foreach (object item in items)
			{
				newItems.Add(applyFunc(item));
			}
			return newItems;
		}

        public static IList<U> MapImmediate<T, U> (Func<U, T> applyFunc, IEnumerable<T> items)
        {
            List<U> newItems = new List<U> ();
            foreach (T item in items)
            {
                newItems.Add(applyFunc(item));
            }
            return newItems;
        }

        public static IEnumerable<T> Filter<T> (Func<bool, T> filterFunc, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                if (filterFunc(item))
                {
                    yield return item;
                }
            }
        }

        public static IList<T> FilterImmediate<T> (Func<bool, T> filterFunc, IEnumerable<T> items)
        {
            List<T> newItems = new List<T>();
            foreach (T item in items)
            {
                if (filterFunc(item))
                {
                    newItems.Add(item);
                }
            }
            return newItems;
        }

		/// <summary>
		/// If the type T has a parameterless constructor, make a new instance of it, otherwise use default(T)
		/// </summary>
		public static T BuildDefault<T>()
		{
			T result = default(T);
			foreach (ConstructorInfo ci in typeof(T).GetConstructors())
			{
				if (ci.GetParameters().Length == 0)
				{
					result = (T)Activator.CreateInstance(typeof(T));
					break;
				}
			}
			return result;
		}

        public static U Reduce<T, U>(Func<U, U, T> reduceFunc, IEnumerable<T> items)
        {
			return Reduce<T, U>(reduceFunc, items, BuildDefault<U>());
        }

        public static U Reduce<T, U>(Func<U, U, T> reduceFunc, IEnumerable<T> items, U startVal)
        {
            foreach (T item in items)
            {
                startVal = reduceFunc(startVal, item);
            }
            return startVal;
        }

		/// <summary>
		/// Non-generic form of Reduce
		/// </summary>
		public static U Reduce<U>(Func<U, U, object> reduceFunc, IEnumerable items, U startVal)
		{
			foreach (object item in items)
			{
				startVal = reduceFunc(startVal, item);
			}
			return startVal;
		}

		public static U Reduce<U>(Func<U, U, object> reduceFunc, IEnumerable items)
		{
			return Reduce<U>(reduceFunc, items, BuildDefault<U>());
		}
    }
}
