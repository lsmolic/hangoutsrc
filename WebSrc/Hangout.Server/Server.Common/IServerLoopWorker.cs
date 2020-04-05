using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server
{
	public interface IServerLoopWorker
	{
		void DoWork();
	}
}
