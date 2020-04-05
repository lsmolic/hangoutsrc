using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Hangout.Shared;

namespace Hangout.Server
{
	/// <summary>
	/// This class is meant to take Xml and return useful escrow information
	/// </summary>
	public static class EscrowXmlUtil
	{
		public static int GetTotalCoins(XmlDocument xmlResponse)
		{
			int totalCoins = 0;
			//look for all Transaction Nodes in the xml response
			XmlNodeList nodeList = xmlResponse.SelectNodes("EscrowTransactions/Transaction");
			foreach (XmlNode node in nodeList)
			{
				totalCoins += Convert.ToInt32(node.Attributes["Value"].Value);
			}
			return totalCoins;
		}
	}
}
