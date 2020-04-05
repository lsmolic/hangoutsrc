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
    public class ServiceCommand
    {
		private string noun;
		private string verb;
        private NameValueCollection parameters = new NameValueCollection();
        private PostFile postFile = new PostFile();
 
		[XmlAttribute("noun")]
		public string Noun
		{
			get { return noun; }
			set { noun = value; }
		}

		[XmlAttribute("verb")]
		public string Verb
		{
			get { return verb; }
			set { verb = value; }
		}


        [XmlIgnore()]
        public NameValueCollection Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        [XmlArray(ElementName = "Parameters")]
        [XmlArrayItem(ElementName = "Parameter", Type = typeof(StringPair))]
        public NameValueCollectionSerializable ValuesProxy
        {
            get { return new NameValueCollectionSerializable(parameters); }
        }

        [XmlIgnore()]
        public MemoryStream FileData
        {
            get { return postFile.FileData; }
            set { postFile.FileData = value; }
        }

        [XmlElement("PostFile", IsNullable = false)]
        public PostFile PostFile
		{
            get { return  postFile; }
            set { postFile = value; }
		}
	}

    public class ServiceCommandAsync
    {
        ServiceCommand serviceCommandData = new ServiceCommand();
        List<Guid> recipients = new List<Guid>();
        string result = "";
        Action<string> callback = null;

        public ServiceCommand ServiceCommandData
        {
            get { return serviceCommandData; }
            set { serviceCommandData = value; }
        }

        public List<Guid> Recipients
        {
            get { return recipients; }
            set { recipients = value; }
        }

        public string Result
        {
            get { return result; }
            set { result = value; }
        }

        public Action<string> Callback
        {
            get { return callback; }
            set { callback = value; }
        }
    }
  }