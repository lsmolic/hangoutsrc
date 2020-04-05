using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.ComponentModel;
using System.Threading;
using System.Configuration;
using log4net;

using Hangout.Server.StateServer;
using Hangout.Shared;

//to do 
// handle retry

namespace Hangout.Server
{
    public class MoneyPaymentNotifyClient
    {
        private BackgroundWorker mBackgroundWorker = null;

        private int mPollValue = -1;
        private int mThrottleValue = -1;
        private bool mOff = false;
                     
        private ILog mLogger;

        private AutoResetEvent mAutoTerminateEvent = new AutoResetEvent(false);

        private Hangout.Shared.Action<Message, Guid> mSendMessageToClientCallback = null;
        private string mWebServicesBaseUrl = ConfigurationSettings.AppSettings["WebServicesBaseUrl"];


        /// <summary>
        /// Notify the client of complete transaction
        /// Updated the database with notify state
        /// </summary>
        /// <param name="sendMessageToClientCallback"></param>
        public MoneyPaymentNotifyClient(Hangout.Shared.Action<Message, Guid> sendMessageToClientCallback)
        {
            mLogger = LogManager.GetLogger("MoneyPaymentNotifyClient");
            mSendMessageToClientCallback = sendMessageToClientCallback;

            GetNotifyConfigInfo();

            if (!mOff)
            {
                StartNotifyThread();
            }
        }

        /// <summary>
        /// Reads the MoneyNotify app config file node
        /// </summary>
        private void GetNotifyConfigInfo()
        {
            try
            {
                MoneyNotifySection notifyConfig = (MoneyNotifySection)ConfigurationManager.GetSection("MoneyNotify");
                mPollValue = ConvertToInt(notifyConfig.Notify.Poll, -1);
                mThrottleValue = ConvertToInt(notifyConfig.Notify.Throttle, -1);
                mOff = ConvertToBool(notifyConfig.Notify.Off, false);
            }

            catch (Exception ex)
            {
                mLogger.Error(String.Format("Error in GetNotifyConfigInfo Error: {1} ", ex));
            }
        }

        /// <summary>
        /// Background worker thread to poll the database via services for transaction status
        /// Process complete transaction and if money is added to the account notify the client
        /// </summary>
        private void StartNotifyThread()
        {
            mBackgroundWorker = new BackgroundWorker();
            mBackgroundWorker.WorkerSupportsCancellation = false;
            mBackgroundWorker.DoWork += new DoWorkEventHandler(bwMainDoWork);
            mBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwMainRunWorkerCompleted);
            mBackgroundWorker.RunWorkerAsync();
        }

        /// Background worker thread that polls the database via services for transaction status
        /// Waits mPollValue in seconds between database calls
        private void bwMainDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    BackgroundMain(e);
					
                    if (mPollValue == -1)
                    {
                        break;
                    }

                    TimeSpan timeSpan = new TimeSpan(0, 0, mPollValue);
                    int eventIndex = WaitHandle.WaitAny(new WaitHandle[] { mAutoTerminateEvent }, timeSpan, false);
                    if (eventIndex != WaitHandle.WaitTimeout)
                    {
                        break;
                    }
                }
            }

            catch (Exception ex)
            {
                mLogger.Error(String.Format("Error in bwMainDoWork {0} ", ex));
            }
        }

        /// <summary>
        /// Main background loop.  
        /// 1. Process Complete transactions 
        /// 2. Process Retry Tranactions
        /// </summary>
        /// <param name="e"></param>
        private void BackgroundMain(DoWorkEventArgs e)
        {
            ProcessCompleteTransactions();
            ProcessRetryTransactions();
        }

        /// <summary>
        /// Process the complete transactions.
        /// Call GetTransactionsToNotify (complete transactions) async
        /// then call ProcessCompleteTransactions with the returned xml. 
        /// </summary>
        private void ProcessCompleteTransactions()
        {
            Action<XmlDocument> getTransactionsToNotifyCallback = delegate(XmlDocument xmlTransactionsToNotify)
            {
                ProcessCompleteTransactions(xmlTransactionsToNotify);
            };
            GetTransactionsToNotify(getTransactionsToNotifyCallback);
        }

        /// <summary>
        /// Porcess the complete transactions
        /// Will process up to the throttle number of transaction, before exiting the loop.
        /// Reads the XML and then call ProcessTransactionNode 
        /// This is so that a tranasaction that is causing an exception will not stop other
        /// transactions from being processed.
        /// </summary>
        /// <param name="xmlTransactionsToNotify"></param>
        private void ProcessCompleteTransactions(XmlDocument xmlTransactionsToNotify)
        {
            try
            {
                int loopCount = 0;
                if (xmlTransactionsToNotify.SelectSingleNode("/RESTerror") != null)
                {
                    string error = String.Format("RESTerror xmlTransactionsToNotify,{0})", xmlTransactionsToNotify.SelectSingleNode("/RESTerror").InnerXml);
                    mLogger.Error(error);
                    throw new Exception(error);
                }

                XmlNodeList transactionNodes = xmlTransactionsToNotify.SelectNodes("/Transactions/Transaction");
                foreach (XmlNode transactionNode in transactionNodes)
                {
                    ProcessTransactionNode(transactionNode);

                    if (IsThrottle(++loopCount))
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(String.Format("Error in ProcessCompleteTransactions {0} ", ex));
            }
        }

        /// <summary>
        /// Process an individual node (transaction)
        /// Log the Event setting to the transaction to complete
        /// If the transaction was successful notify the client
        /// </summary>
        /// <param name="transactionNode">The transaction node from the database query. </param>
        private void ProcessTransactionNode(XmlNode transactionNode)
        {
            try
            {
                string externalTransactionId = GetNodeValue(transactionNode, "ExternalTransactionId");
                string moneyType = GetNodeValue(transactionNode, "MoneyType");
                string sessionGuid = GetNodeValue(transactionNode, "SessionGuid");
                string transactionStatus = GetNodeValue(transactionNode, "TransactionStatus");
                string notifyInfo = String.Format("<Response noun='HangoutPurchase' verb='NotifyCashUpdate' type='{0}'></Response>", moneyType);

                CallLogEvent(externalTransactionId, moneyType);
                if (IsTransactionSuccess(externalTransactionId, transactionStatus))
                {
                    CallNotifyClient(sessionGuid, notifyInfo);
                }
            }

            catch(Exception ex)
            {
                mLogger.Error(String.Format("Error in ProcessTransactionNode {0} ", ex));
            }
        }

        private void ProcessRetryTransactions()
        {
            try
            {
                string xmlInfo = "";
                string response = "";

                PayPalCallbackHandler handler = new PayPalCallbackHandler();
              //  response = handler.PayPalCallBack(xmlInfo, mWebServicesBaseUrl);
            }

            catch (Exception ex)
            {
                mLogger.Error(String.Format("Error in ProcessRetryTransactions {0} ", ex));
            }

         }

        /// <summary>
        /// Background worker completed
        /// </summary>
        /// <param name="sender">sender not used</param>
        /// <param name="e">ServiceCommandAsync parameters containing the results and information required for the callback</param>
        private void bwMainRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }
       
        private void GetTransactionsToNotify(Action<XmlDocument> getTransactionToNotifyCallback)
        {
            try
            {
                WebServiceRequest getTransactionToNotifyService = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "MoneyPaymentsLog", "GetTransactionsToNotify");
                getTransactionToNotifyService.AddParam("state", "3");
                getTransactionToNotifyService.GetWebResponseAsync(getTransactionToNotifyCallback);
            }
            catch (Exception ex)
            {
                mLogger.Error(String.Format("Error in GetTransactionsToNotify {0} ", ex));
            }
        }

        private bool IsTransactionSuccess(string externalTransactionId, string transactionStatus)
        {
            bool transactionSuccess = false;
            if (externalTransactionId.Length > 0)
            {
                switch (transactionStatus)
                {
                    case "OK":
                    case "Complete":
                        transactionSuccess = true;
                        break;
                }
            }
           return transactionSuccess;
        }

        private bool IsThrottle(int loopCount)
        {
            bool throttle = false;

            if (mThrottleValue > 0)
            {
                if (loopCount >= mThrottleValue)
                {
                    throttle = true;
                }
            }
            return throttle;
        }
        
        private void CallNotifyClient(string sessionId, string notifyInfo)
        {
            try
            {
                if (sessionId.Length > 0)
                {
                    Guid sessionIdGuid = new Guid(sessionId);

                    Message sendPaymentItemNofityToClientMessage = new Message();

                    List<object> dataObject = new List<object>();
                    dataObject.Add(notifyInfo);
                    sendPaymentItemNofityToClientMessage.PaymentItemsMessage(dataObject);

                    mSendMessageToClientCallback(sendPaymentItemNofityToClientMessage, sessionIdGuid);
                }
            }

            catch(Exception ex)
            {
                mLogger.Error(String.Format("Error in CallNotifyClient Error: {0} ", ex));
            }
        }

        private void CallLogEvent(string externalTransactionId, string moneyType)
        {
            MoneyTransactionLogging transaction = new MoneyTransactionLogging();
            transaction.NotifyClientComplete(externalTransactionId, moneyType);
        }

        private string GetNodeValue(XmlNode transactionNode, string element)
        {
            string retValue = "";

            try
            {
                retValue = transactionNode.SelectSingleNode(element).InnerText;
            }
            catch { }

            return retValue;
        }

        /// <summary>
        /// Convert string value to integer, if the passed in value is not a valid integer than return the default value
        /// </summary>
        /// <param name="value">string value to convert to an integer</param>
        /// <param name="defaultValue">default integer value</param>
        /// <returns>string value converted to integer, if the passed in value is not a valid integer than return the default value</returns>
        private int ConvertToInt(string value, int defaultValue)
        {
            int returnValue = defaultValue;

            int tempValue = -1;
            if (int.TryParse(value, out tempValue))
            {
                returnValue = tempValue;
            }

            return returnValue;
        }

        /// <summary>
        /// Convert string value to bool, if the passed in value is not a valid bool than return the default value
        /// </summary>
        /// <param name="value">string value to convert to an bool</param>
        /// <param name="defaultValue">default integer value</param>
        /// <returns>string value converted to bool, if the passed in value is not a valid bool than return the default value</returns>
        private bool ConvertToBool(string value, bool defaultValue)
        {
            bool returnValue = defaultValue;

            bool tempValue = false;
            if (bool.TryParse(value, out tempValue))
            {
                returnValue = tempValue;
            }

            return returnValue;
        }
    }

    /// <summary>
    /// Define the MoneyNotify custom section 
    /// </summary>
    public class MoneyNotifySection : ConfigurationSection
    {
        [ConfigurationProperty("notify")]
        public NotifyConfigElement Notify
        {
            get { return ((NotifyConfigElement)base["notify"]); }
        }
    }

    /// <summary>
    /// The Notify Configuration Element
    /// </summary>
    public class NotifyConfigElement : ConfigurationElement
    {
        /// <summary>
        /// The poll configuration property specifies the poll time in seconds
        /// </summary>
        [ConfigurationProperty("poll", IsRequired = true)]
        public string Poll
        {
            get { return (string)this["poll"]; }
            set { this["poll"] = value; }
        }

        /// <summary>
        /// The throttle configuration property specifies the int throttle number
        /// </summary>
        [ConfigurationProperty("throttle")]
        public string Throttle
        {
            get { return (string)this["throttle"]; }
            set { this["throttle"] = value; }
        }

        /// <summary>
        /// The off configuration property specifies the bool off bool value
        /// </summary>
        [ConfigurationProperty("off")]
        public string Off
        {
            get { return (string)this["off"]; }
            set { this["off"] = value; }
        }
    }
 }
