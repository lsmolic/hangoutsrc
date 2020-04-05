/**  --------------------------------------------------------  *
 *   XmlUtilityTest.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 11/17/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Client;
using Hangout.Shared.UnitTest;

namespace Hangout.Client.UnitTest
{
	[TestFixture]
	public class XmlUtilityTest
	{
		private static readonly string TEST_XML =
			"<Root>\n" +
			"\t<DataNode name=\"Data\"/>\n" +
			"\t<ComplexDataNode name=\"ComplexData\">\n" +
			"\t\t<XRef path=\"Root/DataNode\"/>\n" +
			"\t\t<XRef path=\"Root/DataNode\"/>\n" +
			"\t\t<XRef path=\"Root/DataNode\"/>\n" + 
			"\t</ComplexDataNode>\n" + 
			"\t<XRef path=\"Root/DataNode\"/>\n" +
			"\t<XRef path=\"Root/DataNode\"/>\n" +
			"\t<XRef path=\"Root/DataNode\"/>\n" +
			"\t<XRef path=\"Root/DataNode\" name=\"TestName\" testAttrib=\"1\"/>\n" +
			"\t<XRef path=\"Root/ComplexDataNode\"/>\n" +
			"</Root>";
		private XmlDocument mTestDoc = new XmlDocument();

		public XmlUtilityTest()
		{
			mTestDoc.LoadXml(TEST_XML);
			XmlUtility.ResolveXRefs(mTestDoc);
		}

		[Test]
		public void XRefVerification()
		{
			Assert.AreEqual(11, mTestDoc.SelectNodes("//DataNode").Count);
		}

		[Test]
		public void XRefAttributesCopyAndOverride()
		{
			Assert.AreEqual("1", mTestDoc.SelectSingleNode("//DataNode/@testAttrib").InnerText);
			Assert.IsNotNull(mTestDoc.SelectSingleNode("//DataNode[@name='TestName']"));
		}
	}

}
