using Strados.Obd;
using Strados.Obd.Exceptions;
using Strados.Obd.Extensions;
using Strados.Obd.Specification;
using Strados.Vehicle.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Strados.Vehicle.Obd
{
	public class ObdService
	{
		Stream input, output;
		public ObdService(Stream inputStream, Stream outputStream)
		{
			input = inputStream;
			output = outputStream;
		}

		/// <summary>
		/// Runs an ObdPid command and returns the result parsed by ObdParser
		/// </summary>
		/// <param name="command">OBDPid to send to ECU</param>
		/// <returns>ObdParser parsed response from ECU</returns>
		public Task<ObdResult> Run(ObdPid command)
		{
			return Task.Run(async () =>
			{
				return ObdParser.Parse(await Run(command.StringValue()));
			});
		}

		/// <summary>
		/// Runs an ObdPid command and returns the result parsed by a provided parser
		/// </summary>
		/// <param name="command"></param>
		/// <param name="parser"></param>
		/// <returns></returns>
		public Task<object> Run(ObdPid command, Func<string, object> parser)
		{
			return Task.Run(async () =>
			{
				return parser.Invoke(await Run(command.StringValue()));
			});
		}

		/// <summary>
		/// Runs a string command and returns the result parsed by a provided parser
		/// </summary>
		/// <param name="command">Command to send to ECU</param>
		/// <param name="parser">Anonymous method to parse ECU response</param>
		/// <returns>ECU response parsed by provided parser</returns>
		public Task<object> Run(string command, Func<string, object> parser)
		{
			return Task.Run(async () =>
			{
				return parser.Invoke(await Run(command));
			});
		}

		/// <summary>
		/// Sends a command to the output stream and waits for a response on the input stram
		/// Expects ELM327/STN111x formatted responses
		/// </summary>
		/// <param name="command">Command to send to ECU</param>
		/// <returns>Output from ECU</returns>
		public Task<string> Run(string command)
		{
			return Task.Run(() =>
			{
				//send command to ECU
				output.WriteLine(command);
				output.Flush();

				string text = "";
				var strm = input as MemoryStream;
				var data = input.ReadByte();

				if (text.Contains("DATA") || text.Contains("SEARCHING"))
					text = "NO DATA";

				var result = input.ReadLine('>');
				input.Flush();

				if (result.Contains("NO DATA"))
					throw new ObdNoDataException();
				else if (result.Contains("?"))
					throw new ObdBadCommandException();
				else if (unhandled.Count(s => result.Contains(s)) > 0)
					throw new ObdException(result);
				else
				{
					if (result.Contains("SEARCHING..."))
						result.Replace("SEARCHING...", "");

					return result;
				}
			});
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
