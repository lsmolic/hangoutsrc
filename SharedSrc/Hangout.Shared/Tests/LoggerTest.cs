using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using Hangout.Shared.UnitTest;

namespace Hangout.Shared
{
    [TestFixture]
    public class LoggerTest
    {
        private class TestReporter : ILogReporter
        {
            private readonly List<string> mInfoLogs = new List<string>();
            public List<string> InfoLogs
            {
                get { return mInfoLogs; }
            }

            public string Info
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string info in mInfoLogs)
                    {
                        sb.Append(info);
                    }
                    return sb.ToString();
                }
            }

            private readonly List<string> mErrorLogs = new List<string>();
            public List<string> ErrorLogs
            {
                get { return mErrorLogs; }
            }

            public void Report(ILogMessage message)
            {
                if (message.Level == LogLevel.Info)
                {
                    mInfoLogs.Add(message.Message);
                }
                else if (message.Level == LogLevel.Error)
                {
                    mErrorLogs.Add(message.Message);
                }
            }
        }

        private readonly Logger mLogger;
        private readonly TestReporter mReporter;
        public LoggerTest()
        {
            mLogger = new Logger();
            mReporter = new TestReporter();
            mLogger.AddReporter(mReporter);
        }

        [Test]
        public void LoggerWorksOnMultipleThreads()
        {
            uint threadCount = 5;
            uint threadIterations = 3;
            ThreadStart loggingThread = new ThreadStart(delegate()
            {
                for (int i = 0; i < threadIterations; ++i)
                {
                    mLogger.Log("Hey!");
                    mLogger.Log("Hey Error!", LogLevel.Error);
                }
            });

            List<Thread> threads = new List<Thread>();

            for (uint i = 0; i < threadCount; ++i)
            {
                Thread thread = new Thread(loggingThread);
                threads.Add(thread);
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            mLogger.Flush();

            Assert.AreEqual((long)(threadCount * threadIterations), (long)mReporter.InfoLogs.Count);
            Assert.AreEqual((long)(threadCount * threadIterations), (long)mReporter.ErrorLogs.Count);
        }
    }
}