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
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;

namespace Affirma.ThreeSharp.Model
{
    /// <summary>
    /// The request object for an ObjectCopy operation
    /// </summary>
    public class ObjectCopyRequest : S3Request
    {
		/// <summary>
		/// Constructor
		/// </summary>
		public ObjectCopyRequest(String sourceBucketName, String sourceKey, String destinationBucketName, String destinationKey)
		{
			this.Method = "PUT";

			this.BucketName = destinationBucketName;
			this.Key = destinationKey;

			this.headers.Add("x-amz-copy-source", sourceBucketName + "/" + sourceKey);
		}

		/// <summary>
		/// Make it work
		/// </summary>
		/// <param name="sourceBucketName"></param>
		/// <param name="sourceKey"></param>
		/// <param name="timestamp"></param>
		/// <param name="eTag"></param>
		/// <param name="destinationBucketName"></param>
		/// <param name="destinationKey"></param>
		public ObjectCopyRequest(String sourceBucketName, String sourceKey, String timestamp, String eTag, String destinationBucketName, String destinationKey)
        {
            this.Method = "PUT";

            this.BucketName = destinationBucketName;
            this.Key = destinationKey;

            this.headers.Add("x-amz-copy-source", sourceBucketName + "/" + sourceKey);
			this.headers.Add("x-amz-copy-source-if-modified-since", timestamp);
			//this.headers.Add("x-amz-copy-source-if-none-match", eTag);
			this.headers.Add("x-amz-acl", "public-read");
        }
    }
}