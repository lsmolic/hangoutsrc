using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Hangout.Shared
{
    [Serializable]
    public class ItemId : ObjectId
    {
        protected override string SerializationString
        {
            get { return "ItemId"; }
        }

        public ItemId() : base() { } 
        public ItemId(ItemId copyObject)
            : base(copyObject)
        {
        }

        public ItemId(uint itemIdValue)
            : base(itemIdValue)
        {
        }

        public ItemId(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
