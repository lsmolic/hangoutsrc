using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Hangout.Server.WebServices
{
    [XmlType(TypeName = "inviteUserProfile")]
    public class InviteUserProfile
    {
        private string _firstName;

        [XmlElement(ElementName = "firstName")]
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        private string _lastName;

        [XmlElement(ElementName = "lastName")]
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        private int _inviteRoomId;

        [XmlElement(ElementName = "inviteRoomId")]
        public int InviteRoomId
        {
            get { return _inviteRoomId; }
            set { _inviteRoomId = value; }
        }
    }
}
