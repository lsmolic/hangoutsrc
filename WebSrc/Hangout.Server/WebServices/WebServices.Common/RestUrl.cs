using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Hangout.Server.WebServices {
    
    public class RestUrl {
        //APPEND THIS TO THE NOUN FOR THE SERVICES NAMESPACE
        public static string mServicesRoot = "Hangout.Server.WebServices.";

        public static string noun {
            get {
                try {
                    if (UrlHasVersion()) {
                        return unversionedNoun + "." + version;
                    } else {
                        return unversionedNoun;
                    }
                } catch (System.Exception) {
                    throw new ArgumentException("no noun specifed in REST request URL");
                }
            }
        }
        public static string unversionedNoun {
            get {
                try {
                    if (UrlHasVersion()) {
                        return mServicesRoot + TrimTrailingSlash(Segments[2]);
                    } else {
                        return mServicesRoot + TrimTrailingSlash(Segments[1]);
                    }
                } catch (System.Exception) {
                    throw new ArgumentException("no noun specifed in REST request URL");
                }
            }
        }
        public static string verb {
            get {
                try {
                    if (UrlHasVersion()) {
                        return TrimTrailingSlash(Segments[3]);
                    } else {
                        return TrimTrailingSlash(Segments[2]);
                    }
                } catch (System.Exception) {
                    throw new ArgumentException("no verb specifed in REST request URL");
                }
            }
        }
        public static int version {
            get {
                try {
                    if (UrlHasVersion()) {
                        return System.Convert.ToInt32(TrimTrailingSlash(Segments[1]));
                    } else {
                        return 1;
                    }
                } catch (System.Exception) {
                    throw new ArgumentException("unable to parse REST request URL");
                }
            }
        }
        private static string[] Segments {
            get {
                return HttpContext.Current.Request.Url.Segments;
            }
        }
        private static string TrimTrailingSlash(string segment) {
            return segment.TrimEnd('/');
        }
        private static bool UrlHasVersion() {
            return !(Segments.Length == 3);
        }
    }
}
