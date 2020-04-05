/**  --------------------------------------------------------  *
 *   XmlAsset.cs
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 11/05/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;

using System;
using System.Xml;

using Hangout.Shared;

namespace Hangout.Client
{
	public class XmlAsset : Asset
	{
		public static string UniqueKeyNamespace
		{
			get { return "Path:"; }
		}

		private readonly XmlDocument mXmlDocument;
		public XmlDocument XmlDocument
		{
			get { return mXmlDocument; }
		}

		public XmlAsset(string xml, string path)
			: base(AssetSubType.NotSet, xml, path, UniqueKeyNamespace + path)
		{
			mXmlDocument = new XmlDocument();
			mXmlDocument.LoadXml(xml);
		}
	}
}