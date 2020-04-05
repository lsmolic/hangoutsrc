using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Hangout.Shared
{
	public class StringWriterWithEncoding : StringWriter
	{
		Encoding encoding;

		public StringWriterWithEncoding(Encoding encoding)
		{
			this.encoding = encoding;
		}

		public override Encoding Encoding
		{
			get { return encoding; }
		}
	}
}
