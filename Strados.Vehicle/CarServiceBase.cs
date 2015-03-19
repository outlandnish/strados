using Strados.Obd.Extensions;
using Strados.Obd.Specification;
using Strados.Vehicle.Obd;
using Strados.Vehicle.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Strados.Vehicle
{
    public abstract class CarServiceBase : ICarService
    {
        protected const int MAX_RESETS = 3;
        protected int resets = 0;
        protected Vehicle car;
        protected Drive drive;

        public abstract object Run(ObdCommand command);

        public bool TryConnect(bool reset)
        {
            Debug.WriteLine("Trying to connect, reset = " + reset);
            if (reset)
            {
                Run(ObdCommands.Reset);
                Run(ObdCommands.Info);
            }

            Run(ObdCommands.Toggle(ObdPid.Elm327Echo, true));
            Run(ObdCommands.Toggle(ObdPid.Elm327LineFeed, true));
            Run(ObdCommands.Toggle(ObdPid.Elm327Headers, true));

            bool found = false;
            var protocols = Enum.GetNames(typeof(ObdProtocol));

            if (car == null || car.Protocol == ObdProtocol.NOT_SET)
            {
                Run(ObdCommands.Protocol(ObdProtocol.Auto));
                var d = Run(ObdCommands.RPM);

                if (d == null)
                {
                    for (int i = 0; i < protocols.Length - 1; i++)
                    {
                        var protocol = (ObdProtocol)Enum.Parse(typeof(ObdProtocol), protocols[i]);
                        Run(ObdCommands.Protocol(protocol));
                        //var data = await Run (ObdCommands.ModeSupport (ObdPid.PidSupport_01_20));
                        var data = Run(ObdCommands.RPM);

                        if (data != null)
                        {
                            //dummy data
                            Run(ObdCommands.RPM);
                            //store car protocol
                            car.Protocol = protocol;

                            found = true;
                            break;
                        }
                    }
                }
                else
                {
                    //store car protocol
                    car.Protocol = ObdProtocol.Auto;
                    //dummy data
                    Run(ObdCommands.RPM);
                    found = true;
                }
            }
            else
            {
                //dummy data
                Run(ObdCommands.RPM);
                found = true;
            }

            //if(found)
            //await Run (ObdCommands.BatteryVoltage);

            return found;
        }

        public Task GetVehicleDetails(bool closeOnCompletion)
        {
            return Task.Run(async () =>
            {
                var details = this.car;
                var published = GetPublishedPids();
                var supported = GetSupportedPids();

                try
                {
                    details.SupportedPids = supported.Select(s => (ObdPid)Enum.Parse(typeof(ObdPid), s)).ToList();
                    details.PublishedPids = published.Select(s => (ObdPid)Enum.Parse(typeof(ObdPid), s)).ToList();
                }
                catch (ArgumentException err)
                {
                    Debug.WriteLine(err.Message + "\n" + err.StackTrace);
                }

                if (supported.Contains(ObdPid.MonitorStatus.StringValue()))
                    details.Status = (MonitorStatus)Run(ObdCommands.Status);
                if (supported.Contains(ObdPid.FuelSystemStatus.StringValue()))
                {
                    var fuelSystemStatuses = (List<FuelSystemStatus>)Run(ObdCommands.FuelSystemStatus);
                    details.FuelSystemStatuses = fuelSystemStatuses;
                }

                if (details.Status != null)
                {
                    Debug.WriteLine(string.Format("# of codes: {0}", details.Status.DTCCount));
                    Debug.WriteLine(string.Format("Malfunction Indicator Light on: {0}", details.Status.CheckEngineLightOn));

                    if (details.Status.DTCCount > 0)
                    {
                        var codes = (List<DiagnosticTroubleCode>)Run(ObdCommands.TroubleCodes(details.Status.DTCCount));
                        if (details.Status.DTCCount > 0 && codes == null)
                        {
                            //let's send a reset because this must mean the previous DTC codes haven't been cleared, even if fixed
                        }
                        else
                        {
                            foreach (var code in codes)
                            {
                                //await code.LookupCode();
                                //Debug.WriteLine(code.Code + " -> " + code.Description);
                            }
                        }
                    }

                    foreach (OnBoardTest test in (OnBoardTest[])System.Enum.GetValues(typeof(OnBoardTest)))
                    {
                        bool incomplete;
                        if (details.Status.Test(test, out incomplete))
                            Debug.WriteLine(string.Format("{0}: Available, Complete: {1}", test, !incomplete));
                        else
                            Debug.WriteLine(string.Format("{0}: Unavailable", test));
                    }
                    car = details;
                }
            });
        }

        public IEnumerable<string> GetPublishedPids()
        {
            List<ObdPid> supported = new List<ObdPid>();

            supported.AddRange(getPublishedPids(ObdPid.PidSupport_01_20));
            supported.AddRange(getPublishedPids(ObdPid.PidSupport_21_40));
            supported.AddRange(getPublishedPids(ObdPid.PidSupport_41_60));
            supported.AddRange(getPublishedPids(ObdPid.PidSupport_61_80));
            supported.AddRange(getPublishedPids(ObdPid.PidSupport_81_A0));

            return supported.Select(s => s.ToString()).ToList();
        }

        public List<ObdPid> getPublishedPids(ObdPid pidRange)
        {
            if(!(pidRange == ObdPid.PidSupport_01_20 || pidRange == ObdPid.PidSupport_21_40
             || pidRange == ObdPid.PidSupport_41_60 || pidRange == ObdPid.PidSupport_61_80 || pidRange == ObdPid.PidSupport_81_A0))
                throw new ArgumentException("Argument not a pid range OBDPid");

            var support = Run(ObdCommands.ModeSupport(pidRange)) as Dictionary<ObdPid, bool>;

            if(support == null)
                return new List<ObdPid>();
            else
                return support.Keys.Where(k => support[k]).ToList();
        }

        public IEnumerable<string> GetSupportedPids()
        {
            List<ObdPid> supported = new List<ObdPid>();
            foreach (var pid in Enum.GetNames(typeof(ObdPid)))
            {
                var p = (ObdPid)Enum.Parse(typeof(ObdPid), pid);
                if (p.StringValue().StartsWith("01"))
                {
                    var result = (string) Run(new ObdCommand((c, o) =>
                    {
                        Debug.WriteLine(string.Format("{0}, {1}", c, o));
                        return o;
                    }, p));
                    if (result != null && !result.Contains("NO DATA"))
                        supported.Add(p);
                }
            }

            return supported.Select(s => s.ToString()).ToList();
        }

        public abstract long QueueJob(Obd.ObdCommand command);

        public abstract void ExecuteQueue();

        public abstract void StopService();
    }
}
