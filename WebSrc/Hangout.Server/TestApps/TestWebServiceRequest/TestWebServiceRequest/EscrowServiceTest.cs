using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
using log4net;

namespace Hangout.Server
{
    public class EscrowServiceTest
    {
        private string mNumRequests = ConfigurationSettings.AppSettings["NumberOfRequests"];
        private string mWebServicesBaseUrl = ConfigurationSettings.AppSettings["WebServicesBaseUrl"];
        private ILog mLogger;
        private int mTotalTests = 0;

        public EscrowServiceTest()
        {
            mLogger = LogManager.GetLogger("Test");
            int numRequests = 0;

            bool isInt = false;
            if (mNumRequests != null)
            {
                isInt = Int32.TryParse(mNumRequests, out numRequests);
            }
            if (isInt)
            {
                mLogger.Debug("Attemping " + numRequests + " Service Calls to " + mWebServicesBaseUrl + "Escrow/CreateEscrowTransaction");
                mTotalTests = numRequests;
                for (int i = 0; i < numRequests; i++)
                {
                    TestWebServiceRequest callService = new TestWebServiceRequest(mWebServicesBaseUrl, "Escrow", "CreateEscrowTransaction");
                    callService.AddParam("toFacebookAccountId", "13001919");
                    callService.AddParam("fromAccountId", "17501155");
                    callService.AddParam("transactionType", "FashionCityJobCoins");
                    callService.AddParam("value", "20");
                    //mTotalTests++;
                    mLogger.Debug("ServiceRequest num: " + i + "\n");
                    //Console.Write("ServiceRequest num: " + i + ", TotalTests: " + totalTests + "\n");

                    if (ConfigurationSettings.AppSettings["SendRequestsAsync"] == "true")
                    {
                        if (ConfigurationSettings.AppSettings["ProcessResultsWithDelegate"] == "true")
                        {
                            callService.GetWebResponseAsync(delegate(XmlDocument xmlResponse)
                            {
                                mTotalTests--;
                                //mLogger.Debug(xmlResponse.OuterXml + "\n");
                                mLogger.Debug("TestsRemaining: " + mTotalTests + "\n");
                                //Console.Write(xmlResponse.OuterXml + "\n");
                                //Console.Write("TestsRemaining: " + totalTests + "\n");
                            });
                        }
                        else
                        {
                            callService.GetWebResponseAsync(ProcessResult);
                        }
                    }
                    else
                    {
                        XmlDocument response = callService.GetWebResponse();
                        mTotalTests--;
                        mLogger.Debug("TestsRemaining: " + mTotalTests + "\n");
                    }
                }
            }
            else
            {
                mLogger.Debug("NumberOfRequests is not a valid Int32: " + mNumRequests);
                //Console.WriteLine("NumberOfRequests is not a valid Int32: " + mNumRequests);
            }
        }

        private void ProcessResult(XmlDocument response)
        {
            mTotalTests--;
            //mLogger.Debug(xmlResponse.OuterXml + "\n");
            mLogger.Debug("TestsRemaining: " + mTotalTests + "\n");
        }
    }
}
