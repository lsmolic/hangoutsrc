using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Hangout.Shared
{
    interface IMessage : ISerializable
    {
        bool Broadcast { get; }
        bool StoreInRam { get; }
        DistributedObjectId DistributedObjectId { get; }
        List<object> Data { get; }
        MessageType MessageType { get; }
    }
}
