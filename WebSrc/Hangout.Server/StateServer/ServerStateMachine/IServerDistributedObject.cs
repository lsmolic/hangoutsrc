using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Server
{
    public interface IServerDistributedObject : IDistributedObject
    {
        bool DistributedObjectStateUpdate(out Message outgoingMessage);
    }
}
