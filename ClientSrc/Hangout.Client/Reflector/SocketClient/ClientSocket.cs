using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Hangout.Shared;

namespace Hangout.Client.SocketClient
{
    internal class ClientSocket
    {
		// 2147483647 is 2^31 - 1... the largest value for a signed int
        const int mMaxMessageSize = 2147483647;

        private Socket mClientSocket = null ;
		private bool mAreWeConnected = false;
		private bool mIsConnecting = false;
        private Packetizer mPacketizer;
        private int mRestrictToThreadId;

        byte[] mReadBuffer = new byte[1048576];
        MemoryStream mWriteBuffer = new MemoryStream();

		public ClientSocket()
		{
            mPacketizer = new Packetizer();
            mRestrictToThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
		}
      
        /// <summary>
        /// Returns the state of client socket connection to the server.
        /// 1 = connect; 0 = mConnectInProgress; -1 = not conneted
        /// </summary>
        public int IsConnected
        {
            get
            {
				if (mAreWeConnected)
					return 1;
				else if (mIsConnecting)
					return 0;
				else
					return -1;
            }
        }

        /// <summary>
        /// Gets my ipaddress
        /// </summary>
        public string ClientIpAddress
        {
            get
            {
                string ipAddress = "";
                if (mClientSocket != null)
                {
                    ipAddress = ((IPEndPoint)mClientSocket.LocalEndPoint).Address.ToString();
                }
                return ipAddress.ToString();               
            }
        }

        /// <summary>
        /// Opens a socket connection to the Server.
        /// <param name="serverEndPoint">The endpoint to connect to</para>
        /// </summary>
        /// <returns>Returns true if connections successful and false if connection failed</returns>
		public bool ConnectToServer(string address, int port)
		{
            EnforceSingleThread();
			try
			{
				if (!mIsConnecting && !mAreWeConnected)
				{
					mIsConnecting = true;
					mClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

					IPAddress ipaddress = IPAddress.Parse(address);
					mClientSocket.Connect(ipaddress, port);

                    mClientSocket.Blocking = false;

					bool socketConnected = mClientSocket.Connected;
					if (socketConnected)
					{
						mIsConnecting = false;
						mAreWeConnected = true;
					}
					else
					{
						Disconnect();
					}
					return socketConnected;
				}
				return false;
			}
			catch (System.Exception ex)
			{
				Disconnect();
				return false;
			}
		}

        /// <summary>
        /// Closes a socket connection to the Server.
        /// </summary>
        public void Disconnect()
        {
            EnforceSingleThread();
			mAreWeConnected = false;
			mIsConnecting = false;

			if (mClientSocket != null)
			{
				try
				{
					try
					{
						mClientSocket.Shutdown(SocketShutdown.Both);
					}
					catch (System.Exception ex)
					{
						Console.WriteLine("Exception caught while shutting down the socket (in most cases this just means the socket was disconnected while trying to call shutdown): " + ex);
					}
					mClientSocket.Close();
				}
				catch (System.Exception ex)
				{
					throw new System.Exception("Something terrible terrible happened when disconnecting the socket", ex);
				}
			}
			else
			{
				throw new System.Exception("Client socket is null");
			}
        }

		/// <summary>
		/// Receives a stream of data from the server.  This is NOT a blocking call.  Writes the data to mBufferStream.
		/// </summary>
		/// <returns>Returns true if packet received and false if no data was available (due to either empty network buffer or error).</returns>
		public bool ReceiveDataNonBlocking()
		{
            EnforceSingleThread();

			if (mClientSocket == null || !mClientSocket.Connected)
			{
                Console.WriteLine("Discovered when trying to do a read that our socket was null or disconnected.", LogLevel.Error);
				Disconnect();
				return false;
			}
			else if (!mClientSocket.Poll(0, SelectMode.SelectRead))
			{
				return false;
			}

			int numOfBytes = 0;
            SocketError err = SocketError.Success;

            numOfBytes = mClientSocket.Receive(mReadBuffer, 0, mReadBuffer.Length, SocketFlags.None, out err);

            switch (err)
            {
                case SocketError.Success:
                    break;
                case SocketError.WouldBlock:
                    Console.WriteLine("TryReceive got SocketError.WouldBlock after testing with Poll.  Investigate this.", LogLevel.Error);
                    return false;
                case SocketError.ConnectionReset:
                case SocketError.Disconnecting:
                case SocketError.ConnectionAborted:
					Console.WriteLine("Clean disconnect (ConnectionReset/ConnectionAborted/Disconnecting error).", LogLevel.Info);
                    Disconnect();
                    return false;
                default:
					Console.WriteLine("Unexpected SocketError in Receive.  Disconnecting client: " + err, LogLevel.Error);
                    Disconnect();
                    return false;
            }

			if (numOfBytes == 0)
			{
				Console.WriteLine("Clean disconnect.", LogLevel.Info);
                Disconnect();
                return false;
			}

            mPacketizer.AppendBytes(mReadBuffer, 0, numOfBytes);

			return true;
		}


        public IEnumerable<MemoryStream> GetPackets()
        {
            EnforceSingleThread();
            return mPacketizer.GetPackets();
        }


        // Queues up a send in the secondary outgoing buffer for later transmission
        public bool SendPacket(MemoryStream packet)
        {
            EnforceSingleThread();

            long prevPos = mWriteBuffer.Position;

            mWriteBuffer.Position = mWriteBuffer.Length;

            if (packet.Length > 16383)
            {
				Console.WriteLine("Invalid dataLength in SendPacket: " + packet.Length, LogLevel.Error);
                throw new ArgumentOutOfRangeException("Invalid dataLength in SendPacket: " + packet.Length);
            }

            byte[] packetHeader = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int32)packet.Length));
            mWriteBuffer.Write(packetHeader, 0, packetHeader.Length);
            packet.WriteTo(mWriteBuffer);

            // Don't worry about keeping buffer size limited on the client; 
            // whenever it all gets sent we'll shrink it to zero
            mWriteBuffer.Position = prevPos;

            return FlushBufferedSends();
        }

        // Attempts to send all messages sitting in the outgoing buffer
        private bool FlushBufferedSends()
        {
            EnforceSingleThread();

            try
            {
                byte[] data = mWriteBuffer.GetBuffer();
                int dataLen = (int)(mWriteBuffer.Length - mWriteBuffer.Position);
                int bytesSent = 0;

                if (!mClientSocket.Poll(0, SelectMode.SelectWrite))
                {
                    throw new Exception("Unable to write to mClientSocket in SendPacket.  Something broke.");
                }

                bytesSent = mClientSocket.Send(data, (int)mWriteBuffer.Position, dataLen, SocketFlags.None);

                if (bytesSent < dataLen)
                {
                    // We have bytes left over
                    mWriteBuffer.Position = mWriteBuffer.Position + bytesSent;
                }
                else
                {
                    // We sent everything, clear the outgoing buffer
                    mWriteBuffer.SetLength(0);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Disconnect();
				Console.WriteLine("Exception in SendPacket. Disconnecting. " + ex);
                return false;
            }
        }

        // Call this to make sure the thread of execution is the same as the one that constructed this object
        private void EnforceSingleThread()
        {
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != mRestrictToThreadId)
            {
                throw new Exception("Calling single-threaded method from non-allowed thread!");
            }
        }
    }
}
