using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using Hangout.Server.StateServer;
using Hangout.Server;
using System.Xml;
using System.Security;


namespace Hangout.Shared.UnitTest
{
	[TestFixture]
	class WebServiceRequestTest
	{
		
		public void TestSyncronousUnencryptedGET()
		{
			int intA = 100;
			uint uintB = Convert.ToUInt32(1000000000);
			//No <, >,&,+,\ if you don't urlEncode the data it will throw an exception when 
			//loaded into and XML document
			string value = "?*340-24 s8sdf ~!@#$%^()_ `12345678 9 0 -  ={}|[]:;',./ s08d";
			string stringC = "<DNA><value>"+ value +"</value></DNA>"; 

			WebServiceRequest webServiceRequest = new WebServiceRequest(StateServerConfig.WebServicesBaseUrl, "UnitTests", "TestBasicTypes");
			webServiceRequest.AddParam("intA", intA.ToString());
			webServiceRequest.AddParam("uintB", uintB.ToString());
			webServiceRequest.AddParam("stringC", stringC);
			webServiceRequest.Encrypted = false;
			webServiceRequest.Method = FormMethod.GET;

			XmlDocument xmlResponse = webServiceRequest.GetWebResponse();

			XmlNode xmlError = xmlResponse.SelectSingleNode("//RESTerror");
			Assert.IsNull(xmlError);
			
			int returnedInt = Convert.ToInt32(xmlResponse.SelectSingleNode("//intA").InnerText);
			Assert.AreEqual(intA,returnedInt);

			uint returnedUint = Convert.ToUInt32(xmlResponse.SelectSingleNode("//uintB").InnerText);
			Assert.AreEqual(uintB,returnedUint);

			string returnedString = xmlResponse.SelectSingleNode("//stringC").InnerXml;
			Assert.AreEqual(stringC,returnedString);
			
		}


	}
}
