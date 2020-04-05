using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;

namespace Hangout.Server
{
    /// <summary>
    /// This class is meant to take Xml and convert the data into useful objects to construct Room Distributed Objects
    /// </summary>
    public static class RoomXmlUtil
    {
        public static XmlDocument CreateEmptyRoomDnaXml()
        {
            //setup a skeleton room xml node so we can fill out some information about the room
            return CreateRoomDnaXml("", RoomType.Default, new List<ItemId>());
        }

        public static XmlDocument CreateRoomDnaXml(string roomName, RoomType roomType, List<ItemId> itemIds)
        {
            //setup a skeleton room xml node so we can fill out some information about the room
            StringBuilder roomDnaXmlStringBuilder = new StringBuilder();
            roomDnaXmlStringBuilder.Append("<RoomDna RoomName=\"" + roomName + "\" RoomType=\"" + roomType.ToString() + "\"><Items>");
            foreach(ItemId itemId in itemIds)
            {
                roomDnaXmlStringBuilder.Append(itemId.ToString() + ",");
            }
            roomDnaXmlStringBuilder.Append("</Items></RoomDna>");

            XmlDocument roomDnaXml = new XmlDocument();
            roomDnaXml.LoadXml(roomDnaXmlStringBuilder.ToString());
            return roomDnaXml;
        }

        /// <summary>
        /// return true in the event of an enabled room with valid data
        /// return false in the event of a non-enabled room or a room with missing / invalid data
        /// roomNodeXml is expected to be in the following format:
		/// <Room RoomId="1" AccountId="696969" IsEnabled="1" PrivacyLevel="0">
        ///     <RoomDna RoomName="" RoomType=""><Items> </Items> </RoomDna>
        /// </Room>
        /// </summary>
        public static bool GetRoomInfoFromRoomXmlNode(XmlNode roomNodeXml, out AccountId accountId, out PrivacyLevel privacyLevel, out string roomName, out RoomId roomId, out RoomType roomType, out List<ItemId> itemIds)
        {
            accountId = null;
            roomName = string.Empty;
            roomId = null;
            roomType = RoomType.Default;
            itemIds = new List<ItemId>();
            privacyLevel = PrivacyLevel.Default;

            XmlAttributeCollection attributes = roomNodeXml.Attributes;
            XmlAttribute isEnabledAttribute = attributes["IsEnabled"];
            if (isEnabledAttribute == null)
            {
				StateServerAssert.Assert(new System.Exception("Error: Could not find the IsEnabled xml attribute."));
				return false;
            }

            if (isEnabledAttribute.Value == "1")
            {
                GetRoomInfoFromRoomXmlNode(roomNodeXml, out accountId, out privacyLevel, out roomName, out roomId, out roomType);

                //find the room items in the DNA
                XmlNode roomItemsXmlNode = roomNodeXml.SelectSingleNode("RoomDna/Items");
                if (roomItemsXmlNode != null)
                {
                    //parse out the item ids separated by comma's in the xml node and convert them to ItemId objects
                    string roomItems = roomItemsXmlNode.InnerText;
                    string[] roomItemStrings = roomItems.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string roomItemString in roomItemStrings)
                    {
                        uint rawUIntValue = 0;
                        try
                        {
                            rawUIntValue = Convert.ToUInt32(roomItemString);
                            itemIds.Add(new ItemId(rawUIntValue));
                        }
                        catch (System.Exception ex)
                        {
							StateServerAssert.Assert(ex);
							return false;
                        }
                    }
                }
                else
                {
					StateServerAssert.Assert(new System.Exception("Error: Could not find RoomDna/Items in the xml document."));
					return false;
                }
                return true;
            }
            return false;
        }

        public static bool GetRoomInfoFromRoomXmlNode(XmlNode roomNode, out AccountId accountId, out PrivacyLevel privacyLevel, out string roomName, out RoomId roomId, out RoomType roomType)
        {
            roomId = null;
            roomName = String.Empty;
            roomType = RoomType.Default;
            accountId = null;
			privacyLevel = PrivacyLevel.Default;

            XmlAttributeCollection attributes = roomNode.Attributes;
            XmlAttribute roomIdAttribute = attributes["RoomId"];
            if (roomIdAttribute != null)
            {
                try
                {
                    roomId = new RoomId(Convert.ToUInt32(roomIdAttribute.Value));
                }
                catch (System.Exception ex)
                {
					StateServerAssert.Assert(ex);
					return false;
                }
            }
            else
            {
				StateServerAssert.Assert(new System.Exception("RoomId xml attribute found in the Room xml node"));
				return false;
            }

            XmlAttribute accountIdAttribute = attributes["AccountId"];
            if (accountIdAttribute != null)
            {
                try
                {
                    uint accountIdUInt = Convert.ToUInt32(accountIdAttribute.Value);
                    accountId = new AccountId(accountIdUInt);
                }
                catch (System.Exception ex)
                {
					StateServerAssert.Assert(ex);
					return false;
                }
            }
            else
            {
				StateServerAssert.Assert(new System.Exception("AccountId xml attribute not found in the Room xml node"));
				return false;
            }

            XmlAttribute privacyLevelAttribute = attributes["PrivacyLevel"];
            if (privacyLevelAttribute != null)
            {
                try
                {
                    privacyLevel = (PrivacyLevel)(Convert.ToInt32(privacyLevelAttribute.Value));
                }
                catch (System.Exception ex)
                {
					StateServerAssert.Assert(ex);
					return false;
                }
            }
            else
            {
				StateServerAssert.Assert(new System.Exception("PrivacyLevel xml attribute not found in the Room xml node"));
				return false;
            }

            XmlNode roomDnaXmlNode = roomNode.SelectSingleNode("RoomDna");
            if (roomDnaXmlNode != null)
            {
                XmlAttribute roomTypeAttribute = roomDnaXmlNode.Attributes["RoomType"];
                try
                {
                    roomType = (RoomType)Enum.Parse(typeof(RoomType), roomTypeAttribute.Value);
                }
                catch (System.Exception ex)
                {
					StateServerAssert.Assert(ex);
					return false;
                }
                roomName = roomDnaXmlNode.Attributes["RoomName"].Value;
            }
            else
            {
				StateServerAssert.Assert(new System.Exception("RoomDna xml node not found in the Room xml node"));
				return false;
            }
			return true;
        }

		public static List<RoomProperties> GetRoomsPropertiesFromXml(XmlDocument xmlResponse, IServerAssetRepository serverAssetRepository)
		{
			//store available rooms that are rooms marked as Enabled="true" in the xml
			List<RoomProperties> availableRooms = new List<RoomProperties>();

			//look for all Room Nodes in the xml response
			XmlNodeList roomNodeListXml = xmlResponse.SelectNodes("Rooms/Room");
			foreach (XmlNode roomXmlNode in roomNodeListXml)
			{
				RoomProperties room = GetRoomPropertiesFromXml(roomXmlNode, serverAssetRepository);
				if (room != null)
				{
					availableRooms.Add(room);
				}
			}

			return availableRooms;
		}

		/// <summary>
		/// this expects a document with a top level <Room> </Room> xml node
		/// </summary>
		/// <param name="roomXmlDocument"></param>
		/// <returns></returns>
		public static RoomProperties GetRoomPropertiesFromXml(XmlDocument roomXmlDocument, IServerAssetRepository serverAssetRepository)
		{
			XmlNode roomXmlNode = roomXmlDocument.SelectSingleNode("Rooms/Room");
			if (roomXmlNode != null)
			{
				//the room node is parsed out of the xml doc and passed to the GetRoomFromXml(XmlNode) function
				return GetRoomPropertiesFromXml(roomXmlNode, serverAssetRepository);
			}
			else
			{
				StateServerAssert.Assert(new System.Exception("Error: could not find top level Room node"));
			}
			return null;
		}

		public static RoomProperties GetRoomPropertiesFromXml(XmlNode roomXmlNode, IServerAssetRepository serverAssetRepository)
		{
			RoomProperties returnRoomProperties = null;
			string roomName = string.Empty;
			RoomId roomId = null;
			RoomType roomType = RoomType.Default;
			List<ItemId> itemIds = new List<ItemId>();
			AccountId accountId = null;
			PrivacyLevel privacyLevel = PrivacyLevel.Default;

			//if we find a room that's marked as Enabled="true", load it into memory
			if (RoomXmlUtil.GetRoomInfoFromRoomXmlNode(roomXmlNode, out accountId, out privacyLevel, out roomName, out roomId, out roomType, out itemIds))
			{
				XmlDocument roomItemsDnaXml = serverAssetRepository.GetXmlDna(itemIds);
				returnRoomProperties = new RoomProperties(accountId, roomName, roomId, roomType, privacyLevel, roomItemsDnaXml);

			}
			return returnRoomProperties;
		}
    }
}
