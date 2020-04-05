using System.Xml;
using Hangout.Shared;

namespace Hangout.Server
{
    public class ServerDistributedMiniGameRoom : ServerDistributedRoom
    {

        public ServerDistributedMiniGameRoom(ServerObjectRepository serverObjectRepository, AccountId roomOwnerAccountId/*ServerAccount roomOwnerAccount*/, string roomName, RoomId roomId, PrivacyLevel privacyLevel, DistributedObjectId doId, XmlNode itemIdXml)
            : base(serverObjectRepository, roomOwnerAccountId, roomName, RoomType.MiniGameRoom, roomId, privacyLevel, doId, itemIdXml)
        {
        }
    }
}
