using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Hangout.Shared
{
    /// <summary>
    /// The command class.
    /// </summary>
    public class ReflectorMessage
    {
        private Command mCommand;
        private Guid mSenderId;
        private Guid mTargetId;
        private string mSenderName;
        private string mTargetName;
        private string mCommandData;

        /// <summary>
        /// The type of command to send.If you wanna use the ReflectorMessage command type,create a ReflectorMessage class instead of command.
        /// </summary>
        public Command Command
        {
            get { return mCommand; }
            set { mCommand = value; }
        }

        /// <summary>
        /// [Gets/Sets] The ID of sender.
        /// </summary>
        public Guid SenderId
        {
            get { return mSenderId; }
            set { mSenderId = value; }
        }

        /// <summary>
        /// [Gets/Sets] The name of command sender.
        /// </summary>
        public string SenderName
        {
            get { return mSenderName; }
            set { mSenderName = value; }
        }

        /// <summary>
        /// [Gets/Sets] The name of the target.
        /// </summary>
        public string TargetName
        {
            get { return mTargetName; }
            set { mTargetName = value; }
        }

        /// <summary>
        /// [Gets/Sets] The target machine that will receive the command.
        /// </summary>
        public Guid TargetId
        {
            get { return mTargetId; }
            set { mTargetId = value; }
        }

        /// <summary>
        /// The body of the command. 
        /// <param name="mCommandData">The ReflectorMessage body.</para>
        /// </summary>
        public string CommandData
        {
            get { return mCommandData; }
            set { mCommandData = value; }
        }

        /// <summary>
        /// Creates an instance of command object.
        /// </summary>
        /// <param name="command">The type of command to send over the network.</param>
        /// <param name="targetName">The target machine that will receive the command.</param>
        /// <param name="commandData">
        /// The body of the command.This string is different in various commands.
        /// </param>
        public ReflectorMessage(Command command, string targetName, string commandData)
        {
            mCommand = command;
            mTargetName = targetName;
            mCommandData = commandData;
        }

        /// <summary>
        /// Creates an instance of command object.
        /// </summary>
        /// <param name="command">The type of command.</param>
        /// <param name="targetName">The target machine that will receive the command.</param>
        public ReflectorMessage(Command command, string targetName)
        {
            mCommand = command;
            mTargetName = targetName;
            mCommandData = "";
        }

        /// <summary>
        /// Creates an instance of command object.
        /// </summary>
        /// <param name="command">The type of command.</param>
        /// <param name="targetId">The target machine id that will receive the command.</param>
        public ReflectorMessage(Command command, Guid targetId)
        {
            mCommand = command;
            mTargetId = targetId;
            mCommandData = "";
        }

        /// Creates an instance of command object.
        /// </summary>
        /// <param name="streamData">The packet received from the client.</param>
        /// <param name="clientSender">The client that sent the command.</param>
        public ReflectorMessage(MemoryStream streamData, Guid clientSender)
        {
            byte[] buffer = null;

            try
            {
                //Read the command's Type.
                buffer = GetDataBufferFromStream(streamData, 0, 4);
                mCommand = (Command)BitConverter.ToInt32(buffer, 0);

                buffer = GetDataBufferFromStream(streamData, 0, 4);
                int ipSize = BitConverter.ToInt32(buffer, 0);

                if (ipSize > 0)
                {
                    buffer = GetDataBufferFromStream(streamData, 0, ipSize);
                    mTargetName = System.Text.Encoding.ASCII.GetString(buffer);
                }
                else
                {
                    mTargetName = "default";
                }

                buffer = GetDataBufferFromStream(streamData, 0, 4);
                int dataSize = BitConverter.ToInt32(buffer, 0);

                if (dataSize > 0)
                {
                    buffer = GetDataBufferFromStream(streamData, 0, dataSize);
                    mCommandData = System.Text.Encoding.Unicode.GetString(buffer);
                }
                else
                {
                    mCommandData = "";
                }

                mSenderId = clientSender;
                mSenderName = "";
            }

            catch (Exception ex)
            {
                Console.WriteLine(String.Format(" Exception: {0} ", ex.Message));
            }
        }

        /// <summary>
        /// Create a Packet from the Command object to send to the client or state server.
        /// </summary>
        /// <param name="encrypt">true to encrypt the packet.</param>
        public MemoryStream CreatePacketFromMessage(bool encrypt)
        {
            MemoryStream memoryStream = new MemoryStream();

            //Type
            byte[] buffer = new byte[4];
            buffer = BitConverter.GetBytes((int)this.mCommand);
            memoryStream.Write(buffer, 0, 4);

            //Sender Name
            if (mSenderName == null || mSenderName.Length == 0)
            {
                mSenderName = "default";
            }

            byte[] senderNameBuffer = Encoding.Unicode.GetBytes(mSenderName);
            buffer = new byte[4];
            buffer = BitConverter.GetBytes(senderNameBuffer.Length);
            memoryStream.Write(buffer, 0, 4);
            memoryStream.Write(senderNameBuffer, 0, senderNameBuffer.Length);

            if (mCommandData == null || mCommandData == "")
            {
                mCommandData = "";
            }

            byte[] dataBuffer = Encoding.Unicode.GetBytes(mCommandData);
            buffer = new byte[4];
            buffer = BitConverter.GetBytes(dataBuffer.Length);
            memoryStream.Write(buffer, 0, 4);
            memoryStream.Write(dataBuffer, 0, dataBuffer.Length);
            memoryStream.Position = 0;

            if (encrypt)
            {
                SimpleCrypto crypt = new SimpleCrypto();
                memoryStream = crypt.TDesEncrypt(memoryStream);
                memoryStream.Position = 0;
            }

            return memoryStream;
        }

        /// <summary>
        /// Retrieves a byte buffer from the memory stream.
        /// </summary>
        /// <param name="offset">offset to start copying bytes from the stream.</param>
        /// <param name="size">Number of bytes to copy from the stream.</param>
        private byte[] GetDataBufferFromStream(MemoryStream streamData, int offset, int size)
        {
            byte[] buffer = new byte[size];
            if (size > 0)
            {
                int readBytes = streamData.Read(buffer, offset, size);
                if ((size > 0) && (readBytes == 0))
                {
                    throw new Exception("Incomplete Receive Message");
                }
            }
            return buffer;
        }
    }
}
