using Strados.Obd.Extensions;
using Strados.Obd.Specification;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Strados.Obd
{
    public class ObdParser
    {
        private static ObdParser parser = new ObdParser();
        public static object Parse(string hexData)
        {
            var normalized = normalize(hexData);

            //extract mode + command from data string
            var mode = parseMode(normalized[0]);
            var command = parseCommand(normalized[1]);

            var pid = (ObdPid)((mode - 1) * 200 + command);

            //check if the pid value has a corresponding function
            if (!Enum.IsDefined(typeof(ObdPid), pid))
                throw new NotSupportedException(string.Format("Mode {0}, Command {1} is not supported", mode, command));

            var methods = parser.GetType().GetRuntimeMethods();
            var parseFunction = methods.Where(m => m.Name == pid.ToString()).FirstOrDefault();

            if (parseFunction != null)
            {
                try
                {
                    var result = parseFunction.Invoke(parser, new object[] { normalized.ToList().Skip(2).ToArray() });
                    return result;
                }
                catch (Exception err)
                {
                    throw new ArgumentException(string.Format("Unable to parse {0}", parseFunction.Name));
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

        public double VehicleSpeed(string[] data)
        {
            double speed;
            speed = Convert.ToInt32(data[0], 16);
            return speed;
        }

        public double EngineRPM(string[] data)
        {
            double rpm;
            rpm = (double)((Convert.ToInt16(data[0], 16) * 256) + Convert.ToInt32(data[1], 16)) / 4.0;
            return rpm;
        }
    }
}
