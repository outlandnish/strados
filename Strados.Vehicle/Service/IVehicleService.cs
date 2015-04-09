using Strados.Obd;
using Strados.Obd.Specification;
using Strados.Vehicle.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Strados.Vehicle
{
    public interface IVehicleService
    {
        Task<VehicleStatus> ConnectToVehicle(bool reset = true, bool firstTime = false);
        Task<List<ObdPid>> GetReportedPids();
        Task<List<ObdPid>> GetSupportedPids();
        Task<ObdResult> Run(ObdPid command);
        Task<object> Run(ObdPid command, Func<string, object> parser);
        Task<object> Run(string command, Func<string, object> parser);
        Task<string> Run(string command);
    }
}
