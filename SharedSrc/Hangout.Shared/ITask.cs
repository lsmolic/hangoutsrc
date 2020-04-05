/**  --------------------------------------------------------  *
 *   ITask.cs  
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
	public interface ITask : IReceipt
	{
		bool IsRunning { get; }
		void AddOnExitAction(Hangout.Shared.Action onExit);
		void ForceExit();
	}
}
