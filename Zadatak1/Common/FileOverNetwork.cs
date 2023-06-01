using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum FileType { FORECAST, MEASURED }
    [DataContract]
    public class FileOverNetwork : IDisposable
    {
        [DataMember]
        public MemoryStream MS { get; set; }

        [DataMember]
        public string FileName { get; set; }

        private bool disposed = false;

        public FileOverNetwork(MemoryStream ms, string fileName)
        {
            this.MS = ms;
            this.FileName = fileName;
        }

        public FileType GetFileType()
        {
            if (FileName.ToUpper().StartsWith("FORECAST"))
            {
                return FileType.FORECAST;
            }
            else
            {
                return FileType.MEASURED;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    MS.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileOverNetwork()
        {
            Dispose(false);
        }
    }
}

