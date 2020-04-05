using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Server
{
	public class CallToServiceCallbackManager : IServerLoopWorker
	{
		private ConcurrentQueue<Pair<Action<string>, string>> mCallbackQueue = new ConcurrentQueue<Pair<Action<string>, string>>();

		public void DoWork()
		{
			Pair<Action<string>, string> serviceResult = null;
			if (mCallbackQueue.Dequeue(out serviceResult))
			{
				serviceResult.First(serviceResult.Second);
			}
		}

		public void Enqueue(Action<string> callback, string callbackArgument)
		{
			mCallbackQueue.Enqueue(new Pair<Action<string>, string>(callback, callbackArgument));
		}
	}
}
