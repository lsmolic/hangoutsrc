using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Hangout.Shared
{
    [Serializable]
    public class ZoneId : ObjectId
    {
        public static readonly ZoneId LimboZone = new ZoneId(0);

        protected override string SerializationString
        {
            get { return "zoneId"; }
        }

        public ZoneId(uint value)
            : base(value)
        {
        }

        public ZoneId(SerializationInfo info, StreamingContext context)
            : base(info, context)
		{
		}
    }
}
