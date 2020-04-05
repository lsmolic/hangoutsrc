using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using Hangout.Shared;


namespace Hangout.Client
{
    public class PaymentItemsCommand
    {
        string mClientIPAddress = "";
        string mSecurePaymentInfo = "";

        public PaymentItemsCommand(string clientIPAddress)
        {
            mClientIPAddress = clientIPAddress;
        }

        public void LaunchMoneyAccountFunding ()
        {
            try
            {
                //  string urlBase = "http://127.0.0.1:5000";
                string urlBase = "https://secure.hangoutdev.net";

                string url = String.Format("{0}/PurchaseGw.aspx?p={1}", urlBase, mSecurePaymentInfo);

                System.Diagnostics.Process.Start(url);
            }
            catch{}
       }

        public string CreatePaymentCommand(string commandName, Dictionary<string, string> commandArgs)
        {
            string command = "";    

            try
            {
                PaymentCommand paymentCommand = null;

                switch (commandName)
                {
                    case "GetUserBalance":
                        paymentCommand = CreateSimpleCommand("GetUserBalance", commandArgs);
                        break;

                    case "AddVirtualCoinForUser":
                        paymentCommand = AddVirtualCoinForUser(commandArgs);
                        break;

                    case "GetUserInventory":
                        paymentCommand = GetUserInventory(commandArgs);
                        break;

                    case "GetStoreInventory":
                        paymentCommand = GetHangoutStoreInventory(commandArgs);
                        break;

                    case "PurchaseItems":
                        paymentCommand = PurchaseItems(commandArgs);
                        break;

                    case "PurchaseItemsGift":
                        paymentCommand = PurchaseItemsGift(commandArgs);
                        break;

                    case "GameCurrencyOffers":
                        paymentCommand = CreateSimpleCommand("PurchaseOffers", commandArgs);
                        break;

                    case "PurchaseGameCurrencyPayPal":
                        paymentCommand = PurchaseGameCurrencyPayPal(commandArgs);
                        break;

                    case "PurchaseGameCurrencyCreditCard":
                        paymentCommand = PurchaseGameCurrencyCreditCard(commandArgs);
                        break;

                    case "HealthCheck":
                        paymentCommand = CreateCommandWithNoParams("HealthCheck");
                        break;

                    case "SecurePaymentInfo":
                        paymentCommand = CreateCommandWithNoParams("SecurePaymentInfo");
                        break;

                    default:
                         throw (new Exception("Invalid Payment Items Command"));
                }

                ServiceCommandSerializer serializer = new ServiceCommandSerializer();
                command = serializer.SerializeCommandData(paymentCommand, typeof(PaymentCommand));
            }

            catch (Exception ex)
            {
                Console.Write("Error CreatePaymentCommand: ",  ex.Message);
            }

            return command;
        }

        public void ResponseHandler(string response)
        {
            Console.Write(String.Format("PaymentItems Response: {0}\n\n", response));

            try
            {
                XmlDocument xmlResponse = new XmlDocument();
                xmlResponse.LoadXml(response);

             //   xmlResponse.Save(@"c:\clientPaymentItemsXml.xml");

                string noun = xmlResponse.SelectSingleNode("/Response").Attributes["noun"].InnerText;
                string verb = xmlResponse.SelectSingleNode("/Response").Attributes["verb"].InnerText;

                ParseResponse(noun, verb, xmlResponse);
            }
            catch (Exception ex)
            {
                Console.Write(String.Format("Error ResponseHandler: {0}\n\n", ex.Message));
            }
        }

        private void ParseResponse(string commandNoun, string commandVerb, XmlDocument xmlResponse)
        {
            XmlNodeList itemOfferList;

            switch (commandVerb)
            {
                case "PurchaseGameCurrencyPayPal":
                    string payPalURL = xmlResponse.SelectSingleNode("/Response/paypalURL").InnerText;
                    System.Diagnostics.Process.Start(payPalURL);
                    break;
                case "GetStoreInventory":
                    itemOfferList = xmlResponse.SelectNodes("//store/itemOffers/itemOffer/item");
                    foreach (XmlElement itemOffer in itemOfferList)
                    {
                        string description = itemOffer.GetAttribute("description");
                        string itemName = itemOffer.GetAttribute("name");
                        string imageUrl = itemOffer.GetAttribute("smallImageUrl");
                        string itemOfferId = ((XmlElement)itemOffer.ParentNode).GetAttribute("id");
                    }
                    // Update the pages
                    XmlElement itemOffersNode = (XmlElement)xmlResponse.SelectSingleNode("//store/itemOffers");
                    double startIndex = Double.Parse(itemOffersNode.GetAttribute("startIndex"));
                    double blockSize = Double.Parse(itemOffersNode.GetAttribute("blockSize"));
                    double totalItems = Double.Parse(itemOffersNode.GetAttribute("total"));

                    break;
                case "GetUserInventory":
                    itemOfferList = xmlResponse.SelectNodes("//itemInstances/itemInstance");
                    foreach (XmlElement itemOffer in itemOfferList)
                    {
                        string itemName = itemOffer.GetAttribute("itemName");
                    }
                    break;
                case "GetUserBalance":
                    XmlElement vcoinNode = (XmlElement)xmlResponse.SelectSingleNode("Response/user/accounts/account[@currencyName='VCOIN']");
                    string vcoin = vcoinNode.GetAttribute("balance");
                    XmlElement houtsNode = (XmlElement)xmlResponse.SelectSingleNode("Response/user/accounts/account[@currencyName='HOUTS']");
                    string houts = houtsNode.GetAttribute("balance");
                    break;

                case "SecurePaymentInfo":
                    mSecurePaymentInfo = xmlResponse.SelectSingleNode("/Response/SecureInfo").InnerText;
                    break;

            }
        }
      
        private string CreateErrorDoc(string message)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<Error><Message>" + message + "</Message></Error>");
            return sb.ToString();
        }

        private PaymentCommand CreateSimpleCommand(string commandName, Dictionary<string, string> commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Verb = commandName;

            cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
            return cmd;
        }


        private PaymentCommand CreateSimpleCommandWithFilter(string commandName, Dictionary<string, string> commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Verb = commandName;

            cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
            cmd.Parameters.Add("startIndex", GetStringValueFromCommandArgs("startIndex", commandArgs, ""));
            cmd.Parameters.Add("blockSize", GetStringValueFromCommandArgs("blockSize", commandArgs, ""));

            return cmd;
        }

        private PaymentCommand AddVirtualCoinForUser(Dictionary<string, string> commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Verb = "AddVirtualCoinForUser";

            cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
            cmd.Parameters.Add("amount", GetStringValueFromCommandArgs("amount", commandArgs));
            cmd.Parameters.Add("ipAddress", mClientIPAddress); ;

            return cmd;
        }


        private PaymentCommand GetUserInventory(Dictionary<string, string> commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Verb = "GetUserInventory";

            cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
            cmd.Parameters.Add("startIndex", GetStringValueFromCommandArgs("startIndex", commandArgs, ""));
            cmd.Parameters.Add("blockSize", GetStringValueFromCommandArgs("blockSize", commandArgs, ""));
            cmd.Parameters.Add("itemTypeNames", GetStringValueFromCommandArgs("itemTypeNames", commandArgs, ""));

            return cmd;
        }


        private PaymentCommand GetHangoutStoreInventory(Dictionary<string, string> commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Verb = "GetStoreInventory";

            cmd.Parameters.Add("storeName",  GetStringValueFromCommandArgs("storeName", commandArgs, null));
            cmd.Parameters.Add("startIndex", GetStringValueFromCommandArgs("startIndex", commandArgs, ""));
            cmd.Parameters.Add("blockSize", GetStringValueFromCommandArgs("blockSize", commandArgs, ""));
            cmd.Parameters.Add("itemTypeNames", GetStringValueFromCommandArgs("itemTypeNames", commandArgs, ""));

            return cmd;
        }

        private PaymentCommand PurchaseItems(Dictionary<string, string> commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Verb = "PurchaseItems";

            cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
            cmd.Parameters.Add("currencyName", GetStringValueFromCommandArgs("currencyName", commandArgs));
            cmd.Parameters.Add("offerIds", GetStringValueFromCommandArgs("offerIds", commandArgs));
            cmd.Parameters.Add("ipAddress", mClientIPAddress); ;

            return cmd;
        }


        private PaymentCommand PurchaseItemsGift(Dictionary<string, string> commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Verb = "PurchaseItemsGift";

            cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
            cmd.Parameters.Add("currencyName", GetStringValueFromCommandArgs("currencyName", commandArgs));
            cmd.Parameters.Add("offerIds", GetStringValueFromCommandArgs("offerIds", commandArgs));
            cmd.Parameters.Add("recipientUserId", GetStringValueFromCommandArgs("recipientUserId", commandArgs));
            cmd.Parameters.Add("noteToRecipient", GetStringValueFromCommandArgs("noteToRecipient", commandArgs));
            cmd.Parameters.Add("ipAddress", mClientIPAddress);

            return cmd;
        }

        private PaymentCommand PurchaseGameCurrencyPayPal(Dictionary<string, string> commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Verb = "PurchaseGameCurrencyPayPal";

            cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
            cmd.Parameters.Add("offerId", GetStringValueFromCommandArgs("offerId", commandArgs));
            cmd.Parameters.Add("ipAddress", mClientIPAddress);

            return cmd;
        }

        private PaymentCommand PurchaseGameCurrencyCreditCard(Dictionary<string, string> commandArgs)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Verb = "PurchaseGameCurrencyCreditCard";

            cmd.Parameters.Add("userSession", GetStringValueFromCommandArgs("userSession", commandArgs));
            cmd.Parameters.Add("offerId", GetStringValueFromCommandArgs("offerId", commandArgs));
            cmd.Parameters.Add("ipAddress", mClientIPAddress);
            cmd.Parameters.Add("creditCardNumber", GetStringValueFromCommandArgs("creditCardNumber", commandArgs));
            cmd.Parameters.Add("creditCardType", GetStringValueFromCommandArgs("creditCardType", commandArgs));
            cmd.Parameters.Add("expireDate", GetStringValueFromCommandArgs("expireDate", commandArgs));
            cmd.Parameters.Add("securityCode", GetStringValueFromCommandArgs("securityCode", commandArgs));
            cmd.Parameters.Add("firstName", GetStringValueFromCommandArgs("firstName", commandArgs));
            cmd.Parameters.Add("lastName", GetStringValueFromCommandArgs("lastName", commandArgs));
            cmd.Parameters.Add("address", GetStringValueFromCommandArgs("address", commandArgs));
            cmd.Parameters.Add("city", GetStringValueFromCommandArgs("city", commandArgs));
            cmd.Parameters.Add("state", GetStringValueFromCommandArgs("state", commandArgs));
            cmd.Parameters.Add("zipCode", GetStringValueFromCommandArgs("zipCode", commandArgs));
            cmd.Parameters.Add("countryCode", GetStringValueFromCommandArgs("countryCode", commandArgs));
            cmd.Parameters.Add("phoneNumber", GetStringValueFromCommandArgs("phoneNumber", commandArgs));

            return cmd;
        }

        private PaymentCommand CreateCommandWithNoParams(string commandName)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Verb = commandName;

            return cmd;
        }

        private string GetStringValueFromCommandArgs(string key, Dictionary<string, string> commandArgs)
        {
            return (GetStringValueFromCommandArgs(key, commandArgs, null));
        }

        private string GetStringValueFromCommandArgs(string key, Dictionary<string, string> commandArgs, string defaultValue)
        {
            string value = "";

            if (!commandArgs.TryGetValue(key, out value))
            {
                if (defaultValue == null)
                {
                    throw (new Exception(String.Format("Argument {0} not found", key)));
                }
                else
                {
                    value = defaultValue;
                }
            }

            return value;
        }
    }
}
