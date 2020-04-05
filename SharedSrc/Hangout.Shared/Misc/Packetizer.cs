using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Hangout.Shared
{
    public class Packetizer
    {
		private static readonly int mMaxMessageSize = 262144 - mPacketHeaderSize;
        private const int mPacketHeaderSize = 4;
        private const int mMaxBufferSize = 1048576;

        public MemoryStream mBufferStream = new MemoryStream();

        //private ILogger mLogger;

        // Append new data to mBufferStream
        public void AppendBytes(byte[] buffer, int Offset, int bytesTransferred)
        {
            if (bytesTransferred > 0)
            {
                long bufferStartPos = mBufferStream.Position;
                mBufferStream.Position = mBufferStream.Length;
                mBufferStream.Write(buffer, Offset, bytesTransferred);
                mBufferStream.Position = bufferStartPos;
            }
            
            if (mBufferStream.Length > mMaxBufferSize)
            {
                mBufferStream.SetLength(0);
                mBufferStream.Position = 0;
                throw new BufferOverflowException("AppendBytes: Intermediate buffer flushed due to overflow.  Try calling GetPackets more often?");
            }
        }

        // Read ALL complete packets.
        public IEnumerable<MemoryStream> GetPackets()
        {
                MemoryStream packet = ReadPacket();
                while (packet != null)
                {
                    yield return packet;
                    packet = ReadPacket();
                }
        }

        // Read a single packet from the buffer if enough data is available.
		private MemoryStream ReadPacket()
		{
            // Remember where we started.  Non-zero if we consumed but did not remove a packet from the buffer.
            long bufferStartPos = mBufferStream.Position;

			if (mBufferStream.Length < bufferStartPos + mPacketHeaderSize)
			{
                // Not enough bytes for a header.  Leave any data on the buffer and abort.
				return null;
			}

            // According to the packet header, how big is this packet and where does the next one start?
            byte[] header = new byte[mPacketHeaderSize];
            mBufferStream.Read(header, 0, mPacketHeaderSize);
            int packetLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 0));
            int nextPacketPos = (int)bufferStartPos + mPacketHeaderSize + packetLength;

            if (packetLength < 0 || packetLength > mMaxMessageSize)
			{
                throw new System.Net.ProtocolViolationException("ReadPacket: Invalid message length " + packetLength.ToString() + " (max: " + mMaxMessageSize + ").\n" +
                                                                "Header bytes: " + header[0] + " " + header[1] + " " + header[2] + " " + header[3] + "\n" +
                                                                "---BEGIN BUFFER TO UTF8---\n" + System.Text.Encoding.UTF8.GetString(mBufferStream.GetBuffer()) + "\n" +
                                                                "---END BUFFER TO UTF8---");
			}

            if (mBufferStream.Length < nextPacketPos)
            {
                // Haven't yet read enough data for a whole packet.  Keep the data and come back later.
                mBufferStream.Position = bufferStartPos;
                return null;
			}
            else
			{
                // Got enough data for a whole packet!  Copy it from the buffer and return it.
                MemoryStream packet = new MemoryStream(packetLength);
                mBufferStream.Position = bufferStartPos + mPacketHeaderSize;

                while (mBufferStream.Position < nextPacketPos)
                {
                    packet.WriteByte((byte)mBufferStream.ReadByte());
                }

                if (mBufferStream.Position == mBufferStream.Length)
                {
                    // Consumed the entire buffer.  Free up memory.
                    mBufferStream.SetLength(0);
                    mBufferStream.Position = 0;
                }
                else if (mBufferStream.Length > (mMaxBufferSize - mMaxMessageSize))
                {
                    // The buffer is in danger of overflowing.  Copy out the unconsumed region.
                    // For mMaxBufferSize >> mMaxMessageSize, this should be super-rare, only caused by weird client behavior.
                    //mLogger.Warn("Buffer in danger of overflow.  Cleaning up.");
                    //mLogger.Warn("Buffer size: " + mBufferStream.Length + "/" + mMaxBufferSize);
                    throw new Exception("Buffer in danger of overflowing, omgomgomg");
                    MemoryStream keepMe = new MemoryStream((int)(mBufferStream.Length - mBufferStream.Position));
                    while (mBufferStream.Position < mBufferStream.Length)
                    {
                        keepMe.WriteByte((byte)mBufferStream.ReadByte());
                    }
                    mBufferStream.SetLength(0);
                    keepMe.WriteTo(mBufferStream);
                    mBufferStream.Position = 0;
                    //mLogger.Warn("New buffer size: " + mBufferStream.Length);
                }
                packet.Position = 0;
                return packet;
			}
		}
    }
}
