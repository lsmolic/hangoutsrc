using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using Hangout.Server;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{
    public class HangoutPurchase : HangoutCommandBase 
    {
        public HangoutPurchase() : base(System.Reflection.MethodBase.GetCurrentMethod()) { }

        /// <summary>
        /// Returns an XML document with the available purchase offers
        /// </summary>
        /// <param name="userId">The userId to find the available purchase offers for</param>
        /// <returns>XMLDocument with the available purchase offers</returns>
        public XmlDocument PurchaseCashOffers(UserId userId)
        {
            string cashCurrencyId = GetConfigurationAppSetting("CashCurrencyId", "");
            return (PurchaseOffers("PurchaseCashOffers",  userId, cashCurrencyId));
        }


        public XmlDocument PurchaseCoinOffers(UserId userId)
        {
            string coinCurrencyId = GetConfigurationAppSetting("CoinCurrencyId", "");
            return (PurchaseOffers("PurchaseCoinOffers", userId, coinCurrencyId));
        }

        private XmlDocument PurchaseOffers(string verb, UserId userId, string currencyId)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Users";
            cmd.Verb = "UserWidget";

            cmd.Parameters.Add("userId", userId.ToString());

            cmd.Parameters.Add("virtualCurrencyId", currencyId);
            XmlDocument serviceResponse = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());
            XmlNodeList offersNodeList = serviceResponse.SelectNodes("/Response/userWidgetConfig/offers/offer");
            XmlNode paymentMethodsNode = serviceResponse.SelectSingleNode("/Response/userWidgetConfig/paymentMethods");

            XmlDocument offerDoc = new XmlDocument();
            offerDoc.LoadXml(String.Format("<Response noun='HangoutPurchase' verb='{0}'><offers></offers></Response>", verb));

            foreach (XmlNode offerNode in offersNodeList)
            {
                string id = offerNode.Attributes["id"].InnerText;
                string description = offerNode.Attributes["description"].InnerText;
                XmlNode offerNodeItem = GetOfferNode(id, description);
                AppendNode(offerDoc, "/Response/offers", offerNodeItem);
            }

            AppendNode(offerDoc, "/Response", paymentMethodsNode);

            return offerDoc;
        }


        /// <summary>
        /// Given an offerId and a description returns the offer node.
        /// </summary>
        /// <param name="offerId">The offerId to create the node for</param>
        /// <param name="description">The description of the offer</param>
        /// <returns>XML Node containing the offer</returns>
        private XmlNode GetOfferNode(string offerId, string description)
        {
            StringBuilder sb = new StringBuilder();

            string usdMoney = "";
            int usdPos = description.IndexOf("USD");
            if (usdPos > 0)
            {
                usdMoney = description.Substring(0, usdPos).Trim();
            }

            string vMoney = "";
            int endPos = description.IndexOf("Cash");
            if (endPos > 0)
            {
                vMoney = description.Substring(usdPos + 7, endPos - (usdPos + 7)).Trim();
            }
        
            sb.Append(String.Format("<Offer id='{0}' usd='{1}' vMoney='{2}'>", offerId, usdMoney, vMoney));
            sb.Append(String.Format("<Description>{0}</Description>", description));
            sb.Append("</Offer>");

            XmlDocument offerDoc = new XmlDocument();
            offerDoc.LoadXml(sb.ToString());

            XmlNode offerNode = offerDoc.SelectSingleNode("/Offer");
            return offerNode;
        }

        /// <summary>
        /// Purchase Game Currency using PayPal
        /// </summary>
        /// <param name="userId">UserId and IP address</param>
        /// <param name="purchaseInfo">OfferId and ExternalTxnId</param>
        /// <returns>XML Response document with PayPal url to call to have the user continue the transaction.</returns>
        public XmlDocument PurchaseGameCurrencyPayPal(UserId userId, PurchaseInfo purchaseInfo)
        {
            XmlDocument response = null;
            try
            {
                PaymentCommand cmd = new PaymentCommand();
                cmd.Noun = "Purchase";
                cmd.Verb = "PayPalCheckout";

                cmd.Parameters.Add("userId", userId.ToString());
                cmd.Parameters.Add("ipAddress", userId.IPAddress);
                cmd.Parameters.Add("offerId", purchaseInfo.OfferId);
                cmd.Parameters.Add("externalTxnId", purchaseInfo.ExternalTxnId);

                response = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());
            }

            catch (Exception ex)
            {
                logError ("PurchaseGameCurrencyPayPal", ex);
                response = new SimpleResponse(MethodBase.GetCurrentMethod(), "purchaseResult", "Error");
            }

            return response;

        }

        /// <summary>
        /// Purchase Game Currency using Credit Card
        /// </summary>
        /// <param name="userId">UserId, IP address and SecureKey (optional)</param>
        /// <param name="purchaseInfo">OfferId and ExternalTxnId </param>
        /// <param name="creditCardInfo">CreditCardnumber, CreditCardtype, ExpireDate, SecurityCode, FirstName, MiddleName, LastName
        /// Address, City, StateProvince, ZipCode, CountryCode, PhoneNumber</param>
        /// <returns>SimpleResponse XML Document containing the purchaseResult</returns>
        public XmlDocument PurchaseGameCurrencyCreditCard(UserId userId, PurchaseInfo purchaseInfo, CreditCardInfo creditCardInfo)
        {
            XmlDocument response = null;
            try
            {
                PaymentCommand cmd = new PaymentCommand();
                cmd.Noun = "Purchase";
                cmd.Verb = "PurchaseCreditCard";

                cmd.Parameters.Add("userId", userId.ToString());
                cmd.Parameters.Add("ipAddress", userId.IPAddress);
                cmd.Parameters.Add("secureKey", userId.SecureKey);
                cmd.Parameters.Add("offerId", purchaseInfo.OfferId);
                cmd.Parameters.Add("externalTxnId", purchaseInfo.ExternalTxnId);
                cmd.Parameters.Add("creditCardNumber", creditCardInfo.CreditCardnumber);
                cmd.Parameters.Add("creditCardType", creditCardInfo.CreditCardtype);
                cmd.Parameters.Add("expireDate", creditCardInfo.ExpireDate);
                cmd.Parameters.Add("securityCode", creditCardInfo.SecurityCode);
                cmd.Parameters.Add("firstName", creditCardInfo.FirstName);
                cmd.Parameters.Add("middleName", creditCardInfo.MiddleName);
                cmd.Parameters.Add("lastName", creditCardInfo.LastName);
                cmd.Parameters.Add("address", creditCardInfo.Address);
                cmd.Parameters.Add("city", creditCardInfo.City);
                cmd.Parameters.Add("state", creditCardInfo.StateProvince);
                cmd.Parameters.Add("zipCode", creditCardInfo.ZipCode);
                cmd.Parameters.Add("countryCode", creditCardInfo.CountryCode);
                cmd.Parameters.Add("phoneNumber", creditCardInfo.PhoneNumber);

                XmlDocument serviceResponse = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());

                cmd.Parameters.Remove("creditCardNumber");
                serviceResponse = UpdateResponseForErrors(serviceResponse, cmd.Parameters);

                XmlNode purchaseResultNode =  serviceResponse.SelectSingleNode("Response/purchaseResult");

                if (purchaseResultNode != null)
                {
                    string externalTxnId = serviceResponse.SelectSingleNode("Response/purchaseResult").Attributes["externalTxnId"].InnerText;
                    string result = serviceResponse.SelectSingleNode("Response/purchaseResult").Attributes["statusText"].InnerText;
                    response = new SimpleResponse(MethodBase.GetCurrentMethod(), "purchaseResult", result);
                    AddAttribute(response, "/Response/purchaseResult", "externalTxnId", externalTxnId);
                }
                else
                {
                    string responseError = serviceResponse.SelectSingleNode("Response/errors/error").InnerText;
                    string responseErrorCode = serviceResponse.SelectSingleNode("Response/errors/error").Attributes["code"].InnerText;
                    logError("PurchaseGameCurrencyCreditCard", serviceResponse.InnerXml);
                    response = new SimpleResponse(MethodBase.GetCurrentMethod(), "purchaseResult", "Error");
                    AddAttribute(response, "/Response/purchaseResult", "externalTxnId", purchaseInfo.ExternalTxnId);
                    AddAttribute(response, "/Response/purchaseResult", "error", responseError);
                    AddAttribute(response, "/Response/purchaseResult", "errorCode", responseErrorCode);
                }
            }

            catch (Exception ex)
            {
                logError("PurchaseGameCurrencyCreditCard", ex);
                response = new SimpleResponse(MethodBase.GetCurrentMethod(), "purchaseResult", "Error");
                AddAttribute(response, "/Response/purchaseResult", "externalTxnId", purchaseInfo.ExternalTxnId);
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="paymentInfo">PaymentInfo including countryCode and CurrencyCode</param>
        /// <param name="externalTxnId">The external transaction id</param>
        /// <returns>The response xml</returns>
        public XmlDocument PurchaseCashZongOffers(UserId userId, PaymentInfo paymentInfo, string externalTxnId)
        {
            string cashCurrencyId = GetConfigurationAppSetting("CashCurrencyId", "");
            return (PurchaseZongOffers("PurchaseCashZongOffers", userId, cashCurrencyId, paymentInfo, externalTxnId));
        }

        /// <summary>
        /// Purchase Coin Zong offers 
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="paymentInfo">PaymentInfo including countryCode and CurrencyCode</param>
        /// <param name="externalTxnId">The external transaction id</param>
        /// <returns>The response xml</returns>
        public XmlDocument PurchaseCoinZongOffers(UserId userId, PaymentInfo paymentInfo, string externalTxnId)
        {
            string coinCurrencyId = GetConfigurationAppSetting("CoinCurrencyId", "");
            return (PurchaseZongOffers("PurchaseCoinOffers", userId, coinCurrencyId, paymentInfo, externalTxnId));
        }

        /// <summary>
        /// Purchase zong offers
        /// </summary>
        /// <param name="verb">The callers verb</param>
        /// <param name="userId">The userid</param>
        /// <param name="currencyId">The currency to buy</param>
        /// <param name="paymentInfo">PaymentInfo including countryCode and CurrencyCode</param>
        /// <param name="externalTxnId">The external transaction id</param>
        /// <returns>The response xml</returns>
        private XmlDocument PurchaseZongOffers(string verb, UserId userId, string currencyId, PaymentInfo paymentInfo, string externalTxnId)
        {
            XmlDocument response = null;
            XmlDocument offerDoc = new XmlDocument();

            try
            {
                PaymentCommand cmd = new PaymentCommand();
                cmd.Noun = "Users";
                cmd.Verb = "PaymentOptions";

                cmd.Parameters.Add("userId", userId.ToString());
                cmd.Parameters.Add("PaymentGatewayName", "ZongPaymentGateway");
                cmd.Parameters.Add("virtualCurrencyIds", currencyId);
                cmd.Parameters.Add("countryCode", paymentInfo.CountryCode);
                cmd.Parameters.Add("currencyCode", paymentInfo.CurrencyCode);

                XmlDocument serviceResponse = CallTwoFishService(cmd, MethodBase.GetCurrentMethod());
                XmlNodeList offersNodeList = serviceResponse.SelectNodes("/Response/paymentOptions/paymentOption");
                XmlNode paymentMethodsNode = serviceResponse.SelectSingleNode("/Response/userWidgetConfig/paymentMethods");

                offerDoc.LoadXml(String.Format("<Response noun='HangoutPurchase' verb='{0}'><offers></offers></Response>", verb));

                foreach (XmlNode offerNode in offersNodeList)
                {
                    string offerId = offerNode.Attributes["forexOfferId"].InnerText;
                    string price = offerNode.Attributes["price"].InnerText;
                    string currencyCode = offerNode.Attributes["currencyCode"].InnerText;
                    string vMoneyAmout = offerNode.Attributes["virtualCurrencyAmount"].InnerText;
                    string vMoneyName = offerNode.Attributes["virtualCurrencyName"].InnerText;
                    string iframeURL =  offerNode.Attributes["iframeURL"].InnerText;
                    string description = String.Format("{0} {1} for {2} {3}",  price, currencyCode, vMoneyAmout, vMoneyName);

                    StringBuilder sb = new StringBuilder();

                    sb.Append(String.Format("<Offer id='{0}' usd='{1}' vMoney='{2}'>", offerId, price, vMoneyAmout));
                    sb.Append(String.Format("<Description>{0}</Description>", description));
                    sb.Append(String.Format("<iframeURL><![CDATA[{0}]]></iframeURL>", iframeURL));
                    sb.Append("</Offer>");

                    XmlDocument offerNodeDoc = new XmlDocument();
                    offerNodeDoc.LoadXml(sb.ToString());

                    XmlNode offerNodeItem = offerNodeDoc.SelectSingleNode("/Offer");
                    AppendNode(offerDoc, "/Response/offers", offerNodeItem);
                }
            }

            catch (Exception ex)
            {
                logError("PurchaseGameCurrencyZong", ex);
                response = new SimpleResponse(MethodBase.GetCurrentMethod(), "purchaseResult", "Error");
                AddAttribute(response, "/Response/purchaseResult", "externalTxnId", externalTxnId);
            }

            return offerDoc;
        }

        /// <summary>
        /// Purchase Gambit offers Will transfer cash to the users account after a gambit purchase.
        /// </summary>
        /// <param name="userId">The userId</param>
        /// <param name="cashAmount">The cash amount of the transaction</param>
        /// <returns>The users balance xml</returns>
        public XmlDocument PurchaseGameCurrencyGambit(UserId userId, string cashAmount)
        {
            XmlDocument response = null;
            try
            {
                HangoutUsers user = new HangoutUsers();

                XmlDocument userBalance = user.GetUserBalance(userId.ToString());

                string cashCurrencyId = GetConfigurationAppSetting("CashCurrencyId", "");
                string xPathCashAccount = String.Format("/Response/user/accounts/account[@currencyId='{0}']", cashCurrencyId);
                XmlNode currencyCashNode = userBalance.SelectSingleNode(xPathCashAccount);

                string cashDebitAccountId = GetConfigurationAppSetting("CashDebitAccountId", "");
                user.AddFundsToUserAccount(currencyCashNode, cashAmount, cashDebitAccountId, userId.IPAddress);

                response = userBalance;
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("PurchaseGameCurrencyGambit", ex);
            }

            return response;
        } 
    }
}

