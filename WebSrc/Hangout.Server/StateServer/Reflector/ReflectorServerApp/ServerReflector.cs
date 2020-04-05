using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using log4net;
using log4net.Config;
using Hangout.Shared;
using log4net.Appender;

namespace Hangout.Server
{
    public class ServerReflector : IServerLoopWorker
    {
        private log4net.ILog mLogger;
        private string mServerIpAddress;
        private int mServerPort;

        private ConcurrentQueue<ConnectionEventArgs> mReceiveMessageQueue = new ConcurrentQueue<ConnectionEventArgs>();

        private ConnectionManager mConnectionManager = new ConnectionManager();

        private SocketListener mSocketListener;

		private Action<Guid> mSessionDisconnectCallback = null;
		private Action<ConnectionEventArgs> mProcessIncomingMessage = null;

		public ServerReflector(Action<Guid> sessionDisconnectCallback, Action<ConnectionEventArgs> processIncomingMessage)
		{
			mSessionDisconnectCallback = sessionDisconnectCallback;
			mProcessIncomingMessage = processIncomingMessage;
            mLogger = LogManager.GetLogger("ServerReflector");
		}

        public string GetClientIPAddress(Guid sessionId)
        {
            string clientIPAddress = "";
            try
            {
                IPAddress clientAddress = mConnectionManager.GetClientIPAddress(sessionId);
                if (clientAddress != IPAddress.None)
                {
                    clientIPAddress = clientAddress.ToString();
                }
            }
            catch(Exception ex)
            {
                mLogger.Error("Error GetClientIPAddress", ex);
            }
            return clientIPAddress;
        }

        public int GetNumConnections()
        {
            return mSocketListener.NumConnectedSockets;
        }

        public void StartServerReflector(string ipAddress, int serverPort)
        {
            // XXX this should not be here
            FileInfo log4netConfigFile = new FileInfo(log4net.Util.SystemInfo.ApplicationBaseDirectory + "/StateServerConsole.exe.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(log4netConfigFile);
            mLogger.Info("Application BaseDirectory: " + log4net.Util.SystemInfo.ApplicationBaseDirectory);
            mLogger.Info("StateServerConsole.exe.config Directory: " + log4netConfigFile.DirectoryName + "\\" + log4netConfigFile.Name);

            mServerPort = serverPort;
            mServerIpAddress = ipAddress;
			mSocketListener = new SocketListener(1000, mConnectionManager, mLogger);
			mSocketListener.MessageReceivedEvent += new MessageReceivedEventHandler(EnqueueMessageFromClient);
			mSocketListener.Start(mServerPort);
		}

		#region Interface_Stuff

			public void DoWork()
			{
				try
				{
					List<ConnectionEventArgs> receivedMessages = null;
					// ReadFromAllConnections is where all the magic happens.
					// It runs Select on all sockets and reads data from any socket that won't block.
					// If there are completed packets, they are placed in receivedMessages.
					// Sleeping happens via Select's timeout.  We no longer sleep blindly.
					// Timeout argument is in MICROseconds, 1000000 microseconds = 1 second
					if (ReadFromAllConnections(out receivedMessages, 100000))
					{
						foreach (ConnectionEventArgs connectionEventArgsMessage in receivedMessages)
						{
							mProcessIncomingMessage(connectionEventArgsMessage);
						}
					}

					// Attempt to process outbound message queues
					SendToAllConnections();

					// Accept one new connection, if available
					// TODO: Move this into the select inside ReadFromAllConnections.
					TryAcceptSingleConnection();
				}
				catch (System.Exception ex)
				{
					StateServerAssert.Assert(ex);
				}
			}

		#endregion

		#region Process_Client_Connections
			public void TryAcceptSingleConnection()
			{
				mSocketListener.TryAcceptSingleConnection();
			}
		#endregion

		#region Message_Send

		/// <summary>
			/// this function should be called from the main thread
			/// </summary>
			/// <param name="serializedMessage"></param>
			/// <param name="sessionIds"></param>
			public void SendMessageToClient(MemoryStream serializedMessage, List<Guid> sessionIds) //object sender, ConnectionEventArgs args)
			{
				try
				{
					ConnectionEventArgs connectionEventArgs = new ConnectionEventArgs(serializedMessage, sessionIds, DateTime.Now.Ticks);

					connectionEventArgs.MessagePacket.Position = 0;
                    mConnectionManager.SendMessage(sessionIds, serializedMessage);

					//mLogger.DebugFormat("SendMessageToClient {0} {1}{2}{3} Ticks R:{4} Ticks S Diff:{5}", cmd.SenderId, cmd.Command, cmd.TargetName, cmd.CommandData, connectionEventArgs.TimeReceived, DateTime.Now.ToFileTime() - connectionEventArgs.TimeReceived);
				}

				catch (Exception ex)
				{
					mLogger.Error("SendMessageToClient", ex);
				}
			}

            public void SendToAllConnections()
            {
                List<Connection> writable, errors;
                mConnectionManager.SelectWritableConnections(out writable, out errors);

                foreach (Connection c in errors)
                {
                    mLogger.Warn("Socket error in SendToAllConnections: " + (SocketError)c.InnerSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error));
                    c.Disconnect();
                    if (writable.Contains(c))
                    {
                        writable.Remove(c);
                    }
                }

                foreach (Connection c in writable)
                {
                    c.SendQueuedPackets();
                }
            }
		#endregion

		#region Message_Receive

			/// This queue remains as a workaround for the Connect message which is constructed in 
            /// SocketListener.  It is no longer used by Connection.
            private void EnqueueMessageFromClient(ConnectionEventArgs connectionEventArgs)
            {
                mReceiveMessageQueue.Enqueue(connectionEventArgs);
            }

			///this function should only be called from the main thread!!!  be careful that no other threads have read / remove access from this queue..
			/// we don't want any race conditions spawning up!
            public bool ReadFromAllConnections(out List<ConnectionEventArgs> receivedMessages, int timeoutMicroseconds)
			{
                List<Connection> readable, errors;
                receivedMessages = new List<ConnectionEventArgs>();

                mConnectionManager.SelectReadableConnections(out readable, out errors, timeoutMicroseconds);

                foreach (Connection c in errors)
                {
                    mLogger.Warn("Socket error in ReadFromAllConnections: " + (SocketError)c.InnerSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error));
                    c.Disconnect();
                    if (readable.Contains(c))
                    {
                        readable.Remove(c);
                    }
                }
                
                List<ConnectionEventArgs> result;
                foreach (Connection c in readable)
                {
                    if (c.TryReceive(out result))
                    {
                        receivedMessages.AddRange(result);
                    }
                }

                ConnectionEventArgs arg = null;
                while (mReceiveMessageQueue.Dequeue(out arg))
                {
                    receivedMessages.Add(arg);
                }

                return (receivedMessages.Count > 0);
			}

		#endregion

		#region Disconnect_Stuff
			public void ForceClientDisconnect(Guid sessionIdToDisconnect)
			{
				if (mConnectionManager.Contains(sessionIdToDisconnect))
				{
					mSessionDisconnectCallback(sessionIdToDisconnect);
					mConnectionManager.RemoveSession(sessionIdToDisconnect);
				}
				else
				{
					mLogger.Error("Error.. trying to disconnect a client that doesn't have a session id guid in the ConnectionHandler.");
				}
			}

			public void DisconnectServer()
			{
				mConnectionManager.DisconnectAllServerConnection();

				GC.Collect();
			}
		#endregion
	}
}
