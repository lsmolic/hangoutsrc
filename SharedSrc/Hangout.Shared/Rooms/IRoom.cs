using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
    public interface IRoom
    {
        string RoomName { get; }
        RoomId RoomId { get; }
        RoomType RoomType { get; }
        PrivacyLevel PrivacyLevel { get; }
    }
}
