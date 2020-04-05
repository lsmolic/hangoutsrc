using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Hangout.Server
{
    public class HangoutCookiesCollection : MarshalByRefObject
    {
        private Dictionary<string, HangoutCookie> _cookies = new Dictionary<string, HangoutCookie>();

        public Dictionary<string, HangoutCookie> CookieCollection
        {
            get { return _cookies; }
            set { _cookies = value; }
        }


        public void InitCookies(HttpCookieCollection cookies)
        {
            foreach (string cookieName in cookies)
            {
                HttpCookie cookie = cookies[cookieName];
                HangoutCookie appCookie = new HangoutCookie();

                appCookie.Name = cookie.Name;
                appCookie.Domain = cookie.Domain;
                appCookie.Expires = cookie.Expires;
                appCookie.Value = cookie.Value;
                appCookie.Values = cookie.Values;
                appCookie.IsDirty = false;
                _cookies.Add(cookieName, appCookie);
            }
        }

        public Dictionary<string, HangoutCookie> GetCookies()
        {
            return (_cookies);
        }

        public void ClearHangoutCookie()
        {
            HangoutCookie oCookie;
            if (_cookies.TryGetValue("hangout", out oCookie))
            {
                oCookie.CookieClear();
            }       
        }

        public void WriteCookies()
        {
            try
            {
                foreach (KeyValuePair<string, HangoutCookie> kvpCookie in _cookies)
                {
                    HangoutCookie cookie = kvpCookie.Value;
                    cookie.WriteCookie();
                }
            }
            catch { }

        }
    }
}
