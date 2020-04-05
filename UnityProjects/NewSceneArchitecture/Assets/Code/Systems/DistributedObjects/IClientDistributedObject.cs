using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;

namespace Hangout.Client
{
    public interface IClientDistributedObject : IDistributedObject, IDisposable
    {

        IEntity Entity
        {
            get;            
        }
    }
}
