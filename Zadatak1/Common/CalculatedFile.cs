using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace Common
{
    [DataContract]
    public class CalculatedFile : IDisposable
    {
        [DataMember]
        public MemoryStream MS { get; set; }

        [DataMember]
        public string FileName { get; set; }

        private bool disposed = false;

        public CalculatedFile(MemoryStream ms, string fileName)
        {
            this.MS = ms;
            this.FileName = fileName;
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

        ~CalculatedFile()
        {
            Dispose(false);
        }
    }
}