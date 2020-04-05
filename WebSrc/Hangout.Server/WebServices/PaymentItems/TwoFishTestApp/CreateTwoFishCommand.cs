using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;
using System.Configuration;
using Hangout.Server;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    public class CreateTwoFishCommand
    {
        public PaymentCommand CreateUser(string userId)
        {
            string name = RandomString(6, true);
            string dob = RandomDate(new DateTime(1960, 1, 1), new DateTime(1995, 12, 31));

            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Users";
            cmd.Verb = "CreateUser";

            //name = "ddmqcg";

            cmd.Parameters.Add("name", name);
            cmd.Parameters.Add("namespaceId", "0");
           // cmd.Parameters.Add("countryCode", "US");
           // cmd.Parameters.Add("externalKey", "e31df244-df7d-41d9-bfff-309966f216dc");  //Guid.NewGuid().ToString());
          //  cmd.Parameters.Add("gender", "M");
           // cmd.Parameters.Add("dateOfBirth", dob);
          //  cmd.Parameters.Add("emailAddress", name + "@hangout.net");
            //cmd.Parameters.Add("tags", "test,tags");
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }


        public PaymentCommand UpdateUser(string userId)
        {
            string name = RandomString(6, true);
            string dob = RandomDate(new DateTime(1960, 1, 1), new DateTime(1995, 12, 31));

            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Users";
            cmd.Verb = "UpdateUser";

            cmd.Parameters.Add("userId", "477");
            cmd.Parameters.Add("countryCode", "US");
            cmd.Parameters.Add("externalKey", "2c92e4ff-4fa8-491d-9d25-6c17056f44ce");
            cmd.Parameters.Add("gender", "M");
            cmd.Parameters.Add("dateOfBirth", dob);
            cmd.Parameters.Add("emailAddress", name + "@hangout.net");
            cmd.Parameters.Add("tags", "new");
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }


        public PaymentCommand UserInfo(string userId, Form1 form1)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Users";
            cmd.Verb = "UserInfo";

            cmd.Parameters.Add("userId", GetUserId(userId, form1.GetUserList));

            return cmd;
        }

        public PaymentCommand SecureKey(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Users";
            cmd.Verb = "SecureKey";

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }


        public PaymentCommand UserWidget(string userId, Form1 form1)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Users";
            cmd.Verb = "UserWidget";

            cmd.Parameters.Add("userId", GetUserId(userId, form1.GetUserList));

            string cashCurrencyId = ConfigurationManager.AppSettings["CashCurrencyId"];
            cmd.Parameters.Add("virtualCurrencyId", cashCurrencyId);

            return cmd;
        }


        public PaymentCommand PaymentOptions(string userId, Form1 form1)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Users";
            cmd.Verb = "PaymentOptions";

            cmd.Parameters.Add("userId", GetUserId(userId, form1.GetUserList));

            string cashCurrencyId = ConfigurationManager.AppSettings["CashCurrencyId"];
            cmd.Parameters.Add("PaymentGatewayName", "ZongPaymentGateway");
            cmd.Parameters.Add("virtualCurrencyIds", cashCurrencyId);
            cmd.Parameters.Add("countryCode", "US");
            cmd.Parameters.Add("currencyCode", "USD");

            return cmd;
        }


        public PaymentCommand FindItem(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Items";
            cmd.Verb = "FindItem";

            cmd.Parameters.Add("itemName", "Blue Jeans");
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }


        public PaymentCommand CreateItem(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Items";
            cmd.Verb = "CreateItem";

            string itemName = "Blue Jeans New";
            string itemTypeName = "Pants";
            string buttonName = "Blue Jeans New";
            string description = "Blue Jeans New";
            string smallImageUrl = "";
            string mediumImageUrl = "";
            string largeImageUrl = "";
            string available = "10";

            cmd.Parameters.Add("itemName", itemName);
            cmd.Parameters.Add("itemTypeName", itemTypeName);
            cmd.Parameters.Add("buttonName", buttonName);
            cmd.Parameters.Add("description", description);
            cmd.Parameters.Add("smallImageUrl", smallImageUrl);
            cmd.Parameters.Add("mediumImageUrl", mediumImageUrl);
            cmd.Parameters.Add("largeImageUrl", largeImageUrl);
            cmd.Parameters.Add("available", available);
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }


        public PaymentCommand UpdateItem(string userId, object form1, XmlDocument oldCommand)
        {
    
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Items";
            cmd.Verb = "UpdateItem";
            string xPath = "/Response/item";

            string itemName = "Blue Jeans";
            string itemTypeName = "Pants";
            string buttonName = "Blue Jeans";
            string description = "Blue Jeans";
            string smallImageUrl = "";
            string mediumImageUrl = "";
            string largeImageUrl = "";
            string available = "-1";

            if (oldCommand != null)
            {
                itemName = GetXMLDocumentParameter(oldCommand, xPath, "name");
                itemTypeName = GetXMLDocumentParameter(oldCommand, xPath, "itemTypeName");
                buttonName = GetXMLDocumentParameter(oldCommand, xPath, "buttonName");
                description = GetXMLDocumentParameter(oldCommand, xPath, "description");
                smallImageUrl = GetXMLDocumentParameter(oldCommand, xPath, "smallImageUrl");
                mediumImageUrl = GetXMLDocumentParameter(oldCommand, xPath, "mediumImageUrl");
                largeImageUrl = GetXMLDocumentParameter(oldCommand, xPath, "largeImageUrl");
                available = "50"; // GetXMLDocumentParameter(oldCommand, xPath, "available");
            }

            cmd.Parameters.Add("itemName", itemName);
            cmd.Parameters.Add("itemTypeName", itemTypeName);
            cmd.Parameters.Add("buttonName", buttonName);
            cmd.Parameters.Add("description", description);
            cmd.Parameters.Add("smallImageUrl", smallImageUrl);
            cmd.Parameters.Add("mediumImageUrl", mediumImageUrl);
            cmd.Parameters.Add("largeImageUrl", largeImageUrl);
            cmd.Parameters.Add("available", available);
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }


        public PaymentCommand ItemPrices(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Items";
            cmd.Verb = "ItemPrices";

            cmd.Parameters.Add("itemName", "Blue Jeans");
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }

        public PaymentCommand ItemInstance(string userId, Form1 form1)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Items";
            cmd.Verb = "ItemInstance";

            cmd.Parameters.Add("userId", GetUserId(userId, form1.GetUserList));
            // cmd.Parameters.Add("itemName", "White Shirt");
            cmd.Parameters.Add("filter", "all");
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }

        public PaymentCommand ItemTypes(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Items";
            cmd.Verb = "ItemTypes";

            cmd.Parameters.Add("storeName", GetStoreName());

            return cmd;
        }

        public PaymentCommand PayPalCheckout(string userId, Form1 form1)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Purchase";
            cmd.Verb = "PayPalCheckout";

            cmd.Parameters.Add("userId", GetUserId(userId, form1.GetUserList));
            cmd.Parameters.Add("offerId", "29");
            cmd.Parameters.Add("externalTxnId", Guid.NewGuid().ToString());
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }

        public PaymentCommand PayPalPurchase(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Purchase";
            cmd.Verb = "PayPalPurchase";

            cmd.Parameters.Add("userId", "545");
            cmd.Parameters.Add("offerId", "29");
            cmd.Parameters.Add("token", "EC-15P20606JT6360714");
            cmd.Parameters.Add("payerId", "LGAMUNKPUYPUW");
            cmd.Parameters.Add("externalTxnId", "d0d88375-fb2b-4876-abc9-06e46149e1b2");
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }

        public PaymentCommand PayPalCancel(string userId, Form1 form1)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Purchase";
            cmd.Verb = "PayPalCancel";

            cmd.Parameters.Add("userId", GetUserId(userId, form1.GetUserList));
            cmd.Parameters.Add("offerId", "29");
            cmd.Parameters.Add("payerId", "12345");
            cmd.Parameters.Add("externalTxnId", Guid.NewGuid().ToString());
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }


        public PaymentCommand PayPalRecurringCheckout(string userId, Form1 form1)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Purchase";
            cmd.Verb = "PayPalRecurringCheckout";

            string tomorrow = new System.DateTime(System.DateTime.Today.AddDays(1).Ticks).ToString("yyyy/MM/dd");         //YYYY/MM/DD

            cmd.Parameters.Add("userId", GetUserId(userId, form1.GetUserList));
            cmd.Parameters.Add("offerId", "29");
            cmd.Parameters.Add("externalTxnId", Guid.NewGuid().ToString());
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());
            cmd.Parameters.Add("billingDescription", "Customer will be billed at $1 per month for 1 years");
            cmd.Parameters.Add("startDate",  tomorrow);
            cmd.Parameters.Add("numPayments", "12");
            cmd.Parameters.Add("payFrequency", "MONTHLY");

            return cmd;
        }

        public PaymentCommand CreditCardPurchase(string userId, Form1 form1, XmlDocument oldCommand)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Purchase";  
            cmd.Verb = "PurchaseCreditCard";

            userId = GetUserId(userId, form1.GetUserList);
            string hangoutAccountId = GetHangoutId(userId, form1.GetUserList);
            string secureKey = GetSecureKey(hangoutAccountId);

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("secureKey", secureKey);
            cmd.Parameters.Add("offerId", "29");
            cmd.Parameters.Add("externalTxnId", Guid.NewGuid().ToString());
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());
            cmd.Parameters.Add("creditCardNumber", "4108026996140735");
            cmd.Parameters.Add("creditCardType", "VISA");
            cmd.Parameters.Add("expireDate", "072019");
            cmd.Parameters.Add("securityCode", "000");
            cmd.Parameters.Add("firstName", "Test");
            cmd.Parameters.Add("lastName", "User");
            cmd.Parameters.Add("address", "1 Main St");
            cmd.Parameters.Add("city", "San Jose");
            cmd.Parameters.Add("state", "CA");
            cmd.Parameters.Add("zipCode", "95131");
            cmd.Parameters.Add("countryCode", "US");
            cmd.Parameters.Add("phoneNumber", "4086780945");

            return cmd;
        }

        public PaymentCommand CreditCardPurchaseRecurring(string userId, Form1 form1, XmlDocument docSecureKey)
        {
            string tomorrow = new System.DateTime(System.DateTime.Today.AddDays(1).Ticks).ToString("yyyy/MM/dd");         //YYYY/MM/DD

            PaymentCommand cmd = CreditCardPurchase(userId, form1, docSecureKey);
            cmd.Verb = "PurchaseCreditCardRecurring";

            cmd.Parameters.Add("startDate", tomorrow);
            cmd.Parameters.Add("numPayments", "12");
            cmd.Parameters.Add("payFrequency", "MONTHLY");

            return cmd;
        }

        public PaymentCommand CreditCardPurchaseOneClick(string userId, Form1 form1, XmlDocument docSecureKey)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Purchase";
            cmd.Verb = "PurchaseCreditCardOneClick";

            userId = GetUserId(userId, form1.GetUserList);
            string hangoutAccountId = GetHangoutId(userId, form1.GetUserList);
            string secureKey = GetSecureKey(hangoutAccountId);

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("secureKey", secureKey);
            cmd.Parameters.Add("offerId", "29");
            cmd.Parameters.Add("externalTxnId", Guid.NewGuid().ToString());
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());
            cmd.Parameters.Add("securityCode", "000");

            return cmd;
        }

        public PaymentCommand PaymentServiceURL(string userId, Form1 form1, XmlDocument docSecureKey)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Purchase";
            cmd.Verb = "PaymentServiceURL";

            userId = GetUserId(userId, form1.GetUserList);

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("offerId", "29");
            cmd.Parameters.Add("paymentGatewayConfigId", "26");
            cmd.Parameters.Add("returnURL", "http://hangoutdev.net/paypal/purchaseCallbackURL.aspx");
            cmd.Parameters.Add("cancelURL", "http://hangoutdev.net/paypal/purchaseCancelledCallbackURL.aspx");

            return cmd;
        }
        
        public PaymentCommand PurchaseHistory(string userId, Form1 form1)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Purchase";
            cmd.Verb = "PurchaseHistory";

            cmd.Parameters.Add("userId", GetUserId(userId, form1.GetUserList));

            return cmd;
        }

        public PaymentCommand FindCurrency(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Currency";
            cmd.Verb = "FindCurrency";

            // cmd.Parameters.Add("currencyId", "4");
            //cmd.Parameters.Add("currencyAppId", "1");

            return cmd;
        }

        public PaymentCommand FindTransfer(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Transfer";
            cmd.Verb = "FindTransfer";

            cmd.Parameters.Add("id", "1510");

            return cmd;
        }


        public PaymentCommand CreateTransfer(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Transfer";
            cmd.Verb = "CreateTransfer";

            cmd.Parameters.Add("amount", "1000");
            cmd.Parameters.Add("debitAccountId", "1064");
            cmd.Parameters.Add("creditAccountId", "1070");
            // cmd.Parameters.Add("transferType", "NORMAL_SALE");
            cmd.Parameters.Add("transferType", "INITIAL_AMOUNT");
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }


        public PaymentCommand StoreList(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Store";
            cmd.Verb = "StoreList";

            return cmd;
        }

        public PaymentCommand FindStore(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Store";
            cmd.Verb = "FindStore";

            cmd.Parameters.Add("storeName", GetStoreName());
           // cmd.Parameters.Add("itemTypeNames", "Furniture,Food");

           // cmd.Parameters.Add("startIndex", "9");
           // cmd.Parameters.Add("blockSize", "10");

           cmd.Parameters.Add("filter", "itemOffer_title Like '%basic%'");
           cmd.Parameters.Add("orderBy", "itemOffer_endDate desc, item_itemTypeName asc");

            return cmd;
        }

        public PaymentCommand StoreBulk(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Store";
            cmd.Verb = "StoreBulkGet";

            cmd.Parameters.Add("storeName", GetStoreName());
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }

        public PaymentCommand StoreBulkLoad(string userId, Form1 form1)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Store";
            cmd.Verb = "StoreBulkLoad";
            cmd.FileData = GetCSVFile(form1);
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }

        public PaymentCommand PurchaseItems(string userId, Form1 form1)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Store";
            cmd.Verb = "PurchaseItems";

            cmd.Parameters.Add("userId", GetUserId(userId, form1.GetUserList));
            cmd.Parameters.Add("accountId", "1070");
            cmd.Parameters.Add("offerIds", "81");
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }

        public PaymentCommand PurchaseGift(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Store";
            cmd.Verb = "PurchaseGift";

            cmd.Parameters.Add("userId", "408");
            cmd.Parameters.Add("recipientUserId", "411");
            cmd.Parameters.Add("noteToRecipient", "This is a Gift");
            cmd.Parameters.Add("accountId", "626");
            cmd.Parameters.Add("offerIds", "60");
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }

        public PaymentCommand Catalog(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Catalog";
            cmd.Verb = "FindCatalog";

            //cmd.Parameters.Add("itemTypeNames", "1,2");
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }


        public PaymentCommand CreateCatalog(string userId, Form1 form1)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Catalog";
            cmd.Verb = "CreateCatalog";
            cmd.FileData = GetCSVFile(form1);
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }

        public PaymentCommand WidgetConfig(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Widget";
            cmd.Verb = "WidgetConfig";

            return cmd;
        }

        public PaymentCommand HealthCheck(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Widget";
            cmd.Verb = "HealthCheck";

            return cmd;
        }

        public PaymentCommand SystemInfo(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Widget";
            cmd.Verb = "SystemInfo";

            return cmd;
        }

        public PaymentCommand UploadCatalogStore(string userId, Form1 form1)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Upload";
            cmd.Verb = "UploadCatalogStore";
            cmd.FileData = GetCSVFile(form1);
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }

        public PaymentCommand GetUploadCatalogStoreFile(string userId, Form1 form1)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Upload";
            cmd.Verb = "GetUploadCatalogStoreFile";
            cmd.FileData = GetCSVFile(form1);

            return cmd;
        }

        private string GetXMLDocumentParameter(XmlDocument xmlDoc, string xPath, string attribute)
        {
            return (xmlDoc.SelectSingleNode(xPath).Attributes[attribute]).InnerText;
        }

        private MemoryStream GetCSVFile(Form1 form1)
        {
            MemoryStream streamData = null;

            string fileName = form1.InvokeOpenFileDialog("CSV files|*.csv");

            if (fileName.Length > 0)
            {
                FileStream fs = File.OpenRead(fileName);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                streamData = new MemoryStream();
                streamData.Write(buffer, 0, buffer.Length);
            }

            return streamData;
        }

        private string GetUserId(string userId, Dictionary<string, string> userIdList)
        {
            if ((userId == null) || (userId.Trim().Length == 0))
            {
                List<string> userList = new List<string>();

                foreach (KeyValuePair<string, string> keyValue in userIdList)
                {
                    userList.Add(keyValue.Key);
                }

                SelectForm formSelect = new SelectForm(userList, "Select User");
                formSelect.ShowDialog();

                userId = formSelect.SelectedComboBox1;
            }
            return userId;
        }

        private string GetHangoutId(string userId, Dictionary<string, string> userIdList)
        {
            string hangoutUserId = "";
            if (userIdList.TryGetValue(userId, out hangoutUserId) == false)
            {
                hangoutUserId = "";
            }
            return hangoutUserId;
        }


        private string GetSecureKey(string hangoutAccountId)
        {
            RESTCommand restCommand = new RESTCommand();
            restCommand.Noun = "Accounts";
            restCommand.Verb = "GetAccounts";
            restCommand.Parameters.Add("encrypted", "false");
            restCommand.Parameters.Add("accountId", hangoutAccountId);

            string servicesPath = ConfigurationManager.AppSettings["ServicesBasePath"];

            RESTParser restParser = new RESTParser(servicesPath);
            string response = restParser.ProcessRestService(restCommand);

            XmlDocument responseDoc = new XmlDocument();
            responseDoc.LoadXml(response);

            string secureKey = responseDoc.SelectSingleNode("/Accounts/Account").Attributes["PISecureKey"].InnerText;

            return secureKey;
        }

        private string GetStoreName()
        {
            
            List<string> storeNameList = new List<string>();
            storeNameList.Add("Hangout_Store_Coin");
            storeNameList.Add("Hangout_Store_Cash");
            storeNameList.Add("Test_Store");
            storeNameList.Add("Test_Store_1");
            storeNameList.Add("Test_Store_2");

            SelectForm formSelect = new SelectForm (storeNameList, "Select Store");
            formSelect.ShowDialog();

            return (formSelect.SelectedComboBox1);
        }

        private string GetMyIpAddress()
        {
            string ipAddress = "";
            try
            {
                IPAddress[] ipAddrList = Dns.GetHostAddresses(Dns.GetHostName());
                if (ipAddrList.Length > 0)
                {
                    ipAddress = ipAddrList[0].ToString();
                }
            }
            catch { }

            return ipAddress;
        }

        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;

            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
            {
                return builder.ToString().ToLower();
            }
            return builder.ToString();
        }


        private string RandomDate(DateTime minDate, DateTime maxDate)
        {
            Random random = new Random();
            TimeSpan ts = maxDate - minDate;

            int totalDays = (int)Math.Floor(ts.TotalDays);
            int randomDays = random.Next(0, totalDays);
            DateTime randomDate = minDate.AddDays(randomDays);

            return (randomDate.ToString("yyyy/MM/dd"));
        }
    }
}
