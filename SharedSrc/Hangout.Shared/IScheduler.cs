/**  --------------------------------------------------------  *
 *   IScheduler.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 06/10/2009
 *	 
 *   --------------------------------------------------------  *
 */


using System;
using System.Collections.Generic;

namespace Hangout.Shared
{
	public interface IScheduler 
	{	
		ITask StartCoroutine( IEnumerator<IYieldInstruction> task );
	}	
}
