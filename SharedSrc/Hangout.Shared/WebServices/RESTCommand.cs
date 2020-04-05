using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Hangout.Shared
{
    [Serializable]
    [XmlRootAttribute("Request", IsNullable = false)]
    public class RESTCommand : ServiceCommand
    {
    }
}

	
