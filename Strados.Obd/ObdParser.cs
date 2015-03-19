using Strados.Obd.Extensions;
using Strados.Obd.Helpers;
using Strados.Obd.Specification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Strados.Obd
{
    public class ObdParser
    {
        private static ObdParser parser = new ObdParser();
        private static IEnumerable<MethodInfo> methods;
        public static ObdResult Parse(string hexData)
        {
            var normalized = normalize(hexData);

            //extract mode + command from data string
            var mode = parseMode(normalized[0]);
            var command = parseCommand(normalized[1]);

            var pid = (ObdPid)((mode - 1) * 200 + command);

            //check if the pid value has a corresponding function
            if (!Enum.IsDefined(typeof(ObdPid), pid))
                throw new NotSupportedException(string.Format("Mode {0}, Command {1} is not supported", mode, command));

            //populate our methods list using reflection if it hasn't already been populated
            if (methods == null)
                methods = parser.GetType().GetRuntimeMethods();

            MethodInfo parseFunction;
            List<object> parameters = new List<object>() { normalized.ToList().Skip(2).ToArray() };

            //check for special cases (o2, temperature, etc)
            if (pid.ToString().Contains("PidSupport"))
            {
                parseFunction = methods.FirstOrDefault(m => m.Name == "PidSupport");
                parameters.Add(pid);
            }
            else if (pid.ToString().Contains("MonitorStatus"))
                parseFunction = methods.FirstOrDefault(m => m.Name == "MonitorStatus");
            else if (pid.ToString().Contains("CatalystTemperature"))
                parseFunction = methods.FirstOrDefault(m => m.Name == "CatalystTemperature");
            else if (pid.ToString().Contains("Temperature"))
                parseFunction = methods.FirstOrDefault(m => m.Name == "Temperature");
            else if (pid.ToString().Contains("TermFuelPercentTrim"))
                parseFunction = methods.FirstOrDefault(m => m.Name == "FuelTrim");
            else if (pid.ToString().Contains("SecondaryO2Sensor"))
                parseFunction = methods.FirstOrDefault(m => m.Name == "SecondaryBank");
            else if (pid.ToString().Contains("ThrottlePosition") || pid.ToString().Contains("AbsolutePedal"))
                parseFunction = methods.FirstOrDefault(m => m.Name == "ThrottlePosition");
            else
                parseFunction = methods.Where(m => m.Name == pid.ToString()).FirstOrDefault();

            if (parseFunction != null)
            {
                try
                {
                    var result = parseFunction.Invoke(parser, parameters.ToArray());
                    return new ObdResult { Mode = mode, Command = command, Name = pid.ToString(), Value = result };
                }
                catch (Exception err)
                {
                    throw new ArgumentException(string.Format("Unable to parse {0}", parseFunction.Name), err);
                }
            }
            else
                throw new NotSupportedException(string.Format("Mode {0}, Command {1} ({2}) is not supported", mode, command, pid.ToString()));
        }

        private static int parseMode(string data)
        {
            int mode;
            if (int.TryParse(data, out mode))
                mode = mode - 40;
            else
                throw new ArgumentException("Invalid mode string");

            return mode;
        }

        private static int parseCommand(string data)
        {
            return Convert.ToInt32(data, 16);
        }

        private static string[] normalize(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentException("Invalid data");

            //remove leading/trailing whitespace
            data = data.Trim();

            //split up the data into two character chunks if it isn't already
            if (!data.Contains(" "))
                for (int i = 2; i < data.Length; i += 2)
                {
                    data = data.Insert(i, " ");
                    i++;
                }

            return data.Split(' ');
        }

        private static Dictionary<ObdPid, bool> PidSupport(string[] data, ObdPid pid)
        {
            int offset;
            switch (pid)
            {
                case ObdPid.PidSupport_01_20:
                    offset = 0;
                    break;
                case ObdPid.PidSupport_21_40:
                    offset = 32;
                    break;
                case ObdPid.PidSupport_41_60:
                    offset = 64;
                    break;
                case ObdPid.PidSupport_61_80:
                    offset = 96;
                    break;
                case ObdPid.PidSupport_81_A0:
                    offset = 128;
                    break;
                default:
                    offset = 0;
                    break;
            }

            var binary = HexHelper.Sanitize(data);

            //dictionary of pids and car support for each one
            Dictionary<ObdPid, bool> pidSupport = new Dictionary<ObdPid, bool>();
            for (int i = 0; i < binary.Length; i++)
                pidSupport.Add((ObdPid)(i + 1) + offset, binary[i] == '1');

            return pidSupport;
        }

        private MonitorStatus MonitorStatus(string[] data)
        {
            var binary = HexHelper.Sanitize(data);

            //get # of DTC codes using bits A0 - A6
            var dtcCount = Convert.ToInt32(binary.Substring(1, 6), 2);

            //convert all bits to boolean flags for polling on board tests
            bool[] tests = new bool[binary.Length];
            for (int i = 0; i < binary.Length; i++)
                tests[i] = binary[i] == '1';

            return new MonitorStatus() { CheckEngineLightOn = tests[7], DTCCount = dtcCount, OnBoardTests = tests };
        }

        private List<FuelSystemStatus> FuelSystemStatus(string[] data)
        {
            var binary = HexHelper.Sanitize(data);

            List<FuelSystemStatus> FuelSystemStatuses = new List<FuelSystemStatus>();

            try
            {
                var fs = Convert.ToInt32(binary.Substring(0, 4), 2);
                FuelSystemStatuses.Add(new FuelSystemStatus(fs));

                var fs2 = Convert.ToInt32(binary.Substring(4, 4), 2);
                FuelSystemStatuses.Add(new FuelSystemStatus(fs2));
            }
            catch (Exception err)
            { }

            return FuelSystemStatuses;
        }

        private static double CalcEngineLoad(string[] data)
        {
            return (double)Convert.ToInt32(data[0], 16) * 100.0 / 255;
        }

        private static double Temperature(string[] data)
        {
            return (double)Convert.ToInt32(data[0], 16) - 40.0;
        }

        private static double FuelTrim(string[] data)
        {
            return (double)Convert.ToInt32(data[0], 16) * 100 / 128;
        }

        private static int FuelPressure(string[] data)
        {
            return Convert.ToInt32(data[0], 16) * 3;
        }

        private static double IntakeManifoldAbsolutePressure(string[] data)
        {
            return Convert.ToInt32(data[0], 16);
        }

        private static double EngineRPM(string[] data)
        {
            return (double)IntegerRange(data) / 4.0;
        }

        private static int VehicleSpeed(string[] data)
        {
            return Convert.ToInt32(data[0], 16);
        }

        private static double TimingAdvance(string[] data)
        {
            return ((double)Convert.ToInt32(data[0], 16) - 128.0) / 2;
        }

        private static double MAFRate(string[] data)
        {
            return ((double)IntegerRange(data) / 100.0);
        }

        private static double ThrottlePosition(string[] data)
        {
            return Percentage(data);
        }

        private static string CommandedSecondaryAirStatus(string[] data)
        {
            return ((SecondaryAirStatus)Convert.ToInt32(data[0], 16)).ToString();
        }

        private bool[] OxygenSensorsPresent(string[] data)
        {
            var binary = HexHelper.Sanitize(data);
            bool[] sensors = new bool[8];
            for(int i = 0; i < sensors.Length; i++)
            {
                sensors[i] = binary[i] == '1';
            }

            return sensors;
        }

        private static object BankSensorVoltage(string[] data)
        {
            return new
            {
                Voltage = (double)Convert.ToInt32(data[0], 16) / 200.0,
                Trim = (double)Convert.ToInt32(data[1], 16) / 128.0,
                TrimUsed = Convert.ToInt32(data[1], 16) != 255
            };
        }

        private static ObdStandard OBDStandard(string[] data)
        {
            return (ObdStandard)Convert.ToInt32(data[0]);
        }

        private static object AuxilaryInputStatus(string[] data)
        {
            return new { PowerTakeOff = HexHelper.HexStringToBinary(data[0])[0] == '1' };
        }

        private static TimeSpan RunTimeSinceEngineStart(string[] data)
        {
            return TimeSpan.FromSeconds(IntegerRange(data));
        }

        private static int DistanceTraveledWithMILOn(string[] data)
        {
            return IntegerRange(data);
        }

        private static double FuelRailPressure_RelativeToManifoldVacuum(string[] data)
        {
            return (double)IntegerRange(data) * 0.079;
        }

        private static double FuelRailPressure_Diesel_GasolineDirectInject(string[] data)
        {
            return (double)IntegerRange(data) * 10.0;
        }

        private object EquivalenceRatioVoltage(string[] data)
        {
            return new
            {
                EquivalenceRatioCurrent = (double)IntegerRange(data) / 32768.0,
                Voltage = (double)IntegerRange(data, true) / 8192.0
            };
        }

        private static double EGRError(string[] data)
        {
            return Percentage(data);
        }

        private static int NumberWarmUpsSinceCodeCleared(string[] data)
        {
            return Convert.ToInt32(data[0], 16);
        }

        private static double EvaporationSystemVaporPressure(string[] data)
        {
            return IntegerRange(data) / 4;
        }

        private static double BarometricPressure(string[] data)
        {
            return Convert.ToInt32(data[0], 16);
        }

        private static object EquivalenceRatioCurrent(string[] data)
        {
            return new
            {
                Ratio = (double)IntegerRange(data) / 32768.0,
                Voltage = (double)IntegerRange(data, true) / 256.0 - 128.0
            };
        }

        private static double CatalystTemperature(string[] data)
        {
            return ((double)IntegerRange(data) / 10.0) - 40.0;
        }

        private static double ControlModuleVoltage(string[] data)
        {
            return (double)IntegerRange(data) / 1000.0;
        }

        private static double AbsoluteLoadValue(string[] data)
        {
            return (double)IntegerRange(data) * 100.0 / 255.0;
        }

        private static double FuelAirCommandedEquivalenceRatio(string[] data)
        {
            return (double)IntegerRange(data) / 32768.0;
        }

        private static double CommandedThrottleActuator(string[] data)
        {
            return Percentage(data);
        }

        private static TimeSpan TimeRunWithMILOn(string[] data)
        {
            return TimeSpan.FromMinutes(IntegerRange(data));
        }

        private static TimeSpan TimeSinceTroubleCodesCleared(string[] data)
        {
            return TimeSpan.FromMinutes(IntegerRange(data));
        }

        private static object MaxEquivRatioO2SensorIMAP(string[] data)
        {
            return new
            {
                Ratio = Convert.ToInt32(data[0], 16),
                O2Voltage = Convert.ToInt32(data[1], 16),
                O2Current = Convert.ToInt32(data[2], 16),
                IntakeMAP = Convert.ToInt32(data[3], 16),
            };
        }

        private static int FuelType(string[] data)
        {
            return Convert.ToInt32(data[0], 16);
        }

        private static double EthanolFuelPercent(string[] data)
        {
            return Percentage(data);
        }

        private static double AbsoluteEvapVaporPressure(string[] data)
        {
            return (double)IntegerRange(data) / 200.0;
        }

        private static int EvapVaporPressure(string[] data)
        {
            return IntegerRange(data) - 32767;
        }

        private static double[] SecondaryBank(string[] data)
        {
            double firstBank = ((double)Convert.ToInt32(data[0], 16) - 128.0) * 100.0 / 128.0;
            double secondBank = ((double)Convert.ToInt32(data[1], 16) - 128.0) * 100.0 / 128.0;

            return new double[] { firstBank, secondBank };
        }

        private static int FuelRailPressure(string[] data)
        {
            return IntegerRange(data) * 10;
        }

        private static double RelativeAcceleratorPedalPosition(string[] data)
        {
            return Percentage(data);
        }

        private static double HybridBatteryPackRemainingLife(string[] data)
        {
            return Percentage(data);
        }

        private static double FuelInjectionTiming(string[] data)
        {
            return ((double)IntegerRange(data) - 26880.0) / 128.0;
        }

        private static double EngineFuelRate(string[] data)
        {
            return (double)IntegerRange(data) * 0.05;
        }

        /*private static double EmissionsRequirements(string[] data)
        {
            
        }*/

        private static double DriverDemandEnginePercentTorque(string[] data)
        {
            return Convert.ToInt32(data[0], 16) - 125;
        }

        private static double ActualEnginePercentTorque(string[] data)
        {
            return Convert.ToInt32(data[0], 16) - 125;
        }

        private static double EngineReferenceTorque(string[] data)
        {
            return IntegerRange(data);
        }

        private static object EnginePercentTorqueData(string[] data)
        {
            return new
            {
                Idle = Convert.ToInt32(data[0], 16),
                Point1 = Convert.ToInt32(data[1], 16),
                Point2 = Convert.ToInt32(data[2], 16),
                Point3 = Convert.ToInt32(data[3], 16),
                Point4 = Convert.ToInt32(data[4], 16)
            };
        }

        /*private static double AuxilaryIOSupported(string[] data)
        {

        }*/

        private static int IntegerRange(string[] data, bool second = false)
        {
            string byteOne = second ? data[2] : data[0];
            string byteTwo = second ? data[3] : data[1];

            return Convert.ToInt32(byteOne, 16) * 256 + Convert.ToInt32(byteTwo, 16);
        }

        private static double Percentage(string[] data)
        {
            return (double)Convert.ToInt32(data[0], 16) * 100.0 / 255.0;
        }
    }
}
