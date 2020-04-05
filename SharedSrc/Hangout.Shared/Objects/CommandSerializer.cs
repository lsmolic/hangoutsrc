using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace Hangout.Shared
{
    public class ServiceCommandSerializer
    {
        /// <summary>
        /// Deserialize the ServiceCommand object
        /// </summary>
        /// <param name="xmlCommand">XML string containing the ServiceCommand</param>
        /// <returns>ServiceCommand object</returns>
        public ServiceCommand DeserializeCommandData(string xmlCommand, Type type)
        {
            // Create an instance of the XmlSerializer specifying type and namespace.
            XmlSerializer serializer = new XmlSerializer(type);

            StringReader stringReader = new StringReader(xmlCommand);
            XmlTextReader xmlReader = new XmlTextReader(stringReader);

            // Use the Deserialize method to restore the object's state.
            ServiceCommand command = (ServiceCommand)serializer.Deserialize(xmlReader);

            return command;
        }

        /// <summary>
        /// Serialize the ServiceCommand object
        /// </summary>
        /// <param name="cmd">ServiceCommand object</param>
        /// <returns>XML string containing the ServiceCommand</returns>
        public string SerializeCommandData(ServiceCommand cmd, Type type)
        {
            //Create our own namespaces for the output
            XmlSerializerNamespaces nameSpace = new XmlSerializerNamespaces();

            //Add an empty namespace and empty value
            nameSpace.Add("", "");

            // Create an instance of the XmlSerializer specifying type and namespace.
            XmlSerializer commandData = new XmlSerializer(type);

            StringWriterWithEncoding stringWriter = new StringWriterWithEncoding(Encoding.UTF8);
            XmlTextWriterFormattedNoDeclaration xmlTextWriter = new XmlTextWriterFormattedNoDeclaration(stringWriter);

            commandData.Serialize(xmlTextWriter, cmd, nameSpace);

            string xmlCommandData = stringWriter.ToString();

            return xmlCommandData;
        }
    }
}
