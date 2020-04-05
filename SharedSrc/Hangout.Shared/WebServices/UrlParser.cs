using System;
using System.Collections.Generic;
using System.Text;

namespace Hangout.Shared
{
    public class UrlParser
    {
        private readonly string mBaseUrl;
        public string BaseUrl
        {
            get { return mBaseUrl; }
        }
        
        private readonly string mQueryString;
	    public string QueryString
	    {
		    get { return mQueryString; }
	    }

        private readonly Dictionary<string,string> mParamDict;
	    public Dictionary<string,string> ParamDict
	    {
		    get { return mParamDict; }
	    }

        public UrlParser(string url)
        {
            Uri myUri = new Uri(url);

            // Get base url
            mBaseUrl = myUri.AbsolutePath;

            // Get query string
            mQueryString = myUri.Query;

            // Remove ?
            char[] trim = {'?'};
            string trimString = mQueryString.TrimStart(trim);

            // Get each query param separately
            char[] paramSeperator = {'&'};
            string[] paramList = trimString.Split(paramSeperator);

            mParamDict = new Dictionary<string,string>();
            foreach (string paramString in paramList)
            {
                char[] eq = {'='};
                string[] paramSplit = paramString.Split(eq);
                if (paramSplit.Length == 2)
                {
                    mParamDict[paramSplit[0]] = paramSplit[1];
                }
            }
        }
    }
}
