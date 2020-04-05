/*
 * ConcurrentQueue
 * 
 * Fast, non-blocking, thread-safe concurrent queue.
 * Used to pass messages between server threads in a safe manner.
 * Supports an unlimited number of simultaneous producers and consumers.
 * Algorithm based on Michael and Scott, JPDC 1998
 * http://www.research.ibm.com/people/m/michael/jpdc-1998.pdf (pseudocode on page 9).
 * 
 * Note on the ABA problem: We do NOT need to utilize counter values to augment our 
 * pointer comparisons here because we are holding references to all potential points 
 * of comparison and using GC rather than explicit deletes.  Because we allocate a 
 * new Node for each item we enqueue, it is impossible for Node A in memory location X
 * to be compared to a Node B that occurs later in the queue but is also in memory
 * location X.  Even if one thread removes Node A from the head of the list, any 
 * thread holding a reference to Node A for later comparison will prevent memory 
 * location X from being reused.  If Node A is able to be compared to another Node, 
 * then memory location X must not have been free for the allocation of new Node B.
 * By contradiction, the ABA problem cannot impact our reference comparisons.
 * 
 */

using System.Threading;

namespace Hangout.Shared
{
    public class ConcurrentQueue<T>
    {
        private class Node
        {
            public T message;
            public Node next;
        }

        private Node mHead;
        private Node mTail;
        private int mCount;

        public ConcurrentQueue()
        {
            mHead = new Node();
            mHead.next = null;
            mTail = mHead;
            mCount = 0;
        }

        // Returns an APPROXIMATE element count
        // If you are the only consumer and Count > 0, your dequeue is guaranteed to succeed
        // If you are not the only consumer, this value means very little
        public int Count
        {
            get { return mCount; }
        }

        // Enqueues a message.  Always succeeds, does not block.
        public void Enqueue(T message)
        {
            Node newNode = new Node();
            newNode.message = message;
            newNode.next = null;

            Node prevTail = null;
            Node prevNext = null;

            bool done = false;
            while (!done)
            {
                prevTail = mTail;
                prevNext = prevTail.next;
                if (prevTail == mTail)
                {
                    if (prevNext == null)
                    {
                        // Try to get newNode on the tail.  mTail.next -> newNode iff mTail.next was null
                        // We are done only if someone else doesn't beat us.
                        done = (Interlocked.CompareExchange(ref mTail.next, newNode, null) == null);
                    }
                    else
                    {
                        // Another thread inserted before we could.  Make sure tail gets updated.
                        // mTail -> prevNext iff mTail was prevTail
                        Interlocked.CompareExchange(ref mTail, prevNext, prevTail);
                    }
                }
            }
            // newNode has been inserted, now make sure tail gets updated.
            // If it doesn't happen here it's because another thread did it or is about to do it for us.
            // mTail -> newNode iff mTail was prevTail
            Interlocked.CompareExchange(ref mTail, newNode, prevTail);
            Interlocked.Increment(ref mCount);
        }

        // Dequeues a message.  Returns true on success, false on unable to dequeue (queue was empty).
        // Never blocks but can ALWAYS fail if there are multiple threads dequeuing.
        public bool Dequeue(out T message)
        {
            message = default(T);

            bool success = false;
            while (!success)
            {
                Node prevHead = mHead;
                Node prevTail = mTail;
                Node prevNext = prevHead.next;

                if (prevHead == mHead)
                {
                    if (prevHead == prevTail)
                    {
                        if (prevNext == null)
                        {
                            // Queue is empty, we're done
                            return false;
                        }
                        else
                        {
                            // Tail is not current, advance it
                            Interlocked.CompareExchange(ref mTail, prevNext, prevTail);
                        }
                    }
                    else
                    {
                        // The queue is not empty.  Read the first item.
                        // Return the item we claimed ONLY if we successfully advance the head pointer beyond the current node,
                        // knowing that this was the only thread to do so.
                        message = prevNext.message;
                        success = (Interlocked.CompareExchange(ref mHead, prevNext, prevHead) == prevHead);
                    }
                }
            }
            Interlocked.Decrement(ref mCount);
            return true;
        }
    }
}
