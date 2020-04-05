using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Server;
using Hangout.Shared;



namespace Hangout.Server.WebServices
{
    /// <summary>
    /// This is the interface for the PaymentItems.dll.  
    /// Implements the IPaymentItemInterface
    /// Implements MarshalByRefObject
    /// The StateServer should use the public string ProcessMessage(string xmlMessage) interface method.
    /// This method limits the calls to the HangoutCommandHandler.  
    /// Admin tools can use public XmlDocument ProcessMessage(PaymentCommand command)
    /// This method gives access to both HangoutCommandHandler and TwoFishCommand Handler
    /// </summary>
    /// 
    public class PaymentItem : MarshalByRefObject, IPaymentItemInterface
    {
        /// <summary>
        /// Main entry point for the state server to use the PaymentItems 
        /// This will limited to Hangout commands
        /// </summary>
        /// <param name="xmlMessage">PaymentCommand string xml message</param>
        /// <returns>string XML response message</returns>
        public string ProcessMessage(string xmlMessage)
        {
            string response = "";

            try
            {
                ServiceCommandSerializer paymentSerializer = new ServiceCommandSerializer();
                PaymentCommand command = (PaymentCommand)paymentSerializer.DeserializeCommandData(xmlMessage, typeof(PaymentCommand));

                Hangout.Server.WebServices.HangoutCommandBase parser = new Hangout.Server.WebServices.HangoutCommandBase("InvokePaymentItemsProcess");
                response = parser.ProcessRequest(command).InnerXml;
            }

            catch (Exception ex)
            {
               response = CreateErrorDoc(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Main entry point for the admin toolto use the PaymentItems 
        /// This will allow for both Hangout commands, Twofish commands to be run
        /// </summary>
        /// <param name="xmlMessage">PaymentCommand xml document</param>
        /// <returns>XMLDocument with response message</returns>
        public XmlDocument ProcessMessage(PaymentCommand command)
        {
            XmlDocument response = null;

            try
            {
                Hangout.Server.WebServices.CommandParser parser = GetCommandParser(command.Noun);
                response = parser.ProcessRequest(command);
            }

            catch (Exception ex)
            {
                response = CreateErrorXmlDoc(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Main entry point for the PayPal callback handler.
        /// </summary>
        /// <param name="xmlMessage">string xml message from PayPal callback.</param>
        /// <returns>string xmlMessage response</returns>
        public string CallBackMessage(string xmlMessage)
        {
            string response = "";

            try
            {
                PayPalCallBackHandler callBackHandler = new PayPalCallBackHandler("PayPalCallBack");
                response = callBackHandler.PayPalCallBack(xmlMessage).InnerXml;
            }

            catch (Exception ex)
            {
                response = CreateErrorDoc(ex.Message);
            }

            return response;
        }


        public XmlDocument AdminProcessMessage(PaymentCommand command)
        {
            XmlDocument response = null;

            try
            {
                Hangout.Server.WebServices.AdminCommandBase parser = new Hangout.Server.WebServices.AdminCommandBase("InvokePaymentItemsProcess");
                response = parser.ProcessRequest(command);
            }

            catch (Exception ex)
            {
                response = CreateErrorXmlDoc(ex.Message);
            }

            return response;
        }


        /// <summary>
        /// Find the CommandParser to use.
        /// Tries to find the class in hangoutCommand if not then then must be TwoFish
        /// </summary>
        /// <param name="noun">string is the noun, class to use.</param>
        /// <returns>Returns the CommandParser either HangoutCommand or TwoFishCommand</returns>
        private Hangout.Server.WebServices.CommandParser GetCommandParser(string noun)
        {
            Hangout.Server.WebServices.CommandParser parser = null;

            Hangout.Server.WebServices.HangoutCommandBase hangoutParser = new Hangout.Server.WebServices.HangoutCommandBase("InvokePaymentItemsProcess");
            if (hangoutParser.GetCommandClassType(noun) != null)
            {
                parser = (Hangout.Server.WebServices.CommandParser)hangoutParser;
            }
            else
            {
                parser = new TwoFishCommandBase("HangoutCommand");
            }
            return parser;
        }

        /// <summary>
        /// Create an XML error document string
        /// </summary>
        /// <param name="message">Error message</param>
        /// <returns>Response error xml string</returns>
        private string CreateErrorDoc(string message)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<response><Error><Message>" + message + "</Message></Error></response>");
            return sb.ToString();
        }

        /// <summary>
        /// Create an XML error document
        /// </summary>
        /// <param name="message">Error message</param>
        /// <returns>Response error xml document</returns>
        private XmlDocument CreateErrorXmlDoc(string message)
        {
            XmlDocument errDocument = null;
            try
            {
                errDocument = new XmlDocument();
                errDocument.LoadXml(CreateErrorDoc(message));
            }

            catch { }

            return errDocument;
        }
    }
        
}
