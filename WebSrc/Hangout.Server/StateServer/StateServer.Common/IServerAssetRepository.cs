using System;
using System.Collections.Generic;
using System.Text;
using Hangout.Shared;
using System.Xml;

namespace Hangout.Server
{
	public interface IServerAssetRepository
	{
		XmlDocument GetXmlDna(ItemId itemId);
		XmlDocument GetXmlDna(IEnumerable<ItemId> itemIdsForObject);
	}
}
