using Strados.Obd.Specification;
using System.Collections.Generic;

namespace Strados.Vehicle
{
    public class Vehicle
    {
        public string Name { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Variant { get; set; }
        public int Year { get; set; }
        public string VIN { get; set; }
        public MonitorStatus Status { get; set; }
        public List<FuelSystemStatus> FuelSystemStatuses { get; set; }
        public List<ObdPid> PublishedPids { get; set; }
        public List<ObdPid> SupportedPids { get; set; }
        public ObdProtocol Protocol { get; set; }

        public Vehicle()
        {
            Protocol = ObdProtocol.NOT_SET;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Year, Make, Model, Variant);
        }
    }
}
