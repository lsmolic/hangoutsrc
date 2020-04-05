/**  --------------------------------------------------------  *
 *   MiscDelegates.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 05/15/2009
 *	 
 *   --------------------------------------------------------  *
 */


using System;

namespace Hangout.Shared
{
	public delegate void Action();
	// Action<T> is defined in the system library
	public delegate void Action<T, U>(T argument1, U argument2);
	public delegate void Action<T, U, V>(T argument1, U argument2, V argument3);
	public delegate void Action<T, U, V, W>(T argument1, U argument2, V argument3, W argument4);
	
	public delegate T Func<T>();
	public delegate T Func<T, U>(U argument);
	public delegate T Func<T, U, V>(U argument1, V argument2);
}