using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Affirma.ThreeSharp.Model
{
    /// <summary>
    /// ACLGrantee
    /// </summary>
    public class ACLGrantee
    {
        private string id;
        private string displayName;
        private GranteeType type = GranteeType.User;
        private GrantPermission permission = GrantPermission.Read;

        /// <summary>
        /// GrantPermission
        /// </summary>
        public enum GrantPermission : int
        {
            /// <summary>
            /// READ
            /// </summary>
            Read = 0,
            /// <summary>
            /// WRITE
            /// </summary>
            Write,
            /// <summary>
            /// FULL_CONTROL
            /// </summary>
            FullControl
        }

        /// <summary>
        /// GranteeType
        /// </summary>
        public enum GranteeType : int
        {
            /// <summary>
            /// AmazonCustomerByEmail
            /// </summary>
            Email = 0,
            /// <summary>
            /// CanonicalUser
            /// </summary>
            User,
            /// <summary>
            /// Group
            /// </summary>
            Group,
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ACLGrantee()
        {

        }

        /// <summary>
        /// String equivalent of the GranteeType
        /// </summary>
        private string GetTypeValue()
        {
            return (type == GranteeType.Group) ? "Group" : (type == GranteeType.Email) ? "AmazonCustomerByEmail" : "CanonicalUser";
        }

        /// <summary>
        /// String equivalent of the GrantPermission
        /// </summary>
        private string GetPermissionValue()
        {
            return (permission == GrantPermission.FullControl) ? "FULL_CONTROL" : (permission == GrantPermission.Write) ? "WRITE" : "READ";
        }

        /// <summary>
        /// Generates XML describing the ACLGrantee
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string xmlStr = "<Grantee xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:type=\"" + this.GetTypeValue() + "\">";

            if (this.type == GranteeType.Email)
                xmlStr += "<EmailAddress>" + this.id + "</EmailAddress>";
            else if (this.type == GranteeType.Group)
                xmlStr += "<URI>" + this.id + "</URI>";
            else // GranteeType.User
            {
                xmlStr += "<ID>" + this.id + "</ID>";
                xmlStr += "<DisplayName>" + this.displayName + "</DisplayName>";
            }

            xmlStr += "</Grantee>";
            xmlStr += "<Permission>" + this.GetPermissionValue() + "</Permission>";

            return xmlStr;
        }

        /// <summary>
        /// ID (User Id, URI, or Email Address)
        /// </summary>
        public string ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// DisplayName (Only used for type CanonicalUser)
        /// </summary>
        public string DisplayName
        {
            get { return this.displayName; }
            set { this.displayName = value; }
        }

        /// <summary>
        /// GranteeType 
        /// </summary>
        public GranteeType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        /// <summary>
        /// GrantPermission 
        /// </summary>
        public GrantPermission Permission
        {
            get { return this.permission; }
            set { this.permission = value; }
        }
    }
}
