using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Server
{
    public class XMLDataBase
    {
        private string auditFilePath;
        private string importedFilePath;
        private string loadFilePath;

        public XMLDataBase()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.auditFilePath = Path.Combine(baseDirectory, "Audits.xml");
            this.importedFilePath = Path.Combine(baseDirectory, "ImportedFiles.xml");
            this.loadFilePath = Path.Combine(baseDirectory, "Loads.xml");
        }

        public void SaveAudits(Dictionary<int, Audit> audits)
        {
            Dictionary<int, Audit> existingAudits = LoadAudits();

            foreach (Audit audit in audits.Values)
            {
                existingAudits[audit.Id] = audit;
            }

            if (!File.Exists(auditFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(auditFilePath));
            }

            using (FileStream fileStream = new FileStream(auditFilePath, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Audit>));
                serializer.Serialize(fileStream, existingAudits.Values.ToList());
            }
        }

        public void SaveImportedFiles(Dictionary<int, ImportedFile> importedFiles)
        {
            Dictionary<int, ImportedFile> existingImportedFiles = LoadImportedFiles();

            foreach (ImportedFile importedFile in importedFiles.Values)
            {
                existingImportedFiles[importedFile.Id] = importedFile;
            }

            if (!File.Exists(importedFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(importedFilePath));
            }

            using (FileStream fileStream = new FileStream(importedFilePath, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<ImportedFile>));
                serializer.Serialize(fileStream, existingImportedFiles.Values.ToList());
            }
        }

        public void SaveLoads(Dictionary<int, Load> loads)
        {
            Dictionary<int, Load> existingLoads = LoadLoads();

            foreach (Load load in loads.Values)
            {
                existingLoads[load.Id] = load;
            }

            if (!File.Exists(loadFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(loadFilePath));
            }

            using (FileStream fileStream = new FileStream(loadFilePath, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Load>));
                serializer.Serialize(fileStream, existingLoads.Values.ToList());
            }
        }

        public Dictionary<int, Audit> LoadAudits()
        {
            if (File.Exists(auditFilePath))
            {
                long length = new FileInfo(loadFilePath).Length;
                if (length > 0)
                {
                    using (XmlReader reader = XmlReader.Create(auditFilePath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Audit>));
                        return ((List<Audit>)serializer.Deserialize(reader)).ToDictionary(audit => audit.Id);
                    }
                }
            }
            return new Dictionary<int, Audit>();
        }

        public Dictionary<int, ImportedFile> LoadImportedFiles()
        {
            if (File.Exists(importedFilePath))
            {
                long length = new FileInfo(auditFilePath).Length;
                if (length > 0)
                {
                    using (XmlReader reader = XmlReader.Create(importedFilePath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<ImportedFile>));
                        return ((List<ImportedFile>)serializer.Deserialize(reader)).ToDictionary(importedFile => importedFile.Id);
                    }
                }
            }
            return new Dictionary<int, ImportedFile>();
        }

        public Dictionary<int, Load> LoadLoads()
        {
            if (File.Exists(loadFilePath))
            {
                long length = new FileInfo(auditFilePath).Length;
                if (length > 0)
                {
                    using (XmlReader reader = XmlReader.Create(loadFilePath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Load>));
                        return ((List<Load>)serializer.Deserialize(reader)).ToDictionary(load => load.Id);
                    }
                }
            }
            return new Dictionary<int, Load>();
        }

        public void OnListOfFilesProccessed(object source, EventArgs e)
        {
            InMemoryDatabase db = new InMemoryDatabase();
            SaveAudits(db.GetAllAudits());
            SaveImportedFiles(db.GetAllImportedFiles());
            SaveLoads(db.GetAllLoads());
        }
    }

}
