using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Server;

namespace Hangout.Shared.UnitTest
{
    public class TestRoomManager : RoomManager
    {
        public TestRoomManager(ServerObjectRepository serverObjectRepository, ZoneIdManager zoneIdManager, DistributedObjectIdManager distributedObjectIdManager, ServerEngine serverEngine, SessionManager sessionManager, ServerAssetRepository serverAssetRepository, Hangout.Shared.Action<Message, Guid> sendMessageToClientCallback)
        : base (null) 
        {

        }        
        public override void CreateNewRoomInDatabase(Guid sessionId, string roomName, RoomType roomType, PrivacyLevel privacyLevel, System.Action<IServerDistributedRoom> createRoomFinishedCallback)
        {
            createRoomFinishedCallback(null);
        }
		protected override void SendClientAvailableRooms(Guid sessionId, MessageSubType roomRequestType)
        {

        }

    }

    [TestFixture]
    public class RoomManagerTest
    {
        [Test]
        public void CreateRoomFromRoomPurchaseSuccessTest()
        {
            string roomPurchase = "<Response noun=\"HangoutUsers\" verb=\"PurchaseItems\"><purchaseResults userId=\"551\" purchaseDate=\"2009/10/07 15:40:22 PDT\" eventGroupId=\"3137\" accountId=\"1138\" accountBalance=\"380.0000\" externalTxnId=\"\"><purchaseResult><offer endDate=\"2050/01/01 00:00:00 PST\" numAvailable=\"-1\" startDate=\"2009/10/07 14:55:48 PDT\" name=\"Talk each others ears off under a Starry Night\" title=\"Talk each others ears off under a Starry Night\" description=\"Talk each others ears off under a Starry Night\" id=\"2224\" special=\"false\" specialType=\"\" type=\"ITEM_OFFER\"><item id=\"1060\" name=\"105\" appName=\"HANGOUT_FAMILY_Application_1\" itemTypeName=\"Room\" buttonName=\"Starry Night\" description=\"Talk each others ears off under a Starry Night\" smallImageUrl=\"http://s3.amazonaws.com/HangoutDevFileBucket/48141003.jpg\" mediumImageUrl=\"\" largeImageUrl=\"http://s3.amazonaws.com/HangoutDevFileBucket/48141003.jpg\" available=\"-1\"><properties><property key=\"Description\" value=\"Talk each others ears off under a Starry Night\" /><property key=\"SmallImageURL\" value=\"http://s3.amazonaws.com/HangoutDevFileBucket/48141003.jpg\" /><property key=\"MediumImageURL\" value=\"\" /><property key=\"ButtonName\" value=\"Starry Night\" /><property key=\"LargeImageURL\" value=\"http://s3.amazonaws.com/HangoutDevFileBucket/48141003.jpg\" /></properties></item><price><money currencyId=\"124456\" currencyName=\"VCOIN\" amount=\"10.0000\" /></price><tradeInItems /></offer><price><money currencyId=\"124456\" currencyName=\"VCOIN\" amount=\"10.0000\" /></price><itemInstances><itemInstance id=\"1145\" createdDate=\"2009/10/07 15:40:22 PDT\" userId=\"551\" appId=\"12475196524081\" appName=\"HANGOUT_FAMILY_Application_1\" itemId=\"1060\" itemName=\"105\" itemTypeId=\"71\" itemTypeName=\"Room\" gift=\"false\" giftGiverUserId=\"\" giftVisibility=\"\" giftMessage=\"\"><properties /></itemInstance></itemInstances><tradeInItemInstances /></purchaseResult></purchaseResults><accounts><account id=\"1070\" currencyId=\"1247519653409\" currencyName=\"HOUTS\" balance=\"130.0000\" /><account id=\"1138\" currencyId=\"124456\" currencyName=\"VCOIN\" balance=\"380.0000\" /></accounts></Response>";

            XmlDocument response = new XmlDocument();
            response.LoadXml(roomPurchase);

            Guid testGuid = Guid.Empty;

            // A successful room purchase
            RoomManager roomManager = new TestRoomManager(null, null, null, null, null, null, null);
            XmlNodeList purchasedRoomItems = response.SelectNodes("/Response/purchaseResults/purchaseResult/offer/item[@itemTypeName='" + ItemType.ROOM + "']");
            bool roomCreated = roomManager.RoomItemPurchaseCallback(purchasedRoomItems, testGuid);
            Assert.IsTrue(roomCreated);
        }

        [Test]
        public void CreateRoomFromPurchaseBadPurchaseTest()
        {
            Guid testGuid = Guid.Empty;
            RoomManager roomManager = new TestRoomManager(null, null, null, null, null, null, null);
            string purchaseError = "<Response noun=\"HangoutUsers\" verb=\"PurchaseItems\"></Response>";
            // No purchase went through
            XmlDocument badresponse = new XmlDocument();
            badresponse.LoadXml(purchaseError);
            XmlNodeList purchasedRoomItems = badresponse.SelectNodes("/Response/purchaseResults/purchaseResult/offer/item[@itemTypeName='" + ItemType.ROOM + "']");
            bool roomCreated = roomManager.RoomItemPurchaseCallback(purchasedRoomItems, testGuid);
            Assert.IsFalse(roomCreated);
        }

        [Test]
        public void CreateRoomFromPurchaseOtherPurchaseTest()
        {
            Guid testGuid = Guid.Empty;
            RoomManager roomManager = new TestRoomManager(null, null, null, null, null, null, null);
            string roomBackgroundPurchase = "<Response noun=\"HangoutUsers\" verb=\"PurchaseItems\"><purchaseResults userId=\"551\" purchaseDate=\"2009/10/07 15:40:22 PDT\" eventGroupId=\"3137\" accountId=\"1138\" accountBalance=\"380.0000\" externalTxnId=\"\"><purchaseResult><offer endDate=\"2050/01/01 00:00:00 PST\" numAvailable=\"-1\" startDate=\"2009/10/07 14:55:48 PDT\" name=\"Talk each others ears off under a Starry Night\" title=\"Talk each others ears off under a Starry Night\" description=\"Talk each others ears off under a Starry Night\" id=\"2224\" special=\"false\" specialType=\"\" type=\"ITEM_OFFER\"><item id=\"1060\" name=\"105\" appName=\"HANGOUT_FAMILY_Application_1\" itemTypeName=\"Room Backdrop\" buttonName=\"Starry Night\" description=\"Talk each others ears off under a Starry Night\" smallImageUrl=\"http://s3.amazonaws.com/HangoutDevFileBucket/48141003.jpg\" mediumImageUrl=\"\" largeImageUrl=\"http://s3.amazonaws.com/HangoutDevFileBucket/48141003.jpg\" available=\"-1\"><properties><property key=\"Description\" value=\"Talk each others ears off under a Starry Night\" /><property key=\"SmallImageURL\" value=\"http://s3.amazonaws.com/HangoutDevFileBucket/48141003.jpg\" /><property key=\"MediumImageURL\" value=\"\" /><property key=\"ButtonName\" value=\"Starry Night\" /><property key=\"LargeImageURL\" value=\"http://s3.amazonaws.com/HangoutDevFileBucket/48141003.jpg\" /></properties></item><price><money currencyId=\"124456\" currencyName=\"VCOIN\" amount=\"10.0000\" /></price><tradeInItems /></offer><price><money currencyId=\"124456\" currencyName=\"VCOIN\" amount=\"10.0000\" /></price><itemInstances><itemInstance id=\"1145\" createdDate=\"2009/10/07 15:40:22 PDT\" userId=\"551\" appId=\"12475196524081\" appName=\"HANGOUT_FAMILY_Application_1\" itemId=\"1060\" itemName=\"105\" itemTypeId=\"71\" itemTypeName=\"Room Backdrop\" gift=\"false\" giftGiverUserId=\"\" giftVisibility=\"\" giftMessage=\"\"><properties /></itemInstance></itemInstances><tradeInItemInstances /></purchaseResult></purchaseResults><accounts><account id=\"1070\" currencyId=\"1247519653409\" currencyName=\"HOUTS\" balance=\"130.0000\" /><account id=\"1138\" currencyId=\"124456\" currencyName=\"VCOIN\" balance=\"380.0000\" /></accounts></Response>";
            // Purchase went through but for something other than a room
            XmlDocument otherpurchase = new XmlDocument();
            otherpurchase.LoadXml(roomBackgroundPurchase);

            XmlNodeList purchasedRoomItems = otherpurchase.SelectNodes("/Response/purchaseResults/purchaseResult/offer/item[@itemTypeName='" + ItemType.ROOM + "']");
            bool roomCreated = roomManager.RoomItemPurchaseCallback(purchasedRoomItems, testGuid);
            Assert.IsFalse(roomCreated);
        }
    }
}
