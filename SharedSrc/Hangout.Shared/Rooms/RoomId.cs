using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Hangout.Shared
{
    [Serializable]
    public class RoomId : ObjectId
    {
        protected override string SerializationString
        {
            get { return "RoomId"; }
        }

		public RoomId(string roomIdString) : base(roomIdString)
		{
		}

        public RoomId(RoomId copyObject) : base(copyObject)
        {
        }

        public RoomId(uint roomIdValue) : base(roomIdValue)
        {
        }

        public RoomId(SerializationInfo info, StreamingContext context)
            : base(info, context)
		{
		}
    }
}
