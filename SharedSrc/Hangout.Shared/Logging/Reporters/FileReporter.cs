using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Hangout.Shared
{
    public class FileReporter : ILogReporter
    {
        private FileInfo mFileInfo;

        public FileReporter(FileInfo fileInfo)
        {
            //TODO: make this way more secure..
            mFileInfo = fileInfo;
        }

        public void Report(ILogMessage logMessage)
        {
            File.AppendAllText(mFileInfo.FullName, logMessage.Message + "\n");
        }
    }
}
