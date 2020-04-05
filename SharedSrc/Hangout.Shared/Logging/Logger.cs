using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
	/// <summary>
	/// This is the default, thread-safe, generic logger. Any reporters registered with this logger will have their Report() called in the same thread that called Flush().
	/// </summary>
	public class Logger : ILogger, IDisposable
	{
		private class ReporterReceipt : IReceipt
		{
			private readonly Hangout.Shared.Action mCleanupReporter;
			public ReporterReceipt(Hangout.Shared.Action cleanupReporter)
			{
				if( cleanupReporter == null )
				{
					throw new ArgumentNullException("cleanupReporter");
				}
				mCleanupReporter = cleanupReporter;
			}

			public void Exit()
			{
				mCleanupReporter();
			}
		}

		private class LogMessage : ILogMessage, ICloneable
		{
			private string mMessage;
			public string Message
			{
				get { return mMessage; }
			}

			private LogLevel mLevel;
			public LogLevel Level
			{
				get { return mLevel; }
			}

			private DateTime mTimestamp;
			public System.DateTime Timestamp
			{
				get { return mTimestamp; }
			}

			public LogMessage(string message, LogLevel level)
			{
				mTimestamp = DateTime.Now;
				mMessage = message;
				mLevel = level;
			}
			
			public LogMessage(LogMessage copy)
			{
				mTimestamp = copy.Timestamp;
				mMessage = copy.Message;
				mLevel = copy.Level;
			}
			
			public object Clone()
			{
				return new LogMessage(this);
			}
		}

		public void Log(string message, LogLevel level)
		{
			//lock(mMessageQueueLock)
			//{
				LogMessage newMessage = new LogMessage(message, level);
				mMessageQueue.Enqueue(newMessage);
			//}
		}

		public void Log(string message)
		{
			Log(message, LogLevel.Info);
		}

		public void LogLine(string message)
		{
			this.Log(message + "\n");
		}

		public void LogLine(string message, LogLevel logLevel)
		{
			this.Log(message + "\n", logLevel);
		}

		/// <summary>
		/// Flush actually sends the messages to the Reporters. If this isn't called externally, it will only be called when the Logger is destroyed.
		/// </summary>
		public void Flush()
		{
            //List<LogMessage> messageQueue;
            //lock(mMessageQueueLock)
            //{
            //    messageQueue = mMessageQueue;
            //    mMessageQueue = new List<LogMessage>();
            //}

            //messageQueue.Sort(delegate(LogMessage a, LogMessage b)
            //{
            //    return a.Timestamp.CompareTo(b.Timestamp);
            //});

            //lock(mReportersLock)
            //{
            //    foreach (LogMessage message in messageQueue)
            //    {
            //        foreach (ILogReporter reporter in mReporters)
            //        {
            //            reporter.Report(message);
            //        }
            //    }
            //}
            LogMessage message = null;
            while (mMessageQueue.Dequeue(out message))
            {
                foreach (ILogReporter reporter in mReporters)
                {
                    reporter.Report(message);
                }
            }
		}

		private bool mDisposed = false;

		public Logger()
		{
		}

		public Logger(ILogReporter initialReporter)
		{
			AddReporter(initialReporter);
		}

		~Logger()
		{
			Dispose(true);
		}

		public void Dispose()
		{
			Dispose(false);
		}

		private void Dispose(bool gc)
		{
			if (!mDisposed)
			{
				Flush();
				mDisposed = true;
			}
		}

		private readonly object mMessageQueueLock = new object();
		//private List<LogMessage> mMessageQueue = new List<LogMessage>();
        private ConcurrentQueue<LogMessage> mMessageQueue = new ConcurrentQueue<LogMessage>();

		private readonly object mReportersLock = new object();
		private readonly List<ILogReporter> mReporters = new List<ILogReporter>();
		public IReceipt AddReporter(ILogReporter reporter)
		{
			lock(mReportersLock)
			{
				mReporters.Add(reporter);
			}

			return new ReporterReceipt(delegate()
			{
				lock (mReportersLock)
				{
					mReporters.Remove(reporter);
				}
			});
		}
	}
}
