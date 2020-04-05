/**  --------------------------------------------------------  *
 *   Converters.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 10/28/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Collections.Generic;

namespace Hangout.Shared
{
	public static class Converters
	{
		/// <summary>
		/// Converts an Enumerable of Source_T to an Array of type Result_T
		/// </summary>
		public static Result_T[] Array<Source_T, Result_T>(IEnumerable<Source_T> source, Func<Result_T, Source_T> converter)
		{
			List<Result_T> results = new List<Result_T>();
			foreach (Source_T sourceItem in source)
			{
				results.Add(converter(sourceItem));
			}
			return results.ToArray();
		}
	}	
}
