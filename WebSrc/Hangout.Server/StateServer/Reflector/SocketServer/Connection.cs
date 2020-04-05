using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using log4net;
using log4net.Config;
using Hangout.Shared;


namespace Hangout.Server
{
    public class Connection
    {
        private Socket mSocket;
		private int mSocketHandle;
        private Guid mSessionId;
        private ILog mLogger;
        private IPAddress mLastKnownIP = IPAddress.None;
        private Packetizer mPacketizer;
        private ConcurrentQueue<MemoryStream> mPendingSends = new ConcurrentQueue<MemoryStream>();
        private int mRestrictToThreadId;

		private byte[] mReceiveMessageBuffer = new byte[65536];
        private MemoryStream mSendMessageBuffer = new MemoryStream();

        private DateTime mLastMsgRecvTime = DateTime.UtcNow;

		public delegate void CleanupSocketReferencesEventHandler(Connection socketConnection);
		private event CleanupSocketReferencesEventHandler CleanupSocketReferencesEvent;

		public Connection(Socket socket, Guid sessionId, CleanupSocketReferencesEventHandler cleanupSocketReferencesEvent)
        {
            if (!socket.Connected)
            {
                throw new ArgumentException("Trying to initialize a Connection object with a socket that is not connected.");
            }

            mLogger = LogManager.GetLogger("Connection");

            mPacketizer = new Packetizer();

            mSocket = socket;
			mSocketHandle = (int)mSocket.Handle;
			mSessionId = sessionId;
            mLastKnownIP = ((IPEndPoint)mSocket.RemoteEndPoint).Address;
            mSocket.Blocking = false;

			this.CleanupSocketReferencesEvent += new CleanupSocketReferencesEventHandler(cleanupSocketReferencesEvent);

            mRestrictToThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Gets the IP address of connected remote client.This is 'IPAddress.None' if the client is not connected.
        /// </summary>
        public IPAddress IP
        {
            get
            {
                if (mSocket != null && mSocket.Connected)
                     return ((IPEndPoint)mSocket.RemoteEndPoint).Address;
                else
                    return IPAddress.None;
            }
        }

        public IPAddress LastKnownIP
        {
            get
            {
                if (mSocket != null && mSocket.Connected)
                {
                    mLastKnownIP = ((IPEndPoint)mSocket.RemoteEndPoint).Address;
                }
                return mLastKnownIP;
            }
        }

        /// <summary>
        /// Gets the port number of connected remote client.This is -1 if the client is not connected.
        /// </summary>
        public int Port
        {
            get
            {
                if (mSocket != null)
                {
                    return ((IPEndPoint)mSocket.RemoteEndPoint).Port;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// [Gets] The value that specifies the remote client is connected to this server or not.
        /// </summary>
        public bool Connected
        {
            get
            {
                if (mSocket != null)
                {
                    return mSocket.Connected;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// [Gets] The value that specifies the socket Handle
        /// </summary>
        public int SocketId
        {
			get { return mSocketHandle; }
        }

        /// <summary>
        /// The name of remote client.
        /// </summary>
        public Guid SessionId
        {
            get { return mSessionId; }
		}

        public Socket InnerSocket
        {
            get { return mSocket; }
        }

        public DateTime LastMsgRecvTime
        {
            get { return mLastMsgRecvTime; }
        }

		#region Send_Message

        // Thread-safe deferred send.  Call this from anywhere.
        public void SendPacket(MemoryStream packet)
        {
            mPendingSends.Enqueue(packet);
        }

        private void SpoolUnsentPackets()
        {
            EnforceSingleThread();
            MemoryStream packet = null;
            long prevPos = mSendMessageBuffer.Position;
            while(mPendingSends.Dequeue(out packet))
            {
                mSendMessageBuffer.Position = mSendMessageBuffer.Length;

                byte[] packetHeader = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int32)packet.Length));
                mSendMessageBuffer.Write(packetHeader, 0, packetHeader.Length);
                packet.WriteTo(mSendMessageBuffer);

                mSendMessageBuffer.Position = prevPos;
            }
            if (prevPos > 0 && mSendMessageBuffer.Length > 1000) // XXX this wants to be higher for performance but leave it low for bugchecking
            {
                MemoryStream keepMe = new MemoryStream((int)(mSendMessageBuffer.Length - mSendMessageBuffer.Position));
                while (mSendMessageBuffer.Position < mSendMessageBuffer.Length)
                {
                    keepMe.WriteByte((byte)mSendMessageBuffer.ReadByte());
                }
                mSendMessageBuffer.SetLength(0);
                keepMe.WriteTo(mSendMessageBuffer);
                mSendMessageBuffer.Position = 0;
            }
        }

        // Process all deferred sends.  Only call from the main thread!
		public void SendQueuedPackets()
		{
            EnforceSingleThread();
            if (!mSocket.Poll(0, SelectMode.SelectWrite))
            {
                mLogger.Warn("Called SendQueuedPackets when mSocket was not writable.  Check for this earlier.");
                return;
            }

            SpoolUnsentPackets();

            //Uncomment this to see the hex of the packet being sent to the client
            //mLogger.Debug("Connection::SendPacket \n memory stream size: " + packet.Length);
            //byte[] byteArray = packet.ToArray();
            //mLogger.Debug("binary to hex: " + BitConverter.ToString(byteArray));
            //mLogger.Debug("\n");


            int bytesSent = 0;

            byte[] data = mSendMessageBuffer.GetBuffer();
            int dataLen = (int)(mSendMessageBuffer.Length - mSendMessageBuffer.Position);

            try
            {
                SocketError err;
                bytesSent = mSocket.Send(data, (int)mSendMessageBuffer.Position, dataLen, SocketFlags.None, out err);

                if (bytesSent < dataLen)
                {
                    // We have bytes left over
                    mSendMessageBuffer.Position = mSendMessageBuffer.Position + bytesSent;
                }
                else
                {
                    // We sent everything, clear the outgoing buffer
                    mSendMessageBuffer.SetLength(0);
                }

                if (err == SocketError.WouldBlock)
                {
                    // Couldn't send anything, outgoing buffer doesn't change
                    if (bytesSent != 0)
                    {
                        throw new Exception("WouldBlock and bytesSent > 0, this should neverhappen");
                    }
                    return;
                }
                if (err != SocketError.Success)
                {
                    mLogger.Error("Error on socket.Send: " + err);
                    Disconnect();
                }
            }
            catch (System.Exception ex)
            {
                mLogger.Error("Exception caught in Connection::SendQueuedPackets", ex);
                Disconnect();
            }
		}

		#endregion

		#region Receive_Message

			public bool TryReceive(out List<ConnectionEventArgs> receivedMessages)
			{
				Int32 bytesTransferred = 0;
                SocketError err;
                receivedMessages = null;

                EnforceSingleThread();

                if (!mSocket.Poll(0, SelectMode.SelectRead))
                {
                    mLogger.Warn("TryReceive called on socket that was not ready to read, should have checked before this");
                    return false;
                }

                try
                {
                    bytesTransferred = mSocket.Receive(mReceiveMessageBuffer, 0, mReceiveMessageBuffer.Length, SocketFlags.None, out err);
                }
                catch (SocketException ex)
                {
                    mLogger.Error("Unexpected SocketException in mSocket.Receive: " + ex);
                    Disconnect();
                    return false;
                }

                switch (err)
                {
                    case SocketError.Success:
                        break;
                    case SocketError.WouldBlock:
                        mLogger.Warn("TryReceive got SocketError.WouldBlock after testing with Poll.  Investigate this.");
                        return false;
                    case SocketError.ConnectionReset:
                    case SocketError.Disconnecting:
                        Disconnect();
                        return false;
                    default:
                        mLogger.Warn("Unexpected SocketError in Receive.  Disconnecting client: " + err);
                        Disconnect();
                        return false;
                }

				if (bytesTransferred <= 0)
				{
                    // This is a clean disconnect.  Do not panic.
					Disconnect();
					return false;
				}

				// We have data!  Packetize it and return any completed messages.
				try
				{
                    receivedMessages = new List<ConnectionEventArgs>();
                    mPacketizer.AppendBytes(mReceiveMessageBuffer, 0, bytesTransferred);
					foreach (MemoryStream messagePacket in mPacketizer.GetPackets())
					{
                        receivedMessages.Add(new ConnectionEventArgs(messagePacket, SessionId, DateTime.Now.ToFileTime()));
					}
				}
				catch (System.Net.ProtocolViolationException ex)
				{
					mLogger.Warn("Terminating connection due to protocol violation by client at " + IP + ".  Details:\n" + ex);
					Disconnect();
					return false;
				}
				catch (BufferOverflowException ex)
				{
					mLogger.Error("BufferOverflowException reading from client at " + IP + ":\n" + ex + "\n");
					Disconnect();
					return false;
				}

                mLastMsgRecvTime = DateTime.UtcNow;
                return true;
			}
		#endregion

        public void Disconnect()
        {
            EnforceSingleThread();
            // log the current stack trace to see why we're disconnecting
            //System.Diagnostics.StackTrace currentStackTrace = new System.Diagnostics.StackTrace();
            //mLogger.Debug("-- Begin disconnect stackTrace --\n" + currentStackTrace.ToString());
            //mLogger.Debug("-- End disconnect stackTrace --");
			if(mSocket != null)
			{
				try
				{
					mSocket.Shutdown(SocketShutdown.Both);
				}
				catch (System.Net.Sockets.SocketException ex)
				{
                    if (ex.SocketErrorCode != SocketError.NotConnected)
                    {
                        mLogger.Warn("Unexpected socket error on shutdown: " + ex.ToString());
                    }
				}
				mSocket.Close();
				mSocket = null;
				CleanupSocketReferencesEvent(this);
			}
        }

        // Call this to make sure the thread of execution is the same as the one that constructed this Connection
        private void EnforceSingleThread()
        {
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != mRestrictToThreadId)
            {
                throw new Exception("Calling single-threaded method from non-allowed thread!");
            }
        }
    }
}
