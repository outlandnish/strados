using Newtonsoft.Json;
using Strados.Obd.Specification;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strados.Vehicle.Models
{
    public class Leg : IDisposable
    {
        public List<Reading<double>> speedReadings { get; set; }
        public List<Reading<double>> rpmReadings { get; set; }
        public List<Reading<Location>> distanceReadings { get; set; }
        public List<Reading<double>> mafReadings { get; set; }
        public List<Reading<DiagnosticTroubleCode>> troubleCodeReadings { get; set; }
        public Vehicle Car { get; set; }
        public Reading<MonitorStatus> Status { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        private bool _completed;
        public bool Completed { get { return _completed; } }
        string dirPath;

        public Leg()
        {
            speedReadings = new List<Reading<double>>();
            rpmReadings = new List<Reading<double>>();
            distanceReadings = new List<Reading<Location>>();
            mafReadings = new List<Reading<double>>();
            Start = DateTimeOffset.UtcNow;
        }

        public Leg(Vehicle car, string path)
        {
            Car = car;
            speedReadings = new List<Reading<double>>();
            rpmReadings = new List<Reading<double>>();
            distanceReadings = new List<Reading<Location>>();
            mafReadings = new List<Reading<double>>();
            Start = DateTimeOffset.UtcNow;
            dirPath = path;
        }

        public void UpdateSpeed(double speed)
        {
            if (speedReadings.Count > 0 && speedReadings.Last().Value == 0 && speed == 0)
            {
            }
            else if (speedReadings.Count > 0 && speedReadings.Last().Value != 0 && speed == 0)
            {
                _completed = true;
                End = DateTimeOffset.UtcNow;
                speedReadings.Add(new Reading<double>(speed));
            }
            else
                speedReadings.Add(new Reading<double>(speed));
        }

        public void UpdateRPM(double rpm)
        {
            rpmReadings.Add(new Reading<double>(rpm));
        }

        public void UpdateMAF(double maf)
        {
            mafReadings.Add(new Reading<double>(maf));
        }

        public void UpdateTroubleCode(DiagnosticTroubleCode dtc)
        {
            troubleCodeReadings.Add(new Reading<DiagnosticTroubleCode>(dtc));
        }

        public void UpdateLocation(Location location)
        {
            var locReading = new Reading<Location>(location);
            distanceReadings.Add(locReading);
        }

        public override string ToString()
        {
            var text = "";
            text += string.Format("Leg Completed: {0}\n", Completed);
            if (speedReadings.Count > 0)
            {
                var driveTime = speedReadings.Last().Timestamp - speedReadings.First().Timestamp;
                text += string.Format("Time: {0}\n", driveTime.ToString());
            }
            if (speedReadings.Count > 0)
                text += string.Format("Speed Readings: {0}, Average: {2}, Fastest: {1}\n",
                    speedReadings.Count, speedReadings.Average(s => s.Value), speedReadings.Max(s => s.Value));
            if (rpmReadings.Count > 0)
                text += string.Format("RPM Readings: {0}, Average: {1}, Highest: {1}\n",
                    rpmReadings.Count, rpmReadings.Average(r => r.Value), rpmReadings.Max(r => r.Value));
            if (mafReadings.Count > 0)
                text += string.Format("MAF Readings: {0}, Average: {1}, Highest: {1}\n",
                    mafReadings.Count, mafReadings.Average(r => r.Value), mafReadings.Max(r => r.Value));

            return text;
        }

        public string Save()
        {
            return JsonConvert.SerializeObject(this);
        }

        #region IDisposable implementation
        public void Dispose()
        {
            //clear out all of the lists
            speedReadings.Clear();
            rpmReadings.Clear();
            distanceReadings.Clear();
            mafReadings.Clear();
        }
        #endregion
    }
}
