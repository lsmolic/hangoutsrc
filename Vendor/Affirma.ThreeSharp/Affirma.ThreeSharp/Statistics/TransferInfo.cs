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
using System.Collections.Generic;
using System.Text;
using Affirma.ThreeSharp.Model;

namespace Affirma.ThreeSharp.Statistics
{
    /// <summary>
    /// Contains info about a transfer, for use by the ThreeSharpStatistics class
    /// </summary>
    public class TransferInfo
    {
        private String id;
        private String method;
        private String bucketName;
        private String key;
        private long bytesTransferred = 0;
        private long bytesTotal = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public TransferInfo(Transfer t)
        {
            this.id = t.ID;
            this.method = t.Method;
            this.bucketName = t.BucketName;
            this.key = t.Key;
            this.bytesTotal = t.BytesTotal;
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
        /// The number of bytes transferred in the operation
        /// </summary>
        public long BytesTransferred
        {
            get { return this.bytesTransferred; }
            set { this.bytesTransferred = value; }
        }

        /// <summary>
        /// The total number of bytes that will be transferred in the operation
        /// </summary>
        public long BytesTotal
        {
            get { return this.bytesTotal; }
            set { this.bytesTotal = value; }
        }

    }
}
