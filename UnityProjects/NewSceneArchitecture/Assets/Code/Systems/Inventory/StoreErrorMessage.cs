using System.Collections.Generic;

namespace Hangout.Client
{
    public class StoreErrorMessage
    {
        private static Dictionary<string, string> mErrorCodeToMessage = new Dictionary<string, string>();

        private static StoreErrorMessage mInstance;
        public static StoreErrorMessage Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new StoreErrorMessage();
                }
                return mInstance;
            }
        }

        private StoreErrorMessage()
        {
            mErrorCodeToMessage.Add("270007", Translation.PURCHASE_ERROR_INSUFFIENT_FUNDS);
            mErrorCodeToMessage.Add("990004", Translation.PURCHASE_ERROR_ACCOUNT_NOT_FOUND);
            mErrorCodeToMessage.Add("980015", Translation.PURCHASE_ERROR_STORE_NOT_FOUND);
        }

        public string GetError(string errorCode)
        {
            string errorMessage = Translation.PURCHASE_ERROR_GENERIC;
            mErrorCodeToMessage.TryGetValue(errorCode, out errorMessage);
            return errorMessage + " (" + errorCode + ")";
        }

    }
}
