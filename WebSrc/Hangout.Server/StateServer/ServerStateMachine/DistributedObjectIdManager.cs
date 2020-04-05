using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Server
{
    public class DistributedObjectIdManager
    {
        private uint mDistributedObjectIdGenerator = 0;

        public DistributedObjectIdManager()
        {
        }

        public DistributedObjectId GetNewId()
        {
            return new DistributedObjectId(++mDistributedObjectIdGenerator);
        }
    }
}
