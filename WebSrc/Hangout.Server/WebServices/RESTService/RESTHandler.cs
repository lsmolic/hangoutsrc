using System;
using System.Net;
using System.Web;
using System.Configuration;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Policy;
using Hangout.Server;
using Hangout.Shared;

using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace Hangout.Server.WebServices
{
	//
	// Use reflection to dispatch the "verb" in the REST request.
	// We can only handle service object methods with either int or string arguments.
	// The results can badArgumentException any XML serializable type.
	//
    public class RESTHandler<T> : IHttpHandler where T : Hangout.Server.WebServices.IRESTService
	{
		private ServicesLog mServiceLog = new ServicesLog("RESTHandler");
		private bool mDebugMode = (ConfigurationManager.AppSettings["DebugMode"] == "true") ? true : false;
        AppRoomCatalog appRoomCatalog = new AppRoomCatalog();

        public void ProcessRequest(HttpContext context)
		{
			Globals.AddCurrentRestRequest();
			mServiceLog.Log.Debug("++++ Current RestService Requests: " + Globals.GetCurrentRestRequests);
			mServiceLog.Log.Debug("ProcessRequest: " + context.Request.RawUrl.ToString() + context.Request.QueryString.ToString());
			if (mDebugMode)
			{
				mServiceLog.Log.Debug("Total RestService Requests: " + Globals.GetTotalRestRequests);
			}
			
			try
			{
                //
				// Initialize the response to badArgumentException non-cachable XML
				//
                context.Response.AddHeader("p3p", @"CP=\""IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\""");
				context.Response.ContentType = "text/xml";
				context.Response.Cache.SetCacheability( HttpCacheability.NoCache );
				context.Response.Cache.SetNoStore();
				context.Response.Cache.SetExpires( DateTime.MinValue );

                string verb = RestUrl.verb;
				if (mDebugMode)
				{
					mServiceLog.Log.Debug("ProcessRequest (" + verb + ")");
				}
                int roomCatalog = RestUrl.version;

                int defaultRoomAppDomain = 1;

                if(Int32.TryParse(ConfigurationManager.AppSettings["DefaultRoomAppDomain"], out defaultRoomAppDomain))
                {
                    roomCatalog = defaultRoomAppDomain;
                }

                if (isExecuteInAppDomain(roomCatalog))
                {
                    MemoryStream GetValueCollectionStream = new MemoryStream();

                    NameValueCollection GetValueCollection = Util.GetHttpValueArray(context);
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(GetValueCollectionStream, GetValueCollection);
                    GetValueCollectionStream.Position = 0;

                    ExecuteInAppDomain(typeof(T), roomCatalog, context, verb, GetValueCollectionStream, context.Request.Cookies);
                }
                else
                {
                    DoProcessRequest(context, verb);
                }
            }

            catch
            {


            }
        }

        private bool isExecuteInAppDomain(int RoomCatalog)
        {
            bool retValue = false;

            try
            {
                if (RoomCatalog != 1)
                {
                    string catalogDir = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("RESTService.DLL", "").Replace("file:///", "") +RoomCatalog.ToString();
                    if (Directory.Exists(catalogDir))
                    {
                        retValue = true;
                    }
                }
            }

            catch { }

            return retValue;
       }

        public  void DoProcessRequest(HttpContext context, string verb)
        {
            try
            {
				// find the method using the verb name
				// TODO: add argIndex test to make sure it is argIndex static method
				MethodInfo methodInfo = typeof( T ).GetMethod( verb );
				if ( methodInfo == null )
				{
					throw new ArgumentException( "Service verb could not be found: " + verb );
				}
				// loop though each parameter in the method and look for argIndex corresponding
				// query parameter in the REST request URL.
				string argString = "";
				string delimiter = "";
				int numArgs = methodInfo.GetParameters().Length;
				object[] args = new object[numArgs];
				for ( int argIndex = 0; argIndex < numArgs; argIndex++ )
				{
					ParameterInfo parameterInfo = methodInfo.GetParameters()[argIndex];
					
					if ( parameterInfo.ParameterType == typeof( int ) )
					{
						string arg = Util.GetHttpValue( parameterInfo.Name );
						int p = Int32.Parse( arg );
						args[argIndex] = p;
						argString += delimiter + parameterInfo.Name + "=" + Util.GetHttpValue(parameterInfo.Name);
						delimiter = "; ";
					}
					else if (parameterInfo.ParameterType == typeof(long))
					{
						string arg = Util.GetHttpValue(parameterInfo.Name);
						long p = Int64.Parse(arg);
						args[argIndex] = p;
						argString += delimiter + parameterInfo.Name + "=" + Util.GetHttpValue(parameterInfo.Name);
						delimiter = "; ";
					}
					else if (parameterInfo.ParameterType == typeof(uint))
					{
						string arg = Util.GetHttpValue(parameterInfo.Name);
						uint p = UInt32.Parse(arg);
						args[argIndex] = p;
						argString += delimiter + parameterInfo.Name + "=" + Util.GetHttpValue(parameterInfo.Name);
						delimiter = "; ";
					}
					else if ( parameterInfo.ParameterType == typeof( string ) )
					{
						args[argIndex] = Util.GetHttpValue( parameterInfo.Name );
						argString += delimiter + parameterInfo.Name + "=" + Util.GetHttpValue(parameterInfo.Name);
						delimiter = "; ";
					}
                    else if (parameterInfo.ParameterType == typeof(HangoutPostedFile))
					{
                        HangoutPostedFile oFile = new HangoutPostedFile();
                        oFile.InitPostedFile(context.Request.Files[parameterInfo.Name]);
                        args[argIndex] = oFile;
					}
					else if ( parameterInfo.ParameterType == typeof( Session.UserId ) )
					{
                        Session.UserId userId;
                        string stringUserId = Util.GetHttpValue("userId");
                        if (stringUserId == null)
                        {
                            userId = Session.GetCurrentUserId();
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
                    //    args[argIndex] = RestUrl.version;

                    //} 
                    else if (parameterInfo.ParameterType == typeof(HangoutCookiesCollection)) {
                        HangoutCookiesCollection oCookies = new HangoutCookiesCollection();
                        oCookies.InitCookies(context.Request.Cookies);
                        args[argIndex] = oCookies;
                    } else {
                        throw new ArgumentException("unsupported argument type");
                    }
				}
				if (mDebugMode)
				{
					mServiceLog.Log.Debug("DoProcessRequest (Invoke Method) (" + methodInfo.ReflectedType.Name + ")(" + verb + ") args: " + argString);
				}
                //Log4NetHandler logger = new Log4NetHandler();
                //logger.SendToLogger("RESTService", "INFO", String.Format("Noun: {0} Verb: {1}", methodInfo.ReflectedType.Name, verb));
                T instance = Activator.CreateInstance<T>();
                object reflectionObject = methodInfo.Invoke(instance, args);

                // refresh the session if it has one
                Session.Refresh();

                // serialize the response based on the methods return type
                new XmlSerializer(methodInfo.ReturnType).Serialize(context.Response.OutputStream, reflectionObject);
				Globals.RemoveCurrentRestRequest();
				if (mDebugMode)
				{
					mServiceLog.Log.Debug("---- Current RestService Requests: " + Globals.GetCurrentRestRequests);
				}
				return;
			}
			catch ( System.Exception ex )
			{
				if ( ex.GetType() == typeof( TargetInvocationException ) )
					ex = ex.InnerException;

				bool debugMode = ( ConfigurationManager.AppSettings["DebugMode"] == "true" ) ? true : false;

                HangoutException hangoutException = ex as HangoutException;
                if (hangoutException == null)
                {
                    hangoutException = new HangoutException(ex);
                }

                RESTerror e = hangoutException.GetRestError( debugMode );

                // Add additional debug information
                hangoutException.RequestType = context.Request.RequestType;
                hangoutException.RequestUrl  = context.Request.Url.ToString();
                hangoutException.ClientIp    = context.Request.UserHostAddress;
                hangoutException.TimeStamp   = context.Timestamp;
                hangoutException.MachineName = Dns.GetHostName().ToString();
                if (Session.GetCurrentUserId() != null)
                    hangoutException.CurrentUserId = Session.GetCurrentUserId().Id;
				if (mDebugMode)
				{


					mServiceLog.Log.Debug("DoProcessRequest (HangoutException) (" + verb + "): " +
						"RequestType: " + hangoutException.RequestType +
						"RequestUrl: " + hangoutException.RequestUrl +
						"ClientIp: " + hangoutException.ClientIp +
						"TimeStamp: " + hangoutException.TimeStamp +
						"MachineName: " + hangoutException.MachineName
						);
				}
                //Log4NetHandler logger = new Log4NetHandler();
                //logger.SendToLogger("RESTService", "ERROR", String.Format("RequestUrl: {0} User: {1}", context.Request.Url.ToString(), hangoutException.CurrentUserId), ex);

                // Handle exception through exception handling application block.
                // TODO: lucas commented this out... eventually we'll figure out if this needs to be in here 
                //ExceptionPolicy.HandleException( hangoutException, "Global Policy" );

                context.Response.StatusCode = 500;
				if (mDebugMode)
				{
					mServiceLog.Log.Debug("DoProcessRequest (ErrorMessage) (" + verb + "): " + e.ErrorMessage);
				}
                new XmlSerializer( typeof( RESTerror ) ).Serialize( context.Response.OutputStream, e );
				Globals.RemoveCurrentRestRequest();
				if (mDebugMode)
				{
					mServiceLog.Log.Debug("---- Current RestService Requests: " + Globals.GetCurrentRestRequests);
				}
				return;
			}
		}


        private void ExecuteInAppDomain(Type T, int roomCatalogId, HttpContext context, string verb, MemoryStream GetValueCollectionStream, HttpCookieCollection cookieCollection)
        {
           try
           {
               //Log4NetHandler logger = new Log4NetHandler();
               //logger.SendToLogger("RESTService", "INFO", String.Format("ExecuteInAppDomain roomCatalogId: {0} Verb: {1}", roomCatalogId, verb));

                AppDomain appDomain  = appRoomCatalog.GetCatalogAppDomain(roomCatalogId.ToString());
                if (appDomain == null)
                    appDomain = CreateAppDomain(roomCatalogId);

                if (appDomain == null)
                {
                    throw new SystemException (String.Format("CreateAppDomain roomCatalogId: {0} Error: appDomain is null", roomCatalogId));
                }

                //hangoutCookies
                HangoutCookiesCollection oCookies = new HangoutCookiesCollection();
                oCookies.InitCookies(context.Request.Cookies);
                appDomain.SetData("AppCookies", oCookies);

                HangoutPostedFile oFile = new HangoutPostedFile();
                if (context.Request.Files.Count > 0)
                {
                    oFile.InitPostedFile(context.Request.Files[0]);
                }
                appDomain.SetData("PostedFile", oFile);

                AppErrorInformation oErrorInfo = new AppErrorInformation();
                oErrorInfo.RequestType = context.Request.RequestType;
                oErrorInfo.URL = context.Request.Url.ToString();
                oErrorInfo.UserHostAddress = context.Request.UserHostAddress;
                oErrorInfo.Timestamp = context.Timestamp;
                oErrorInfo.MachineName = Dns.GetHostName().ToString();
                appDomain.SetData("AppErrorInfo", oErrorInfo);

				mServiceLog.Log.Debug("ExecuteInAppDomain (create instance of ) (" + verb + ")");
                AppRestHandler mbrt = (AppRestHandler)appDomain.CreateInstanceAndUnwrap("AppService", "Hangout.Server.WebServices.AppService.AppRestHandler");

				mServiceLog.Log.Debug("ExecuteInAppDomain (process request through specific dll) (" + verb + ")");
                //Specified room catalog of 1 for the data  if we branch db this needs to change to be the roomCatalogId.
                MemoryStream outputStream = mbrt.ProcessRequest(T, verb, 1, GetValueCollectionStream);

                Session.Refresh();
				mServiceLog.Log.Debug("ExecuteInAppDomain (sending response) (" + verb + ")");
                // serialize the response based on the methods return type
                context.Response.OutputStream.Write(outputStream.GetBuffer(), 0, (int)outputStream.Length);

                // System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                // string sTemp = enc.GetString(outputStream.GetBuffer());

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

                // Add additional debug information
                hangoutException.RequestType = context.Request.RequestType;
                hangoutException.RequestUrl = context.Request.Url.ToString();
                hangoutException.ClientIp = context.Request.UserHostAddress;
                hangoutException.TimeStamp = context.Timestamp;
                hangoutException.MachineName = Dns.GetHostName().ToString();
                if (Session.GetCurrentUserId() != null)
                    hangoutException.CurrentUserId = Session.GetCurrentUserId().Id;

                //Log4NetHandler logger = new Log4NetHandler();
                //logger.SendToLogger("RESTService", "ERROR", String.Format("RequestUrl: {0} User: {1}", context.Request.Url.ToString(), hangoutException.CurrentUserId), ex);
                
                // Handle exception through exception handling application block.
                //ExceptionPolicy.HandleException(hangoutException, "Global Policy");

                //context.Response.StatusCode = 404;
                new XmlSerializer(typeof(RESTerror)).Serialize(context.Response.OutputStream, e);
                return;
            }

             //   finally
              //  {
                //    AppDomain.Unload(appDomain);
               // }
        }

        private AppDomain CreateAppDomain(int roomCatalogId)
        {
            AppDomain appDomain = null;

            try
            {
                AppDomainSetup appDomainSetup = new AppDomainSetup();      //Instantiate AppDomainSetup object

                //Configure AppDomainSetup Setup Information
                //Directory where CLR will look during probing to resolve private assemblies.
                //By default this is the directory containing the assembly.
                appDomainSetup.ApplicationBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("RESTService.DLL", "").Replace("file:///", "") + roomCatalogId.ToString();

                //Create evidence for new appdomain.
                Evidence adevidence = AppDomain.CurrentDomain.Evidence;

                //Configuration name file used by code loaded into assembly.
                //By default it is stored in the same folder as the assembly.   
                appDomainSetup.ConfigurationFile = "Web.Config";

                //Semicolon-seprated list of directories that the runtime uses when probing for
                //private assemblies. these directories are relative to the directory specified in ApplicationBase.
                appDomainSetup.PrivateBinPath = "bin";

                string assemblyName = "AppService" + roomCatalogId;

                //Remember to save a Reference to the new AppDomain as this Cannot be retrived any other way
                appDomain = AppDomain.CreateDomain(assemblyName, adevidence, appDomainSetup);

                //appDomain.InitializeLifetimeService();

                appRoomCatalog.AddCatalogAppDomain(roomCatalogId.ToString(), appDomain);

            }
            catch (System.Exception ex)
            {
                throw new SystemException (String.Format("CreateAppDomain roomCatalogId: {0} Error: {1}", roomCatalogId, ex.Message));
            }

            return appDomain;
        }




		// since the dispatcher is stateless and thread-safe, it could badArgumentException used in parallel
		// if the opportunity ever arises

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}
	}
}