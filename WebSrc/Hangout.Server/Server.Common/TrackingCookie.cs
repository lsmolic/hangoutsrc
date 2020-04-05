using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration;
using Hangout.Shared;

namespace Hangout.Server
{
    public class TrackingCookie
    {
        public void SetAddCookieUserOnRampTracking(string name, string value)
        {
            string cookieValue = "";

            if (name == null) 
                cookieValue = value;
            else if (name.Length > 0)
                cookieValue = "||" + name + "-->" + value;
            else
                cookieValue = value;

            HttpCookie existsCookie = HttpContext.Current.Request.Cookies.Get("onRampTracking");
            SimpleCrypto cryptographer = new SimpleCrypto();
            if (existsCookie != null)
            {
                
                string currentCookie = cryptographer.TDesDecrypt(existsCookie.Value).Trim();
                if (currentCookie.Contains(cookieValue.Trim()))
                {
                    cookieValue = currentCookie;
                }
                else
                {
                    cookieValue = currentCookie + "||" + cookieValue;
                }
            }

            cookieValue = cryptographer.TDesEncrypt(cookieValue);

            HttpCookie newOnRampTrackingCookie = new HttpCookie("onRampTracking", cookieValue);

            newOnRampTrackingCookie.Domain = ConfigurationManager.AppSettings["domain"];

            if (existsCookie == null)
            {
                HttpContext.Current.Response.Cookies.Add(newOnRampTrackingCookie);
                HttpContext.Current.Request.Cookies.Add(newOnRampTrackingCookie);
            }
            else
            {
                HttpContext.Current.Response.Cookies.Set(newOnRampTrackingCookie);
                HttpContext.Current.Request.Cookies.Set(newOnRampTrackingCookie);
            }
        }
    }
}

