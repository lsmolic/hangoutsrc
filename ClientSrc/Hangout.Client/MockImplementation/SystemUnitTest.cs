using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading;
using Hangout.Shared.UnitTest;
using Hangout.Shared;

namespace Hangout.Client
{
    [TestFixture]
    public class SystemUnitTest : SystemUnitTestsBase
    {
        /*
        [Test]
        public void StateServerConnect()
        {
            RegisterMessage(MessageType.Connect, StateServerConnectReturn);
            ConnectToStateServer();
        }

        private void StateServerConnectReturn(Message message)
        {
            base.ReceiveMessage(message);
            SessionId = (Guid)message.Data[0];
            TestComplete();
        }
        */

        [Test]
	    public void StateServerAlive()
        {
            RegisterMessage(MessageType.Heartbeat, StateServerAliveReturn);
            Message heartbeatMessage = new Message(MessageType.Heartbeat, new List<object>());
            StartTest(heartbeatMessage);
        }

        private void StateServerAliveReturn(Message message)
        {
            base.ReceiveMessage(message);
            TestComplete();
        }

        [Test]
        public void PaymentItemsAlive()
        {
            Dictionary<string, string> commandParameters = new Dictionary<string, string>();

            RegisterMessage(MessageType.PaymentItems, PaymentItemsAliveReturn);
            ProcessPaymentItemCommand("HealthCheck", commandParameters);
        }


        private void PaymentItemsAliveReturn(Message message)
        {
            string data = base.ReceiveMessage(message).ToString();
            XmlDocument returnDoc = VerifyXMLDocument(data);
            SelectSingleNodeWithVerify(returnDoc, "/Response/HealthCheck/@tfel", "OK");
            SelectSingleNodeWithVerify(returnDoc, "/Response/HealthCheck/@aes", "OK");
            SelectSingleNodeWithVerify(returnDoc, "/Response/HealthCheck/@eazyedb", "OK");
            SelectSingleNodeWithVerify(returnDoc, "/Response/HealthCheck/@PaymentItems", "OK");
            TestComplete();
        }

        [Test]
        public void ServerStoresCombinedTest()
        {
            Dictionary<string, string> commandParameters = new Dictionary<string, string>();
            commandParameters.Add("storeName", "Hangout_Store_Combined");

            RegisterMessage(MessageType.PaymentItems, ServerStoresReturn);
            ProcessPaymentItemCommand("GetStoreInventory", commandParameters);
        }

        private void ServerStoresReturn(Message message)
        {
            string data = base.ReceiveMessage(message).ToString();
            XmlDocument returnDoc = VerifyXMLDocument(data);

            List<string> xPathVerifyList = new List<string>();
            xPathVerifyList.Add("item");
            xPathVerifyList.Add("item/Assets/Asset");
            xPathVerifyList.Add("price/money");
            SelectNodeListWithVerify(returnDoc, "/Response/store/itemOffers/itemOffer", xPathVerifyList);
            TestComplete();
        }

        [Test]
        public void LoginUser()
        {
            RegisterMessage(MessageType.Connect, LoginUserReturn);

            List<object> data = new List<object>();
            data.Add(GetConfigFileAppSettingWithAssert("Login", "MockFacebookAccountId"));
            data.Add(GetConfigFileAppSettingWithAssert("Login", "MockFacebookSessionKey"));
            data.Add(GetConfigFileAppSettingWithAssert("Login", "MockFacebookNickName"));
            data.Add(GetConfigFileAppSettingWithAssert("Login", "MockFacebookFirstName")); ; //first name
            data.Add(GetConfigFileAppSettingWithAssert("Login", "MockFacebookLastName")); ; //last name
            data.Add(GetConfigFileAppSettingWithAssert("Login", "MockFacebookCampaignId")); ; //campaign
            data.Add(GetConfigFileAppSettingWithAssert("Login", "MockFacebookReferrerId")); ; //referrerId
            data.Add(ConvertStringToAvatarIdWithAssert(GetConfigFileAppSettingWithAssert("Login", "MockDefaultAvatarId")));

            Message loginMessage = new Message(MessageType.Connect, MessageSubType.RequestLogin, data);
            StartTest(loginMessage);
        }

        private void LoginUserReturn(Message message)
        {
            string data = base.ReceiveMessage(message).ToString();
            VerifyStringData(message.Data[0]);
            SessionId = (Guid)message.Data[0];
            VerifyStringData(message.Data[1]);
            TestComplete();
        }

        [Test]
        public void UserInvertoryTest()
        {
            Dictionary<string, string> commandParameters = new Dictionary<string, string>();
            commandParameters.Add("userSession", SessionId.ToString());

            RegisterMessage(MessageType.PaymentItems, UserInvertoryReturn);
            ProcessPaymentItemCommand("GetUserInventory", commandParameters);
        }

        private void UserInvertoryReturn(Message message)
        {
            string data = (string)base.ReceiveMessage(message);
            XmlDocument returnDoc = VerifyXMLDocument(data);

            List<string> xPathVerifyList = new List<string>();
            xPathVerifyList.Add("item");
            xPathVerifyList.Add("item/Assets/Asset");
           // SelectNodeListWithVerify(returnDoc, "/Response/itemInstances/itemInstance", xPathVerifyList);
            TestComplete();
        }

        [Test]
        public void WebServicesDatabaseAlive()
        {
            RegisterMessage(MessageType.Admin, WebServicesDatabaseAliveReturn);
            Message WebServicesDatabaseAliveMessage = new Message();
            List<object> dataObject = new List<object>();

            WebServicesDatabaseAliveMessage.AdminDataMessage(dataObject);
            WebServicesDatabaseAliveMessage.Callback = (int)MessageSubType.HealthCheck;

            StartTest(WebServicesDatabaseAliveMessage);
        }

        private void WebServicesDatabaseAliveReturn(Message message)
        {
            string data = base.ReceiveMessage(message).ToString();
            XmlDocument returnDoc = VerifyXMLDocument(data);
            SelectSingleNodeWithVerify(returnDoc, "/HealthCheck/HealthCheck/@Services", "OK");
            SelectSingleNodeWithVerify(returnDoc, "/HealthCheck/HealthCheck/@AccountsDB", "OK");
            SelectSingleNodeWithVerify(returnDoc, "/HealthCheck/HealthCheck/@InventoryDB", "OK");
            SelectSingleNodeWithVerify(returnDoc, "/HealthCheck/HealthCheck/@AvatarsDB", "OK");
            SelectSingleNodeWithVerify(returnDoc, "/HealthCheck/HealthCheck/@RoomsDB", "OK");
            SelectSingleNodeWithVerify(returnDoc, "/HealthCheck/HealthCheck/@MiniGameDB", "OK");
            SelectSingleNodeWithVerify(returnDoc, "/HealthCheck/HealthCheck/@LoggingDB", "OK");
            TestComplete();
         }

    }
}
