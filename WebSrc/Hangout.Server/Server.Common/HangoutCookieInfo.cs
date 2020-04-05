using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Hangout.Server
{
    public class HangoutCookieInfo : MarshalByRefObject
    {
        private string _domain;
        private DateTime _expires;
        private string _name;
        private string _value;
        private NameValueCollection _values = new NameValueCollection();
        private bool _isDirty = false;

        public string Domain
        {
            get { return _domain; }
            set { _domain = value; _isDirty = true; }
        }

        public DateTime Expires
        {
            get { return _expires; }
            set { _expires = value; _isDirty = true; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; _isDirty = true; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; _isDirty = true; }
        }

        public NameValueCollection Values
        {
            get { return _values; }
            set { _values = value; _isDirty = true; }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }
    }
}
