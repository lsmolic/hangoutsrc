using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Hangout.Client.SocketClient
{

    public delegate void ConnectingFailedEventHandler(object sender, EventArgs e);
    public delegate void ConnectingSuccessedEventHandler(object sender, EventArgs e);
    public delegate void DisconnectEventHandler(object sender, EventArgs e);
    public delegate void MessageReceivedEvent(object sender, ConnectionEventArgs e);
    public delegate void MessageSendSucessEventHandler(object sender, EventArgs e);
    public delegate void MessageSendFailedEventHandler(object sender, EventArgs e);
    public delegate void MessageSendBusyEventHandler(object sender, EventArgs e);
    public delegate void NetworkAliveEventHandler(object sender, EventArgs e);
    
    /// <summary>
    /// Occurs when a command received from a client.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">The received command object.</param>
    public delegate void MessageReceivedEventHandler(object sender, ConnectionEventArgs e);

    /// <summary>
    /// The class that contains the connection event args
    /// </summary>
    public class ConnectionEventArgs : EventArgs
    {
        private MemoryStream messagePacket;
        private int senderId;

        /// <summary>
        /// The received command.
        /// </summary>
        public MemoryStream MessagePacket
        {
            get { return messagePacket; }
        }

        /// <summary>
        /// The Sender id.
        /// </summary>
        public int SenderId
        {
            get { return senderId; }
        }

        /// <summary>
        /// Creates an instance of CommandEventArgs class.
        /// </summary>
        /// <param name="messagePacket">The received packet.</param>
        ///<param name="senderId">The senderID.</param>
        public ConnectionEventArgs(MemoryStream messagePacket, int senderId)
        {
            this.messagePacket = messagePacket;
            this.senderId = senderId;
        }
    }
}
