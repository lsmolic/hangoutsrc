using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using Hangout.Server.StateServer;

namespace Hangout.Server
{
    class Program
    {
        static void Main(string[] args)
        {
#if (DEBUG)
			Console.WriteLine("Running the server in Debug mode!");
#else
			Console.WriteLine("Running the server in Release mode!");
#endif

            StateServerConfig.RunGlobalSetup();

            ServerStateMachine serverStateMachine = new ServerStateMachine();
			serverStateMachine.RunForever();
        }
    }
}
