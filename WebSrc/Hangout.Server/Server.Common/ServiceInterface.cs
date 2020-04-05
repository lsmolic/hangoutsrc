using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Hangout.Shared;

namespace Hangout.Server
{
    /// <summary>
    /// Remoting REST Service Interface
    /// </summary>
    public interface IServiceInterface
    {
        string ProcessRestService(RESTCommand restCommand);
    }
}
    