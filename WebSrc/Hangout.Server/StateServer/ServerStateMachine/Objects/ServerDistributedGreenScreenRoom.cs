using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using System.Xml;

namespace Hangout.Server
{
    public class ServerDistributedGreenScreenRoom : ServerDistributedRoom
    {
        private ServerAssetRepository mServerAssetRepository = null;
        private List<ItemId> mItemIds = new List<ItemId>();
        private ItemId mRoomBackgroundItemId = null;

		public ServerDistributedGreenScreenRoom(ServerObjectRepository serverObjectRepository, ServerAssetRepository serverAssetRepository, AccountId roomOwnerAccountId/*ServerAccount roomOwnerAccount*/, string roomName, RoomId roomId, PrivacyLevel privacyLevel, DistributedObjectId doId, XmlNode itemIdXml)
            : base(serverObjectRepository, roomOwnerAccountId, roomName, RoomType.GreenScreenRoom, roomId, privacyLevel, doId, itemIdXml)
        {
            mServerAssetRepository = serverAssetRepository;
            ParseItemsFromXml(itemIdXml);
        }

        protected override void RegisterMessageActions()
        {
            RegisterMessageAction((int)MessageSubType.ChangeBackground, ChangeBackground);
        }

        private void ParseItemsFromXml(XmlNode itemIdXml)
        {
            XmlNodeList itemXmlNodes = itemIdXml.SelectNodes("Item");
            foreach (XmlNode itemXmlNode in itemXmlNodes)
            {
                XmlNode itemIdNode = itemXmlNode.SelectSingleNode("Id");
                ItemId itemId = new ItemId(Convert.ToUInt32(itemIdNode.InnerText));
                mItemIds.Add(itemId);

                XmlNodeList assetXmlNodes = itemXmlNode.SelectNodes("Assets/Asset");
                foreach (XmlNode assetXmlNode in assetXmlNodes)
                {
                    AssetSubType assetSubType = (AssetSubType)Enum.Parse(typeof(AssetSubType), assetXmlNode.Attributes["AssetSubType"].Value);
                    switch (assetSubType)
                    {
                        case AssetSubType.RoomBackgroundTexture:
                            mRoomBackgroundItemId = itemId;
                            mItemIds.Remove(mRoomBackgroundItemId);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// the expected node for the item xml is the following:
        /// <item id="1061" name="106" appName="HANGOUT_FAMILY_Application_1" itemTypeName="Room Backdrop" buttonName="Two Fast Two Furious" description="Live life a quarter mile at a time" smallImageUrl="http://s3.amazonaws.com/HangoutDevFileBucket/48141004.jpg" mediumImageUrl="" largeImageUrl="http://s3.amazonaws.com/HangoutDevFileBucket/48141004.jpg" available="-1">
        ///     <Assets />
        /// </item>
        /// </summary>
        /// <param name="receivedMessage"></param>
        private void ChangeBackground(Message receivedMessage)
        {
            mRoomBackgroundItemId = CheckType.TryAssignType<ItemId>(receivedMessage.Data[0]);

            List<ItemId> roomItems = UpdateRoomItems();

            XmlDocument updatedRoomDna = RoomXmlUtil.CreateRoomDnaXml(this.RoomName, this.RoomType, roomItems);
            //update the database with the new items
            RoomManagerServiceAPI.UpdateRoomDnaService(this.RoomId, updatedRoomDna, delegate(XmlDocument xmlResponse) 
                {
                    XmlNode successNode = xmlResponse.SelectSingleNode("Success");
                    if (successNode != null && successNode.InnerText == "true")
                    {
                        XmlNode backgroundItemXml = mServerAssetRepository.GetXmlDna(new ItemId[] { mRoomBackgroundItemId });
                        List<object> backgroundChangeMessageData = new List<object>();
                        backgroundChangeMessageData.Add(backgroundItemXml.OuterXml);

                        Message broadcastBackgroundChangeMessage = new Message();
                        broadcastBackgroundChangeMessage.UpdateObjectMessage(true, false, this.DistributedObjectId, (int)MessageSubType.ChangeBackground, backgroundChangeMessageData);

                        BroadcastMessage(broadcastBackgroundChangeMessage);
                    }
                    else
                    {
                        StateServerAssert.Assert(new System.Exception("Error setting background in room on database."));
                    }
                }
            );
        }

        private List<ItemId> UpdateRoomItems()
        {
            List<ItemId> updatedItemIds = new List<ItemId>(mItemIds);
            //make sure the background item is not added twice
            if (!updatedItemIds.Contains(mRoomBackgroundItemId))
            {
                updatedItemIds.Add(mRoomBackgroundItemId);
            }

            //update the internal room items
            XmlNode itemsXml = mServerAssetRepository.GetXmlDna(updatedItemIds);
            mObjectData[3] = itemsXml.OuterXml;

            return updatedItemIds;
        }
    }
}
