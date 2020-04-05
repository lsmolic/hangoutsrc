using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Server
{
	public class ServerProcessingLoop
	{
		private List<IServerLoopWorker> mServerLoopWorkers = new List<IServerLoopWorker>();

		public void AddServerLoopWorker(IServerLoopWorker asyncResponseObject)
		{
			mServerLoopWorkers.Add(asyncResponseObject);
		}


		public void StartLoop()
		{
			while (true)
			{
				try
				{
					foreach (IServerLoopWorker serverLoopWorker in mServerLoopWorkers)
					{
						//we need to make sure that objects running in this loop dont 
						// starve the other objects that need to run
						serverLoopWorker.DoWork();
					}
				}
				catch (System.InvalidOperationException)
				{
					//this is to handle the event where someone adds a serverLoopWorker after the start loop has already spun up (from another thread for example)
					// we don't have to do anything here, we just need to not stop running this loop
					// this condition shouldn't happen very often anyway.. and if it does.. we have other things to worry about
				}
				catch(System.Exception e)
				{
					StateServerAssert.Assert(e);
				}

			}
		}
	}
}
