using System;
using System.Xml;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Hangout.Server;

using System.Drawing;
using System.Drawing.Imaging;

namespace Hangout.Server.WebServices {
    public class ImageHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            //
            // Initialize the response to badArgumentException non-cachable XML
            //
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetExpires(DateTime.MinValue);

            //create blank images with the color white
            int width = 10;
            int height = 10;
            Bitmap bitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(bitmap);
            if (context.Request.Url.Segments.Length == 2)
            {
                //this is a login webbug
                //this is for backwards compatability url like /Webbug?userName=sdf
                try
                {
                    LoginUser();
                    graphics.Clear(Color.White);
                }
                catch
                {
                    graphics.Clear(Color.Blue);
                }
            }
            else
            {
                try
                {
                    switch (RestUrl.verb.ToLower())
                    {
                        case "logout":
                            Session.Clear();
                            break;
                        case "login":
                            LoginUser();
                            break;
                    }
                    graphics.Clear(Color.White);
                }
                catch (System.Exception)
                {
                    graphics.Clear(Color.Blue);
                }
            }
            context.Response.ContentType = "image/gif";
            bitmap.Save(context.Response.OutputStream, ImageFormat.Gif);
            return;
        }
        private void LoginUser()
        {
            //SimpleResponse loginRes = UserManagement.LoginUser(Util.GetHttpValue("userName"), Util.GetHttpValue("password"), Util.GetHttpValue("requiresValidation"));
            SimpleResponse loginRes = new SimpleResponse("success", "true");
            XmlNodeList nodes = loginRes.GetElementsByTagName("userId");
            int userId = -1;
            if (nodes.Count == 0)
            {
                throw new System.Exception("no userId returned");
            }
            else
            {
                userId = Convert.ToInt32(nodes[0].InnerXml);
            }

            Session.SetCurrentUserId(userId);
        }
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
