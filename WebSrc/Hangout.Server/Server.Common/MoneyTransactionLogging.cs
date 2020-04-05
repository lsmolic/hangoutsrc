using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Configuration;
using log4net;
using Hangout.Shared;

using System.Threading;

namespace Hangout.Server    
{
    public class MoneyTransactionLogging
    {
        private ILog mMoneyPaymentsLog;
        private ILog mErrorLogger;

        public MoneyTransactionLogging()
        {
            mMoneyPaymentsLog = LogManager.GetLogger("MoneyPayments");
            mErrorLogger = LogManager.GetLogger("GeneralUse");
        }

        /// <summary>
        /// Log MoneyPaymentCommand to the MoneyPayment transaction log 
        /// if toState is greater than -1 then log to MoneyPayment state log  
        /// </summary>  
        /// <param name="commandArgs">Payment command arguments</param>
        public void LogMoneyPaymentCommand(string hangoutUserId, string sessionGuid, NameValueCollection commandArgs, string emailAddress, string moneyType)
        {
            try
            {
                commandArgs.Add("Email", emailAddress);
                XmlDocument moneyPaymentDoc = CreateMoneyPaymentXmlDocument(hangoutUserId, sessionGuid, commandArgs, "Send", moneyType, 0);
                LogMoneyTransactonEvent(moneyPaymentDoc);
            }
            catch (Exception Exception)
            {
                mErrorLogger.Error("Error LogMoneyPaymentCommand", Exception);
            }

        }

        public void PayPalLogResponse(XmlDocument response, string status)
        {
            try
            {
                switch (status)
                {
                    case "InProgress":
                        PayPalLogResponse(response, status, 1);
                        break;

                    case "CallBackBegin":
                        PayPalCallBackResponse(response, status, 2);
                        break;

                    case "CallBackComplete":
                        PayPalCallBackComplete(response);
                        break;

                    default:
                        mErrorLogger.Error(String.Format("Error PayPalLogResponse wrong status {0), {1}", status, response));
                        break;
                }
            }

            catch (Exception ex)
            {
                mErrorLogger.Error("Error PayPalLogResponse wrong status", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        private void PayPalLogResponse(XmlDocument response, string status, int toState)
        {
            try
            {
                if (response.SelectSingleNode("/Response") != null)
                {
                    string externalTransactionId = response.SelectSingleNode("/Response/externalTxnId").InnerText;

                    //<paypalURL>https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&amp;token=EC-5GR63402L4990934U</paypalURL>
                    string payalUrl = response.SelectSingleNode("/Response/paypalURL").InnerText;

                    string findString = "token=";
                    int tokenIndex = payalUrl.IndexOf(findString) + findString.Length;
                    string token = payalUrl.Substring(tokenIndex);

                    XmlDocument moneyPaymentDoc = CreateMoneyPaymentXmlDocument(status, "Paypal", toState);
                    string xPath = "/MoneyPayment/Transaction";
                    AddValueAttribute(moneyPaymentDoc, xPath, "extTransactId", externalTransactionId);
                    AddValueAttribute(moneyPaymentDoc, xPath, "token", token);
                    LogMoneyTransactonEvent(moneyPaymentDoc);
                }
            }

            catch (Exception Exception)
            {
                mErrorLogger.Error("Error PayPalLogResponse", Exception);
            }
        }



        // <Response command='PaypalReturn'>
        //    <data name='userId' value='545'> /data>
        //    <data name='offerId' value='29'> </data>
        //    <data name='externalTxnId' value='954a9f42-1df9-4e16-bf56-5b8abcc7611e'> 
        //    <data name='token' value='EC-57K58313BY1773505'> </data>
        //    <data name='payerId' value='8FBU5YKUF3MM6'> </data>
        //    <data name='ipAddress' value='192.168.163.28'></data>
        // </Response>
        private void PayPalCallBackResponse(XmlDocument response, string status, int toState)
        {
            try
            {
                if (response.SelectSingleNode("/Response") != null)
                {
                    XmlDocument moneyPaymentDoc = CreateMoneyPaymentXmlDocument(status, "Paypal", toState);
                    string xPath = "/MoneyPayment/Transaction";

                    AddValueAttribute(moneyPaymentDoc, xPath, "piUserId", XmlUtil.GetAttributeFromXml(response, "/Response/data[@name='userId']", "value", ""));
                    AddValueAttribute(moneyPaymentDoc, xPath, "offerId", XmlUtil.GetAttributeFromXml(response, "/Response/data[@name='offerId']", "value", ""));
                    AddValueAttribute(moneyPaymentDoc, xPath, "extTransactId", XmlUtil.GetAttributeFromXml(response, "/Response/data[@name='externalTxnId']", "value", ""));
                    AddValueAttribute(moneyPaymentDoc, xPath, "token", XmlUtil.GetAttributeFromXml(response, "/Response/data[@name='token']", "value", ""));
                    AddValueAttribute(moneyPaymentDoc, xPath, "payerId", XmlUtil.GetAttributeFromXml(response, "/Response/data[@name='payerId']", "value", ""));
                    AddValueAttribute(moneyPaymentDoc, xPath, "ipAddress", XmlUtil.GetAttributeFromXml(response, "/Response/data[@name='ipAddress']", "value", ""));
                    AddValueAttribute(moneyPaymentDoc, xPath, "billingDesc", XmlUtil.GetAttributeFromXml(response, "/Response/data[@name='billingDescription']", "value", ""));
                    AddValueAttribute(moneyPaymentDoc, xPath, "startDate", XmlUtil.GetAttributeFromXml(response, "/Response/data[@name='startDate']", "value", ""));
                    AddValueAttribute(moneyPaymentDoc, xPath, "numPayments", XmlUtil.GetAttributeFromXml(response, "/Response/data[@name='numPayments']", "value", ""));
                    AddValueAttribute(moneyPaymentDoc, xPath, "payFrequency", XmlUtil.GetAttributeFromXml(response, "/Response/data[@name='payFrequency']", "value", ""));
                    LogMoneyTransactonEvent(moneyPaymentDoc);
                }
            }

            catch (Exception Exception)
            {
                mErrorLogger.Error("Error PayPalCallBackResponse", Exception);
            }

        }

        //    <Response noun="Purchase" verb="PayPalCancel">
        //    <purchaseResult statusCode="200" statusText="OK"
        private void PayPalCallBackComplete(XmlDocument response)
        {   

            try
            {
                int state = 4;
                string status = "";

                if (response.SelectSingleNode("/Response") != null)
                {
                    string errorDescription = "";
                    string externalTransactionId = response.SelectSingleNode("/Response/purchaseResult").Attributes["externalTxnId"].InnerText;
                    if (response.SelectSingleNode("/Response/errors/error/message") != null)
                    {
                        errorDescription = response.SelectSingleNode("/Response/errors/error/message").InnerText;
                        state = 4;
                        status = "Error";
                    }
                    else if (response.SelectSingleNode("/Response").Attributes["verb"].InnerText == "PayPalCancel")
                    {
                        status = "Canceled";
                        state = 3;
                        errorDescription = response.SelectSingleNode("/Response/purchaseResult").Attributes["statusText"].InnerText;
                    }
                    else
                    {
                        status = "Complete";
                        state = 3;
                        errorDescription = response.SelectSingleNode("/Response/purchaseResult").Attributes["statusText"].InnerText;
                    }

                    XmlDocument moneyPaymentDoc = CreateMoneyPaymentXmlDocument(status, "PayPalCallBackComplete", state);
                    AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "extTransactId", externalTransactionId);
                    AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "errorDescription", errorDescription);
                    LogMoneyTransactonEvent(moneyPaymentDoc);
                }
            }
                        
            catch (Exception Exception)
            {
                mErrorLogger.Error("Error PayPalCallBackComplete", Exception);
            }
        }

        public void NotifyClientComplete(string externalTransactionId, string moneyType )
        {
            XmlDocument moneyPaymentDoc = CreateMoneyPaymentXmlDocument("NotifyComplete", moneyType, 99);
            AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "extTransactId", externalTransactionId);
            LogMoneyTransactonEvent(moneyPaymentDoc);
        }

        public void CreditCardLogResponse(XmlDocument response)
        {
            try
            {
                if (response.SelectSingleNode("/Response") != null)
                {
                    string errorDescription = "";
                    string status = response.SelectSingleNode("/Response/purchaseResult").InnerText;
                    string externalTransactionId = response.SelectSingleNode("/Response/purchaseResult").Attributes["externalTxnId"].InnerText;

                    if (response.SelectSingleNode("/Response/purchaseResult").Attributes["error"] != null)
                    {
                        errorDescription = response.SelectSingleNode("/Response/purchaseResult").Attributes["error"].InnerText;
                        status = "Error";
                    }

                    XmlDocument moneyPaymentDoc = CreateMoneyPaymentXmlDocument(status, "CreditCard", 3);
                    AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "extTransactId", externalTransactionId);
                    AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "errorDescription", errorDescription);
                    LogMoneyTransactonEvent(moneyPaymentDoc);
                }
            }
            catch (Exception Exception)
            {
                mErrorLogger.Error("Error CreditCardLogResponse", Exception);
            }
        }

        public void GambitLogResponse(XmlDocument xmlInfo, string status)
        {
            //uid=21&amount=2000&time=1229281667&oid=1021 
            try 
            {

                string sessionId = XmlUtil.GetAttributeFromXml(xmlInfo, "/Response/data[@name='sessionId']", "value", "");
                string paymentItemsUserId = XmlUtil.GetAttributeFromXml(xmlInfo, "/Response/data[@name='paymentItemsUserId']", "value", "");
                string hangoutUserId = XmlUtil.GetAttributeFromXml(xmlInfo, "/Response/data[@name='hangoutUserId']", "value", "");
                string amount = XmlUtil.GetAttributeFromXml(xmlInfo, "/Response/data[@name='amount']", "value", "");
                string ipAddress = XmlUtil.GetAttributeFromXml(xmlInfo, "/Response/data[@name='ipAddress']", "value", "");
                string uid = XmlUtil.GetAttributeFromXml(xmlInfo, "/Response/data[@name='uid']", "value", "");
                string oid = XmlUtil.GetAttributeFromXml(xmlInfo, "/Response/data[@name='oid']", "value", "");

                XmlDocument moneyPaymentDoc = CreateMoneyPaymentXmlDocument(status, "Gambit", 3);
                AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "piUserId", paymentItemsUserId);
                AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "hangoutUserId", hangoutUserId);
                AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "amount", amount);
                AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "extTransactId", sessionId);
                AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "transactionRef", uid);
                AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "offerId", oid);
                AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "ipAddress", ipAddress);

                LogMoneyTransactonEvent(moneyPaymentDoc);
            }

            catch (Exception Exception)
            {
                mErrorLogger.Error("Error GambitLogResponse", Exception);
            }
        }

        
        /// <summary>
        /// Create the MoneyPayment xml document used to log MoneyPayment Transactions
        /// </summary>
        /// <param name="commandArgs">The command arguments</param>
        /// <param name="status"></param>
        /// <param name="moneyType"></param>
        /// <param name="toState"></param>
        /// <returns></returns>
        private XmlDocument CreateMoneyPaymentXmlDocument(string hangoutUserId, string sessionGuid, NameValueCollection commandArgs, string status, string moneyType, int toState)
        {
            Dictionary<string, string> logElements = new Dictionary<string, string>();
            logElements.Add("userId", "piUserId");
            logElements.Add("externalTxnId", "extTransactId");
            logElements.Add("errorDescription", "errorDescription");
            logElements.Add("transactionRef", "transactionRef");
            logElements.Add("offerId", "offerId");
            logElements.Add("amount", "amount");
            logElements.Add("payerId", "payerId");
            logElements.Add("ipAddress", "ipAddress");
            logElements.Add("secureKey", "secureKey");
            logElements.Add("moneyType", "moneyType");
            logElements.Add("firstName", "firstName");
            logElements.Add("lastName", "lastName");
            logElements.Add("address", "address");
            logElements.Add("city", "city");
            logElements.Add("state", "state");
            logElements.Add("zipCode", "zipCode");
            logElements.Add("countryCode", "countryCode");
            logElements.Add("phoneNumber", "phoneNumber");
            logElements.Add("email", "email");
            logElements.Add("billingDesc", "billingDesc");
            logElements.Add("startDate", "startDate");
            logElements.Add("numPayments", "numPayments");
            logElements.Add("payFrequncy", "payFrequncy");
            logElements.Add("transactDate", "transactDate");

            XmlDocument moneyPaymentDoc = CreateMoneyPaymentXmlDocument(status, moneyType, toState);

            string xPath = "/MoneyPayment/Transaction";

            foreach (KeyValuePair<string, string> kvp in logElements)
            {
                AddValueAttribute(moneyPaymentDoc, xPath, kvp.Value, GetStringValueFromArgs(commandArgs, kvp.Key, ""));
            }

            AddValueAttribute(moneyPaymentDoc, xPath, "hangoutUserId", hangoutUserId);
            AddValueAttribute(moneyPaymentDoc, xPath, "sessionGuid", sessionGuid);

            return moneyPaymentDoc;
        }

        private XmlDocument CreateMoneyPaymentXmlDocument(string status, string moneyType, int toState)
        {
            XmlDocument moneyPaymentDoc = new XmlDocument();
            StringBuilder sb = new StringBuilder();

            sb.Append("<MoneyPayment><Transaction>");
            sb.Append("<piUserId />");
            sb.Append("<offerId />");
            sb.Append("<amount />");
            sb.Append("<token />");
            sb.Append("<payerId />");
            sb.Append("<ipAddress />");
            sb.Append("<secureKey />");
            sb.Append("<moneyType />");
            sb.Append("<firstName />");
            sb.Append("<lastName />");
            sb.Append("<address />");
            sb.Append("<city />");
            sb.Append("<state />");
            sb.Append("<zipCode />");
            sb.Append("<countryCode />");
            sb.Append("<phoneNumber />");
            sb.Append("<email />");
            sb.Append("<billingDesc />");
            sb.Append("<startDate />");
            sb.Append("<numPayments />");
            sb.Append("<payFrequncy />");
            sb.Append("<transactDate />");
            sb.Append("<extTransactId />");
            sb.Append("<errorDescription />");
            sb.Append("<hangoutUserId />");
            sb.Append("<sessionGuid />");
            sb.Append("<transactionRef />");
            sb.Append("<transactionStatus />");
            sb.Append("</Transaction><State>");
            sb.Append("<fromTransactState />");
            sb.Append("<toTransactState />");
            sb.Append("<lastRetryTime />");
            sb.Append("</State></MoneyPayment>");

            moneyPaymentDoc.LoadXml(sb.ToString());

            AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "transactionStatus", status);
            AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/Transaction", "moneyType", moneyType);

            if (toState > -1)
            {
                AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/State", "fromTransactState", "-1");
                AddValueAttribute(moneyPaymentDoc, "/MoneyPayment/State", "toTransactState", toState.ToString());
            }

            return moneyPaymentDoc;
        }

        private void AddValueAttribute(XmlDocument xmlDoc, string xPath, string element, string attributeValue)
        {
            if (attributeValue.Trim().Length > 0)
            {
                XmlNode xmlDocNode = xmlDoc.SelectSingleNode(xPath + "/" + element);
                XmlAttribute attribute = xmlDoc.CreateAttribute("value");
                attribute.Value = attributeValue;
                xmlDocNode.Attributes.Append(attribute);
            }
        }

        /// Retrieve the value given the commandArgs and a key
        /// </summary>
        /// <param name="commandArgs">The command arguments</param>
        /// <param name="key">The key </param>
        /// <param name="defaultValue">If an error occurs or the key is not available then returns the default value</param>
        /// <returns>The command argument value or if an error occurs or the key is not available then returns the default value</returns>
        private string GetStringValueFromArgs(NameValueCollection commandArgs, string key, string defaultValue)
        {
            string value = defaultValue;
            try
            {
                value = commandArgs[key];
                if (value == null)
                {
                    value = defaultValue;
                }
            }

            catch (Exception Exception)
            {
                mErrorLogger.Error("Error GetStringValueFromArgs", Exception);
            }

            return value;
        }

       private void LogMoneyTransactonEvent(XmlDocument moneyPaymentDoc)
        {
           // mMoneyPaymentsLog.Info(moneyPaymentDoc);

            string logKey = ConfigurationSettings.AppSettings["Key"]; 

            using (new SynchronizationContextSwitcher())
            {
                MoneyPaymentsInfoAppender logAppender = new MoneyPaymentsInfoAppender();

                logAppender.Key = logKey; 
                logAppender.LogEvent(moneyPaymentDoc, DateTime.Now);
            }
        }
    }
    public class SynchronizationContextSwitcher : IDisposable
    {
        private ExecutionContext _executionContext;
        private readonly SynchronizationContext _oldContext;
        private readonly SynchronizationContext _newContext;

        public SynchronizationContextSwitcher()
            : this(new SynchronizationContext())
        {
        }

        public SynchronizationContextSwitcher(SynchronizationContext context)
        {
            _newContext = context;
            _executionContext = Thread.CurrentThread.ExecutionContext;
            _oldContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(context);
        }

        public void Dispose()
        {
            if (null != _executionContext)
            {
                if (_executionContext != Thread.CurrentThread.ExecutionContext)
                    throw new InvalidOperationException("Dispose called on wrong thread.");

                if (_newContext != SynchronizationContext.Current)
                    throw new InvalidOperationException("The SynchronizationContext has changed.");

                SynchronizationContext.SetSynchronizationContext(_oldContext);
                _executionContext = null;
            }
        }
    }
}
