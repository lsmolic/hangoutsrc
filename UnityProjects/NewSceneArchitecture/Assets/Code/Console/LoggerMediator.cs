using System;
using System.Collections.Generic;

using Hangout.Shared;
using PureMVC.Patterns;

namespace Hangout.Client
{
	public class LoggerMediator: Mediator
	{
		private readonly ILogger mLogger;
		public ILogger Logger
		{
			get { return mLogger; }
		}
		
		public LoggerMediator(ILogger logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			mLogger = logger;
		}
	}
}
