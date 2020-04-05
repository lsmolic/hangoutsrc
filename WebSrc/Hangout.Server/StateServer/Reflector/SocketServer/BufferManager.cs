using System;
using System.Collections.Generic; 
using System.Net.Sockets;

//TODO: delete this file?  i don't think we need (or use) it anymore
namespace Hangout.Server
{
    /// <summary>
    /// Based on example from http://msdn2.microsoft.com/en-us/library/bb517542.aspx
    /// This class creates a single large buffer which can be divided up 
    /// and assigned to SocketAsyncEventArgs objects for use with each 
    /// socket I/O operation.  
    /// This enables bufffers to be easily reused and guards against 
    /// fragmenting heap memory.
    /// </summary>
    /// <remarks>The operations exposed on the BufferManager class are not thread safe.</remarks>
    public class BufferManager
    {
        /// <summary>
        /// The underlying Byte array maintained by the Buffer Manager.
        /// </summary>
        private Byte[] mBuffer;                

        /// <summary>
        /// Size of the underlying Byte array.
        /// </summary>
        private Int32 mBufferSize;

        /// <summary>
        /// Current index of the underlying Byte array.
        /// </summary>
        private Int32 mCurrentIndex;

        /// <summary>
        /// Pool of indexes for the Buffer Manager.
        /// </summary>
        private Stack<Int32> mFreeIndexPool;     

        /// <summary>
        /// The total number of bytes controlled by the buffer pool.
        /// </summary>
        private Int32 mNumBytes;

        /// <summary>
        /// Instantiates a buffer manager.
        /// </summary>
        /// <param name="totalBytes">The total number of bytes for the buffer pool.</param>
        /// <param name="bufferSize">Size of the buffer pool.</param>
        public BufferManager(Int32 totalBytes, Int32 bufferSize)
        {
            mNumBytes = totalBytes;
            mCurrentIndex = 0;
            mBufferSize = bufferSize;
            mFreeIndexPool = new Stack<Int32>();
        }

        /// <summary>
        /// Removes the buffer from a SocketAsyncEventArg object. 
        /// This frees the buffer back to the buffer pool.
        /// </summary>
        /// <param name="args">SocketAsyncEventArgs where is the buffer to be removed.</param>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            mFreeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

        /// <summary>
        ///  Allocates buffer space used by the buffer pool. 
        /// </summary>
        public void InitBuffer()
        {
            // Create one big large buffer and divide that out to each SocketAsyncEventArg object.
            mBuffer = new Byte[mNumBytes];
        }

        /// <summary>
        /// Assigns a buffer from the buffer pool to the specified SocketAsyncEventArgs object.
        /// </summary>
        /// <param name="args">SocketAsyncEventArgs where is the buffer to be allocated.</param>
        /// <returns>True if the buffer was successfully set, else false.</returns>
        public Boolean SetBuffer(SocketAsyncEventArgs args)
        {
            if (mFreeIndexPool.Count > 0)
            {
                args.SetBuffer(mBuffer, mFreeIndexPool.Pop(), mBufferSize);
            }
            else
            {
                if ((mNumBytes - mBufferSize) < mCurrentIndex)
                {
                    return false;
                }
                args.SetBuffer(mBuffer, mCurrentIndex, mBufferSize);
                mCurrentIndex += mBufferSize;
            }

            return true;
        }
    }
}

