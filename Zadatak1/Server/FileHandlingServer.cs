using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class FileHandlingServer : IFileHandling
    {
        FileProcessing fileProcessor = new FileProcessing();
        public List<CalculatedFile> SendFiles(List<FileOverNetwork> listOfFiles)
        {
            return fileProcessor.ProcessFiles(listOfFiles);
        }
    }
}
