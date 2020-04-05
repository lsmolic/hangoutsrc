using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;
using Hangout.Shared;

namespace Hangout.Server
{
	public static class Session
	{
		private static readonly string cookieName = "hangout";
		private static readonly string[] stringSeparators = new string[] {"&"}; 
        private static readonly string cookieToken = "||";
        private static readonly string contextCookieName = "hangoutCurrentContext";
        private static readonly string _domain = ConfigurationManager.AppSettings["domain"];
        private static readonly string _ServicesSiteRoot = ConfigurationManager.AppSettings["ServicesSiteRoot"];

        public static string CookieDomain
        {
            get { return _domain; }
        }

        public static UserId GetCurrentUserId()
        {
            try
            {
                HttpContext context = HttpContext.Current;
                HttpCookie userCookie = context.Request.Cookies[cookieName];

                if (userCookie != null)
                {
                    string userCookieValue = userCookie.Value;
                    return (GetCurrentUser(userCookieValue));
                }
                return null;
            }

            catch (System.Exception)
            {
                return null;
            }

        }

        public static UserId GetCurrentUserId(Dictionary<string, HangoutCookie> cookieCollection)
        {
            try
            {
                HangoutCookie userCookie = new HangoutCookie();

                if (cookieCollection.TryGetValue(cookieName, out userCookie))
                {
                    string userCookieValue = userCookie.Value;
                    return (GetCurrentUser(userCookieValue));
                }
                return null;
            }

            catch (System.Exception)
            {
                return null;
            }

        }

        public static UserId GetCurrentUser(string userCookieValue)
		{
			try
			{
                if (userCookieValue != null)
                {
                    if (userCookieValue != "")
                    {
                        SimpleCrypto cryptographer = new SimpleCrypto();
                        userCookieValue = cryptographer.TDesDecrypt(userCookieValue);
                        int id = System.Convert.ToInt32(userCookieValue.Substring(0, userCookieValue.IndexOf(cookieToken)));

                        DateTime expiration = DateTime.Parse(userCookieValue.Substring(userCookieValue.IndexOf(cookieToken) + 2));

                        if (expiration < DateTime.Now || id == 0)
                        {
                            return null;
                        }
                        else
                        {
                            return new UserId(id);
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
			}
			catch ( System.Exception )
			{
				return null;
			}
		}
		public static void SetCurrentUserId( int val )
		{
			string cookieValue = val + cookieToken + DateTime.Now.AddHours( 1 );
            SimpleCrypto cryptographer = new SimpleCrypto();
            cookieValue = cryptographer.TDesEncrypt(cookieValue);
            HttpCookie cookie = new HttpCookie(cookieName, cookieValue);

            cookie.Domain = _domain;
            cookie.Value = cookieValue;
            HttpCookie existsCookie = HttpContext.Current.Request.Cookies.Get(cookieName);
            if (existsCookie == null)
            {
                HttpContext.Current.Response.Cookies.Add(cookie);
                HttpContext.Current.Request.Cookies.Add(cookie);
            }
            else
            {
                HttpContext.Current.Response.Cookies.Set(cookie);
                HttpContext.Current.Request.Cookies.Set(cookie);
            }
		}

		public static void Refresh()
		{
            if ( GetCurrentUserId() != null )
            {
                SetCurrentUserId( GetCurrentUserId().Id );
            }
		}

		public static void Clear()
		{
			HttpContext context = HttpContext.Current;

            context.Response.Cookies[cookieName].Domain = _domain;
            context.Response.Cookies[cookieName].Value = "";
            context.Response.Cookies[cookieName].Expires = DateTime.Now.Subtract(TimeSpan.FromHours( 5000 ));
            context.Request.Cookies[cookieName].Domain = _domain;
            context.Request.Cookies[cookieName].Value = "";
            context.Request.Cookies[cookieName].Expires = DateTime.Now.Subtract(TimeSpan.FromHours(5000));
		}
		public class UserId
		{
			private int _userId;
			public UserId( int id )
			{
				_userId = id;
			}
			public int Id
			{
				get
				{
					return _userId;
				}
			}
            public override string ToString()
            {
                return _userId.ToString();
            }
		}

        public static string GetCurrentRoomContext(string roomId)
        {
            UserId userID = GetCurrentUserId();
            return (GetCurrentRoomContext(roomId, userID, null));
        }

        public static string GetCurrentRoomContext(string roomId, UserId userId, Dictionary<string, HangoutCookie> cookieCollection)
        {
             Dictionary<string, string> userContexts = null; 
            if (cookieCollection == null)
            {
                userContexts = GetRoomContexts();
            }
            else
            {
                userContexts = GetRoomContexts(cookieCollection);
            }

            if (IsRoomOwner(roomId, userId))
            {
                return "RoomOwner";
            }
            else if (IsRoomOwnerFriend(roomId, userId))
            {
                return "Friend";
            } 
            else if (userContexts != null && userContexts.ContainsKey(roomId))
            {
                return userContexts[roomId];
            }
            else
            {
                return "NoAccess";
            }   
        }

        public static Dictionary<string, string> GetRoomContexts()
        {
            try
            {
                HttpContext context = HttpContext.Current;
                HttpCookie userCookie = context.Request.Cookies[contextCookieName];
                if (userCookie != null)
                {
                    string userCookieValue = userCookie.Value;
                    return (GetRoomContexts(userCookieValue));
                }
                return new Dictionary<string, string>();
            }

            catch (System.Exception)
            {
                return null;
            }
        }

        public static Dictionary<string, string> GetRoomContexts(Dictionary<string, HangoutCookie> cookieCollection)
        {
            try
            {
               HangoutCookie userCookie = new HangoutCookie();

               if (cookieCollection.TryGetValue(contextCookieName, out userCookie))
                {
                    return (GetRoomContexts(userCookie.Value));
                } 
 
                return new Dictionary<string, string>();
            }

            catch (System.Exception)
            {
                return null;
            }

        }

        public static Dictionary<string, string> GetRoomContexts(string userCookieValue)
        {
            try
            {
                if (userCookieValue != "")
                {
                    SimpleCrypto cryptographer = new SimpleCrypto();
                    userCookieValue = cryptographer.TDesDecrypt(userCookieValue);

                    Dictionary<string, string> userContexts = new Dictionary<string, string>() ;

                    if (userCookieValue != null && userCookieValue != "")
                    {
                        string[] userContext = userCookieValue.Split('&');
                        foreach (string con in userContext)
                        {
                            string roomId = con.Split('=')[0];
                            string roomContext = con.Split('=')[1];
                            userContexts.Add(roomId, roomContext);

                        }
                        return userContexts;
                    }
                    
                }
                return new Dictionary<string, string>();
            }
            catch (System.Exception)
            {
                return null;
            }
        }
       
        public static void SetCurrentContext( string roomId, string newUserContext )
        {
            Dictionary<string, string> userContexts = GetRoomContexts();
            string currentRoomContext = GetCurrentRoomContext(roomId, Session.GetCurrentUserId(), null);
            if (userContexts != null)
            {
                if (currentRoomContext != null)
                {
                    if (roomId != null && roomId != "")
                    {
                        if (userContexts.ContainsKey(roomId))
                        {
                            userContexts.Remove(roomId);
                        }
                    }
                }
            }
            else
            {
                userContexts = new Dictionary<string, string>();
            }
            userContexts.Add(roomId, newUserContext);
            string cookieValue = "";
            string delimiter = "";
            foreach (KeyValuePair<string, string> userCon in userContexts)
            {
                cookieValue += delimiter + userCon.Key + "=" + userCon.Value;
                delimiter = "&";
            }
            SimpleCrypto cryptographer = new SimpleCrypto();
            cookieValue = cryptographer.TDesEncrypt(cookieValue);
            HttpCookie cookie = new HttpCookie(contextCookieName, cookieValue);

            cookie.Domain = _domain;
            cookie.Value = cookieValue;
            HttpCookie existsCookie = HttpContext.Current.Request.Cookies.Get(contextCookieName);
            if (existsCookie == null)
            {
                HttpContext.Current.Response.Cookies.Add(cookie);
                HttpContext.Current.Request.Cookies.Add(cookie);
            }
            else
            {
                HttpContext.Current.Response.Cookies.Set(cookie);
                HttpContext.Current.Request.Cookies.Set(cookie);
            }
        }
        public static bool IsRoomOwner(string roomId, UserId userId)
        {

            WebServiceRequest getRoomOwner = new WebServiceRequest(_ServicesSiteRoot,"Rooms","IsRoomOwner");
            if (userId != null)
            {
                getRoomOwner.AddParam("userId", userId.Id.ToString());
            }
            getRoomOwner.AddParam("roomId", roomId);
            XmlDocument serviceResponse = getRoomOwner.GetWebResponse();

			XmlNodeList isRoomOwnerNode = serviceResponse.GetElementsByTagName("isRoomOwner");
            if (isRoomOwnerNode.Count > 0)
            {
                if (isRoomOwnerNode[0] != null)
                {
                    if (Convert.ToBoolean(isRoomOwnerNode[0].InnerText))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;

        }
        public static bool IsRoomOwnerFriend(string roomId, UserId userId)
        {
            WebServiceRequest getRoomOwnerRelationship = new WebServiceRequest(_ServicesSiteRoot, "Friends", "IsRoomOwnerFriend");
            if (userId != null)
            {
                getRoomOwnerRelationship.AddParam("userId", userId.Id.ToString());
            }
            getRoomOwnerRelationship.AddParam("roomId", roomId);
            XmlDocument serviceResponse = getRoomOwnerRelationship.GetWebResponse();


			XmlNodeList isRoomOwnerFriendNode = serviceResponse.GetElementsByTagName("isRoomOwnerFriend");
            if (isRoomOwnerFriendNode.Count > 0)
            {
                if (isRoomOwnerFriendNode[0] != null)
                {
                    if (Convert.ToBoolean(isRoomOwnerFriendNode[0].InnerText))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
	}
}
