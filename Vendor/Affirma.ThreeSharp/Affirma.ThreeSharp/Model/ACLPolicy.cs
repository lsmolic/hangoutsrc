using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Affirma.ThreeSharp.Model
{
    /// <summary>
    /// ACLPolicy
    /// </summary>
    public class ACLPolicy
    {
        private string ownerId;
        private string ownerName;
        private List<ACLGrantee> grantees = new List<ACLGrantee>();

        /// <summary>
        /// Constructor
        /// </summary>
        public ACLPolicy()
        {

        }

        /// <summary>
        /// Generates XML describing the ACLPolicy
        /// </summary>
        /// <returns></returns>
        public XmlDocument ToXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(this.ToString());

            return xmlDoc;
        }

        /// <summary>
        /// Generates XML describing the ACLPolicy
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string xmlStr = string.Format("<AccessControlPolicy xmlns=\"http://s3.amazonaws.com/doc/2006-03-01/\"><Owner><ID>{0}</ID><DisplayName>{1}</DisplayName></Owner><AccessControlList>", ownerId, ownerName);

            foreach (ACLGrantee grantee in grantees)
            {
                xmlStr += "<Grant>";
                xmlStr += grantee.ToString();
                xmlStr += "</Grant>";
            }

            xmlStr += "</AccessControlList></AccessControlPolicy>";

            return xmlStr;
        }

        /// <summary>
        /// OwnerId
        /// </summary>
        public string OwnerId
        {
            get { return this.ownerId; }
            set { this.ownerId = value; }
        }

        /// <summary>
        /// OwnerName (DisplayName)
        /// </summary>
        public string OwnerName
        {
            get { return this.ownerName; }
            set { this.ownerName = value; }
        }

        /// <summary>
        /// List of Grantees
        /// </summary>
        public List<ACLGrantee> Grantees
        {
            get { return this.grantees; }
            set { this.grantees = value; }
        }
    }
}
