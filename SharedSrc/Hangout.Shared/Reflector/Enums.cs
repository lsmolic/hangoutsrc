using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
    /// <summary>
    /// The type of commands used by the client for the reflector
    /// </summary>
    public enum Command
    {
         /// <summary>
        /// Send a text message to the client or server.
        /// </summary>
        Message,
        /// <summary>
        /// This command will be sent on connect.
        /// </summary>
        Connect,
        /// <summary>
        /// This command will be sent on disconnect.
        /// </summary>
        Disconnect
    }
}
