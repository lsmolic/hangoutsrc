using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using log4net.Config;
using System.IO;
using Hangout.Server;

namespace Hangout.Server.WebServices
{
    public class ServicesLog : Log4netHandler
    {
        public ServicesLog( string serviceName )
        {
            FileInfo fileInfo = new FileInfo(log4net.Util.SystemInfo.ApplicationBaseDirectory + "/Web.config");
            base.Initialize(serviceName, fileInfo);
        }
    }
}
