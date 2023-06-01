using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Server
{
    public class FileProcessing
    {
        public event EventHandler ListOfFilesProccessed;
        public event EventHandler<Audit> FileValidated;
        public event EventHandler<Load> RowProccessed;
        public event EventHandler<ImportedFile> StreamProccessed;

        private InMemoryDatabase baza = new InMemoryDatabase();
        private XMLDataBase xmlBaza = new XMLDataBase();
        string metodaProcesiranja = ConfigurationManager.AppSettings["calculationType"];
        private bool useXML = ConfigurationManager.AppSettings["storageType"].Equals("XMLDB") ? true : false;


        public List<CalculatedFile> ProcessFiles(List<FileOverNetwork> listOfFiles)
        {

            if (useXML)
            {
                ListOfFilesProccessed += xmlBaza.OnListOfFilesProccessed;
            }


            List<ImportedFile> processedFiles = new List<ImportedFile>(listOfFiles.Count);
            foreach (FileOverNetwork fon in listOfFiles)
            {
                fon.MS.Seek(0, SeekOrigin.Begin);
                var strim = new StreamReader(fon.MS);
                Audit izvestajZaFajl = ValidateFile(fon.FileName, strim);
                if (izvestajZaFajl.MessageType == MessageType.Info || izvestajZaFajl.MessageType == MessageType.Warning)
                {
                    processedFiles.Add(ProcessStream(strim, fon));

                }
                strim.Dispose();
                fon.Dispose();
            }

            List<ImportedFile> kojiImajuObeVrednosti = FilesWithBothValues(processedFiles);
            processedFiles.Clear();
            List<CalculatedFile> results = new List<CalculatedFile>(kojiImajuObeVrednosti.Count);
            foreach (ImportedFile file in kojiImajuObeVrednosti)
            {
                List<Load> loads = LoadsFromFile(file);
                WriteLoadsToFile(loads, file, results);
            }
            if (useXML)
            {
                OnListOfFilesProccessed();
                ListOfFilesProccessed -= xmlBaza.OnListOfFilesProccessed;
            }
            return results;
        }

        private void WriteLoadsToFile(List<Load> loads, ImportedFile file, List<CalculatedFile> results)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            string header = metodaProcesiranja == "Squared" ? "SQUARED_DEVIATION" : "ABSOLUTE_PERCENTAGE_DEVIATION";
            sw.WriteLine("TIMESTAMP," + header);
            foreach (Load l in loads)
            {
                double rezultat = Izracunaj(l, metodaProcesiranja);
                sw.Write(l.TimeStamp.ToString("yyyy-MM-dd HH:mm") + "," + rezultat.ToString() + '\n');
            }
            sw.Flush();
            string resultFileName = "result_" + TakeDate(file.Filename).ToString("yyyy_MM_dd") + ".csv";
            results.Add(new CalculatedFile(ms, resultFileName));
            loads.Clear();
        }

        private List<Load> LoadsFromFile(ImportedFile file)
        {
            List<Load> loads = new List<Load>();
            DateTime datumIzFajla = TakeDate(file.Filename).Date;
            DateTime kranjiDatum = datumIzFajla.AddDays(1);
            while (datumIzFajla != kranjiDatum)
            {
                Load load = baza.GetLoad(datumIzFajla.GetHashCode());
                if (load != null) loads.Add(load);
                datumIzFajla = datumIzFajla.AddHours(1);
            }
            return loads;
        }

        private List<ImportedFile> FilesWithBothValues(List<ImportedFile> processedFiles)
        {
            List<ImportedFile> fwbv = new List<ImportedFile>();
            for (int i = 0; i < processedFiles.Count; ++i)
            {
                for (int j = i + 1; j < processedFiles.Count; ++j)
                {
                    if (TakeDate(processedFiles[i].Filename) == TakeDate(processedFiles[j].Filename))
                    {
                        fwbv.Add(processedFiles[i]);
                        break;
                    }
                }
            }
            return fwbv;
        }

        private double Izracunaj(Load l, string metodaProcesiranja)
        {
            double ostvarena = l.MeasuredValue;
            double prognozirana = l.ForecastValue;
            if (metodaProcesiranja == "Squared")
            {
                l.SquaredDeviation = Math.Pow(((ostvarena - prognozirana) / ostvarena), 2);
                return l.SquaredDeviation;
            }
            else
            {
                l.AbsolutePercentageDeviation = ((Math.Abs(ostvarena - prognozirana)) / ostvarena) * 100;
                return l.AbsolutePercentageDeviation;
            }
        }

        private Audit PovratniAudit(MessageType mt, string fileName, string poruka)
        {
            Audit audit = new Audit(DateTime.Now, mt, "Fajl: " + fileName + " - " + poruka);
            OnFileValidated(audit);
            FileValidated -= baza.OnFileValidated;
            return audit;
        }

        private Audit ProveraIspravnosti(StreamReader strim, string dateString, int num_hours, int hours, string fileName, bool ispravan)
        {

            while (!strim.EndOfStream)
            {
                string row = strim.ReadLine();
                string rowDate = RowDate(row);

                if (rowDate.Equals(dateString))
                {
                    if (ColoumnCheck(row))
                    {

                        num_hours++;
                    }
                    else
                    {
                        return PovratniAudit(MessageType.Error, fileName, "Broj elemenata u vrsti je veci od dozvoljenog");
                    }
                }
                else
                {
                    return PovratniAudit(MessageType.Error, fileName, "FileName Date se ne poklapaju");
                }
            }
            if (!(num_hours == hours))
            {
                return PovratniAudit(MessageType.Error, fileName, "Los broj sati za fajl");
            }
            else
            {
                if (ispravan)
                {
                    return PovratniAudit(MessageType.Info, fileName, "Ispravan");
                }
                else
                {
                    return PovratniAudit(MessageType.Warning, fileName, "Pogresno definisan header");
                }
            }
        }

        private Audit ValidateFile(string fileName, StreamReader strim)
        {
            FileValidated += baza.OnFileValidated;
            string dateString = fnToDate(fileName);
            DateTime time = TakeDate(fileName);
            int month = TakeMonth(fileName);
            int year = TakeYear(fileName);
            int hours = CheckHours(month, LastSundayOfMonth(year, month), time);
            int num_hours = 0;
            string header = strim.ReadLine();

            if (HeaderCheck(header))
            {
                return ProveraIspravnosti(strim, dateString, num_hours, hours, fileName, true);
            }
            else
            {
                return ProveraIspravnosti(strim, dateString, num_hours, hours, fileName, false);
            }
        }

        private ImportedFile ProcessStream(StreamReader sr, FileOverNetwork fon)
        {
            RowProccessed += baza.OnRowProccessed;
            StreamProccessed += baza.OnStreamProccessed;
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            sr.ReadLine();
            ImportedFile trenutniImpf = new ImportedFile(fon.FileName);
            while (!sr.EndOfStream)
            {
                string row = sr.ReadLine();
                string rowDateTime = row.Split(',')[0];
                DateTime dateTime = DateTime.Parse(rowDateTime);
                Load load = baza.GetLoad(dateTime.GetHashCode());
                if (load == null)
                {
                    load = new Load(dateTime);
                }
                if (fon.GetFileType() == FileType.FORECAST)
                {

                    load.ForecastValue = double.Parse(row.Split(',')[1]);
                    load.ForecastFileID = trenutniImpf.Id;
                }
                else
                {
                    load.MeasuredValue = double.Parse(row.Split(',')[1]);
                    load.MeasuredFileID = trenutniImpf.Id;
                }

                OnRowProccessed(load);
            }
            RowProccessed -= baza.OnRowProccessed;

            OnStreamProccessed(trenutniImpf);
            StreamProccessed -= baza.OnStreamProccessed;
            return trenutniImpf;
        }

        private bool HeaderCheck(string header)
        {
            string[] zaglavlje = header.Split(',');
            string timeStamp = zaglavlje[0];
            string forecast_measured = zaglavlje[1];
            if (timeStamp.Equals("TIME_STAMP") &&
               (forecast_measured.Equals("FORECAST_VALUE") || forecast_measured.Equals("MEASURED_VALUE")))
                return true;
            else return false;
        }

        private bool ColoumnCheck(string col)
        {
            string[] colOne = col.Split(',');
            if (colOne.Length == 2) return true;
            else return false;

        }

        private string[] Dates(string fn)
        {
            string[] FileNameTimeStamp = fn.Split('.');
            return FileNameTimeStamp[0].Split('_');
        }

        private string fnToDate(string fn)
        {
            string[] date = Dates(fn);
            return date[1] + "-" + date[2] + "-" + date[3];
        }
        private DateTime TakeDate(string fn)
        {
            string[] date = Dates(fn);
            string[] day_and_month = new string[] { date[1], date[2], date[3] }; 
            return new DateTime(Int32.Parse(day_and_month[0]), Int32.Parse(day_and_month[1]), Int32.Parse(day_and_month[2]));
        }

        private int TakeMonth(string fn)
        {
            string[] date = Dates(fn);
            return Int32.Parse(date[2]);
        }
        private int TakeYear(string fn)
        {
            string[] date = Dates(fn);
            return Int32.Parse(date[1]);
        }
        private string RowDate(string row)
        {
            string[] splitedRow = row.Split(',');
            string[] dateTime = splitedRow[0].Split(' ');
            return dateTime[0];
        }

        public DateTime LastSundayOfMonth(int year, int month)
        {
            DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            int daysToLastSunday = ((int)lastDayOfMonth.DayOfWeek - (int)DayOfWeek.Sunday + 7) % 7;
            return lastDayOfMonth.AddDays(-daysToLastSunday);
        }

        public int CheckHours(int month, DateTime lastSunday, DateTime date)
        {
            if (month == 3 && date == lastSunday)
            {
                return 23;
            }
            else if (month == 10 && date == lastSunday)
            {
                return 25;
            }
            else
            {
                return 24;
            }
        }

        protected virtual void OnListOfFilesProccessed()
        {
            if (ListOfFilesProccessed != null)
            {
                ListOfFilesProccessed(this, EventArgs.Empty);
            }
        }

        protected virtual void OnFileValidated(Audit a)
        {
            if (FileValidated != null)
            {
                FileValidated(this, a);
            }
        }

        protected virtual void OnRowProccessed(Load l)
        {
            if (RowProccessed != null)
            {
                RowProccessed(this, l);
            }
        }

        protected virtual void OnStreamProccessed(ImportedFile iF)
        {
            if (StreamProccessed != null)
            {
                StreamProccessed(this, iF);
            }
        }
    }
}
