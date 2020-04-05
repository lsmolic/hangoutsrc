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
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;

namespace Affirma.ThreeSharp.Model
{
    /// <summary>
    /// The request object for a BucketList operation
    /// </summary>
    public class BucketListRequest : S3Request
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BucketListRequest(String bucketName)
        {
            this.Method = "GET";
            this.BucketName = bucketName;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BucketListRequest(String bucketName, String prefix)
        {
            this.Method = "GET";
            this.BucketName = bucketName;
            this.QueryList.Add("delimeter", "");
            this.QueryList.Add("marker", "");
            this.QueryList.Add("prefix", prefix);
        }
    }
}
