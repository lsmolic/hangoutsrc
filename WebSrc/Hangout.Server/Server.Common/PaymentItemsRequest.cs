using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Resources;
using Hangout.Shared;
using log4net;

namespace Hangout.Server
{
    public class PaymentItemsRequest
    {
        private ILog mErrorLogger;

        public PaymentItemsRequest()
        {
            mErrorLogger = LogManager.GetLogger("GeneralUse");
        }

        public string ProcessMessage(string paymentItemCommand, IPaymentItemInterface paymentItem)
        {
            string response = "";

            try
            {
                response = paymentItem.ProcessMessage(paymentItemCommand);
            }

            catch (Exception ex)
            {
                mErrorLogger.Error("PaymentItemsRequest paymentItemCommand", ex);
            }

            return response;
        }

        public XmlDocument GetPaymentOffers(string userId, string paymentType, string baseServicesRoot)
        {
            PaymentCommand paymentCommand = CreateGetPaymentOffersCommand(userId, paymentType);
            return (ProcessPaymentItem(paymentCommand, baseServicesRoot));
        }

        public XmlDocument GetZongOffers(string userId, string paymentType, string baseServicesRoot)
        {
            PaymentCommand paymentCommand = CreateGetPaymentZongOffersCommand(userId, paymentType);
            return (ProcessPaymentItem(paymentCommand, baseServicesRoot));
        }

        public string PurchaseGameCurrencyPayPal(string piUserId, string hangoutUserId, string sessionGuid, string emailAddress, string offerId, string ipAddress, string baseServicesRoot)
        {
            PaymentCommand paymentCommand = CreatePurchaseGameCurrencyPayPal(piUserId, offerId, ipAddress);

            MoneyTransactionLogging moneyLog = new MoneyTransactionLogging();
            moneyLog.LogMoneyPaymentCommand(hangoutUserId, sessionGuid, paymentCommand.Parameters, emailAddress, "Paypal");

            XmlDocument xmlResponse = ProcessPaymentItem(paymentCommand, baseServicesRoot);

            moneyLog.PayPalLogResponse(xmlResponse, "InProgress");
            string paypalURL = xmlResponse.SelectSingleNode("/Response/paypalURL").InnerText;

            return paypalURL;
        }

        public string PurchaseGameCurrencyCreditCard(string transactionId, string piUserId, string hangoutUserId, string sessionGuid, string emailAddress, string offerId, CreditCardInfo creditCardInfo, string ipAddress, string baseServicesRoot)
        {
            PaymentCommand paymentCommand = CreatePurchaseGameCurrencyCreditCard(transactionId, piUserId, offerId, creditCardInfo, ipAddress);

            MoneyTransactionLogging moneyLog = new MoneyTransactionLogging();
            moneyLog.LogMoneyPaymentCommand(hangoutUserId, sessionGuid, paymentCommand.Parameters, emailAddress, "CreditCard");

            XmlDocument xmlResponse = ProcessPaymentItem(paymentCommand, baseServicesRoot);

            moneyLog.CreditCardLogResponse(xmlResponse);

            return (ParseCreditCardResult(xmlResponse));
        }

        public void PurchaseGameCurrencyZong(string piUserId, string hangoutUserId, string sessionGuid, string emailAddress, string offerDesc, string transactionRef, string ipAddress, string baseServicesRoot)
        {
            NameValueCollection logInfo = new NameValueCollection();
            logInfo.Add("userId", piUserId);
            logInfo.Add("transactionRef", transactionRef);
            logInfo.Add("externalTxnId", Guid.NewGuid().ToString());
            logInfo.Add("offerId", offerDesc);
            logInfo.Add("ipAddress", ipAddress);

            MoneyTransactionLogging moneyLog = new MoneyTransactionLogging();
            moneyLog.LogMoneyPaymentCommand(hangoutUserId, sessionGuid, logInfo, emailAddress, "Zong");
        }


        public string GambitCallBack(string xmlInfo, string baseServicesRoot)
        {
            string response = "";

            try
            {
                XmlDocument callBackInfo = new XmlDocument();
                callBackInfo.LoadXml(xmlInfo);

                MoneyTransactionLogging moneyLog = new MoneyTransactionLogging();
                moneyLog.GambitLogResponse(callBackInfo, "CallBackBegin");

                response = GambitAddMoneyToAccount(callBackInfo, baseServicesRoot);

                moneyLog.GambitLogResponse(callBackInfo, response);
            }

            catch (Exception ex)
            {
                mErrorLogger.Error("GambitCallBack Error", ex);
            }

            return response;
        }

        private string GambitAddMoneyToAccount(XmlDocument xmlInfo, string baseServicesRoot)
        {
            string response = "ERROR:RESEND";
            try
            {
                string userId = XmlUtil.GetAttributeFromXml(xmlInfo, "/Response/data[@name='paymentItemsUserId']", "value", "");
                string cashAmount = XmlUtil.GetAttributeFromXml(xmlInfo, "/Response/data[@name='amount']", "value", "");
                string ipAddress = XmlUtil.GetAttributeFromXml(xmlInfo, "/Response/data[@name='ipAddress']", "value", "");

                if (ValidateGambitValues(userId, cashAmount, ipAddress))
                {
                    PaymentCommand paymentCommand = CreatePurchaseGameCurrencyGambit(userId, cashAmount, ipAddress);
                    XmlDocument xmlResponse = ProcessPaymentItem(paymentCommand, baseServicesRoot);

                    if (xmlResponse.SelectSingleNode("/Response/user/accounts/account[@currencyName='HOUTS']").Attributes["balance"] != null)
                    {
                        response = "OK";
                    }
                }
                else
                {
                    response = "ERROR:FATAL";
                }
            }

            catch (Exception ex)
            {
                mErrorLogger.Error("AddMoneyToAccount Error", ex);
            }

            return response;
        }

        private bool ValidateGambitValues(string userId, string cashAmount, string ipAddress)
        {
            bool validate = true;

            try
            {
               if (String.IsNullOrEmpty(userId.Trim()))
                {
                    validate = false;
                }

                int cashAmountValue = -1;
                if (int.TryParse(cashAmount, out cashAmountValue) == false)
                {
                    validate = false;
                }

                if (String.IsNullOrEmpty(ipAddress.Trim()))
                {
                    validate = false;
                }
            }

            catch (Exception ex)
            {
                mErrorLogger.Error("ValidateGambitValues Error", ex);
            }

            return validate;
        }


        private XmlDocument ProcessPaymentItem(PaymentCommand paymentCommand, string baseServicesRoot)
        {
            ServiceCommandSerializer serializer = new ServiceCommandSerializer();
            string xmlPaymentItemsMessage = serializer.SerializeCommandData(paymentCommand, typeof(PaymentCommand));

            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(xmlPaymentItemsMessage);

            WebServiceRequest request = new WebServiceRequest(baseServicesRoot, "PaymentItemsService", "ProcessPaymentItemString");
            request.AddParam("paymentCommand", byteArray);

            XmlDocument xmlResponse = request.GetWebResponse();

            return xmlResponse;
        }

        private XmlDocument ProcessAdminPaymentItem(PaymentCommand paymentCommand, string baseServicesRoot)
        {
            ServiceCommandSerializer serializer = new ServiceCommandSerializer();
            string xmlPaymentItemsMessage = serializer.SerializeCommandData(paymentCommand, typeof(PaymentCommand));

            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(xmlPaymentItemsMessage);

            WebServiceRequest request = new WebServiceRequest(baseServicesRoot, "PaymentItemsService", "ProcessAdminPaymentItemXml");
            request.AddParam("paymentCommand", byteArray);

            XmlDocument xmlResponse = request.GetWebResponse();

            return xmlResponse;
        }


        private PaymentCommand CreateGetPaymentOffersCommand(string userId, string paymentType)
        {
            PaymentCommand paymentCommand = new PaymentCommand();

            paymentCommand.Noun = "HangoutPurchase";

            if (paymentType == "COIN")
            {
                paymentCommand.Verb = "PurchaseCoinOffers";
            }
            else
            {
                paymentCommand.Verb = "PurchaseCashOffers";
            }

            paymentCommand.Parameters.Add("userId", userId);

            return paymentCommand;
        }


        private PaymentCommand CreateGetPaymentZongOffersCommand(string userId, string paymentType)
        {
            PaymentCommand paymentCommand = new PaymentCommand();

            paymentCommand.Noun = "HangoutPurchase";

            if (paymentType == "COIN")
            {
                paymentCommand.Verb = "PurchaseCoinZongOffers";
            }
            else
            {
                paymentCommand.Verb = "PurchaseCashZongOffers";
            }

            paymentCommand.Parameters.Add("userId", userId);

            return paymentCommand;
        }

        private PaymentCommand CreatePurchaseGameCurrencyPayPal(string userId, string offerId, string ipAddress)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutPurchase";
            cmd.Verb = "PurchaseGameCurrencyPayPal";

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("offerId", offerId);
            cmd.Parameters.Add("externalTxnId", Guid.NewGuid().ToString());
            cmd.Parameters.Add("ipAddress", ipAddress);

            return cmd;
        }

        private PaymentCommand CreatePurchaseGameCurrencyCreditCard(string transactionId, string userId, string offerId, CreditCardInfo creditCardInfo, string ipAddress)
        {

            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutPurchase";
            cmd.Verb = "PurchaseGameCurrencyCreditCard";

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("ipAddress", ipAddress);
            cmd.Parameters.Add("offerId", offerId);
            cmd.Parameters.Add("externalTxnId", transactionId);
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

            return cmd;
        }


        private PaymentCommand CreatePurchaseGameCurrencyGambit(string userId, string cashAmount, string ipAddress)
        {
            PaymentCommand cmd = new PaymentCommand();

            cmd.Noun = "HangoutPurchase";
            cmd.Verb = "PurchaseGameCurrencyGambit";

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("cashAmount", cashAmount);
            cmd.Parameters.Add("ipAddress", ipAddress);

            return cmd;
        }

        public XmlDocument GetTheUserInformationFromHangoutId(string hangoutUserId, string baseServicesRoot)
        {
            XmlDocument userInformation = null;

            WebServiceRequest request = new WebServiceRequest(baseServicesRoot, "Accounts", "GetAccounts");
            request.AddParam("accountId", hangoutUserId);
            userInformation = request.GetWebResponse();

            return userInformation;
        }

        public string GetEmailAddress(string userId, string baseServicesRoot)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutUsers";
            cmd.Verb = "GetUserEmailAddress";

            cmd.Parameters.Add("userId", userId);

            XmlDocument xmlResponse = ProcessPaymentItem(cmd, baseServicesRoot);

            XmlNode userNode = xmlResponse.SelectSingleNode("/Response/user");
            return (userNode.Attributes["emailAddress"].InnerText);
        }

        public void UpdateEmailAddess(string userId, string ipAddress, string emailAddress, string baseServicesRoot)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "HangoutUsers";
            cmd.Verb = "UpdateUserEmail";

            cmd.Parameters.Add("userId", userId);
            cmd.Parameters.Add("emailAddress", emailAddress);
            cmd.Parameters.Add("ipAddress", ipAddress);

            ProcessPaymentItem(cmd, baseServicesRoot);
        }


        private string ParseCreditCardResult(XmlDocument response)
        {
            string result = "Unknown Error Occured";
            string resultCode = "";
            if (response.SelectSingleNode("/Response/purchaseResult") != null)
            {
                result = response.SelectSingleNode("/Response/purchaseResult").InnerText;
                if (response.SelectSingleNode("/Response/purchaseResult").Attributes["error"] != null)
                {
                    result = response.SelectSingleNode("/Response/purchaseResult").Attributes["error"].InnerText;
                    resultCode = response.SelectSingleNode("/Response/purchaseResult").Attributes["errorCode"].InnerText;
                    if (resultCode == "210003")
                    {
                        Dictionary<string, string> resultErrorInfo = ParseErrorInfoIntoDictionay(result);
                        string[] resultArray = GetPurchaseOrderResult(resultErrorInfo, "purchaseOrderState");
                        resultCode = resultArray[0];
                        result = resultArray[1];
                    }
                    else
                    {
                        result = result.Replace("FundRequestException", "").Trim();
                        result = PaymentItemResourceHandler.GetErrorMessageFromResourceFile(result, resultCode);
                        if (result.Contains("does not pass custom validation"))
                        {
                            result = PaymentItemResourceHandler.GetErrorMessageFromResourceFile("", "DefaultCCError"); 
                        }
                    }
                }

            }
            return result;
        }

        private Dictionary<string, string> ParseErrorInfoIntoDictionay(string result)
        {
            Dictionary<string, string> resultDictionary = new Dictionary<string, string>();
            string[] resultArray = result.Split(',');

            foreach (string item in resultArray)
            {
                string[] values = item.Split('=');
                if (values.Length == 2)
                {
                    if (!resultDictionary.ContainsKey(values[0]))
                    {
                        resultDictionary.Add(values[0], values[1]);
                    }
                }
            }
            return resultDictionary;
        }

        private string GetDictionaryValue(Dictionary<string, string> InfoArray, string Key)
        {
            string value = "";

            InfoArray.TryGetValue(Key, out value);
            return value;
        }

        private string[] GetPurchaseOrderResult(Dictionary<string, string> InfoArray, string Key)
        {
            string[] resultArray = new string[2];
            string purchaseOrderState = GetDictionaryValue(InfoArray, "purchaseOrderState");
            resultArray[0] = GetDictionaryValue(InfoArray, "resultCode");
            resultArray[1] = GetDictionaryValue(InfoArray, "responseText");

            if (String.IsNullOrEmpty(resultArray[1]))
            {
                resultArray[1] = PaymentItemResourceHandler.GetErrorMessageFromResourceFile("", "DefaultCCError"); 
            }
            return (resultArray);
        }


        public XmlDocument AdminGetPaymentItemsUserInfo(string userId, string baseServicesRoot)
        {
            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "AdminUsers";
            cmd.Verb = "GetUserInfo";

            cmd.Parameters.Add("userId", userId);

            XmlDocument xmlResponse = ProcessAdminPaymentItem(cmd, baseServicesRoot);

            return xmlResponse;
        }
    }
}
