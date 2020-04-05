using System;
using System.IO;
using System.Web;

namespace Hangout.Shared
{
    public class HangoutPostedFile : MarshalByRefObject
    {
        string _fileName = null;
        string _contentType = null;
        int _contentLength = 0;
        Stream _inputStream = null;

        public string FileName
        {
            get { return _fileName; }
        }

        public string ContentType
        {
            get { return _contentType; }
        }

        public int ContentLength
        {
            get { return _contentLength; }
        }

        public Stream InputStream
        {
			get 
			{ 
				_inputStream.Position = 0; 
				return _inputStream; 
			}
        }

        public void SaveAs(string filename)
        {
            int length = 256;

            _inputStream.Position = 0;

            FileStream writeStream = File.OpenWrite(filename);
            Byte [] buffer = new Byte[length];

            int bytesRead = _inputStream.Read(buffer, 0, length);

            while (bytesRead > 0) // write the required bytes
            {
                writeStream.Write(buffer,0,bytesRead);
                bytesRead = _inputStream.Read(buffer, 0, length);
            }
            writeStream.Close();
        }

        public void InitPostedFile(HttpPostedFile postedFile)
        {
            int length = 256;

            _fileName = postedFile.FileName;
            _contentType = postedFile.ContentType;
            _contentLength = postedFile.ContentLength;

            _inputStream = new MemoryStream();

            byte[] buffer = new byte[length];

            int bytesRead = postedFile.InputStream.Read(buffer, 0, length);

            while (bytesRead > 0) // write the required bytes
            {
                _inputStream.Write(buffer, 0, bytesRead);
                bytesRead = postedFile.InputStream.Read(buffer, 0, length);
            }
        }
    }
}