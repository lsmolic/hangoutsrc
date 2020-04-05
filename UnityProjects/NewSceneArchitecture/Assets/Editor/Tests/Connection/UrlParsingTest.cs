using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Hangout.Shared;

namespace Hangout.Shared.UnitTest
{
    [TestFixture]
    public class UrlParsingTest
    {
        [Test]
        public void AbsolutePathTest()
        {
            string baseUrl = "http://www.test.com/myapp";
            string queryString = "?var1=1&var2=2";
            string testQueryUrl = baseUrl + queryString;

            UrlParser myUrl = new UrlParser(testQueryUrl);

            Assert.Equals(baseUrl.CompareTo(myUrl.BaseUrl), 0);

            UrlParser myQueryUrl = new UrlParser(testQueryUrl);
            Assert.Equals(baseUrl.CompareTo(myQueryUrl.BaseUrl), 0);
        }

        [Test]
        public void ValidQueryStringTest()
        {
            string baseUrl = "http://www.test.com/myapp";
            string queryString = "?var1=1&var2=2";
            string testQueryUrl = baseUrl + queryString;

            UrlParser myUrl = new UrlParser(testQueryUrl);

            string query = myUrl.QueryString;
            // Find query string
            Assert.Equals(queryString, query);

            Dictionary<string, string> paramDict = myUrl.ParamDict;

            Assert.Equals(paramDict.Keys.Count, 2);
            Assert.Equals(paramDict.ContainsKey("var1"), true);
            Assert.Equals(paramDict.ContainsKey("var2"), true);
            Assert.Equals(paramDict["var1"], "1");
            Assert.Equals(paramDict["var2"], "2");
        }
        
        [Test]
        public void NoQueryStringTest()
        {
            string baseUrl = "http://www.test.com/myapp";

            UrlParser myUrl = new UrlParser(baseUrl);
            Assert.Equals(myUrl.BaseUrl, baseUrl);

            Assert.Equals(myUrl.ParamDict.Keys.Count, 0);
        }

    }
}
