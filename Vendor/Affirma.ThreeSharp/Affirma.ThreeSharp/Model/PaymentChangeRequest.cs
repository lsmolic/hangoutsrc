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
    /// The request object for an Payment configuration operation
    /// </summary>
    public class PaymentChangeRequest : S3Request
    {
        /// <summary>
        /// Enumeration for the RequestPaymentConfiguration.payer configuration value
        /// </summary>
        public enum Payer
        {
            BucketOwner,
            Requester
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PaymentChangeRequest(String bucketName, Payer payer)
        {
            this.Method = "PUT";            
            this.BucketName = bucketName;
            this.queryList.Add("requestPayment", string.Empty);
            string payerString = (payer == Payer.Requester) ? "Requester" : "BucketOwner";
            string configXml = "<RequestPaymentConfiguration xmlns='http://s3.amazonaws.com/doc/2006-03-01/'><Payer>" + payerString + "</Payer></RequestPaymentConfiguration>";
            this.LoadStreamWithString(configXml, "text/xml");           
        }
    }
}
