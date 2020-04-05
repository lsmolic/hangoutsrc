using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
	public interface ILogMessage
	{
		string Message { get; }
		LogLevel Level { get; }
		DateTime Timestamp { get; }
	}
}
