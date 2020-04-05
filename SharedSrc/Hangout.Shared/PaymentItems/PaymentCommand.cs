using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Hangout.Shared
{
    [Serializable]
    [XmlRootAttribute("Request", IsNullable = false)]
    public class PaymentCommand : ServiceCommand
    {
    }
}

