using System;
using System.Resources;
using log4net;

namespace Hangout.Server
{
    /// <summary>
    /// Summary description for Resources
    /// </summary>
    public static class PaymentItemResourceHandler
    {
        private static readonly ILog mErrorLogger;

        static PaymentItemResourceHandler()
        {
            mErrorLogger = LogManager.GetLogger("GeneralUse");
        }

        public static string GetStringFromResourceFile(string key)
        {
            string returnString = "";

            try
            {
                ResourceManager rm = new ResourceManager(typeof(Hangout.Server.PaymentItemsResource));

                string resource = rm.GetString(key);
                if (resource != null)
                {
                    returnString = resource;
                }
            }

            catch (Exception ex)
            {
                mErrorLogger.Error(String.Format("Error GetStringFromResourceFile Key: {1}", key), ex);
            }

            return returnString;
        }


        public static string GetErrorMessageFromResourceFile(string errorMessage, string errorCode)
        {
            string errorString = errorMessage;
            try
            {
                errorString = GetStringFromResourceFile(String.Format("E{0}", errorCode));
                if (errorString == null)
                {
                    errorString = errorMessage;
                }
            }
            catch (Exception ex)
            {
                mErrorLogger.Error(String.Format("Error GetErrorMessageFromResourceFile Error Code: {1}", errorCode), ex);
            }

            return errorString;
        }
    }
}
