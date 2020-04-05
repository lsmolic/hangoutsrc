using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
	public interface ILogReporter
	{
		void Report(ILogMessage logMessage);
	}
}
