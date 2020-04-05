using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;
using System.Reflection;
using Hangout.Server;
using System.Web;
using System.Runtime.Remoting.Lifetime;
using Hangout.Shared;

using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;


namespace Hangout.Server.WebServices
{
    public class AppRestHandler : MarshalByRefObject
    {
        HangoutCookiesCollection _cookieCollection = (HangoutCookiesCollection)AppDomain.CurrentDomain.GetData("AppCookies");

        HangoutPostedFile _oFile = (HangoutPostedFile)AppDomain.CurrentDomain.GetData("PostedFile");

        AppErrorInformation _ErrorInfo = (AppErrorInformation)AppDomain.CurrentDomain.GetData("AppErrorInfo");


        /*  Specifing the lease time is not used for now.
         *  If needed then remove the comments from the below code.
         *  and set the lease times.
        /*  public override object  InitializeLifetimeService()
          {
              ILease lease = (ILease)base.InitializeLifetimeService();
              if (lease.CurrentState == LeaseState.Initial)
              {
                  lease.InitialLeaseTime = TimeSpan.FromMinutes(5);
                  lease.SponsorshipTimeout = TimeSpan.FromSeconds(10);
                  lease.RenewOnCallTime = TimeSpan.FromMinutes(2);
              }
              return lease;
          } */

        public MemoryStream ProcessRequest(Type T, string verb, int roomCatalogId, MemoryStream GetValueCollectionStream) 
        {
            MemoryStream returnStream = new MemoryStream();

            try
            {
                Dictionary<string, HangoutCookie> cookieCollection = _cookieCollection.CookieCollection;
                
                BinaryFormatter formatter = new BinaryFormatter();
                NameValueCollection vars = formatter.Deserialize(GetValueCollectionStream) as NameValueCollection;
                
                // find the method using the verb name
				// TODO: add argIndex test to make sure it is argIndex static method
                MethodInfo methodInfo = T.GetMethod(verb);
				if ( methodInfo == null )
				{
					throw new ArgumentException( "Service verb could not be found: " + verb );
				}
				// loop though each parameter in the method and look for argIndex corresponding
				// query parameter in the REST request URL.
				int numArgs = methodInfo.GetParameters().Length;
				object[] args = new object[numArgs];
				for ( int argIndex = 0; argIndex < numArgs; argIndex++ )
				{
					ParameterInfo parameterInfo = methodInfo.GetParameters()[argIndex];
					if ( parameterInfo.ParameterType == typeof( int ) )
					{
                        string arg = GetHttpValue(vars, parameterInfo.Name);
						int p = Int32.Parse( arg );
						args[argIndex] = p;
					}
					else if ( parameterInfo.ParameterType == typeof( string ) )
					{
                        args[argIndex] = GetHttpValue(vars, parameterInfo.Name);
					}
                    else if (parameterInfo.ParameterType == typeof(HangoutPostedFile))
					{
                        args[argIndex] = _oFile;
					}
					else if ( parameterInfo.ParameterType == typeof( Session.UserId ) )
					{
                        Session.UserId userId;
                        string stringUserId = GetHttpValue(vars, "userId");
                        if (stringUserId == null)
                        {
                            userId = Session.GetCurrentUserId(cookieCollection);
                            if (userId == null)
                            {
                                throw new UserNotLoggedin("No user session active and userId not provided.");
                            }
                        }
                        else
                        {
                            int intUserId = System.Convert.ToInt32(stringUserId);
                            userId = new Session.UserId(intUserId);
                        }
                        args[argIndex] = userId;
					} 
                    //else if (parameterInfo.ParameterType == typeof(RoomCatalog)) {
                    //    args[argIndex] = new RoomCatalog(roomCatalogId);
                    //}
                    else if (parameterInfo.ParameterType == typeof(HangoutCookiesCollection))
                    {
                        args[argIndex] = _cookieCollection;
                    } else {
                        throw new ArgumentException("unsupported argument type");
                    }
				}

                //Log4NetHandler logger = new Log4NetHandler();
                //logger.SendToLogger("RESTService", "INFO", String.Format("Noun: {0} Verb: {1}", methodInfo.ReflectedType.Name, verb));
    
                object reflectionObject = methodInfo.Invoke(null, args);

                new XmlSerializer(methodInfo.ReturnType).Serialize(returnStream, reflectionObject);

               // System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
               // string sTemp = enc.GetString(returnStream.GetBuffer());

                return returnStream;
            }

            catch (System.Exception ex)
            {
                if (ex.GetType() == typeof(TargetInvocationException))
                    ex = ex.InnerException;

                bool debugMode = (ConfigurationManager.AppSettings["DebugMode"] == "true") ? true : false;

                HangoutException hangoutException = ex as HangoutException;
                if (hangoutException == null)
                {
                    hangoutException = new HangoutException(ex);
                }

                RESTerror e = hangoutException.GetRestError(debugMode);

                hangoutException.RequestType = _ErrorInfo.RequestType;
                hangoutException.RequestUrl = _ErrorInfo.URL;
                hangoutException.ClientIp = _ErrorInfo.UserHostAddress;
                hangoutException.TimeStamp = _ErrorInfo.Timestamp;
                hangoutException.MachineName = _ErrorInfo.MachineName;

                Dictionary<string, HangoutCookie> cookieCollection = _cookieCollection.CookieCollection;
                Session.UserId userId = Session.GetCurrentUserId(cookieCollection);
                if (userId != null)
                    hangoutException.CurrentUserId = userId.Id;

                //Log4NetHandler logger = new Log4NetHandler();
                //logger.SendToLogger("RESTService", "ERROR", String.Format("RequestUrl: {0} User: {1}", _ErrorInfo.URL, hangoutException.CurrentUserId), ex);

                // Handle exception through exception handling application block.
                ExceptionPolicy.HandleException(hangoutException, "Global Policy");

                //context.Response.StatusCode = 404;
                new XmlSerializer(typeof(RESTerror)).Serialize(returnStream, e);

                return returnStream;
            }
        }
         
        private string GetHttpValue(NameValueCollection vars,  String parameterName )
        {
            string[] values = vars.GetValues(parameterName);

            if (values == null)
            {
                return null;
            }
            else
            {
                return values[0];
            }
      }
   }
}

