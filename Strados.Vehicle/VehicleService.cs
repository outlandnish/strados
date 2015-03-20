using System.Collections.Generic;
using System.Threading.Tasks;
using Strados.Vehicle.Obd;
using Strados.Obd.Specification;

namespace Strados.Vehicle
{
    public interface VehicleService
    {
        object Run(ObdPid pid);
        Task GetVehicleDetails(bool closeOnCompletion);
        IEnumerable<ObdPid> GetPublishedPids();
        IEnumerable<ObdPid> GetSupportedPids();
        bool TryConnect(bool reset);
        long QueueJob(ObdPid command);
        void ExecuteQueue();
        void StopService();
    }
}
