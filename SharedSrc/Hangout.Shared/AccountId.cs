using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Hangout.Shared
{
    [Serializable]
    public class AccountId : ObjectId
    {
        protected override string SerializationString
        {
            get { return "accountId"; }
        }

        public AccountId(uint value)
            : base(value)
        {
        }

        public AccountId(SerializationInfo info, StreamingContext context)
            : base(info, context)
		{
		}
    }
}
