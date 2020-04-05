using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Server;
using Hangout.Shared;
using System.Xml;

namespace ServerToServicesTest
{
	public class MockServerAssetRepository : IServerAssetRepository
	{
		private XmlDocument mMockXmlDna = null;

		public MockServerAssetRepository(string mockXmlDnaString)
		{
			mMockXmlDna = new XmlDocument();
			mMockXmlDna.LoadXml(mockXmlDnaString);
		}

		public XmlDocument GetXmlDna(ItemId itemId)
		{
			return mMockXmlDna;
		}

		public XmlDocument GetXmlDna(IEnumerable<ItemId> itemIdsForObject)
		{
			return mMockXmlDna;
		}
	}
}
