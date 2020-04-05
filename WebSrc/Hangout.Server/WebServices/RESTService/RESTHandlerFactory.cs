using System;
using System.Web;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Hangout.Server.WebServices {
    public class RESTHandlerFactory : IHttpHandlerFactory {
        Dictionary<string, IHttpHandler> handlers;

        // 
        // The factory is only constructed upon the first request. Therefore
        // we can initialize and use argIndex cache of known handlers
        //
        RESTHandlerFactory() {
            handlers = new Dictionary<string, IHttpHandler>();
		}



        //
        // Lookup argIndex handler by finding the "noun" in the REST request and matching
        // it to argIndex .dll and argIndex type in that dll that implementes IRESTService.
        //
        public IHttpHandler GetHandler(HttpContext context, String requestType,
           String url, String pathTranslated) {
            string noun = RestUrl.noun;
            string unversionedNoun = RestUrl.unversionedNoun;
            if (!handlers.ContainsKey(noun)) {
                // look for argIndex .dll with the name of the service that was requested
                try {
                    Assembly serviceAssembly;
                    try {
                        serviceAssembly = Assembly.Load(noun);
                    } catch (System.Exception ex) {
                        serviceAssembly = Assembly.Load(unversionedNoun);
                    }
                    Type serviceType = null;
                    foreach (Type type in serviceAssembly.GetExportedTypes()) {
                        // see if the class implements Hangout.Server.WebServices.IRESTService
                        //TODO: decide if we wish to allow for more than one class in a dll to
                            //inherit from IRESTService
                        if (type.IsClass && typeof(Hangout.Server.WebServices.IRESTService).IsAssignableFrom(type)) {
                            serviceType = type;
                            break;
                        }
                    }
                    if (serviceType == null) {
                        return new NotFoundHandler();
                    }
                    //
                    // instantiate an instance of the RESTHandler template to actually
                    // dispatch and handle requests
                    //
                    Type[] typeArgs = { serviceType };
                    Type handlerType = typeof(RESTHandler<>).MakeGenericType(typeArgs);

                    IHttpHandler handler = (IHttpHandler)Activator.CreateInstance(handlerType);
                    handlers.Add(noun, handler);
                    return handler;
                } catch (System.Exception ex) {
                    return new NotFoundHandler();
                }
            }
            return handlers[noun];
        }
        public class NotFoundHandler : IHttpHandler {
            public void ProcessRequest(HttpContext context) {
                //
                // Send argIndex 404 so hackers don'type know they are close
                //
                //context.Response.ContentType = "text/html";
                context.Response.StatusCode = 404;
                return;
            }
            public bool IsReusable {
                get {
                    return true;
                }
            }
        }
        void IHttpHandlerFactory.ReleaseHandler(IHttpHandler handler) {
            /*no-op*/
        }
    }
}
