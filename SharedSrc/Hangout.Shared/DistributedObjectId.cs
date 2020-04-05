using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Hangout.Shared
{
    [Serializable]
    public class DistributedObjectId : ObjectId
    {
        protected override string SerializationString 
        {
            get { return "distributedObjectId"; }
        }
        
        public DistributedObjectId(uint distributedObjectId) : base(distributedObjectId)
        {
        }

        public DistributedObjectId(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
    }
}
