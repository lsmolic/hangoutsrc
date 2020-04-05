using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Hangout.Shared
{
	public class XmlTextWriterFormattedNoDeclaration : XmlTextWriter
	{
		public XmlTextWriterFormattedNoDeclaration(System.IO.TextWriter w)
			: base(w)
		{
			Formatting = System.Xml.Formatting.Indented;
		}

		public override void WriteStartDocument() { } // suppress
	}
}
