using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Client.Gui;

namespace Hangout.Shared.UnitTest
{
    [TestFixture]
    public class StoreGuiTest : StoreGuiState
    {
        public StoreGuiTest():base(null)
        {

        }

        public override void HandleSearchResults(System.Xml.XmlDocument xmlResponse)
        {
            throw new NotImplementedException();
        }
        public override void EnterState()
        {
            throw new NotImplementedException();
        }
        public override void ExitState()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void ConvertPaymentItemsUrlTest()
        {
            string restOfUrl = "Assets/Thumbs/myimage.jpg";
            string inputUrl = "http://www.assetbase.net/" + restOfUrl;
            string newBaseUrl = "assets://";
            string expectedUrl = newBaseUrl + restOfUrl;

            string outputUrl = ConvertPaymentItemsUrl(inputUrl);
            Assert.AreEqual(outputUrl, expectedUrl);
        }
    }
}
