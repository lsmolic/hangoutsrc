using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using Hangout.Shared;
using Hangout.Server;
using Hangout.Server.WebServices;

namespace WebServices.PaymentItemsService
{
	
    public class PaymentItemsService : ServiceBaseClass
    {
		private ServicesLog mServiceLog = new ServicesLog("PaymentItemsService");
		
        /// <summary>
        /// Process a PaymentItem Command 
        /// Added to the RestCommand paramters are a noun and verb parameters
        /// These are the noun and verb used to call the PaymentItem command.
        /// </summary>
        /// <param name="command">command Paramters</param>
        /// <returns>XMLDocument containing PaymentItem result</returns>
        public XmlDocument ProcessPaymentItem(RESTCommand command)
        {
            XmlDocument response = new XmlDocument();

            PaymentCommand paymentCommand = CreatePaymentCommand(command.Parameters);

            HangoutCommandBase parser = new HangoutCommandBase("PaymentItemsService");

            response = parser.ProcessRequest(paymentCommand);

            return response;
        }


        public XmlDocument ProcessPaymentItemString(HangoutPostedFile paymentCommand)
        {
            XmlDocument response = new XmlDocument();
            byte[] buffer = new byte[paymentCommand.InputStream.Length];
            paymentCommand.InputStream.Read(buffer, 0, (int)paymentCommand.InputStream.Length);

            UTF8Encoding encoding = new UTF8Encoding();
            String paymentCommandString = encoding.GetString(buffer);

            ServiceCommandSerializer serializer = new ServiceCommandSerializer();
            PaymentCommand command = (PaymentCommand)serializer.DeserializeCommandData(paymentCommandString, typeof(PaymentCommand));

            HangoutCommandBase parser = new HangoutCommandBase("PaymentItemsService");

            response = parser.ProcessRequest(command);

            return response;
        }


        public XmlDocument PayPalCallback(HangoutPostedFile xmlInfoBinary)
        {
           XmlDocument response = new XmlDocument();
           byte[] buffer = new byte[xmlInfoBinary.InputStream.Length];
           xmlInfoBinary.InputStream.Read(buffer, 0, (int)xmlInfoBinary.InputStream.Length);

            UTF8Encoding encoding = new UTF8Encoding();
            String xmlInfo = encoding.GetString(buffer);

            PayPalCallBackHandler handler = new PayPalCallBackHandler("PaymentItemsService");

            response = handler.PayPalCallBack(xmlInfo);

            return response;
       }

        public XmlDocument ProcessAdminPaymentItemXml(HangoutPostedFile paymentCommand)
        {
            XmlDocument response = new XmlDocument();
            byte[] buffer = new byte[paymentCommand.InputStream.Length];
            paymentCommand.InputStream.Read(buffer, 0, (int)paymentCommand.InputStream.Length);

            UTF8Encoding encoding = new UTF8Encoding();
            String paymentCommandString = encoding.GetString(buffer);

            ServiceCommandSerializer serializer = new ServiceCommandSerializer();
            PaymentCommand command = (PaymentCommand)serializer.DeserializeCommandData(paymentCommandString, typeof(PaymentCommand));

            AdminCommandBase parser = new AdminCommandBase("PaymentItemsAdminService");

            response = parser.ProcessRequest(command);

            return response;
        }
        

        /// <summary>
        /// Creates the PaymentItem command from the RestCommand NameValueCollection or command arguments
        /// </summary>
        /// <param name="commandArgs">NameValueCollection containing the PaymentItem command 
        /// including a noun and verb parameter which are the PaymentItem command noun and verb</param>
        /// <returns>PaymentCommand</returns>
        private PaymentCommand CreatePaymentCommand(NameValueCollection commandArgs)
        {
            PaymentCommand paymentCommand = new PaymentCommand();

            paymentCommand.Noun = commandArgs["noun"];
            paymentCommand.Verb = commandArgs["verb"]; 
            
            commandArgs.Remove("noun");
            commandArgs.Remove("verb");
            
            paymentCommand.Parameters = commandArgs;

            return paymentCommand;
        }
    }
}
