using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using Hangout.Server;
using Hangout.Shared;
using System.Text.RegularExpressions;

namespace Hangout.Server.WebServices
{
    public class CreateHangoutCommand
    {
        public PaymentCommand CreateNewHangoutUser(string userId)
        {
            string name = RandomString(6, true);
            string dob = RandomDate(new DateTime(1960, 1, 1), new DateTime(1995, 12, 31));

            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutUsers";
            cmd.Verb = "CreateNewUser";

            string pattern = @"\$?[a-zA-Z\xC0-\xD6\xD8-\xF6\xF8-\xFF][a-zA-Z\xC0-\xD6\xD8-\xF6\xF8-\xFF-_\.\d]{2,63}";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            List<string> list = new List<string>();

            Match match = regex.Match(name);
            if (!match.Success)
            {
                SelectForm formSelect = new SelectForm(list, "User Id match Failed ");
                formSelect.ShowDialog();
            }
 
           // name = "PI1000011"; // "ddmqcg";

            cmd.Parameters.Add("name", name);
          //  cmd.Parameters.Add("countryCode", "US");
          //  cmd.Parameters.Add("externalKey", Guid.NewGuid().ToString());
          //  cmd.Parameters.Add("gender", "M");
          //  cmd.Parameters.Add("dateOfBirth", dob);
          //  cmd.Parameters.Add("emailAddress", name + "@hangout.net");
            cmd.Parameters.Add("ipAddress", "64.115.150.114");
            cmd.Parameters.Add("initialCoinAmount", "1500");
            cmd.Parameters.Add("initialCashAmount", "20");

            return cmd;
        }

        public PaymentCommand GetUserBalance(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutUsers";
            cmd.Verb = "GetUserBalance";

            cmd.Parameters.Add("userId", GetUserId(userId));

            return cmd;
        }

        public PaymentCommand AddVirtualCoinForUser(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutUsers";
            cmd.Verb = "AddVirtualCoinForUser";

            cmd.Parameters.Add("userId", GetUserId(userId));
            cmd.Parameters.Add("amount", "200");
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }

        public PaymentCommand GetUserEmailAddress(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutUsers";
            cmd.Verb = "GetUserEmailAddress";

            cmd.Parameters.Add("userId", GetUserId(userId));

            return cmd;
        }

        public PaymentCommand UpdateUserEmail(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutUsers";
            cmd.Verb = "UpdateUserEmail";

            cmd.Parameters.Add("userId", GetUserId(userId));
            cmd.Parameters.Add("emailAddress", "ed.kane@Hangout.net");
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }


        public PaymentCommand GetUserInventory(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutUsers";
            cmd.Verb = "GetUserInventory";

            cmd.Parameters.Add("userId", GetUserId(userId));
           // cmd.Parameters.Add("startIndex", "0");
            //cmd.Parameters.Add("blockSize", "10");

            return cmd;
        }

        public PaymentCommand GetHangoutStoreInventory(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutStore";
            cmd.Verb = "GetStoreInventory";

            cmd.Parameters.Add("storeName", GetStoreName());

           // cmd.Parameters.Add("itemTypeNames", "Furniture,Food");

           // cmd.Parameters.Add("startIndex", "0");
           // cmd.Parameters.Add("blockSize", "10");

            cmd.Parameters.Add("filter", "itemOffer_title Like '%basic%'");
            cmd.Parameters.Add("orderBy", "itemOffer_endDate desc, item_itemTypeName asc");

            return cmd;
        }


         public PaymentCommand PurchaseHangoutItems(string userId, Form1 form1, XmlDocument xmlItemToPurchase)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutUsers";
            cmd.Verb = "PurchaseItems";

            cmd.Parameters.Add("userId", GetUserId(userId));

            string currencyName = xmlItemToPurchase.SelectSingleNode("/Response/currency").Attributes["name"].InnerText;
            string offerId = xmlItemToPurchase.SelectSingleNode("/Response/itemOffer").Attributes["id"].InnerText;
            
            //cmd.Parameters.Add("accountId", "1070");
            cmd.Parameters.Add("currencyName", currencyName);   
            cmd.Parameters.Add("offerIds", offerId);  //87 ERRROR
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }


        public PaymentCommand PurchaseHangoutItemsGift(string userId, Form1 form1, XmlDocument xmlItemToPurchase)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutUsers";
            cmd.Verb = "PurchaseGift";

            string currencyName = xmlItemToPurchase.SelectSingleNode("/Response/currency").Attributes["name"].InnerText;
            string offerId = xmlItemToPurchase.SelectSingleNode("/Response/itemOffer").Attributes["id"].InnerText;

            userId = GetUserId(userId);

            cmd.Parameters.Add("userId", userId);
            //cmd.Parameters.Add("accountId", "1070");
            cmd.Parameters.Add("currencyName", currencyName);
            cmd.Parameters.Add("offerIds", offerId);
            cmd.Parameters.Add("recipientUserId", GetRecipientUserId(userId));
            cmd.Parameters.Add("noteToRecipient", "This is a Gift");
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }

        public PaymentCommand PurchaseCashOffers(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutPurchase";
            cmd.Verb = "PurchaseCashOffers";

            cmd.Parameters.Add("userId", GetUserId(userId));

            return cmd;
        }

        public PaymentCommand PurchaseCoinOffers(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutPurchase";
            cmd.Verb = "PurchaseCoinOffers";

            cmd.Parameters.Add("userId", GetUserId(userId));

            return cmd;
        }

        public PaymentCommand PurchaseCashZongOffers(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutPurchase";
            cmd.Verb = "PurchaseCashZongOffers";

            cmd.Parameters.Add("userId", GetUserId(userId));
            cmd.Parameters.Add("externalTxnId", Guid.NewGuid().ToString());

            cmd.Parameters.Add("currencyName", "CASH");
           // cmd.Parameters.Add("countryCode", "US");
           // cmd.Parameters.Add("currencyCode", "USD");

            return cmd;
        }

        public PaymentCommand PurchaseCoinZongOffers(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutPurchase";
            cmd.Verb = "PurchaseCoinZongOffers";

            cmd.Parameters.Add("userId", GetUserId(userId));
            cmd.Parameters.Add("externalTxnId", Guid.NewGuid().ToString());

            cmd.Parameters.Add("currencyName", "COIN");
            cmd.Parameters.Add("countryCode", "US");
            cmd.Parameters.Add("currencyCode", "USD");

            return cmd;
        }

        public PaymentCommand PurchaseGameCurrencyPayPal(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutPurchase";
            cmd.Verb = "PurchaseGameCurrencyPayPal";

            cmd.Parameters.Add("userId", GetUserId(userId));
            cmd.Parameters.Add("offerId", "29");
            cmd.Parameters.Add("externalTxnId", Guid.NewGuid().ToString());
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }
        
        public PaymentCommand PurchaseGameCurrency(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutPurchase";
            cmd.Verb = "PurchaseGameCurrencyCreditCard";

            string secureKey = "";

            cmd.Parameters.Add("userId", GetUserId(userId));
            cmd.Parameters.Add("secureKey", secureKey);
            cmd.Parameters.Add("offerId", "29");    
            cmd.Parameters.Add("externalTxnId", Guid.NewGuid().ToString());
            cmd.Parameters.Add("ipAddress", "64.115.150.114");
            cmd.Parameters.Add("creditCardNumber", "4159662533655864");
            cmd.Parameters.Add("creditCardType", "VISA");
            cmd.Parameters.Add("expireDate", "092018");
            cmd.Parameters.Add("securityCode", "000");
            cmd.Parameters.Add("firstName", "Test");
            cmd.Parameters.Add("lastName", "User");
            cmd.Parameters.Add("address", "1 Main St");
            cmd.Parameters.Add("city", "San Jose");
            cmd.Parameters.Add("state", "CA");
            cmd.Parameters.Add("zipCode", "95131");
            cmd.Parameters.Add("countryCode", "US");
            cmd.Parameters.Add("phoneNumber", "1234567890");

            return cmd;
        }

        public PaymentCommand PurchaseGameCurrencyGambit(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutPurchase";
            cmd.Verb = "PurchaseGameCurrencyGambit";

            cmd.Parameters.Add("cashAmount", "10");
            cmd.Parameters.Add("userId", GetUserId(userId));
            cmd.Parameters.Add("ipAddress", GetMyIpAddress());

            return cmd;
        }

        public PaymentCommand PaymentItemsHealthCheck(string userId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutInfo";
            cmd.Verb = "HealthCheck";

            return cmd;
        }
           

        private string GetRecipientUserId(string userId)
        {
            return (GetUserId(userId, true));
        }

        private string GetUserId(string userId)
        {
            return (GetUserId(userId, false));
        }

        private string GetUserId(string userId, bool recipientFlag)
        {
            string title = "Select User";

            if (((userId == null) || (userId.Trim().Length == 0)) || (recipientFlag == true)) 
            {
                List<string> userList = new List<string>();
                userList.Add("545");
                userList.Add("547");
                userList.Add("549");
                userList.Add("551");
                userList.Add("554");
                userList.Add("556");
                userList.Add("558");
                userList.Add("559");
                userList.Add("576");
                userList.Add("574");
                userList.Add("652");
                userList.Add("721");

                if (((userId != null) && (userId.Trim().Length > 0)) && (recipientFlag == true))
                {
                    if (userList.Contains(userId.Trim()))
                    {
                        userList.Remove(userId.Trim());
                        title = "Select Gift Recipient's";
                    }
                }

                SelectForm formSelect = new SelectForm(userList, title);
                formSelect.ShowDialog();

                userId = formSelect.SelectedComboBox1;
            }
            return userId;
        }

        private string GetStoreName()
        {
            List<string> storeNameList = new List<string>();
            storeNameList.Add("Hangout_Store_Coin");
            storeNameList.Add("Hangout_Store_Cash");
            storeNameList.Add("Test_Store");
            storeNameList.Add("Test_Store_1");
            storeNameList.Add("Test_Store_2");

            SelectForm formSelect = new SelectForm(storeNameList, "Select Store");
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
