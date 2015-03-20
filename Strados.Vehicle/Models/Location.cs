using System;

namespace Strados.Vehicle.Models
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
