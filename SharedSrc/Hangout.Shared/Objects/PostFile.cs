using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Hangout.Shared
{
    [Serializable]
    [XmlRoot("PostFile", IsNullable = false)]
    public class PostFile
    {
        private MemoryStream fileData = new MemoryStream();
        private string fileContentType;
        private string fileName;

        [XmlAttribute("PostData")]
        public string FileDataString
        {
            get { return MemoryStreamToString(fileData); }
            set { fileData = StringToMemoryStream(value); }
        }

        [XmlIgnore()]
        public MemoryStream FileData
        {
            get { return fileData; }
            set { fileData = value; }
        }

        [XmlAttribute("FileContentType")]
        public string FileContentType
        {
            get { return fileContentType; }
            set { fileContentType = value; }
        }

        [XmlAttribute("FileName")]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private String MemoryStreamToString(MemoryStream data)
        {
            if (data == null)
            {
                return "";
            }

            Byte[] dataBuffer = new Byte[data.Length];
            data.Position = 0;
            data.Read(dataBuffer, 0, (int)data.Length);
            data.Position = 0;
            return UTF8ByteArrayToString(dataBuffer);
        }

        private MemoryStream StringToMemoryStream(String xmlString)
        {
            if (xmlString == null)
            {
                xmlString = "";
            }
            MemoryStream data = new MemoryStream();

            Byte[] dataBuffer = StringToUTF8ByteArray(xmlString);
            data.Write(dataBuffer, 0, dataBuffer.Length);
            data.Position = 0;

            return data;
        }

        private String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }


        private Byte[] StringToUTF8ByteArray(String xmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(xmlString);
            return byteArray;
        }

        public void RemoveLeadingSpacesAndLines()
        {
            MemoryStream fileDataNew = new MemoryStream();
            fileData.Position = 0;

            byte data = (byte)fileData.ReadByte();
            while (data == 0x20 || data == 0x0a)
            {
                data = (byte)fileData.ReadByte();
            }

            if (fileData.Position > 1)
            {
                int sizeOfData = (int)fileData.Length - ((int)fileData.Position + 1);

                Byte[] dataBuffer = new Byte[sizeOfData];
                fileData.Read(dataBuffer, 0, sizeOfData);
                    
                fileDataNew.Write(dataBuffer, 0, sizeOfData);
                fileDataNew.Position = 0;
                    
                fileData = fileDataNew;
            }
        }
    }
 }
