using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Net;
using Hangout.Client.SocketClient;
using System.ComponentModel;
using System.Threading;
using Hangout.Shared;


//**************To do****************************
//Error Handling improvements.
//Retry, reconnect.
//**********************************************

namespace Hangout.Client
{
    /// <summary
    /// Interface to be used to connect to the Reflector Client to receive and send messages to the server.
    /// </summary>
    public interface IClientReflector
    {
        bool Disconnect();
        bool SendMessage(MemoryStream message);
        MemoryStream GetNextMessage();
    }


    public class ClientReflector : IClientReflector, IDisposable
    {
        private Queue<MemoryStream> mReceiveMessageQueue;
        private List<MemoryStream> mSendMessageQueue; // This is a list instead of a Queue because for some reason the Enqueue was taking 40ms... wtf

		private IScheduler mScheduler = null;

        private ClientConnection mClientConnection = null;

		private ITask mMessageSchedulerTask = null;
		Hangout.Shared.Action mClientGotDisconnectedCallback = null;

		public ClientReflector(IScheduler scheduler, Hangout.Shared.Action clientGotDisconnectedCallback)
        {
			mScheduler = scheduler;
			mReceiveMessageQueue = new Queue<MemoryStream>();
			mSendMessageQueue = new List<MemoryStream>();
			mClientGotDisconnectedCallback = clientGotDisconnectedCallback;

			try
			{
				mClientConnection = new ClientConnection(mScheduler, MessageReceived);
			}
			catch(System.Exception ex)
			{
				Console.WriteLine("ERROR cannot create a client connection: " + ex.ToString());
				throw ex;
			}
        }

		public bool Connect(string address, int port)
		{
			if(mClientConnection.IsConnected() == 1)
			{
				return true;
			}
			bool isConnected = mClientConnection.Connect(address, port);
			if (isConnected)
			{
				mMessageSchedulerTask = mScheduler.StartCoroutine(MessageScheduler());
			}
			return isConnected;
		}

		/// <summary>
		/// Gets my ipaddress
		/// </summary>
		public string ClientIpAddress
		{
			get { return (mClientConnection.ClientIpAddress); }
		}


		private IEnumerator<IYieldInstruction> MessageScheduler()
		{
			while (true)
			{
				DoSendEvent();

				yield return new DateTimeYieldForSeconds(0.1f);
			}
		}


		#region Send_Message
			/// <summary>
			/// If the send queue is not empty call then send the message to the server.
			/// </summary>
			private void DoSendEvent()
			{
                if (mClientConnection.IsConnected() != 1)
                {
                    mClientGotDisconnectedCallback();
                    return;
                }

                // Flush the entire queue
				while (mSendMessageQueue.Count > 0)
				{
					MemoryStream message = (MemoryStream)mSendMessageQueue[0];
					mSendMessageQueue.RemoveAt(0);
					if(!mClientConnection.SendCommand(message))
					{
						mClientGotDisconnectedCallback();
					}
				}
			}

			/// <summary>
			/// Send message to server.
			/// <param name="message">The message to be sent to server.</para>
			/// <returns>Returns true if connected and message queued</returns>
			/// </summary>
			public bool SendMessage(MemoryStream message)
			{
				bool retValue = false;

				try
				{
					if (mClientConnection.IsConnected() > 0)
					{
						message.Position = 0;
						mSendMessageQueue.Add(message);
						retValue = true;
					}
				}
				catch (System.Exception ex)
				{
					throw new System.Exception("ClientReflector::SendMessage - Error Encrypting and Enqueueing message -" + ex.Message);
				}

				return retValue;
			}
		#endregion

		#region Receive_Message
			/// <summary>
			/// Get the next received message off the message queue.
			/// </summary>
			/// <returns>Returns  MemoryStream packet.</returns>
			public MemoryStream GetNextMessage()
			{
				MemoryStream message = null;
				try
				{
					if (mReceiveMessageQueue.Count > 0)
					{
						message = mReceiveMessageQueue.Dequeue();
						message.Position = 0;
					}
				}
				catch (System.Exception ex)
				{
					throw new System.Exception("Something went wrong with getting the next message off the queue: " + ex.Message);
				}

				return message;
			}

			private void MessageReceived(MemoryStream messagePacket)
			{
				mReceiveMessageQueue.Enqueue(messagePacket);
			}

		#endregion

		#region Disconnect_And_Dispose
			/// </summary>
			/// <returns>Returns true disconnected false error disconnecting</returns>
			public bool Disconnect()
			{
				bool retValue = false;

				try
				{
					mReceiveMessageQueue.Clear();
					mSendMessageQueue.Clear();

					if (mClientConnection != null && mClientConnection.Disconnect())
					{
						retValue = true;
					}
				}

				catch (System.Exception ex)
				{
					throw new System.Exception("Error disconnecting the client socket: " + ex.Message);
				}

				return retValue;

			}

			public void Dispose()
			{
				if (mMessageSchedulerTask != null)
				{
					mMessageSchedulerTask.Exit();
				}
				Disconnect(); //this disconnects the ClientConnection which in turn disconnects from the ClientSocket

				mClientConnection.Dispose();
			}
		#endregion
	}
}
