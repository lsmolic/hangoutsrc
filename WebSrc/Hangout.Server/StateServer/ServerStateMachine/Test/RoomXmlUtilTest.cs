using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Server;

namespace Hangout.Shared.UnitTest
{
    [TestFixture]
    public class RoomXmlUtilTest
    {
        [Test]
        public void TestCreateEmptyRoomDnaXml()
        {
            XmlDocument emptyRoomDnaXml = RoomXmlUtil.CreateEmptyRoomDnaXml();

            Assert.Equals(emptyRoomDnaXml.OuterXml, "<RoomDna RoomName=\"\" RoomType=\"\"><Items></Items></RoomDna>");
        }

        [Test]
        public void TestGetRoomInfoFromRoomXmlNode()
        {
            string mockXmlDocString = "<Room RoomId=\"15\" IsEnabled=\"1\" AccountId=\"1\" PrivacyLevel=\"1\"><RoomDna RoomName=\"MockNode\" RoomType=\"GreenScreenRoom\"> <Items></Items> </RoomDna></Room>";
            XmlDocument mockXmlDoc = new XmlDocument();
            mockXmlDoc.LoadXml(mockXmlDocString);
            XmlNode roomNode = mockXmlDoc.SelectSingleNode("Room");
            
            string roomName = string.Empty;
            RoomId roomId = null;
            RoomType roomType;
            AccountId accountId = null;
            PrivacyLevel privacyLevel;

            RoomXmlUtil.GetRoomInfoFromRoomXmlNode(roomNode, out accountId, out privacyLevel, out roomName, out roomId, out roomType);
            Assert.Equals(roomName, "MockNode");
            Assert.Equals(roomId, new RoomId(15));
            Assert.Equals(roomType, RoomType.GreenScreenRoom);
            Assert.Equals(accountId.Value, 1);
            Assert.Equals(privacyLevel, PrivacyLevel.Private);
            
            List<ItemId> itemIds = null;
            RoomXmlUtil.GetRoomInfoFromRoomXmlNode(roomNode, out accountId, out privacyLevel, out roomName, out roomId, out roomType, out itemIds);

            Assert.Equals(itemIds.Count, 0);

            string mockXmlDocWithItemsString = "<Room RoomId=\"15\" IsEnabled=\"1\" AccountId=\"1\" PrivacyLevel=\"1\"> <RoomDna RoomName=\"MockNode\" RoomType=\"GreenScreenRoom\"> <Items> 1 , 3, 4, 5 </Items> </RoomDna></Room>";
            mockXmlDoc.LoadXml(mockXmlDocWithItemsString);
            roomNode = mockXmlDoc.SelectSingleNode("Room");

            bool isEnabled = RoomXmlUtil.GetRoomInfoFromRoomXmlNode(roomNode, out accountId, out privacyLevel, out roomName, out roomId, out roomType, out itemIds);

            Assert.Equals(isEnabled, true);

            Assert.Equals(itemIds.Count, 4);
            Assert.IsTrue(itemIds.Contains(new ItemId(1)));
            Assert.IsTrue(itemIds.Contains(new ItemId(3)));
            Assert.IsTrue(itemIds.Contains(new ItemId(4)));
            Assert.IsTrue(itemIds.Contains(new ItemId(5)));

            string mockXmlDocNotEnabled = "<Room RoomId=\"15\" IsEnabled=\"0\" AccountId=\"1\" PrivacyLevel=\"0\"> <RoomDna RoomName=\"MockNode\" RoomType=\"GreenScreenRoom\"> <Items> 1 , 3, 4, 5 </Items> </RoomDna></Room>";
            mockXmlDoc.LoadXml(mockXmlDocNotEnabled);
            roomNode = mockXmlDoc.SelectSingleNode("Room");

            isEnabled = RoomXmlUtil.GetRoomInfoFromRoomXmlNode(roomNode, out accountId, out privacyLevel, out roomName, out roomId, out roomType, out itemIds);

            Assert.Equals(isEnabled, false);
            Assert.Equals(privacyLevel, PrivacyLevel.Public);
        }

        [Test]
        public void TestGetRoomInfoFromRoomXmlNodeMalformedXml()
        {
            XmlDocument mockXmlDoc = new XmlDocument();
            string roomName = string.Empty;
            RoomId roomId = null;
            RoomType roomType;
            List<ItemId> itemIds = null;
            AccountId accountId = null;
            PrivacyLevel privacyLevel;

            string missingRoomId = "<Room IsEnabled=\"1\" AccountId=\"1\"><RoomDna RoomName=\"MockNode\" RoomType=\"GreenScreenRoom\"> <Items></Items> </RoomDna></Room>";
            mockXmlDoc.LoadXml(missingRoomId);
            XmlNode roomNodeMissingRoomId = mockXmlDoc.SelectSingleNode("Room");
            bool caughtException = false;
            try
            {
                bool isEnabled = RoomXmlUtil.GetRoomInfoFromRoomXmlNode(roomNodeMissingRoomId, out accountId, out privacyLevel, out roomName, out roomId, out roomType, out itemIds);
            }
            catch (System.Exception)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);

            string missingIsEnabled = "<Room RoomId=\"15\" AccountId=\"1\"><RoomDna RoomName=\"MockNode\" RoomType=\"GreenScreenRoom\"> <Items></Items> </RoomDna></Room>";
            mockXmlDoc.LoadXml(missingIsEnabled);
            XmlNode roomNodeMissingIsEnabled = mockXmlDoc.SelectSingleNode("Room");
            caughtException = false;
            try
            {
                bool isEnabled = RoomXmlUtil.GetRoomInfoFromRoomXmlNode(roomNodeMissingIsEnabled, out accountId, out privacyLevel, out roomName, out roomId, out roomType, out itemIds);
            }
            catch (System.Exception)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);

            string missingAccountId = "<Room RoomId=\"15\" IsEnabled=\"1\"><RoomDna RoomName=\"MockNode\" RoomType=\"GreenScreenRoom\"> <Items></Items> </RoomDna></Room>";
            mockXmlDoc.LoadXml(missingAccountId);
            XmlNode roomNodeMissingAccountId = mockXmlDoc.SelectSingleNode("Room");
            caughtException = false;
            try
            {
                bool isEnabled = RoomXmlUtil.GetRoomInfoFromRoomXmlNode(roomNodeMissingAccountId, out accountId, out privacyLevel, out roomName, out roomId, out roomType, out itemIds);
            }
            catch (System.Exception)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);


            string missingRoomDna = "<Room RoomId=\"15\" IsEnabled=\"1\"></Room>";
            mockXmlDoc.LoadXml(missingRoomDna);
            XmlNode roomNodeMissingRoomDna = mockXmlDoc.SelectSingleNode("Room");
            caughtException = false;
            try
            {
                bool isEnabled = RoomXmlUtil.GetRoomInfoFromRoomXmlNode(roomNodeMissingRoomDna, out accountId, out privacyLevel, out roomName, out roomId, out roomType, out itemIds);
            }
            catch (System.Exception)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);

            string missingItems = "<Room RoomId=\"15\" IsEnabled=\"1\" AccountId=\"1\"><RoomDna RoomName=\"MockNode\" RoomType=\"GreenScreenRoom\"></RoomDna></Room>";
            mockXmlDoc.LoadXml(missingItems);
            XmlNode roomNodeMissingItems = mockXmlDoc.SelectSingleNode("Room");
            caughtException = false;
            try
            {
                bool isEnabled = RoomXmlUtil.GetRoomInfoFromRoomXmlNode(roomNodeMissingItems, out accountId, out privacyLevel, out roomName, out roomId, out roomType, out itemIds);
            }
            catch (System.Exception)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);

            string missingPrivacy = "<Room RoomId=\"15\" IsEnabled=\"1\" AccountId=\"1\"><RoomDna RoomName=\"MockNode\" RoomType=\"GreenScreenRoom\"> <Items></Items> </RoomDna></Room>";
            mockXmlDoc.LoadXml(missingPrivacy);
            XmlNode roomNodeMissingPrivacy = mockXmlDoc.SelectSingleNode("Room");
            caughtException = false;
            try
            {
                bool isEnabled = RoomXmlUtil.GetRoomInfoFromRoomXmlNode(roomNodeMissingPrivacy, out accountId, out privacyLevel, out roomName, out roomId, out roomType, out itemIds);
            }
            catch (System.Exception)
            {
                caughtException = true;
            }
            Assert.IsTrue(caughtException);

        }

    }
}
