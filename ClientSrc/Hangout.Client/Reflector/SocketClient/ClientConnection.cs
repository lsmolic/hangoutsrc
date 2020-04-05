using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.Threading;
using Hangout.Shared;

using System.Net;
using System.Net.Sockets;
using System.IO;
using Hangout.Client.SocketClient;

namespace Hangout.Client.SocketClient
{
    public class ClientConnection : IDisposable
    {
        private ClientSocket mClientSocket = null;
		private IScheduler mScheduler = null;
		private ITask mReceiveMessagesTask = null;

		private Action<MemoryStream> mMessageReceivedCallback = null;

		/// <summary>
		/// Gets my ipaddress
		/// </summary>
		public string ClientIpAddress
		{
			get { return (mClientSocket.ClientIpAddress); }
		}

		public ClientConnection(IScheduler scheduler, Action<MemoryStream> messageReceivedCallback)
        {
			mScheduler = scheduler;

			mMessageReceivedCallback = messageReceivedCallback;

			mClientSocket = new ClientSocket();
		}

		public bool Connect(string address, int port)
		{
			if(mClientSocket.IsConnected == 1)
			{
				return true;
			}
			bool isConnected = mClientSocket.ConnectToServer(address, port);
			if (isConnected)
			{
				mReceiveMessagesTask = mScheduler.StartCoroutine(StartReceiveMessages());
			}
			return isConnected;
		}

		private IEnumerator<IYieldInstruction> StartReceiveMessages()
        {
			while(true)
			{
                if (mClientSocket.IsConnected == 1)
                {
                    if (mClientSocket.ReceiveDataNonBlocking())
                    {
                        foreach (MemoryStream messagePacket in mClientSocket.GetPackets())
                        {
                            mMessageReceivedCallback(messagePacket);
                        }
                    }
                }
                // Should be redundant
                //else if (mClientSocket.IsConnected == 0)
                //{
                //    Disconnect();
                //}
				yield return new DateTimeYieldForSeconds(0.1f);
			}
        }
		#region Send_Message
			public bool SendCommand(MemoryStream packet)
			{
			    return mClientSocket.SendPacket(packet);
			}
		#endregion

		#region Connectivity_Stuff
			/// <summary>
			/// Disconnect the client from the server and returns true if the client had been disconnected from the server.
			/// </summary>
			/// <returns>True if the client had been disconnected from the server,otherwise false.</returns>
			public bool Disconnect()
			{
				if(mReceiveMessagesTask != null)
				{
					mReceiveMessagesTask.Exit();
				}
				try
				{
					if (mClientSocket != null)
					{
						mClientSocket.Disconnect();
					}
					return true;
				}
				catch
				{
					return false;
				}
			}


			// 1 = connect; 0 = connectInProgress; -1 = not connected
			public int IsConnected()
			{
				int retValue = -1;
				if (mClientSocket != null)
				{
					retValue = mClientSocket.IsConnected;
				}
				return retValue;
			}
		#endregion

		public void Dispose()
		{
			if (mReceiveMessagesTask != null)
			{
				mReceiveMessagesTask.Exit();
			}
		}
	}
}
