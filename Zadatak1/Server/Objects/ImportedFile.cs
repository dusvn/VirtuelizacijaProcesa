using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [Serializable]
    public class ImportedFile
    {
        static int count = 0;
        int id;
        string filename;

        public int Id { get => id; set => id = value; }
        public string Filename { get => filename; set => filename = value; }

        public ImportedFile(int id, string filename)
        {
            Id = id;
            Filename = filename;
        }

        public ImportedFile(string fileName)
        {
            Id = ++count;
            Filename = fileName;
        }

        public ImportedFile()
        {

        }
    }
}
