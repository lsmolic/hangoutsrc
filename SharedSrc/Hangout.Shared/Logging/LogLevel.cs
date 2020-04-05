using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
	/// <summary>
	/// Used to filter out different logging messages. Used by Loggers that implement Hangout.Shared.ILog.
	/// </summary>
	public enum LogLevel
	{
		Info, 
		Error
	}
}
