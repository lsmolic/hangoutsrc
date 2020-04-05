using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using log4net.Config;

namespace Hangout.Server.WebServices
{
    public class Logger
    {
        private ILog logger;

        protected void SetLogger(string loggerName)
        {
            XmlConfigurator.Configure();
            logger = LogManager.GetLogger(loggerName);
        }

        protected void SetLogger(Type loggerType)
        {
            XmlConfigurator.Configure();
            logger = LogManager.GetLogger(loggerType);
        }

        protected void logDebug(string method, string value)
        {
            logger.Debug(String.Format("{0} {1}", method, value));
        }

        protected void logError(string method, Exception ex)
        {
            logger.Error(String.Format("Error {0} ", method), ex);
        }

        protected void logError(string method, string error)
        {
            logger.Error(String.Format("Error {0} {1} ", method, error));
        }
    }
}