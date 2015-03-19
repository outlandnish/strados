using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strados.Vehicle.Log
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Speed { get; set; }
        public double Bearing { get; set; }
        public DateTimeOffset Time { get; set; }

        public Location()
        {
            Latitude = double.NaN;
            Longitude = double.NaN;
            Altitude = double.NaN;
            Speed = double.NaN;
            Bearing = double.NaN;
        }
    }
}
