using System.Collections.Generic;
using System.Threading.Tasks;
using Strados.Vehicle.Obd;

namespace Strados.Vehicle
{
    public interface ICarService
    {
        object Run(ObdCommand command);
        Task GetVehicleDetails(bool closeOnCompletion);
        IEnumerable<string> GetPublishedPids();
        IEnumerable<string> GetSupportedPids();
        bool TryConnect(bool reset);
        long QueueJob(ObdCommand command);
        void ExecuteQueue();
        void StopService();
    }
}
