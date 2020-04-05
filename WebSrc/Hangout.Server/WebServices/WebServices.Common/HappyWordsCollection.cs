using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Hangout.Server.WebServices
{
    [XmlType(TypeName = "happyWords")]
    public class HappyWordsCollection : List<HappyWordEntry>
    {

    }

    public class HappyWordEntry
    {
        private string _swear;
        private string _replacement;

        [XmlElement(ElementName= "replacement")]
        public string Replacement
        {
            get { return _replacement; }
            set { _replacement = value; }
        }

        [XmlElement(ElementName = "swear")]
        public string Swear
        {
            get { return _swear; }
            set { _swear = value; }
        }
    }
}
