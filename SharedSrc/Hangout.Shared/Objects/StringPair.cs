using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Hangout.Shared
{
	[Serializable]
	public class StringPair
	{
		private string key;
		private string theValue;

		[XmlAttribute("name")]
		public string Key
		{
			get { return key; }
			set { key = value; }
		}
		[XmlAttribute("value")]
		public string Value
		{
			get { return theValue; }
			set { theValue = value; }
		}
	}
}
