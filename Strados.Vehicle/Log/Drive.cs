using Newtonsoft.Json;
using Strados.Obd.Specification;
using Strados.Vehicle.Obd;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Strados.Vehicle.Log
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

        public ICarService service;

        double lastSpeed = double.MaxValue, lastRPM = double.MaxValue;

        public List<ObdCommand> Commands = new List<ObdCommand>() {
			ObdCommands.Speed, ObdCommands.RPM, ObdCommands.MassAirFlow(), //ObdCommands.Temperature(ObdPid.EngineCoolantTemperature),
			//ObdCommands.Temperature(ObdPid.EngineOilTemperature), ObdCommands.FuelSystemStatus
		};

        public Drive()
        {
            Current = new Leg();
            Start = DateTimeOffset.UtcNow;
        }

        public Drive(ICarService service, Vehicle car)
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
                service.QueueJob(ObdCommands.PendingTroubleCodes());
            }

            Current.UpdateLocation(location);
        }

        public async void StateUpdate(ObdCommand command, Action legCompleted = null, Action driveCompleted = null)
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
                service.QueueJob(ObdCommands.PendingTroubleCodes());
            }

            string cmd = command.Name;
            string value = command.Value == null ? "NODATA" : command.Value.ToString();

            try
            {
                if (cmd.Contains(ObdPid.VehicleSpeed.ToString()))
                {
                    if (value != "NODATA")
                    {
                        var speed = (double)command.Value;
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
                        var rpm = (double)command.Value;
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
                    var codes = (int)command.Value;
                    if (codes > 0)
                    {
                        service.QueueJob(ObdCommands.TroubleCodes(codes));
                        service.QueueJob(ObdCommands.PendingTroubleCodes());
                    }
                }
                else if (cmd.Contains(ObdPid.RequestTroubleCodes.ToString()))
                {
                    var codes = (List<DiagnosticTroubleCode>)command.Value;
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
