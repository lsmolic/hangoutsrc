/**  --------------------------------------------------------  *
 *   IYieldInstruction.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/10/2009
 *	 
 *   --------------------------------------------------------  *
 */


using System;

namespace Hangout.Shared
{
	public interface IYieldInstruction
	{
		bool Ready { get; }
	}
}