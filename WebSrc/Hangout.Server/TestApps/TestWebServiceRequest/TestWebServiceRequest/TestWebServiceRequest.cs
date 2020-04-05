using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;
using System.Configuration;
using Hangout.Shared;
using System.ComponentModel;
using System.Xml.Serialization;
using log4net;
using System.Threading;

namespace Hangout.Server
{
    /// <summary>
    /// Calls REST Service and returns void
    /// ServiceResponse is an XML Document of the REST results, or just blank
    /// </summary>
    public class TestWebServiceRequest
    {
        private class TimeoutState
        {
            public IAsyncResult result;
            public HttpWebRequest webRequest;
            public AsyncCallback callback;
            public TimeoutState(IAsyncResult res, HttpWebRequest req, AsyncCallback cb)
            {
                result = res;
                webRequest = req;
                callback = cb;
            }
        }

		public static int kNumberOfServicesCalled = 0;
		public static int kNumberOfServicesCompleted = 0;

        public static int mPendingRequests = 0;

        private static WebServiceRequestManager mWebServiceRequestManager = new WebServiceRequestManager();
        public static WebServiceRequestManager WebServiceRequestManager
        {
            get { return mWebServiceRequestManager; }
        }

		private ILog mLogger;
        private Dictionary<string, string> mStringParams = new Dictionary<string, string>();
        private Dictionary<string, byte[]> mBinaryParams = new Dictionary<string, byte[]>();
		private string mServiceUrl = String.Empty;
		private string mNoun = String.Empty;
		private string mVerb = String.Empty;
		private FormMethod mMethod = FormMethod.GET;
        private bool mEncrypted = false;
		//private Action<XmlDocument> mCallback = null;
        private XmlDocument mServiceResponse = new XmlDocument();
		private int mTimeout = int.Parse(ConfigurationSettings.AppSettings["ServicesTimeout"]);

		

		public string ServiceUrl
		{
			get { return mServiceUrl; }
		}

		public FormMethod Method
		{
			get { return mMethod; }
			set { mMethod = value; }
		}

		public bool Encrypted 
		{
			get { return mEncrypted; }
			set { mEncrypted = value; }
		}
    


        public TestWebServiceRequest(string webServicesUrl, string noun, string verb)
        {
            mLogger = LogManager.GetLogger("Test");

            mServiceUrl = webServicesUrl + noun + "/" + verb;
			mNoun = noun;
			mVerb = verb;
            if (Session.GetCurrentUserId() != null)
            {
                AddParam("userId", Session.GetCurrentUserId().Id.ToString());
            }
        }

        //GET METHOD
        public void AddParam(string paramName, string paramValue)
        {
            string tempValue = paramValue;
            if (mEncrypted == true)
            {
                SimpleCrypto cryptographer = new SimpleCrypto();
                tempValue = cryptographer.TDesEncrypt(tempValue);
            }
            if (mStringParams.ContainsKey(paramName))
            {
                mStringParams.Remove(paramName);
            }
            mStringParams.Add(paramName, tempValue);
        }
		
        public void AddParam(string paramName, byte[] paramValue)
        {
			mMethod = FormMethod.POST;
            mBinaryParams.Add(paramName, paramValue);
        }


        /// <summary>
        ///  Synchronously processes a web service via Get/Post
        /// </summary>
        /// <returns>Returns an XmlDocument with the processed data.</returns>
        public XmlDocument GetWebResponse()
        {
            return EndGetResponse(BeginGetResponse(null));
        }


        /// <summary>
        ///  Asynchronously processes a web service via Get/Post
        /// </summary>
        /// <returns>Returns nothing, instead calls callback with the XmlDocument result.</returns>
		public void GetWebResponseAsync(Action<XmlDocument> callback)
        {
            BeginGetResponse(new AsyncCallback(delegate(IAsyncResult res)
            {
                mWebServiceRequestManager.Enqueue(callback, EndGetResponse(res));
            }));
        }


        private IAsyncResult BeginGetResponse(AsyncCallback callback)
        {
            StringBuilder serviceUri = new StringBuilder();
            HttpWebRequest serviceRequest = null;
            
            ++kNumberOfServicesCalled;

            if (mMethod == FormMethod.POST)
            {
                string boundary = Guid.NewGuid().ToString().Replace("-", "");
                MemoryStream postData = GetPostData(boundary);

                byte[] byteData = new byte[postData.Length];
                postData.Read(byteData, 0, (int)postData.Length);
                postData.Close();

                serviceRequest = (HttpWebRequest)WebRequest.Create(mServiceUrl);
                serviceRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                serviceRequest.Method = mMethod.ToString();
                serviceRequest.ContentLength = byteData.Length;

                Stream serviceStreamWriter = serviceRequest.GetRequestStream();
                serviceStreamWriter.Write(byteData, 0, byteData.Length);
                serviceStreamWriter.Close();

            }
            else
            {
                serviceUri.Append(mServiceUrl);
                if (mEncrypted)
                {
                    serviceUri.Append("?encrypted=true");
                }
                else
                {
                    serviceUri.Append("?encrypted=false");
                }
                foreach (KeyValuePair<string, string> param in mStringParams)
                {
                    serviceUri.AppendFormat("&{0}={1}", param.Key, param.Value);
                }
                mServiceUrl = serviceUri.ToString();
                serviceRequest = (HttpWebRequest)WebRequest.Create(mServiceUrl);
                serviceRequest.Method = mMethod.ToString();
                serviceRequest.ContentType = "text/xml;";
            }

            System.Threading.Interlocked.Increment(ref mPendingRequests);
            mLogger.InfoFormat("Num pending requests ({0}), starting: {1} ", mPendingRequests, mServiceUrl);

            int numWorker, numIOCP;
            System.Threading.ThreadPool.GetAvailableThreads(out numWorker, out numIOCP);
            mLogger.InfoFormat("Available threads: " + numWorker + ", " + numIOCP);

            IAsyncResult res = serviceRequest.BeginGetResponse(callback, serviceRequest);

            // Managing our own timeout because async requests don't obey HttpWebRequest.Timeout
            ThreadPool.RegisterWaitForSingleObject(res.AsyncWaitHandle, TimeoutCallback, new TimeoutState(res, serviceRequest, callback), mTimeout, true);

            return res;
        }


        private XmlDocument EndGetResponse(IAsyncResult res)
        {
            XmlDocument serviceResponseXml = new XmlDocument();
            HttpWebRequest serviceRequest = (HttpWebRequest) res.AsyncState;

            try
            {
                HttpWebResponse serviceResponse = (HttpWebResponse)serviceRequest.EndGetResponse(res);
                serviceResponseXml.Load(serviceResponse.GetResponseStream());
                serviceResponse.Close();
            }
            catch (System.Net.WebException ex)
            {
                switch (ex.Status)
                {
                    case WebExceptionStatus.RequestCanceled:
                        mLogger.WarnFormat("Service call to {0} timed out.", mServiceUrl);
                        break;
                    default:
                        if (ex.Response != null)
                        {
                            mLogger.ErrorFormat("WebException on service call {0}, \n\tStatus: {1},\n\tResponse status code: {2}, \n\tResponse description: {3}", mServiceUrl,
                                ex.Status,
                                ((HttpWebResponse)ex.Response).StatusCode,
                                ((HttpWebResponse)ex.Response).StatusDescription);
                        }
                        else
                        {
                            mLogger.ErrorFormat("WebException on service call {0}, \n\tStatus: {1}", mServiceUrl, ex.Status);
                        }
                        break;
                }

                serviceResponseXml = (new SimpleResponse("error", "status code=" + ex.Status + "\n" + ex.ToString()));
            }
            catch (System.Exception ex)
            {
                mLogger.Error("Unexpected error in EndGetResponse: " + ex.ToString());
                serviceResponseXml = (new SimpleResponse("error", ex.ToString()));
            }
            finally
            {
                ++kNumberOfServicesCompleted;
                System.Threading.Interlocked.Decrement(ref mPendingRequests);
                mLogger.InfoFormat("Num pending requests ({0}), finished: {1}/{2} ", mPendingRequests, mNoun, mVerb);
            }

            return serviceResponseXml;
        }


        private WaitOrTimerCallback TimeoutCallback = new WaitOrTimerCallback(delegate(object state, bool timedOut)
        {
            if (timedOut)
            {
                IAsyncResult result = ((TimeoutState)state).result;
                HttpWebRequest serviceRequest = ((TimeoutState)state).webRequest;
                AsyncCallback callback = ((TimeoutState)state).callback;

                serviceRequest.Abort();

                if (callback != null)
                {
                    callback(result);
                }
            }
        });


        private MemoryStream GetPostData(string boundary)
        {
            MemoryStream postData = new MemoryStream();
            StreamWriter sw = new StreamWriter(postData);

            string newLine = "\r\n";

            string encryted = "false";

            if (mEncrypted)
            {
                encryted = "true";
            }

            sw.Write("--" + boundary + newLine);
            sw.Write("Content-Disposition: form-data; name=\"{0}\"{1}{1}{2}{1}", "encrypted", newLine, encryted);

            foreach (KeyValuePair<string, string> param in mStringParams)
            {
                sw.Write("--" + boundary + newLine);
                sw.Write("Content-Disposition: form-data; name=\"{0}\"{1}{1}{2}{1}", param.Key, newLine, param.Value);
            }

            foreach (KeyValuePair<string, byte[]> param in mBinaryParams)
            {
                sw.Write("--" + boundary + newLine);
                sw.Write("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", param.Key, param.Key, newLine);
                sw.Write("Content-Type: application/octet-stream" + newLine + newLine);
                sw.Flush();

                postData.Write(param.Value, 0, param.Value.Length);
                sw.Write(newLine);
            }

            sw.Write("--{0}--{1}", boundary, newLine);
            sw.Flush();

            postData.Position = 0;
            return postData;
        }
    }
}