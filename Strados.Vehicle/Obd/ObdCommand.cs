using Strados.Obd.Exceptions;
using Strados.Obd.Extensions;
using Strados.Obd.Specification;
using Strados.Vehicle.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Strados.Vehicle.Obd
{
    /// <summary>
    /// Provides logic for sending a string to Translator, parse the response using a
    /// given function, and notify of a result
    /// </summary>
	public class ObdCommand : NotifyPropertyChangedBase
	{
        public string Name { get; set; }        //command identifier
        public object Value;                    //value 

		Func<string, string, object> parser;    //delegate for parse function
        string command = "", output = "";

        /// <summary>
        /// Builds OBDCommand from a list of pids and function to parse response
        /// </summary>
        /// <param name="parseFunction">Delegate function used to transform ECU response</param>
        /// <param name="pids">OBD2 Parameter IDs</param>
		public ObdCommand(Func<string, string, object> parseFunction, params ObdPid[] pids){
            if (pids.Length > 6)
                throw new ObdBadCommandException("More than 6 PIDS requested");

			parser = parseFunction;
		
            //build command string from pids and create unique identifier from pid names
			foreach (ObdPid pid in pids) {
				command += pid.StringValue () + " ";
				Name += pid.ToString () + " ";
			}
			command += "\r";
		}

        /// <summary>
        /// Builds OBDCommand from a raw string and a function to parse response
        /// </summary>
        /// <param name="parseFunction">Delegate function used to transform ECU response</param>
        /// <param name="cmd">Raw command to send to ECU</param>
        /// <param name="name">Unique identifier for command</param>
		public ObdCommand(Func<string, string, object> parseFunction, string cmd, string name){
			parser = parseFunction;
			command += cmd + "\r";
			Name = name;
		}

        /// <summary>
        /// Given an input and output stream to the Translator
        /// TODO: Implement timeout for read from the Translator
        /// </summary>
        /// <param name="inputStream">Input stream from Translator</param>
        /// <param name="outputStream">Output stream to Translator</param>
		public void Run(Stream inputStream, Stream outputStream){
            //send command to ECU
			outputStream.WriteLine(command);
			outputStream.Flush();
            
            string text = "";
            var strm = inputStream as MemoryStream;
            var data = inputStream.ReadByte();   

            if (text.Contains("DATA") || text.Contains("SEARCHING"))
                text = "NO DATA";

            //recieve response from ECU
            output = inputStream.ReadLine('>');
			inputStream.Flush();

            if (output.Contains("NO DATA"))
				throw new ObdNoDataException();
            else if (output.Contains("?"))
                throw new ObdBadCommandException();
            else if(unhandled.Count(s => output.Contains(s)) > 0)
				throw new ObdException(output);
            else
            {
                try
                {
					if(output.Contains("SEARCHING..."))
						output.Replace("SEARCHING...", "");
                    Value = parser.Invoke(command, output.Replace('\r', ' '));
                    NotifyPropertyChanged("Value");
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                    Debug.WriteLine("Command: " + command);
                    Debug.WriteLine("Output: " + output);
                }
            }
		}

        public object DebugCommand(string command, string output)
        {
            return parser.Invoke(command, output);
        }

        /// <summary>
        /// List of Translator errors that we don't currently don't handle individually
        /// </summary>
        private List<string> unhandled = new List<string>()
        {
            "UNABLE TO CONNECT", "STOPPED", "LV RESET", "LP ALERT", "RX ERROR", "FB ERROR", "ERR",
            "BUS BUSY", "DATA ERROR", "BUFFER FULL", "CAN ERROR", "ACT ALERT"
        };
	}
}

