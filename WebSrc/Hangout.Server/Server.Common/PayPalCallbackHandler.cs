using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using log4net;

namespace Hangout.Server
{
    public class PayPalCallbackHandler
    {
        private ILog mErrorLogger;

        public PayPalCallbackHandler()
        {
            mErrorLogger = LogManager.GetLogger("GeneralUse");
        }

        public string PayPalCallBack(string xmlInfo, string baseServicesRoot)
        {
            string response = "";

            try
            {
                XmlDocument callBackInfo = new XmlDocument();
                callBackInfo.LoadXml(xmlInfo);  

                MoneyTransactionLogging moneyLog = new MoneyTransactionLogging();
                moneyLog.PayPalLogResponse(callBackInfo, "CallBackBegin");

                XmlDocument callBackResponse = ProcessCallBack(xmlInfo, baseServicesRoot);

                moneyLog.PayPalLogResponse(callBackResponse, "CallBackComplete");

                response = parseCallBackResponse(callBackResponse);
            }

            catch (Exception ex)
            {
                mErrorLogger.Error("PayPalCallBack Error", ex);
            }

            return response;
        }

        private string parseCallBackResponse(XmlDocument callBackResponse)
        {
            string response = "Unknown Error Occured";
            string status = "Unknown";
            string redirectURL = "";
            string errorMessage = "";
            string errorCode = "";

            try
            {
                if (callBackResponse != null)
                {
                    if (callBackResponse.SelectSingleNode("Response/purchaseResult") != null)
                    {
                       status = callBackResponse.SelectSingleNode("Response/purchaseResult").Attributes["statusText"].InnerText;
                       if (callBackResponse.SelectSingleNode("Response/purchaseResult/redirectURL") != null)
                       {
                           redirectURL = callBackResponse.SelectSingleNode("Response/purchaseResult/redirectURL").InnerText;
                       }
                    }
                    else
                    {
                        if (callBackResponse.SelectSingleNode("Response/errors/error") != null)
                        {
                            status = "Error";
                            errorMessage = callBackResponse.SelectSingleNode("Response/errors/error/message").InnerText;
                            errorCode = callBackResponse.SelectSingleNode("Response/errors/error").Attributes["code"].InnerText;
                        }
                    }
                }
                response = CreateResponseString(status, redirectURL, errorMessage, errorCode);
            }

            catch (Exception ex)
            {
                mErrorLogger.Error("parseCallBackResponse Error", ex);
            }

            return response;
        }


        private string CreateResponseString(string status, string redirectURL, string errorMessage, string errorCode)
        {
            string response = "Unknown Error Occured";
            switch (status)
            {
                case "OK":
                    response = redirectURL;
                    break;

                case "Error":
                    response = PaymentItemResourceHandler.GetErrorMessageFromResourceFile(errorMessage, errorCode);
                    break;

                default:
                    break;
            }

            return response;
        }

        private XmlDocument ProcessCallBack(string xmlInfo, string baseServicesRoot)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(xmlInfo);

            WebServiceRequest request = new WebServiceRequest(baseServicesRoot, "PaymentItemsService", "PayPalCallback");
            request.AddParam("xmlInfoBinary", byteArray);

            XmlDocument xmlResponse = request.GetWebResponse();

            return xmlResponse;
        }
    }
}
