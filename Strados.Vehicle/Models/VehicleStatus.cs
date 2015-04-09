using Strados.Obd.Specification;
using System.Collections.Generic;

namespace Strados.Vehicle.Models
{
    public class VehicleStatus
    {
        public MonitorStatus Status { get; set; }
        public List<FuelSystemStatus> FuelSystems { get; set; }
        public List<ObdPid> Pids { get; set; }
    }
}
