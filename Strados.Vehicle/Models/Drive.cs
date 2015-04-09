using Newtonsoft.Json;
using Strados.Obd;
using Strados.Obd.Specification;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Strados.Vehicle.Models
{
    public class Drive
    {
        public Vehicle Car { get; set; }
        public Leg Current { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public string DrivePath { get; set; }
        public List<Leg> Legs { get; set; }
        public bool Completed { get; set; }

        public IVehicleService service;

        double lastSpeed = double.MaxValue, lastRPM = double.MaxValue;

        public List<ObdPid> commands = new List<ObdPid>() {
			ObdPid.VehicleSpeed, ObdPid.EngineRPM, ObdPid.MAFRate, //ObdCommands.Temperature(ObdPid.EngineCoolantTemperature),
			//ObdCommands.Temperature(ObdPid.EngineOilTemperature), ObdCommands.FuelSystemStatus
		};

        public Drive()
        {
            Current = new Leg();
            Start = DateTimeOffset.UtcNow;
        }

        public Drive(IVehicleService service, Vehicle car)
        {
            Start = DateTimeOffset.UtcNow;
            Car = car;
            Current = new Leg(car, DrivePath);
            this.service = service;
        }

        public void StateLocationUpdate(Location location, Action legCompletedAction = null)
        {
            if (Current == null)
                Current = new Leg(Car, DrivePath);
            else if (Current.Completed)
            {
                if (legCompletedAction != null)
                    legCompletedAction.Invoke();

                var legPath = Current.Save();
                Legs.Add(Current);
                Current.Dispose();
                Current = new Leg(Car, DrivePath);
                service.Run(ObdPid.PendingTroubleCodes);
            }

            Current.UpdateLocation(location);
        }

        public async void StateUpdate(ObdResult result, Action legCompleted = null, Action driveCompleted = null)
        {
            if (Car == null)
                throw new Exception("Car details need to be set first");
            if (Current == null)
                Current = new Leg(Car, DrivePath);
            else if (Current.Completed)
            {
                //raise leg completed event
                if (legCompleted != null)
                    legCompleted.Invoke();

                var legPath = Current.Save();
                Current.Dispose();
                Current = new Leg(Car, DrivePath);

                //TODO: do something with the trouble codes
                await service.Run(ObdPid.PendingTroubleCodes);
            }

            string cmd = result.Name;
            string value = result.Value == null ? "NODATA" : result.Value.ToString();

            try
            {
                if (cmd.Contains(ObdPid.VehicleSpeed.ToString()))
                {
                    if (value != "NODATA")
                    {
                        var speed = (double)result.Value;
                        Current.UpdateSpeed(speed);
                        lastSpeed = speed;
                    }
                    else
                        lastSpeed = 0;
                }
                else if (cmd.Contains(ObdPid.EngineRPM.ToString()))
                {
                    if (value != "NODATA")
                    {
                        var rpm = (double)result.Value;
                        Current.UpdateRPM(rpm);
                        lastRPM = rpm;
                    }
                    else
                        lastRPM = 0;

                    if (lastSpeed == 0 && lastRPM == 0)
                    {
                        End = DateTimeOffset.UtcNow;
                        Completed = true;

                        if (driveCompleted != null)
                            driveCompleted.Invoke();

                        SaveDrive();
                    }

                }
                else if (cmd.Contains(ObdPid.PendingTroubleCodes.ToString()))
                {
                    var codes = (int)result.Value;
                    if (codes > 0)
                    {
                        //TODO: store the trouble code results or do something with it
                        await service.Run(ObdPid.RequestTroubleCodes);
                    }
                }
                else if (cmd.Contains(ObdPid.RequestTroubleCodes.ToString()))
                {
                    var codes = (List<DiagnosticTroubleCode>)result.Value;
                    foreach (var code in codes)
                        Current.UpdateTroubleCode(code);
                }
                else
                    Debug.WriteLine(string.Format("{0} -> {1}", cmd, value));
            }
            catch (System.Exception err)
            {
                Debug.WriteLine("Parse Error: {0} -> {1}", cmd, value);
                Debug.WriteLine("{0}\n{1}", err.Message, err.StackTrace);
            }
        }

        public string SaveDrive()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void RestoreDrive(string driveData)
        {
            var temp = JsonConvert.DeserializeObject<Drive>(driveData);

            Car = temp.Car;
            Completed = temp.Completed;
            Legs = temp.Legs;
            Start = temp.Start;
            End = temp.End;
        }
    }
}
