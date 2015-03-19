using Strados.Obd.Specification;
using Strados.Obd.Helpers;
using Strados.Obd.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strados.Vehicle.Obd
{
    public class ObdCommands
    {
        public static ObdCommand Reset = new ObdCommand((c, o) =>
        {
            return o;
        }, ObdPid.Elm327Initialize);

        public static ObdCommand WarmReset = new ObdCommand((c, o) =>
        {
            return o;
        }, ObdPid.Elm327WarmReset);

        public static ObdCommand BatteryVoltage = new ObdCommand((c, o) =>
        {
            return o;
        }, ObdPid.Elm327ReadVoltage);

        public static ObdCommand IgnMonInputLevel = new ObdCommand((c, o) =>
        {
            return o;
        }, ObdPid.Elm327IgnMonInputLevel);

        public static ObdCommand Info = new ObdCommand((c, o) =>
        {
            return o;
        }, ObdPid.Elm327Info);

        public static ObdCommand Toggle(ObdPid pid, bool off)
        {
            int val = off ? 0 : 1;
            return new ObdCommand((c, o) =>
            {
                return o;
            }, string.Format(pid.StringValue(), val), pid.ToString());
        }

        public static ObdCommand Timeout(int timeout)
        {
            var hex = Convert.ToString(0xFF & timeout, 16);
            return new ObdCommand((c, o) =>
            {
                return o;
            }, string.Format(ObdPid.Elm327Timeout.StringValue(), hex), ObdPid.Elm327Timeout.ToString());
        }

        public static ObdCommand Protocol(ObdProtocol protocol)
        {
            return new ObdCommand((c, o) =>
            {
                return o;
            }, string.Format(ObdPid.Elm327Protocol.StringValue(), Convert.ToString((int)protocol, 16)), ObdPid.Elm327Protocol.ToString());
        }

        public static ObdCommand ModeSupport(ObdPid pid)
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

            return new ObdCommand((c, o) =>
            {
                var data = HexHelper.Sanitize(new string[] { o });

                //dictionary of pids and car support for each one
                Dictionary<ObdPid, bool> pidSupport = new Dictionary<ObdPid, bool>();
                for (int i = 0; i < data.Length; i++)
                    pidSupport.Add((ObdPid)(i + 1) + offset, data[i] == '1');

                return pidSupport;
            }, pid);
        }

        public static ObdCommand Status = new ObdCommand((c, o) =>
        {
            var data = HexHelper.Sanitize(new string[] { o });

            //get # of DTC codes using bits A0 - A6
            var dtcCount = Convert.ToInt32(data.Substring(1, 6), 2);

            //convert all bits to boolean flags for polling on board tests
            bool[] tests = new bool[data.Length];
            for (int i = 0; i < data.Length; i++)
                tests[i] = data[i] == '1';

            return new MonitorStatus() { CheckEngineLightOn = tests[7], DTCCount = dtcCount, OnBoardTests = tests };
        }, ObdPid.MonitorStatus);

        public static ObdCommand FuelSystemStatus = new ObdCommand((c, o) =>
        {
            var data = HexHelper.Sanitize(new string[] { o });

            List<FuelSystemStatus> FuelSystemStatuses = new List<FuelSystemStatus>();

            try
            {
                var fs = Convert.ToInt32(data.Substring(0, 4), 2);
                FuelSystemStatuses.Add(new FuelSystemStatus(fs));
            
                var fs2 = Convert.ToInt32(data.Substring(4, 4), 2);
                FuelSystemStatuses.Add(new FuelSystemStatus(fs2));
            }
            catch (Exception err)
            { }

            return FuelSystemStatuses;
        }, ObdPid.FuelSystemStatus);

        public static ObdCommand TroubleCodes(int codes)
        {
            return new ObdCommand((c, o) =>
            {
                HashSet<string> dtcs = new HashSet<string>();
                if (codes > 0)
                {
                    //sanitize the output
                    var lines = o.Split('\r');
                    var singleLine = "";

                    //convert multiline into single line
                    foreach (var line in lines)
                        singleLine += line.Replace("\r", " ");

                    //convert to hex string
                    singleLine = singleLine.Replace(" ", "").Substring(2);
                    singleLine = HexHelper.HexStringToBinary(singleLine);

                    //add to hash set
                    for (int i = 0; i < codes * 6; i += 16)
                        dtcs.Add(DiagnosticTroubleCode.Parse(singleLine.Substring(i, 16)));
                }
                return dtcs.ToList();
            }, ObdPid.RequestTroubleCodes);
        }

        public static ObdCommand PendingTroubleCodes()
        {
            return new ObdCommand((c, o) =>
            {
                List<string> dtcs = new List<string>();
                if (o.Length > 6)
                {
                    var lines = o.Split('\r');
                    var singleLine = "";
                    foreach (var line in lines)
                    {
                        singleLine += line.Replace("\r", " ");
                    }
                    singleLine = Convert.ToString(Convert.ToInt32(singleLine, 16), 2).Remove(0, 8);
                    for (int i = 0; i < singleLine.Length; i += 16)
                    {
                        dtcs.Add(DiagnosticTroubleCode.Parse(singleLine.Substring(i, 16)));
                    }
                }
                return dtcs;
            }, ObdPid.PendingTroubleCodes);
        }

        public static ObdCommand Temperature(ObdPid tempPid)
        {
            return new ObdCommand((c, o) =>
            {
                double temp;
                var bits = o.Substring(6).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                temp = (double)Convert.ToInt32(bits[0], 16) - 40.0;
                return temp;
            }, tempPid);
        }

        public static ObdCommand FuelBank(ObdPid bank)
        {
            return new ObdCommand((c, o) =>
            {
                double percent;
                var bits = o.Substring(6).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                percent = ((double)Convert.ToInt32(bits[0], 16) - 128.0) * (100.0 / 128.0);
                return percent;
            }, bank);
        }

        public static ObdCommand MassAirFlow()
        {
            return new ObdCommand((c, o) =>
            {
                double maf;
                var bits = o.Substring(6).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                maf = ((double)Convert.ToInt32(bits[0], 16) * 256.0 + Convert.ToInt32(bits[1], 16)) / 100.0;
                return maf;
            }, ObdPid.MAFRate);
        }

        /*public ObdCommand CalculatedEngineLoad = new ObdCommand((c, o) => {

		});*/

        public static ObdCommand VINMessageCount = new ObdCommand((c, o) =>
        {
            var lines = o.Split('\r');
            return lines;
        }, ObdPid.VinMessageCount);

		public static ObdCommand VINNumber = new ObdCommand ((c, o) => {
			var lines = o.Split('\r');
            var hex = lines[1].Substring(2).Replace(" ", "");
            hex += lines[2].Substring(2).Replace(" ", "");
            hex += lines[3].Substring(2).Replace(" ", "");

            if(hex.Contains("0201"))
                hex = hex.Substring(hex.IndexOf("0201") + 4);

            return HexHelper.HexStringToAscii(hex);
		}, ObdPid.VINNumber);

        public static ObdCommand Speed = new ObdCommand((c, o) =>
        {
            double speed;
            var bits = o.Substring(6).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            speed = Convert.ToInt32(bits[0], 16);
            return speed;
        }, ObdPid.VehicleSpeed);

        public static ObdCommand RPM = new ObdCommand((c, o) =>
        {
            double rpm;
            var bits = o.Substring(6).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            rpm = (double)((Convert.ToInt16(bits[0], 16) * 256) + Convert.ToInt32(bits[1], 16)) / 4.0;
            return rpm;
        }, ObdPid.EngineRPM);
    }
}
