using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Common;


namespace Client.FileSending
{
    public class FileSender : IFileSender
    {
        private readonly IFileHandling proxy;
        public FileSender(IFileHandling proxy)
        {
            this.proxy = proxy;
        }

        private void Ocisti()
        {
            MessageBox.Show("Fajlovi obradjeni");
            Path.Path.selectedPath = null;
            Path.Path.FilePaths = null;
            Path.Path.fileNames.Clear();
            ((MainWindow)System.Windows.Application.Current.MainWindow).allFiles.Items.Clear();
        }

        private void CuvanjeFajla(CalculatedFile fajl, DirectoryInfo ResultsFolder)
        {
            fajl.MS.Position = 0;
            StreamWriter sw = new StreamWriter(fajl.MS);
            var fs = new FileStream($"{ResultsFolder.FullName}\\{fajl.FileName}", FileMode.Create, FileAccess.Write);
            fajl.MS.WriteTo(fs);
            fs.Close();
            fs.Dispose();
            fajl.Dispose();
        }

        private void FONLista(string filePath, List<FileOverNetwork> listOfFiles)
        {
            var fileName = System.IO.Path.GetFileName(filePath);
            FileOverNetwork fon = new FileOverNetwork(GetMemoryStream(filePath), fileName);
            listOfFiles.Add(fon);
        }

        public void SendFiles(string[] files)
        {
            List<FileOverNetwork> listOfFiles = new List<FileOverNetwork>();
            foreach (string filePath in files)
            {
                FONLista(filePath, listOfFiles);
            }

            var res = proxy.SendFiles(listOfFiles);
            DirectoryInfo ResultsFolder;
            if (!Directory.Exists($"{Path.Path.selectedPath}\\results"))
            {
                ResultsFolder = Directory.CreateDirectory($"{Path.Path.selectedPath}\\results");
            }
            else
            {
                ResultsFolder = new DirectoryInfo($"{Path.Path.selectedPath}\\results");
            }
            foreach (CalculatedFile fajl in res)
            {
                CuvanjeFajla(fajl, ResultsFolder);
            }
            Ocisti();
        }

        /// <summary>
        /// Samo pravi ms object nisam nista menjao 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private MemoryStream GetMemoryStream(string filePath)
        {
            MemoryStream ms = new MemoryStream();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(ms);
                fileStream.Close();
            }
            return ms;
        }
    }
}
