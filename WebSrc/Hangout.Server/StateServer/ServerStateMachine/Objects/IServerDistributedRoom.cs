using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using System.Xml;

namespace Hangout.Server
{
    public interface IServerDistributedRoom : IServerDistributedObject, IRoom
    {
        List<Guid> Population { get; }

        void IncrementPopulation(Guid sessionIdToJoinRoom);
        void DecrementPopulation(Guid sessionIdToLeaveRoom);
        bool ContainsUser(Guid sessionId);
    }
}
