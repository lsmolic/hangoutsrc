using System;
using System.Collections.Generic;
using System.Text;

namespace Affirma.ThreeSharp.Model
{
    /// <summary>
    /// Data object describing a distribution
    /// </summary>
    public class DistributionInfo
    {
        private string id;
        private string eTagHeader;
        private string status;
        private string lastModifiedTime;
        private string domainName;
        private DistributionConfig config;

        /// <summary>
        /// Constructor
        /// </summary>
        public DistributionInfo()
        {

        }        

        /// <summary>
        /// ID
        /// </summary>
        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// ETagHeader
        /// </summary>
        public string ETagHeader
        {
            get { return this.eTagHeader; }
            set { this.eTagHeader = value; }
        }

        /// <summary>
        /// Status
        /// </summary>
        public string Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        /// <summary>
        /// LastModifiedTime
        /// </summary>
        public string LastModifiedTime
        {
            get { return this.lastModifiedTime; }
            set { this.lastModifiedTime = value; }
        }

        /// <summary>
        /// DomainName
        /// </summary>
        public string DomainName
        {
            get { return this.domainName; }
            set { this.domainName = value; }
        }

        /// <summary>
        /// DistributionConfig
        /// </summary>
        public DistributionConfig DistributionConfig
        {
            get { return this.config; }
            set { this.config = value; }
        }
    }
}
