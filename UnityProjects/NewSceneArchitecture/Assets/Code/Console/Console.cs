using System;
using System.Collections.Generic;

using Hangout.Shared;

namespace Hangout.Client
{
	public class Console
	{
		private static Console mInstance = null;
		public static Console Instance
		{
			get
			{
				if( mInstance == null )
				{
					mInstance = new Console();
				}
				return mInstance;
			}
		}
		
		public static void Write(object log)
		{
			if(log == null)
			{
				log = "null";
			}
			Console.Write(log, LogLevel.Info);
		}
		
		public static void WriteLine(object log)
		{
			if(log == null)
			{
				log = "null";
			}
			Console.Write(log.ToString() + "\n", LogLevel.Info);
		}

		public static void WriteLine()
		{
			Console.Write("\n", LogLevel.Info);
		}

		public static void Write(object log, LogLevel level)
		{
			if (log == null)
			{
				log = "null";
			}
			Instance.mLogger.Log(log.ToString(), level);
		}

		public static void WriteLine(object log, LogLevel level)
		{
			if (log == null)
			{
				log = "null";
			}
			Console.Write(log.ToString() + "\n", level);
		}

		public static void Log(object log)
		{
			WriteLine(log);
		}

		public static void LogError(object log)
		{
			WriteLine(log, LogLevel.Error);
		}
		
		private readonly ILogger mLogger;
		private Console()
		{
			// If this Retrieve throws, it means that someone is trying to use the Console.Write functions before the Console is initialized
			mLogger = GameFacade.Instance.RetrieveMediator<LoggerMediator>().Logger;
		}
	}
}
