using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [Serializable]
    public class Load
    {
        int id;
        DateTime timeStamp;
        double forecastValue;
        double measuredValue;
        double absolutePercentageDeviation;
        double squaredDeviation;
        int forecastFileID;
        int measuredFileID;

        public int Id { get => id; set => id = value; }
        public DateTime TimeStamp { get => timeStamp; set => timeStamp = value; }
        public double ForecastValue { get => forecastValue; set => forecastValue = value; }
        public double MeasuredValue { get => measuredValue; set => measuredValue = value; }
        public double AbsolutePercentageDeviation { get => absolutePercentageDeviation; set => absolutePercentageDeviation = value; }
        public double SquaredDeviation { get => squaredDeviation; set => squaredDeviation = value; }
        public int ForecastFileID { get => forecastFileID; set => forecastFileID = value; }
        public int MeasuredFileID { get => measuredFileID; set => measuredFileID = value; }

        public Load(int id, DateTime timeStamp, double forecastValue, double measuredValue, double absolutePercentageDeviation, double squaredDeviation, int forecastFileID, int measuredFileID)
        {
            Id = id;
            TimeStamp = timeStamp;
            ForecastValue = forecastValue;
            MeasuredValue = measuredValue;
            AbsolutePercentageDeviation = absolutePercentageDeviation;
            SquaredDeviation = squaredDeviation;
            ForecastFileID = forecastFileID;
            MeasuredFileID = measuredFileID;
        }

        public Load(DateTime timeStamp)
        {
            Id = timeStamp.GetHashCode();
            TimeStamp = timeStamp;
            AbsolutePercentageDeviation = 0;
            SquaredDeviation = 0;
            ForecastValue = 0;
            MeasuredValue = 0;
        }

        public Load()
        {

        }
    }
}
