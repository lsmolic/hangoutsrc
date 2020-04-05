using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Hangout.Shared
{
    [Serializable]
    public class AvatarId : ObjectId
    {
        protected override string SerializationString
        {
            get { return "avatarId"; }
        }

        public AvatarId(uint value)
            : base(value)
        {
        }

        public AvatarId(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
