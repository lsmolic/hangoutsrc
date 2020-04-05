using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using Hangout.Shared;
using log4net;
using log4net.Config;
using log4net.Appender;

namespace Hangout.Server
{
	/// <summary>
	/// Based on example from http://msdn2.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.aspx
	/// Implements the connection logic for the socket server.  
	/// After accepting a connection, all data read from the client 
	/// is sent back to the client with an additional word, to demonstrate how to manipulate the data. 
	/// The read and echo back to the client pattern is continued until the client disconnects.
	/// </summary>
	public class SocketListener
	{
		private Socket mListenSocket;
		
        private Int32 mNumConnectedSockets;
        public Int32 NumConnectedSockets
        {
            get { return mNumConnectedSockets; }
        }

		private Int32 mNumConnections;
		private ConnectionManager mConnectionManager;
		private ILog mLogger;
        private int mRestrictToThreadId;

		/// <summary>
		/// Occurs when a message is received from a remote client.
		/// </summary>
		public event MessageReceivedEventHandler MessageReceivedEvent;

		/// <summary>
		/// Create an uninitialized server instance.  
		/// To start the server listening for connection requests
		/// call the Init method followed by Start method.
		/// </summary>
		/// <param name="numConnections">Maximum number of connections to be handled simultaneously.</param>
		/// <param name="receiveBufferSize">Buffer size to use for each socket I/O operation.</param>
		public SocketListener(Int32 numConnections, ConnectionManager connectionMngr, log4net.ILog logger)
		{
			mLogger = logger;
			mNumConnectedSockets = 0;
			mNumConnections = numConnections;
			mConnectionManager = connectionMngr;

			FileInfo log4netConfigFile = new FileInfo(log4net.Util.SystemInfo.ApplicationBaseDirectory + "/StateServerConsole.exe.config");
			log4net.Config.XmlConfigurator.ConfigureAndWatch(log4netConfigFile);

			mLogger = LogManager.GetLogger("GeneralUse");
            mRestrictToThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
		}

		/// <summary>
		/// Starts the server such that it is listening for incoming connection requests.    
		/// This should be run on the main thread
		/// </summary>
		/// <param name="localEndPoint">The endpoint which the server will listening for connection requests on.</param>
		public void Start(Int32 port)
		{
            EnforceSingleThread();
			//specify "any" ip address because we don't know what ip address the server will be running at
			IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);

			try
			{
				// Create the socket which listens for incoming connections.
				mListenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                mListenSocket.Blocking = false;
			}
			catch (SocketException socketEx)
			{
				throw new Exception("SocketListener::Start - " + socketEx);
			}
			try
			{
				if (localEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
				{
					// Set dual-mode (IPv4 & IPv6) for the socket listener.
					// 27 is equivalent to IPV6_V6ONLY socket option in the winsock snippet below,
					// based on http://blogs.msdn.com/wndp/archive/2006/10/24/creating-ip-agnostic-applications-part-2-dual-mode-sockets.aspx
					mListenSocket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
					localEndPoint = new IPEndPoint(IPAddress.IPv6Any, localEndPoint.Port);
				}
				// Associate the socket with the local endpoint.
				mListenSocket.Bind(localEndPoint);
			}
			catch (Exception argumentEx)
			{
				throw argumentEx;
			}

			mLogger.Info("SocketListener::Start -  IP: " + localEndPoint.Address.ToString() + ":" + localEndPoint.Port + " bound and listening for connections");
			// Start the server with a listen backlog of 100 connections.
			mListenSocket.Listen(100);

			if(MessageReceivedEvent == null)
			{
				mLogger.Error("SocketListener requires an event to handle the receiving of messages.  For some reason, it is null.  No good.");
				throw new System.Exception("SocketListener requires an event to handle the receiving of messages.  For some reason, it is null.  No good.");
			}
		}


		/// <summary>
		/// Begins an operation to accept a connection request from the client.
		/// this currently has the potential to be called both from the main thread (on initial startup) and from another thread (when processing subsequent connections)
		/// </summary>s
		/// <param name="acceptEventArg">The context object to use when issuing 
		/// the accept operation on the server's listening socket.</param>
		public void TryAcceptSingleConnection()
		{
            EnforceSingleThread();
			if (mNumConnectedSockets < mNumConnections)
			{
                // Try an accept only if we have a pending read or error condition
                if (mListenSocket.Poll(0, SelectMode.SelectRead) || mListenSocket.Poll(0, SelectMode.SelectError))
                {
                    try
                    {
                        Socket s = mListenSocket.Accept();
                        ProcessNewClientConnection(s);
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.WouldBlock)
                        {
                            // We probably shouldn't hit this case unless something weird happens between Poll and Accept
                            mLogger.Warn("Got a WOULDBLOCK error on Accept when Poll should have protected us.  Investigate this.");
                            return;
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                }
			}
		}

		/// <summary>
		/// Process the accept for the socket listener.
		/// </summary>
		/// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>
		private void ProcessNewClientConnection( Socket connectingClientSocket) /*SocketAsyncEventArgs e)*/
		{
            EnforceSingleThread();

            Connection connection = null;

			++mNumConnectedSockets;

			mLogger.Info("Client connected    (" + ((IPEndPoint)connectingClientSocket.RemoteEndPoint).Address + ").     Total concurrent: " + mNumConnectedSockets);
			Metrics.Log(LogGlobals.CATEGORY_SERVER_STATS, LogGlobals.CONCURRENCY, "Connections", mNumConnectedSockets.ToString(), "");

			Guid sessionId = Guid.NewGuid();
			connection = new Connection(connectingClientSocket, sessionId, CloseClientSocket);

			mConnectionManager.AddConnection(connection);
		}

		/// <summary>
		/// Close the socket associated with the client.
		/// </summary>
		/// <param name="e">SocketAsyncEventArg associated with the completed send/receive operation.</param>
		private void CloseClientSocket(Connection socketConnection)
		{
            EnforceSingleThread();

			Guid disconnectedSessionId;
			bool foundSessionIdToDisconnect = mConnectionManager.GetSessionIdFromConnection(socketConnection, out disconnectedSessionId);
			if (foundSessionIdToDisconnect)
			{
				MemoryStream disconnectMessageMemoryStream = CreateDisconnectMessage();
				disconnectMessageMemoryStream.Position = 0;

                ConnectionEventArgs connectionEventArgs = new ConnectionEventArgs(disconnectMessageMemoryStream, disconnectedSessionId, DateTime.Now.ToFileTime());
				MessageReceivedEvent(connectionEventArgs);
			}
			else
			{
				mLogger.Error("Error.. trying to shut a socket connection that doesn't have a session id guid associated with it.");
			}
			mConnectionManager.RemoveConnection(socketConnection);
			--mNumConnectedSockets;
 
			mLogger.Info("Client disconnected (" + socketConnection.LastKnownIP + ").     Total concurrent: " + mNumConnectedSockets);
		}

		private MemoryStream CreateDisconnectMessage()
		{
			MemoryStream disconnectMessagePacket = new MemoryStream();
			//connect message injected into stream headed toward the stateserver

			Message disconnectMessage = new Message();

			disconnectMessage.DisconnectMessage();

			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(disconnectMessagePacket, disconnectMessage);

			disconnectMessagePacket.Position = 0;
			return disconnectMessagePacket;
		}

        // Call this to make sure the thread of execution is the same as the one that constructed this SocketListener
        private void EnforceSingleThread()
        {
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != mRestrictToThreadId)
            {
                throw new Exception("Calling single-threaded method from non-allowed thread!");
            }
        }
	}
}


