using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Server.WebServices
{
    public class AppErrorInformation :MarshalByRefObject 
    {
        private string _requestType;
        private string _url;
        private string _userHostAddress;
        private DateTime _timestamp;
        private string _machineName;
        private string _currentUserId;

        public string RequestType
        {
            get { return _requestType; }
            set { _requestType = value; }
        }

        public string URL
        {
            get { return _url; }
            set { _url = value; }
        }

        public string UserHostAddress
        {
            get { return _userHostAddress; }
            set { _userHostAddress = value; }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        public string MachineName
        {
            get { return _machineName; }
            set { _machineName = value; }
        }

        public string CurrentUserId
        {
            get { return _currentUserId; }
            set { _currentUserId = value; }
        }
    }
}
