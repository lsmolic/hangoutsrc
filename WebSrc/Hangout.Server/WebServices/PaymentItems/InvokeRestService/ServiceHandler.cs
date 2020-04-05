using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Net;
using System.Web;
using System.Xml;
using log4net;
using log4net.Config;

namespace Hangout.Server.WebServices
{
    public class ServiceHandler
    {
        private string baseAddress = "";
        private ILog logger;

        public string BaseAddress
        {
            get { return baseAddress; }
            set { baseAddress = value; }
        }

        public ServiceHandler(string baseAddress)
        {
            BaseAddress = baseAddress;
            XmlConfigurator.Configure();
            logger = LogManager.GetLogger("ServiceHandler");
        }


        /// <summary>
        /// Exceutes a GetXMLRequest web service rest call
        /// Returns the response in the form of an XML document
        /// </summary>
        /// <param name="command">string command name</param>
        /// <param name="commandKeyValue">dictionary of key value pairs representing REST parameters</param>
        /// <returns>rest XML Response returned as an XML Document</returns>
        public XmlDocument GetXMLRequest(string command, Dictionary<string, string> commandKeyValue)
        {
            XmlDocument responseDoc = new XmlDocument();

            try
            {
                string responseData = GetRequest(command, commandKeyValue);
                responseDoc.LoadXml(responseData);
            }

            catch (Exception ex)
            {
                responseDoc = null;
                responseDoc = new XmlDocument();
                responseDoc.LoadXml(CreateErrorDoc(ex.Message));
                logger.Error("GetXMLRequest", ex);
            }

            return responseDoc;

        }

        /// <summary>
        /// Exceutes a GetStringRequest web service REST call. 
        /// Returns the response in the form of a string.
        /// </summary>
        /// <param name="command">string command name</param>
        /// <param name="commandKeyValue">dictionary of key value pairs representing REST parameters</param>
        /// <returns>REST response returned as a string</returns>
        public string GetStringRequest(string command, Dictionary<string, string> commandKeyValue)
        {
            string responseData = "";

            try
            {
                responseData = GetRequest(command, commandKeyValue);
            }

            catch(Exception ex)
            {
               responseData = CreateErrorDoc(ex.Message);
               logger.Error("GetStringRequest", ex);
            }

            return responseData;
        }

        /// <summary>
        /// Does the GetRequest call to the REST Web Service. 
        /// </summary>
        /// <param name="command">string command name</param>
        /// <param name="commandKeyValue">dictionary of key value pairs representing REST parameters</param>
        /// <returns>REST response returned as a string</returns>
        private string GetRequest(string command, Dictionary<string, string> commandKeyValue)
        {
            string responseData = "";
            Uri address = null;


            try
            {
                Uri baseUriAddress = new Uri(baseAddress);
                address = new Uri(baseUriAddress, ConvertCommandToURLString(command, commandKeyValue));

                logger.InfoFormat("GetRequest: {0}", address);

                // Create the web request   
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

                // Add authentication to request   
                // request.Credentials = new NetworkCredential("username", "password");

                long beginTimeTicks = DateTime.Now.Ticks;
                logger.Info(String.Format(" Start GetRequest: {0} Time: {1}", command, beginTimeTicks));

                // Get response   
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    responseData = reader.ReadToEnd();
                }

                logger.Info(String.Format(" End GetRequest: {0} Time: {1} DeltaTime: {2} ms", command, DateTime.Now.Ticks, (DateTime.Now.Ticks - beginTimeTicks)/10000));
            }

            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());

                        responseData = reader.ReadToEnd();
                        responseData = AddRequestToResponse(responseData, address.ToString());
                        logger.Error(String.Format ("GetRequest Response: {0}", responseData), ex);
                    }
                    else
                    {
                        logger.Error("GetRequest", ex);
                        throw ex;
                    }
                }
            }

            return responseData;
        }

        /// <summary>
        /// Exceutes a PostXMLRequest web service REST call.
        /// Returns the response in the form of an XML document.
        /// </summary>
        /// <param name="command">string command name</param>
        /// <param name="commandKeyValue">dictionary of key value pairs representing REST parameters</param>
        /// <returns>REST response returned as an XML Document</returns>
        public XmlDocument PostXMLRequest(string command, Dictionary<string, string> commandKeyValue)
        {
            XmlDocument responseDoc = new XmlDocument();

            try
            {
                string responseData = PostRequest(command, commandKeyValue);
                responseDoc.LoadXml(responseData);
            }

            catch (Exception ex)
            {
                responseDoc = null;
                responseDoc = new XmlDocument();
                responseDoc.LoadXml(CreateErrorDoc(ex.Message));
                logger.Error("PostXMLRequest", ex);
            }

            return responseDoc;
        }

        /// <summary>
        /// Does the PostRequest call to the REST Web Service. 
        /// </summary>
        /// <param name="command">string command name</param>
        /// <param name="commandKeyValue">dictionary of key value pairs representing REST parameters</param>
        /// <returns>REST response returned as a string</returns>
        private string PostRequest(string command, Dictionary<string, string> commandKeyValue)
        {
            string responseData = "";
            Uri address = null;

            try
            {
                Uri baseUriAddress = new Uri(baseAddress);
                address = new Uri(baseUriAddress, command);

                logger.InfoFormat("PostRequest Address: {0}", address);

                // Create the web request   
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

                // Add authentication to request   
                //request.Credentials = new NetworkCredential("username", "password");  

                // Set type to POST   
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";


                // Create a byte array of the data we want to send   
                byte[] byteData = ConvertCommandToPostData(commandKeyValue);

                // Set the content length in the request headers   
                request.ContentLength = byteData.Length;

                // Write data   
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                long beginTimeTicks =  DateTime.Now.Ticks;
                logger.Info(String.Format(" Start PostRequest: {0} Time: {1}", command, beginTimeTicks));

                // Get response   
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    responseData = reader.ReadToEnd();
                }

                logger.Info(String.Format(" End PostRequest: {0} Time: {1}  DeltaTime: {2}", command, DateTime.Now.Ticks, DateTime.Now.Ticks - beginTimeTicks));
            }

            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());

                        responseData = reader.ReadToEnd();
                        responseData = AddRequestToResponse(responseData, address.ToString());
     
                        logger.Error(String.Format("GetRequest Response: {0}", responseData), ex);
                    }
                    else
                    {
                        logger.Error("PostRequest", ex);
                        throw ex;
                    }
                }
            }

            return responseData;
        }

        /// <summary>
        /// Exceutes a DeleteXMLRequest web service REST call.
        /// Returns the response in the form of an XML document.
        /// </summary>
        /// <param name="command">string command name</param>
        /// <param name="commandKeyValue">dictionary of key value pairs representing REST parameters</param>
        /// <returns>REST response returned as an XML Document</returns>
        public XmlDocument DeleteXMLRequest(string command, Dictionary<string, string> commandKeyValue)
        {
            XmlDocument responseDoc = new XmlDocument();

            try
            {
                string responseData = DeleteRequest(command, commandKeyValue);
                responseDoc.LoadXml(responseData);
            }

            catch (Exception ex)
            {
                responseDoc = null;
                responseDoc = new XmlDocument();
                responseDoc.LoadXml(CreateErrorDoc(ex.Message));
                logger.Error("DeleteXMLRequest", ex);
            }

            return responseDoc;
        }


        /// <summary>
        /// Does the DeleteRequest call to the REST Web Service. 
        /// </summary>
        /// <param name="command">string command name</param>
        /// <param name="commandKeyValue">dictionary of key value pairs representing REST parameters</param>
        /// <returns>REST response returned as a string</returns>
        private string DeleteRequest(string command, Dictionary<string, string> commandKeyValue)
        {
            string responseData = "";
            Uri address = null;

            try
            {
                Uri baseUriAddress = new Uri(baseAddress);
                address = new Uri(baseUriAddress, ConvertCommandToURLString(command, commandKeyValue));

                logger.InfoFormat("DeleteRequest Address: {0}", address);

                // Create the web request   
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

                // Add authentication to request   
                //request.Credentials = new NetworkCredential("username", "password");  

                // Specify the DELETE method.
                request.Method = "DELETE";

                // Send the DELETE method request.  
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    responseData = reader.ReadToEnd();
                }
            }

            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());

                        responseData = reader.ReadToEnd();
                        responseData = AddRequestToResponse(responseData, address.ToString());
                        logger.Error(String.Format("DeleteRequest Response: {0}", responseData), ex);
                    }
                    else
                    {
                        logger.Error("DeleteRequest", ex);
                        throw ex;
                    }
                }
            }

            return responseData;
        }



        /// <summary>
        /// Exceutes a MultiPartFormXMLPost web service REST call.
        /// Returns the response in the form of an XML document.
        /// </summary>
        /// <param name="command">string command name</param>
        /// <param name="commandKeyValue">dictionary of key value pairs representing REST parameters</param>
        /// <param name="postData">MemoryStream that contains the file post data.</param>
        /// <returns>REST response returned as an XML Document</returns>
        public XmlDocument MultiPartFormXMLPost(string command, Dictionary<string, string> commandKeyValue, MemoryStream postData)
        {
            XmlDocument responseDoc = new XmlDocument();

            try
            {
                string responseData = MultiPartFormPost(command, commandKeyValue, postData);
                responseDoc.LoadXml(responseData);
            }

            catch (Exception ex)
            {
                responseDoc = null;
                responseDoc = new XmlDocument();
                responseDoc.LoadXml(CreateErrorDoc(ex.Message));
                logger.Error("MultiPartFormXMLPost", ex);
            }

            return responseDoc;
        }


        /// <summary>
        /// Does the MultiPartFormPost call to the REST Web Service. 
        /// </summary>
        /// <param name="command">string command name</param>
        /// <param name="commandKeyValue">dictionary of key value pairs representing REST parameters</param>
        /// <param name="postData">MemoryStream that contains the file post data.</param>
        /// <returns>REST response returned as a string</returns>
        private string MultiPartFormPost(string command, Dictionary<string, string> commandKeyValue, MemoryStream postData)
        {
            string responseData = "";
            Uri address = null;
           
            try
            {
                Uri baseUriAddress = new Uri(baseAddress);
                address = new Uri(baseUriAddress, command);

                logger.InfoFormat("MultiPartFormPost Address: {0}", address);

                // Create the web request   
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

                string boundary = Guid.NewGuid().ToString().Replace("-", "");

                //boundary = "---------------------------7d936a1153067a";  //test

                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Method = "POST";

               // StreamReader sr = new StreamReader("c:\\twofishPostDataTest.txt");
               // int dataLength = (int)sr.BaseStream.Length;
               // char[] cbuffer = new char[dataLength];
                //sr.ReadBlock(cbuffer, 0, dataLength);
                //byte[] byteData = Encoding.UTF8.GetBytes(cbuffer);

                byte[] byteData = GetMultipartFormData(postData, commandKeyValue, boundary);

                // Set the content length in the request headers   
                request.ContentLength = byteData.Length;

                // Write data   
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                //StreamWriter swCommand = new StreamWriter("c:\\twofishPostDataTestLast.txt");

               // string tempString = Encoding.UTF8.GetString(byteData, 0, byteData.Length);
               // swCommand.Write(tempString);
               // swCommand.Close();

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    responseData = reader.ReadToEnd();
                }

            } 

            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());

                        responseData = reader.ReadToEnd();
                        responseData = AddRequestToResponse(responseData, "");
                        logger.Error(String.Format("MultiPartFormPostResponse: {0}", responseData), ex);
                    }
                    else
                    {
                        logger.Error("MultiPartFormPost", ex);
                        throw ex;
                    }
                }
            } 

            return responseData;
        }

        /// <summary>
        /// Generates the multipartfrom data and returns the data in a byte array.
        /// </summary>
        /// <param name="postData">MemoryStream that contains the file post data.</param>
        /// <param name="commandKeyValue">dictionary of key value pairs representing REST parameters</param>
        /// <param name="boundary">The multipartfrom boundary string</param>
        /// <returns>byte array containing the multipartfrom data to post.</returns>
        private byte[] GetMultipartFormData(MemoryStream postData, Dictionary<string, string> commandKeyValue, string boundary)
        {
            MemoryStream dataStream = new MemoryStream();
            Encoding encoding = Encoding.UTF8;

            string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n", boundary, "filename", "Upload.csv", "application/vnd.ms-excel");

            byte[] byteData = new byte[postData.Length];
            postData.Position = 0;
            postData.Read(byteData, 0, (int)postData.Length);
            postData.Close();

            dataStream.Write(encoding.GetBytes(header), 0, header.Length);

            // Write the file data directly to the Stream, rather than serializing it to a string.
            dataStream.Write(byteData, 0, byteData.Length);

            foreach (KeyValuePair<string, string> kvp in commandKeyValue)
            {
                string postInfo = string.Format("\r\n--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}", boundary, kvp.Key, kvp.Value);
                dataStream.Write(encoding.GetBytes(postInfo), 0, postInfo.Length);
            }   
            
            // Add the end of the request
            string footer = "\r\n--" + boundary + "--\r\n";
            dataStream.Write(encoding.GetBytes(footer), 0, footer.Length);

            dataStream.Position = 0;
            byte[] formData = new byte[dataStream.Length];
            dataStream.Read(formData, 0, formData.Length);
            dataStream.Close();

            return formData;
        }


        /// <summary>
        /// Wrap string data response in an response xml tag.
        /// </summary>
        /// <param name="noun">The command noun</param>
        /// <param name="verb">The command verb</param>
        /// <param name="name">The node name to wrap the string data in.</param>
        /// <param name="data">string data to add to the node.</param>
        /// <returns>XMLDocument containing the response.</returns>
        public XmlDocument WrapDataXML(string noun, string verb, string name, string data)
        {
            XmlDocument responseDoc = new XmlDocument();
            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("<response noun='{0}' verb='{1}'><{2}><![CDATA[{3}]]></{2}></response>", noun, verb, name, data));
            responseDoc.LoadXml(sb.ToString());

            return responseDoc;
        }

        /// <summary>
        /// Converts a command and commandKeyValue key value pairs into a URL string.
        /// </summary>
        /// <param name="command">The command name</param>
        /// <param name="commandKeyValue">The key value pair parameters</param>
        /// <returns>URL string</returns>
        private string ConvertCommandToURLString(string command, Dictionary<string, string> commandKeyValue)
        {
            string url = command;
            string delim = "?";

            foreach (KeyValuePair<string, string> kvp in commandKeyValue)
            {
                if (kvp.Value != null && kvp.Value.Trim().Length > 0)
                {
                    url += delim + kvp.Key.Trim() + "=" + kvp.Value.Trim();
                    delim = "&";
                }
            }
            return url;
        }

        /// <summary>
        /// Converts commandKeyValue key value pairs into a UTF8Encoded byte array.
        /// </summary>
        /// <param name="commandKeyValue">The key value pair parameters</param>
        /// <returns>post data in the form of a byte array.</returns>
        private byte[] ConvertCommandToPostData(Dictionary<string, string> commandKeyValue)
        {
            byte[] postData = null;
            StringBuilder data = new StringBuilder();
            StringBuilder dataToLog = new StringBuilder();

            string delim = "";

            foreach (KeyValuePair<string, string> kvp in commandKeyValue)
            {
                string[] itemsToScrub = new string[] { "cc.number", "cc.securityCode" };

                if (kvp.Value != null && kvp.Value.Trim().Length > 0)
                {
                    data.Append(delim + kvp.Key.Trim() + "=" + HttpUtility.UrlEncode(kvp.Value.Trim()));
                    dataToLog.Append(delim + kvp.Key.Trim() + "=");
                    if (IsKeyScrub(kvp.Key, itemsToScrub) == false)
                    {
                        dataToLog.Append(HttpUtility.UrlEncode(kvp.Value.Trim()));
                    }
                    delim = "&";
                }
            }

            if (data.Length > 0)
            {
                logger.InfoFormat("ConvertCommandToPostData Data: {0}", dataToLog);
                postData = UTF8Encoding.UTF8.GetBytes(data.ToString());
            }
            return postData;
        }

        /// <summary>
        /// Creates an response xml errror document.
        /// </summary>
        /// <param name="message">Error message string</param>
        /// <returns>XML error document string </returns>
        private string CreateErrorDoc(string message)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<response><Error><Message>" + message + "</Message></Error></response>");
            return sb.ToString();
        }


        /// <summary>
        /// Check if the key is in the itemsToScrub array
        /// </summary>
        /// <param name="key">The key to check against</param>
        /// <param name="itemsToEncrypt">The string array of key items to scrub</param>
        /// <returns>true if the key matches an item in the itemsToScrub array else false</returns>
        private bool IsKeyScrub(string key, string[] itemsToScrub)
        {
            bool encrypt = false;
            foreach (string item in itemsToScrub)
            {
                if (item == key)
                {
                    encrypt = true;
                    break;
                }
            }
            return encrypt;
        }


        /// <summary>
        /// Escape and HTML document so that it can be placed in a XML document.
        /// </summary>
        /// <param name="message">HTML document</param>
        /// <returns>Escaped HTML document.</returns>
        private string EscapeHTMLdocument(string message)
        {
            string messageEscaped = message.Replace("&", "&amp;");
            messageEscaped = messageEscaped.Replace("<", "&lt;");
            messageEscaped = messageEscaped.Replace(">", "&gt;");
            messageEscaped = messageEscaped.Replace("\"", "&quot;");
            messageEscaped = messageEscaped.Replace("'", "&apos;");

            return messageEscaped;
        }

        /// <summary>
        /// Adds request URL to the response
        /// </summary>
        /// <param name="responseData">response string data</param>
        /// <param name="address">string url</param>
        /// <returns>Response with request URL added.</returns>
        private string AddRequestToResponse(string responseData, string address)
        {
            string newResponse = responseData.Replace("<response>", "<response request='" + address.Replace("&", "&amp;") + "'>");
            return newResponse;
        }
    }
}
