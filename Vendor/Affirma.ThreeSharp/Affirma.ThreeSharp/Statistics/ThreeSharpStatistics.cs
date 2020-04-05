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
    /// Keeps track of all transfers made by the ThreeSharpQuery class.
    /// </summary>
    public class ThreeSharpStatistics
    {
        private Dictionary<string, TransferInfo> transferInfos;

        /// <summary>
        /// Constructor
        /// </summary>
        public ThreeSharpStatistics()
        {
            this.transferInfos = new Dictionary<string, TransferInfo>();
        }

        /// <summary>
        /// Adds a transfer to the list
        /// </summary>
        public void AddTransferInfo(TransferInfo transferInfo)
        {
            this.transferInfos.Add(transferInfo.ID, transferInfo);
        }

        /// <summary>
        /// Removes a transfer from the list
        /// </summary>
        public void RemoveTransferInfo(String id)
        {
            if (!this.transferInfos.ContainsKey(id))
            {
                throw new ArgumentOutOfRangeException("TransferInfo.ID", "The TransferInfo.ID does not exist to be removed.");
            }

            this.transferInfos.Remove(id);
        }

        /// <summary>
        /// Returns the internal list of transferInfos as an array
        /// </summary>
        public TransferInfo[] GetTransferInfos()
        {
            List<TransferInfo> trfrs = new List<TransferInfo>();
            foreach (TransferInfo trfr in this.transferInfos.Values)
            {
                trfrs.Add(trfr);
            }
            return trfrs.ToArray();
        }

        /// <summary>
        /// Returns a single TransferInfo by ID
        /// </summary>
        public TransferInfo GetTransferInfo(String id)
        {
            return this.transferInfos[id];
        }

        /// <summary>
        /// Calculates the total number of bytes uploaded
        /// </summary>
        public long TotalBytesUploaded
        {
            get
            {
                long count = 0;
                foreach (TransferInfo transfer in this.transferInfos.Values)
                {
                    if (transfer.Method == "PUT")
                    {
                        count += transfer.BytesTransferred;
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// Calculates the total number of bytes downloaded
        /// </summary>
        public long TotalBytesDownloaded
        {
            get
            {
                long count = 0;
                foreach (TransferInfo transfer in this.transferInfos.Values)
                {
                    if (transfer.Method == "GET")
                    {
                        count += transfer.BytesTransferred;
                    }
                }
                return count;
            }
        }

    }
}
