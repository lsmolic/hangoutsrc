using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
    public interface IDistributedObject : IDisposable
    {
        List<object> Data { get; }
        DistributedObjectId DistributedObjectId { get; }
        void ProcessMessage(Message msg);
    }
}
