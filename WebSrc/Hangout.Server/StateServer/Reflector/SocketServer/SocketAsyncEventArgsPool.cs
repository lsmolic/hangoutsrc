﻿
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Hangout.Server
{
    /// <summary>
    /// Based on example from http://msdn2.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.socketasynceventargs.aspx
    /// Represents a collection of reusable SocketAsyncEventArgs objects.  
    /// </summary>
    public class SocketAsyncEventArgsPool
    {
        /// <summary>
        /// Pool of SocketAsyncEventArgs.
        /// </summary>
        Stack<SocketAsyncEventArgs> mPool;

        /// <summary>
        /// Initializes the object pool to the specified size.
        /// </summary>
        /// <param name="capacity">Maximum number of SocketAsyncEventArgs objects the pool can hold.</param>
        public SocketAsyncEventArgsPool(Int32 capacity)
        {
            mPool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        /// <summary>
        /// The number of SocketAsyncEventArgs instances in the pool. 
        /// </summary>
        public Int32 Count
        {
            get { return mPool.Count; }
        }

        /// <summary>
        /// Removes a SocketAsyncEventArgs instance from the pool.
        /// </summary>
        /// <returns>SocketAsyncEventArgs removed from the pool.</returns>
        public SocketAsyncEventArgs Pop()
        {
            lock (mPool)
            {
                return mPool.Pop();
            }
        }

        /// <summary>
        /// Add a SocketAsyncEventArg instance to the pool. 
        /// </summary>
        /// <param name="item">SocketAsyncEventArgs instance to add to the pool.</param>
        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null) 
            { 
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); 
            }
            lock (mPool)
            {
                mPool.Push(item);
            }
        }
    }
}
