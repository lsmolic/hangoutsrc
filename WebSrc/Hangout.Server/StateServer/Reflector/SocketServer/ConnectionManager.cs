using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Sockets;
using log4net;

namespace Hangout.Server
{
    public class ConnectionManager
    {
        //local sessionIds for this state server Dict<sessionId, socketId>
        private Dictionary<Guid, int> mSessionIds = new Dictionary<Guid, int>();

        //local socketIds for this state server
        private Dictionary<int, Connection> mSocketConnectionArray = new  Dictionary<int, Connection>();

        private ILog mLogger = LogManager.GetLogger("Connection Manager");

        // Time out silent connections in one minute, much longer than we expect them to be quiet.
        // Heartbeat interval should be less than this value.
        private TimeSpan mIdleTimeout = new TimeSpan(0, 1, 0);

        public Guid AddConnection(Connection connection)
        {
        	mLogger.Debug("Adding Connection: " + connection.SocketId );
            if (!mSocketConnectionArray.ContainsKey(connection.SocketId))
            {
                mSocketConnectionArray.Add(connection.SocketId, connection);
            }
            else
            {
                mSocketConnectionArray[connection.SocketId] = connection;
            }
            
            if (!mSessionIds.ContainsKey(connection.SessionId))
            {
				mSessionIds.Add(connection.SessionId, connection.SocketId);
            }
			return connection.SessionId;
        }

		public bool Contains(Guid sessionId)
		{
			return mSessionIds.ContainsKey(sessionId);
		}

		public bool RemoveSession(Guid sessionIdToRemove)
		{
			int socketId = 0;
			if(mSessionIds.TryGetValue(sessionIdToRemove, out socketId))
			{
				mSessionIds.Remove(sessionIdToRemove);
				mSocketConnectionArray.Remove(socketId);
				return true;
			}
			return false;
		}

        public void RemoveConnection(Connection connection)
        {
			List<Guid> sessionIdsToRemove = new List<Guid>();
            foreach (KeyValuePair<Guid, int> sessionId in mSessionIds)
            {
                if (sessionId.Value == connection.SocketId)
                {
					sessionIdsToRemove.Add(sessionId.Key);
                }
            }
			foreach (Guid sessionIdGuid in sessionIdsToRemove)
			{
				mSessionIds.Remove(sessionIdGuid);
			}
            mSocketConnectionArray.Remove(connection.SocketId);
        }

        public void DisconnectAllServerConnection()
        {
            foreach (KeyValuePair<int, Connection> kvp in mSocketConnectionArray)
            {
                Connection connection = kvp.Value;
                connection.Disconnect();
            }
        }

		public void SendMessage(List<Guid> recipients, MemoryStream packet)
        {
            foreach (Guid recipient in recipients)
            {
                //UNCOMMENT THIS IF YOU WANT TO PRINT OUT THE SESSION IDS OF THE PEOPLE RECEIVING THIS MESSAGE
                //Console.WriteLine("User receiving Message: "+ receipiant);
                SendCommandToTarget(recipient, packet);
            }
        }

        public System.Net.IPAddress GetClientIPAddress(Guid sessionId)
        {
            return (GetConnection(sessionId).IP);
        }

        private Connection GetConnection(int socketId)
        {
            Connection socket = null;

            mSocketConnectionArray.TryGetValue(socketId, out socket);
            return socket;
        }

        private Connection GetConnection(Guid sessionId)
        {
            int socketId = -1;

            mSessionIds.TryGetValue(sessionId, out socketId);

            Connection socket = GetConnection(socketId);

            return socket;
        }


        public Connection GetConnection(Socket socket)
        {
            return (GetConnection((int)socket.Handle));
        }

		public bool GetSessionIdFromConnection(Connection connectionSocket, out Guid outSessionId)
		{
			outSessionId = Guid.Empty;
			int socketId = connectionSocket.SocketId;
			foreach(KeyValuePair<Guid, int> sessionId in mSessionIds)
			{
				if(sessionId.Value == socketId)
				{
					outSessionId = sessionId.Key;
					return true;
				}
			}
			return false;
		}

        public List<Guid> GetSessionIdArray()
        {
            List<Guid> sessionIdsArray = new List<Guid>();

            foreach (KeyValuePair<Guid, int> kvp in mSessionIds)
            {
                sessionIdsArray.Add( kvp.Key);
            }
            return sessionIdsArray;
        }

        private void BroadCastCommand(int targetId, int senderId, MemoryStream packet)
        {
            foreach (KeyValuePair<int, Connection> kvp in mSocketConnectionArray)
            {
                Connection socket = kvp.Value;
                if (socket.SocketId != senderId)
                {
                    socket.SendPacket(packet);
                }
            }
        }

        private void SendCommandToTarget(Guid targetSessionId, MemoryStream packet)
        {
            Connection clientConnection = GetConnection(targetSessionId);
			if(clientConnection != null && clientConnection.Connected == true)
			{
            	clientConnection.SendPacket(packet);
			//	Console.WriteLine(" ?? Socket receiving message - socket: " + socket.SocketId + ", sessionId: " + targetSessionId.ToString() + ", packet length: " + packet.Length);
			}
        }

        public void SelectReadableConnections(out List<Connection> readable, out List<Connection> errors, int timeoutMicroseconds)
        {
            List<Socket> readSockets = new List<Socket>();
            List<Socket> errorSockets = new List<Socket>();
            readable = new List<Connection>();
            errors = new List<Connection>();
            List<Connection> timeouts = new List<Connection>();

            DateTime currentTime = DateTime.UtcNow;

            // XXX performance hit, these should be pre-populated and not constructed for every call
            foreach (Connection c in mSocketConnectionArray.Values)
            {
                if (currentTime - c.LastMsgRecvTime < mIdleTimeout)
                {
                    readSockets.Add(c.InnerSocket);
                    errorSockets.Add(c.InnerSocket);
                }
                else
                {
                    // c.Disconnect removes c from mSocketConnectionArray, defer the call
                    timeouts.Add(c);
                }
            }

            foreach (Connection c in timeouts)
            {
                mLogger.Info("Connection at " + c.LastKnownIP + " went silent when we expect at least regular heartbeats.  Disconnecting.");
                c.Disconnect();
            }

            // Can't call select with zero sockets.
            // Instead, do a blind sleep here if no one is connected.
            if (readSockets.Count == 0)
            {
                if (timeoutMicroseconds > 0)
                {
                    //Argument to Sleep is in milliseconds
                    System.Threading.Thread.Sleep(timeoutMicroseconds / 1000);
                }
                return;
            }

            Socket.Select(readSockets, null, errorSockets, timeoutMicroseconds);

            foreach (Socket s in readSockets)
            {
                readable.Add(mSocketConnectionArray[(int)s.Handle]);
            }
            foreach (Socket s in errorSockets)
            {
                errors.Add(mSocketConnectionArray[(int)s.Handle]);
            }
        }

        public void SelectWritableConnections(out List<Connection> writable, out List<Connection> errors)
        {
            List<Socket> writeSockets = new List<Socket>();
            List<Socket> errorSockets = new List<Socket>();
            writable = new List<Connection>();
            errors = new List<Connection>();

            // XXX performance hit, these should be pre-populated and not constructed for every call
            foreach (Connection c in mSocketConnectionArray.Values)
            {
                writeSockets.Add(c.InnerSocket);
                errorSockets.Add(c.InnerSocket);
            }

            if (writeSockets.Count == 0)
            {
                return;
            }

            Socket.Select(null, writeSockets, errorSockets, 0);

            foreach (Socket s in writeSockets)
            {
                writable.Add(mSocketConnectionArray[(int)s.Handle]);
            }
            foreach (Socket s in errorSockets)
            {
                errors.Add(mSocketConnectionArray[(int)s.Handle]);
            }
        }
    }
}
