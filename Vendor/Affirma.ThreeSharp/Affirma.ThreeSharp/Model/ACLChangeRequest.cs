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
using System.Security.Cryptography;

namespace Affirma.ThreeSharp.Model
{
    /// <summary>
    /// The request object for an ACLChange operation
    /// </summary>
    public class ACLChangeRequest : S3Request
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ACLChangeRequest(String bucketName, String key)
        {
            this.Method = "PUT";            
            this.BucketName = bucketName;
            this.Key = key;
            this.queryList.Add("acl", "");
        }

        /// <summary>
        /// Overloaded for legacy support
        /// </summary>
        public ACLChangeRequest(string bucketName, string key, string xAmzAcl)
        {
            this.Method = "PUT";

            this.BucketName = bucketName;
            this.Key = key;
            this.headers.Add("x-amz-acl", xAmzAcl);
        }
    }
}
