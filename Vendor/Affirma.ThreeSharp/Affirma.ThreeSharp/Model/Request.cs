/******************************************************************************* 
 *  Licensed under the Apache License, Version 2.0 (the "License"); 
 *  
 *  You may not use this file except in compliance with the License. 
 *  You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0.html 
 *  This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
 *  CONDITIONS OF ANY KIND, either express or implied. See the License for the 
 *  specific language governing permissions and limitations under the License.
 * ***************************************************************************** 
 * 
 *  Joel Wetzel
 *  Affirma Consulting
 *  jwetzel@affirmaconsulting.com
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Security.Cryptography;

namespace Affirma.ThreeSharp.Model
{
    /// <summary>
    /// The base class for all Request objects
    /// </summary>
    public class Request : Transfer
    {
        /// <summary>
        /// Timeout for the request
        /// </summary>
        protected int timeout = 50000;

        /// <summary>
        /// Content type for the request
        /// </summary>
        protected String contentType = "";

        /// <summary>
        /// A sorted list of the query parameters
        /// </summary>
        protected SortedList<string, string> queryList;

        /// <summary>
        /// A sorted list of metadata
        /// </summary>
        protected SortedList<string, string> metaData;

        /// <summary>
        /// Redirect Url for the request
        /// </summary>
        protected String redirectUrl;

        /// <summary>
        /// Constructor
        /// </summary>
        public Request()
        {
            this.Method = "PUT";
            this.queryList = new SortedList<string, string>();
            this.metaData = new SortedList<string, string>();
        }

        /// <summary>
        /// Timeout for the request
        /// </summary>
        public int Timeout
        {
            get { return this.timeout; }
        }

        /// <summary>
        /// Content type for the request
        /// </summary>
        public String ContentType
        {
            get { return this.contentType; }
        }

        /// <summary>
        /// A sorted list of query parameters
        /// </summary>
        public SortedList<string, string> QueryList
        {
            get { return this.queryList; }
        }

        /// <summary>
        /// A sorted list of metadata
        /// </summary>
        public SortedList<string, string> MetaData
        {
            get { return this.metaData; }
        }

        /// <summary>
        /// Redirect Url for the request
        /// </summary>
        public String RedirectUrl
        {
            get { return this.redirectUrl; }
            set { this.redirectUrl = value; }
        }

        /// <summary>
        /// Loads the request stream with a byte array.  Content type will be "text/plain"
        /// </summary>
        public void LoadStreamWithBytes(byte[] bytes)
        {
            LoadStreamWithBytes(bytes, "text/plain");
        }

        /// <summary>
        /// Loads the request stream with a byte array and sets the content type
        /// </summary>
        public void LoadStreamWithBytes(byte[] bytes, String contentType)
        {
            this.dataStream = new MemoryStream(bytes.Length);
            this.dataStream.Write(bytes, 0, bytes.Length);
            this.dataStream.Position = 0;

            this.contentType = contentType;
            this.BytesTotal = bytes.Length;
        }

        /// <summary>
        /// Loads the request stream with a string.  Content type will be "text/plain"
        /// </summary>
        public void LoadStreamWithString(String data)
        {
            LoadStreamWithString(data, "text/plain");
        }

        /// <summary>
        /// Loads the request stream with a string and sets the content type
        /// </summary>
        public void LoadStreamWithString(String data, String contentType)
        {
            UTF8Encoding ue = new UTF8Encoding();
            byte[] bytes = ue.GetBytes(data);
            this.LoadStreamWithBytes(bytes, contentType);
        }

        /// <summary>
        /// Loads the request stream with a file.  Content type will be determined by file extension.
        /// </summary>
        public void LoadStreamWithFile(String localfile)
        {
            LoadStreamWithFile(localfile, ThreeSharpUtils.ConvertExtensionToMimeType(Path.GetExtension(localfile)));
        }

        /// <summary>
        /// Loads the request stream with a file and sets the content type
        /// </summary>
        public void LoadStreamWithFile(String localfile, String contentType)
        {
            this.dataStream = File.OpenRead(localfile);

            this.contentType = contentType;
            this.BytesTotal = ((FileStream)this.dataStream).Length;
        }

        /// <summary>
        /// Loads the request stream with any other stream.  Content type will be "application/octet-stream"
        /// </summary>
        public void LoadStreamWithStream(Stream sourceStream)
        {
            LoadStreamWithStream(sourceStream, "application/octet-stream");
        }

        /// <summary>
        /// Loads the request stream with any other stream and sets the content type
        /// </summary>
        public void LoadStreamWithStream(Stream sourceStream, String contentType)
        {
            this.dataStream = sourceStream;

            this.contentType = contentType;
            this.BytesTotal = sourceStream.Length;
        }

        /// <summary>
        /// Encrypts the datastream using a DESCryptoServiceProvider
        /// </summary>
        public void EncryptStream(string encryptionKey, string encryptionIV)
        {
            EncryptStream(new DESCryptoServiceProvider(), encryptionKey, encryptionIV);
        }

        /// <summary>
        /// Encrypts the datastream using any SymmetricAlgorithm
        /// </summary>
        public void EncryptStream(SymmetricAlgorithm cryptoServiceProvider, string encryptionKey, string encryptionIV)
        {
            Stream existingStream = this.dataStream;

            cryptoServiceProvider.Key = ASCIIEncoding.ASCII.GetBytes(encryptionKey);
            cryptoServiceProvider.IV = ASCIIEncoding.ASCII.GetBytes(encryptionIV);
            CryptoStream cryptoStream = new CryptoStream(existingStream, cryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Read);
            this.dataStream = cryptoStream;

            // The encryption algorithm can pad the data by a few bytes, so these lines correct the byte count
            int blockBytes = cryptoServiceProvider.BlockSize / 8;
            this.BytesTotal = this.BytesTotal + blockBytes - (this.BytesTotal % blockBytes);
        }

    }
}
