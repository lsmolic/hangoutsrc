/**  --------------------------------------------------------  *
 *   DebugLogReporter.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 08/10/2009
 *	 
 *   --------------------------------------------------------  *
 */

using UnityEngine;

using System;

using Hangout.Shared;

namespace Hangout.Client
{
	public class DebugLogReporter : ILogReporter
	{
		public void Report(ILogMessage message)
		{
				switch (message.Level)
				{
					case LogLevel.Info:
						if (!Application.isEditor)
						{
							Debug.Log(message.Message);
						}
						break;
					case LogLevel.Error:
						Debug.LogError(message.Message);
						break;
				}
			
		}
	}
}
