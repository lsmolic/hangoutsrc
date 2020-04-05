using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;

namespace Hangout.Server.WebServices
{

    /// <summary>
    /// This implements the callback method from PayPal to complete the purchase process.
    /// This functions expects the XML result from the callback page in the following form:
    /// There are 3 return types:
    /// PaypalReturn  ==> succesfully charged the card.
    /// PaypalCancel  ==> user canceled the transaction
    /// PaypalRecurring ==> a recurring transaction was succesfully initiated.
    /// In each case the twofish service needs to be called to comlete the transaction.
    /// The parameters used to call twofish are the return paramaters from the callback.
    // <Response command='PaypalReturn'>
    //    <data name='userId' value='545'> 
    //    </data><data name='offerId' value='29'> 
    //    </data><data name='externalTxnId' value='954a9f42-1df9-4e16-bf56-5b8abcc7611e'> 
    //    </data><data name='token' value='EC-57K58313BY1773505'> </data>
    //    <data name='payerId' value='8FBU5YKUF3MM6'> </data>
    //    <data name='ipAddress' value='192.168.163.28'></data>
    // </Response>
    ///
    /// Return Values: 
    /// Key: userId Value: 545 
    /// Key: offerId Value: 29 
    /// Key: externalTxnId Value: 954a9f42-1df9-4e16-bf56-5b8abcc7611e 
    /// Key: token Value: EC-57K58313BY1773505
    /// Key: PayerID Value: 8FBU5YKUF3MM6 */
    /// </summary>
    public class PayPalCallBackHandler : CommandHandlerBase
    {
        public PayPalCallBackHandler(string loggerName) : base(loggerName) { }


        /// <summary>
        /// Callback method used by PayPal 
        /// </summary>
        /// <param name="xmlResult">string in the form of XML that is the request variables from PayPal</param>
        /// <returns>XmlDocument containing the response after the PayPal transaction is completed</returns>
        public XmlDocument PayPalCallBack(string xmlResult)
        {
            logDebug("PayPalCallBack", xmlResult);

            XmlDocument response = null;

            try
            {
                XmlDocument docResult = new XmlDocument();
                docResult.LoadXml(xmlResult);

                switch (GetReturnType(docResult))             
                {
                    case "PaypalReturn":
                        response = PayPalPurchase(docResult);
                        break;

                    case "PaypalCancel":
                        response = PayPalCancel(docResult);
                        break;

                    case "PaypalRecurring":
                        response = PayPalRecurring(docResult);
                        break;

                    default:
                        break;
                }
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("PayPalCallBack", ex);
            }

            return response;
        }
        /// <summary>
        /// Check if this PaypalRecurring transaction. 
        /// </summary>
        /// <param name="docResult">The XML response documents from PayPal.</param>
        /// <returns>Returns the command typ</returns>
        private string GetReturnType(XmlDocument docResult)
        {
            string command = GetXMLDocumentParameter(docResult, "/Response", "command", "");
            string recurring = GetXMLDocumentParameter(docResult, "/Response/data[@name='startDate']", "value", "");

            if ((command == "PaypalReturn") && (recurring.Trim().Length > 0))
            {
                command = "PaypalRecurring";
            }
            return command;
        }

        /// <summary>
        /// Call twoFish to complete the transaction.
        /// </summary>
        /// <param name="xmlResult">The XML response documents from PayPal.</param>
        /// <returns>XmlDocument containing the response after the PayPal transaction is completed.</returns>
        private XmlDocument PayPalPurchase(XmlDocument xmlResult)
        {
            XmlDocument response = null;

            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Purchase";
            cmd.Verb = "PayPalPurchase";

            string externalTransactionId = GetXMLDocumentParameter(xmlResult, "/Response/data[@name='externalTxnId']", "value", "");

            cmd.Parameters.Add("userId", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='userId']", "value", ""));
            cmd.Parameters.Add("offerId", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='offerId']", "value", ""));
            cmd.Parameters.Add("token", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='token']", "value", ""));
            cmd.Parameters.Add("payerId", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='payerId']", "value", ""));
            cmd.Parameters.Add("externalTxnId", externalTransactionId);
            cmd.Parameters.Add("ipAddress", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='ipAddress']", "value", ""));

            response = CallTwoFishService(cmd);

            if (response.SelectSingleNode("/Response/purchaseResult") == null)
            {
                AddAttribute(response, "/Response", "externalTxnId", externalTransactionId);
            }
            
            return response;
        }

        /// <summary>
        /// Call twoFish to complete the cancel.
        /// </summary>
        /// <param name="xmlResult">The XML response documents from PayPal.</param>
        /// <returns>XmlDocument containing the response after the PayPal transaction is completed.</returns>
        private XmlDocument PayPalCancel(XmlDocument xmlResult)
        {
            XmlDocument response = null;

            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Purchase";
            cmd.Verb = "PayPalCancel";

            cmd.Parameters.Add("userId", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='userId']", "value", ""));
            cmd.Parameters.Add("offerId", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='offerId']", "value", ""));
            cmd.Parameters.Add("token", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='token']", "value", ""));
            cmd.Parameters.Add("externalTxnId", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='externalTxnId']", "value", ""));
            cmd.Parameters.Add("ipAddress", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='ipAddress']", "value", ""));

            response = CallTwoFishService(cmd);

            return response;
        }

        /// <summary>
        /// Call twoFish to complete the recurring transaction.
        /// </summary>
        /// <param name="xmlResult">The XML response documents from PayPal.</param>
        /// <returns>XmlDocument containing the response after the PayPal recurring transaction is completed.</returns>
        private XmlDocument PayPalRecurring(XmlDocument xmlResult)
        {
            XmlDocument response = null;

            PaymentCommand cmd = new PaymentCommand();
            cmd.Noun = "Purchase";
            cmd.Verb = "PayPalRecurringPurchase";

            cmd.Parameters.Add("userId", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='userId']", "value", ""));
            cmd.Parameters.Add("offerId", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='offerId']", "value", ""));
            cmd.Parameters.Add("externalTxnId", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='externalTxnId']", "value", ""));
            cmd.Parameters.Add("token", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='token']", "value", ""));
            cmd.Parameters.Add("payerId", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='payerId']", "value", ""));
            cmd.Parameters.Add("ipAddress", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='ipAddress']", "value", ""));
            cmd.Parameters.Add("billingDescription", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='billingDescription']", "value", ""));
            cmd.Parameters.Add("startDate", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='startDate']", "value", ""));
            cmd.Parameters.Add("numPayments", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='numPayments']", "value", ""));
            cmd.Parameters.Add("payFrequency", GetXMLDocumentParameter(xmlResult, "/Response/data[@name='payFrequency']", "value", ""));

            response = CallTwoFishService(cmd);

            return response;
        }

        /// <summary>
        /// Method used to call the TwoFishService
        /// </summary>
        /// <param name="command">contains the Twofish PaymentCommand</param>
        /// <returns>The Twofish command response.</returns>
        private XmlDocument CallTwoFishService(PaymentCommand command)
        {
            XmlDocument response = null;

            try
            {
                TwoFishCommandBase parser = new TwoFishCommandBase(System.Reflection.MethodBase.GetCurrentMethod());
                response = parser.ProcessRequest(command);
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
                logError("CallTwoFishService", ex);
            }

            return response;
        }


        /// <summary>
        /// Get the XML document Parameter from the request XmlDocument.
        /// </summary>
        /// <param name="xmlDoc">XMLDocument request document.</param>
        /// <param name="xPath">The XPath for the parameter</param>
        /// <param name="attribute">The parameter name to retrieve.</param>
        /// <param name="defaultValue">If an error occurs or value not found the default value to return.</param>
        /// <returns>string value that is the parameter or default value if the attribute is not found.</returns>
        private string GetXMLDocumentParameter(XmlDocument xmlDoc, string xPath, string attribute, string defaultValue)
        {
            string value = defaultValue;
            try
            {
                value = xmlDoc.SelectSingleNode(xPath).Attributes[attribute].InnerText;
            }
            catch { }

            return value;
        }

        private void AddAttribute(XmlDocument xmlDoc, string xPath, string attributeName, string attributeValue)
        {
            XmlNode xmlDocNode = xmlDoc.SelectSingleNode(xPath);
            XmlAttribute attribute = xmlDoc.CreateAttribute(attributeName);
            attribute.Value = attributeValue;
            xmlDocNode.Attributes.Append(attribute);
        }

    }
}



