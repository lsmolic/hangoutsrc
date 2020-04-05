using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Hangout.Server
{
    public class HangoutCookie : MarshalByRefObject
    {
        HangoutCookieInfo oCookieInfo = new HangoutCookieInfo();

        public string Domain
        {
            get { return oCookieInfo.Domain ; }
            set { oCookieInfo.Domain = value; }
        }

        public DateTime Expires
        {
            get { return oCookieInfo.Expires; }
            set { oCookieInfo.Expires = value; }
        }

        public string Name
        {
            get { return oCookieInfo.Name; }
            set { oCookieInfo.Name = value; }
        }

        public string Value
        {
            get { return oCookieInfo.Value; }
            set { oCookieInfo.Value = value; }
        }

        public NameValueCollection Values
        {
            get { return oCookieInfo.Values; }
            set { oCookieInfo.Values = value; }
        }

        public bool IsDirty
        {
            get { return oCookieInfo.IsDirty; }
            set { oCookieInfo.IsDirty = value; }
        }


        public HangoutCookieInfo GetCookie()
        {
            oCookieInfo.Domain = oCookieInfo.Domain;
            oCookieInfo.Expires = oCookieInfo.Expires;
            oCookieInfo.Name = oCookieInfo.Name;
            oCookieInfo.Value = oCookieInfo.Value;
            oCookieInfo.Values.Add(oCookieInfo.Values);

            return oCookieInfo;
        }

        public void CookieClear()
        {
            Value = "";
            Expires = DateTime.Now.Subtract(TimeSpan.FromHours(5000));
        }

        public void WriteCookie()
        {
            if (IsDirty)
            {
                HttpContext context = HttpContext.Current;

                context.Response.Cookies[Name].Domain = Domain;
                context.Response.Cookies[Name].Value = Value;
                context.Response.Cookies[Name].Expires = Expires;

                context.Request.Cookies[Name].Domain = Domain;
                context.Request.Cookies[Name].Value = Value;
                context.Request.Cookies[Name].Expires = Expires;

                IsDirty = false;
            }

        }
      
    }
}
