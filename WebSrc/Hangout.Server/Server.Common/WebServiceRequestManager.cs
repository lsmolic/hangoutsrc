using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using System.Xml;

namespace Hangout.Server
{
	public class WebServiceRequestManager : IServerLoopWorker
	{
		private ConcurrentQueue<Pair<Action<XmlDocument>, XmlDocument>> mCallbackQueue = new ConcurrentQueue<Pair<Action<XmlDocument>, XmlDocument>>();

		public void DoWork()
		{
			Pair<Action<XmlDocument>, XmlDocument> serviceResult = null;
			if (mCallbackQueue.Dequeue(out serviceResult))
			{
				serviceResult.First(serviceResult.Second);
			}
		}

		public void Enqueue(Action<XmlDocument> callback, XmlDocument xmlDoc)
		{
			mCallbackQueue.Enqueue(new Pair<Action<XmlDocument>, XmlDocument>(callback, xmlDoc));
		}
	}
}
