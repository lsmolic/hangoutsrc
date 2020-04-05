using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Hangout.Server
{
    /// <summary>
    /// Occurs when a command received from a client.
    /// </summary>
	public delegate void MessageReceivedEventHandler(ConnectionEventArgs connectionEventArgs);

    /// <summary>
    /// The class that contains the connection event args
    /// </summary>
    public class ConnectionEventArgs : EventArgs
    {

        //TODO: how should we handle null data here? meeting?
        private MemoryStream mMessagePacket;
        private Guid mSenderId;
        private List<Guid> mRecipients;
        private long mTimeReceived;

        /// <summary>
        /// The received command.
        /// </summary>
        public MemoryStream MessagePacket
        {
            get { return mMessagePacket; }   
        }
        
        /// <summary>
        /// The Sender id.
        /// </summary>
        public Guid SenderId
        {
            get { return mSenderId; }
        }


        /// <summary>
        /// List of recipients.
        /// </summary>
        public List<Guid> Recipients
        {
            get { return mRecipients; }
        }

        /// <summary>
        /// Time Received
        /// </summary>
        public long TimeReceived
        {
            get { return mTimeReceived; }
        }

        /// <summary>
        /// Creates an instance of CommandEventArgs class.
        /// </summary>
        /// <param name="packet">The received command.</param>
        public ConnectionEventArgs(MemoryStream messagePacket, Guid senderId, long timeReceived)
        {
            mMessagePacket = messagePacket;
            mSenderId = senderId;
            mTimeReceived = timeReceived;
        }

        /// <summary>
        /// Creates an instance of CommandEventArgs class.
        /// </summary>
        /// <param name="packet">The received command.</param>
        public ConnectionEventArgs(MemoryStream messagePacket, List<Guid> recipients, long timeReceived)
        {
            mMessagePacket = messagePacket;
            mRecipients = recipients;
            mTimeReceived = timeReceived;
        }
    

        /// <summary>
        /// Creates an instance of CommandEventArgs class.
        /// </summary>
        /// <param name="packet">The received command.</param>
        public ConnectionEventArgs(MemoryStream messagePacket, Guid senderId, List<Guid> recipients, long timeReceived)
        {
            mMessagePacket = messagePacket;
            mRecipients = recipients;
            mSenderId = senderId;
            mTimeReceived = timeReceived;
        }

    }   
}
