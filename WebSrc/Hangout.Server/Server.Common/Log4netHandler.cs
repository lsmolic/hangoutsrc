using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using log4net.Config;
using System.IO;

namespace Hangout.Server
{
    //this is a base class for all server side loggers
    public class Log4netHandler
    {
        private ILog mLog;

        public ILog Log
        {
            get { return mLog; }
        }

        protected void Initialize(string serviceName, FileInfo fileInfo)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(fileInfo);

            mLog = LogManager.GetLogger(serviceName);
        }
    }
}
