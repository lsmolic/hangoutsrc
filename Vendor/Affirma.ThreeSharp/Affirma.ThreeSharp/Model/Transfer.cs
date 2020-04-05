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
using System.Text;

using Affirma.ThreeSharp.Statistics;

namespace Affirma.ThreeSharp.Model
{
    /// <summary>
    /// Represents a GET or a PUT request or response.
    /// The base class for Affirma.ThreeSharp.Model.Request and Affirma.ThreeSharp.Model.Response.
    /// Also used for statistical purposes.
    /// </summary>
    public class Transfer : IDisposable
    {
        /// <summary>
        /// The stream of data that is transferred
        /// </summary>
        protected Stream dataStream;

        /// <summary>
        /// A sorted list of request headers
        /// </summary>
        protected SortedList<string, string> headers;
        
        private bool isDisposed = false;
        private String id;
        private String method;
        private String bucketName;
        private String key;
        private ThreeSharpServiceType serviceType = ThreeSharpServiceType.S3;
        private long bytesTotal = 0;

        private TransferInfo transferInfo;

        /// <summary>
        /// Constructor
        /// </summary>
        public Transfer()
        {
            this.id = System.Guid.NewGuid().ToString();

            this.headers = new SortedList<string, string>();
        }

        /// <summary>
        /// Disposes of unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of unmanaged resources
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    if (dataStream != null)
                    {
                        dataStream.Dispose();
                        dataStream = null;
                    }
                }
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Transfer()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// A unique ID for the transfer
        /// </summary>
        public String ID
        {
            get { return this.id; }
        }

        /// <summary>
        /// The HTTP method of the transfer
        /// </summary>
        public String Method
        {
            get { return this.method; }
            set { this.method = value; }
        }

        /// <summary>
        /// The name of the bucket for the operation
        /// </summary>
        public String BucketName
        {
            get { return this.bucketName; }
            set { this.bucketName = value; }
        }

        /// <summary>
        /// The object key for the operation
        /// </summary>
        public String Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// Is this an S3 or AWS100 operation
        /// </summary>
        public ThreeSharpServiceType ServiceType
        {
            get { return this.serviceType; }
            set { this.serviceType = value; }
        }

        /// <summary>
        /// A sorted list of the HTTP headers
        /// </summary>
        public SortedList<string, string> Headers
        {
            get { return this.headers; }
            set { this.headers = value; }
        }

        /// <summary>
        /// The total number of bytes that will be transferred in the operation
        /// </summary>
        public long BytesTotal
        {
            get { return this.bytesTotal; }
            set { this.bytesTotal = value; }
        }

        /// <summary>
        /// TransferInfo
        /// </summary>
        public TransferInfo TransferInfo
        {
            get { return this.transferInfo; }
            set { this.transferInfo = value; }
        }

        /// <summary>
        /// The stream that is transferred in the operation
        /// </summary>
        public Stream DataStream
        {
            get { return this.dataStream; }
            set { this.dataStream = value; }
        }

    }
}
