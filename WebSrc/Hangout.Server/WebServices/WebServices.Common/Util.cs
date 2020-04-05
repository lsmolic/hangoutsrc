using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Configuration;
using Hangout.Shared;

namespace Hangout.Server.WebServices {
    public class Util {
        public static string GetHttpValue(string key) {
            SimpleCrypto cryptographer = new SimpleCrypto();
            /// searches the current httpcontext for the key
            /// checks first for the key in the url variables, then in the form variables
            HttpContext context = HttpContext.Current;
            string urlarg = context.Request.QueryString[key];
            string formarg = context.Request.Form[key];

            bool encryped = false;
            if (context.Request.QueryString["encrypted"] != null || context.Request.Form["encrypted"] != null)
            {
                if (context.Request.HttpMethod == "GET") {
                    encryped = context.Request.QueryString["encrypted"].ToLower().Equals("true");
                } else {
                    encryped = context.Request.Form["encrypted"].ToLower().Equals("true");
                }
            }
            
            if (urlarg != null) {
                if (!encryped) {
                    return urlarg;
                } else {
                    return cryptographer.TDesDecrypt(urlarg);
                }
            } else if (formarg != null) {
                if (!encryped) {
                    return formarg;
                } else {
                    return cryptographer.TDesDecrypt(formarg);
                }

            } else {
                return null;
            }
        }

        public static NameValueCollection GetHttpValueArray(HttpContext context)
        {
            SimpleCrypto cryptographer = new SimpleCrypto();
            bool encryped = false;
            NameValueCollection HttpValues = null;

            if (context.Request.QueryString["encrypted"] != null || context.Request.Form["encrypted"] != null)
            {
                encryped = context.Request.QueryString["encrypted"].ToLower().Equals("true");
            }

            HttpValues = new NameValueCollection();

            foreach (string key in context.Request.QueryString.AllKeys)
            {
                if (encryped && (key != "encrypted"))
                {
                    HttpValues.Add(key, cryptographer.TDesDecrypt(context.Request.QueryString.Get(key)));
                }
                else
                {
                    HttpValues.Add(key, context.Request.QueryString.Get(key));
                }
            }

            foreach (string key in context.Request.Form.AllKeys)
            {
                if (encryped && (key != "encrypted"))
                {
                    HttpValues.Add(key, cryptographer.TDesDecrypt(context.Request.Form.Get(key)));
                }
                else
                {
                    HttpValues.Add(key, context.Request.Form.Get(key));
                }
            }
            return HttpValues;
        }
    }
}
