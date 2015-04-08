using Strados.Obd;
using Strados.Obd.Exceptions;
using Strados.Obd.Extensions;
using Strados.Obd.Specification;
using Strados.Vehicle.Extensions;
using Strados.Vehicle.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Strados.Vehicle
{
	public abstract class Elm327VehicleServiceBase : IVehicleService
	{
		Stream input, output;
		public Elm327VehicleServiceBase(Stream inputStream, Stream outputStream)
		{
			input = inputStream;
			output = outputStream;
		}

		/// <summary>
		/// List of Translator errors that we don't currently don't handle individually
		/// </summary>
		private List<string> unhandled = new List<string>()
		{
			"UNABLE TO CONNECT", "STOPPED", "LV RESET", "LP ALERT", "RX ERROR", "FB ERROR", "ERR",
			"BUS BUSY", "DATA ERROR", "BUFFER FULL", "CAN ERROR", "ACT ALERT"
		};

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

				var strm = input as MemoryStream;
				var data = input.ReadByte();

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

		public async Task<VehicleStatus> ConnectToVehicle(bool reset = true, bool firstTime = false)
		{
			if (input == null || output == null)
				throw new InvalidOperationException("The OBD Service must be initialized before connecting to a car");

			//initialize the ELM327 / STN11xx variant chipset
			if (reset)
				await resetChipset();
			await turnOffHeaders();

			//store the car protocol somewhere...
			var protocol = await determineCarProtocol();

			//find out some info about the car (including PIDs it supports, VIN, etc.)
			return await GetVehicleDetails(firstTime);
		}

		private async Task resetChipset()
		{
			//reset the ELM327 based firmware
			await Run(ObdPid.Elm327Initialize, (data) =>	{ return data; });

			//request the version of the chipset to confirm we're good to go
			await Run(ObdPid.Elm327Info, (data) => { return data; });
		}

		private async Task turnOffHeaders()
		{
			//turn off command echo
			await Run(string.Format(ObdPid.Elm327Echo.StringValue(), 0), (data) => { return data; });

			//turn off line feeds
			await Run(string.Format(ObdPid.Elm327LineFeed.StringValue(), 0), (data) => { return data; });

			//turn off headers
			await Run(string.Format(ObdPid.Elm327Headers.StringValue(), 0), (data) => { return data; });
		}

		private async Task<ObdProtocol> determineCarProtocol()
		{
			//try to set to automatic protocol detection
			await Run(string.Format(ObdPid.Elm327Protocol.StringValue(), Convert.ToString((int)ObdProtocol.Auto, 16)), (data) => { return data; });
			var received = await Run(ObdPid.EngineRPM) != null;

			if (!received)
			{
				//get a list of all the protocols
				var protocols = Enum.GetNames(typeof(ObdProtocol));

				//detect protocol manually by cycling through each one
				for (int i = 0; i < protocols.Length - 1; i++)
				{
					var protocol = (ObdProtocol)Enum.Parse(typeof(ObdProtocol), protocols[i]);
					await Run(string.Format(ObdPid.Elm327Protocol.StringValue(), Convert.ToString((int)ObdProtocol.Auto, 16)), (data) => { return data; });
					//var data = await Run (ObdCommands.ModeSupport (ObdPid.PidSupport_01_20));
					var dataRecieved = Run(ObdPid.EngineRPM) != null;

					if (dataRecieved)
					{
						//dummy data
						await Run(ObdPid.EngineRPM);
						//store car protocol
						return protocol;
					}
				}

				//we shouldn't ever get here. if we did, none of the protocols worked
				throw new Exception("Unable to determine vehicle protocol");
			}
			else
				return ObdProtocol.Auto;
        }

		/// <summary>
		/// Get PIDs supported by vehicle, current monitor status, and other
		/// current vehicle information
		/// </summary>
		/// <returns>Vehicle Status with monitor, fuel systems, and pids</returns>
		public async Task<VehicleStatus> GetVehicleDetails(bool checkPidSupport = false)
		{
			List<ObdPid> pids = null;
			if (checkPidSupport)
			{
				//get the PIDs to which the car responds
				var reported = await GetReportedPids();
				var actual = await GetSupportedPids();
				pids = reported.Union(actual).ToList();
			}

			//get the current monitor status (Check Engine Light, On Board Tests, Trouble Codes)
			MonitorStatus status;
			if (pids.Contains(ObdPid.MonitorStatus))
				status = (await Run(ObdPid.MonitorStatus)).Value as MonitorStatus;
			else
				throw new NotSupportedException("Car doesn't support monitor status requests");

			//get the current state of the fuel systems (Open/Closed, Status)
			List<FuelSystemStatus> fuelSystems;
			if (pids.Contains(ObdPid.FuelSystemStatus))
				fuelSystems = (await Run(ObdPid.FuelSystemStatus)).Value as List<FuelSystemStatus>;
			else
				throw new NotSupportedException("Car doesn't support fuel system status requests");

			return new VehicleStatus() { Status = status, FuelSystems = fuelSystems, Pids = pids };
		}

		/// <summary>
		/// Polls the vehicle's ECU for the PIDs it "officially" supports. 
		/// Vehicles tend to lie and support some of these and other hidden ones as well
		/// </summary>
		/// <returns>List of OBDPids the car reports that it supports</returns>
		public async Task<List<ObdPid>> GetReportedPids()
		{
			List<ObdPid> supported = new List<ObdPid>();

			supported.AddRange(await getReportedPids(ObdPid.PidSupport_01_20));

			if (supported.Contains(ObdPid.PidSupport_21_40))
				supported.AddRange(await getReportedPids(ObdPid.PidSupport_21_40));

			if (supported.Contains(ObdPid.PidSupport_41_60))
				supported.AddRange(await getReportedPids(ObdPid.PidSupport_41_60));

			if (supported.Contains(ObdPid.PidSupport_61_80))
				supported.AddRange(await getReportedPids(ObdPid.PidSupport_61_80));

			if (supported.Contains(ObdPid.PidSupport_81_A0))
				supported.AddRange(await getReportedPids(ObdPid.PidSupport_81_A0));

			return supported.ToList();
		}

		private async Task<List<ObdPid>> getReportedPids(ObdPid rangePid)
		{
			if (!rangePid.ToString().Contains("PidSupport"))
				throw new ArgumentOutOfRangeException("Not a valid Pid Support OBDPid");

			var reported = (await Run(rangePid)).Value as Dictionary<ObdPid, bool>;

			if (reported == null)
				return new List<ObdPid>();
			else
				return reported.Keys.Where(k => reported[k]).ToList();
		}

		/// <summary>
		/// Polls the vehicle's ECU against all PIDs to see which ones it responds to.
		/// Slower method, but ensures correctness for vehicles that don't properly conform
		/// to On Board Diagnostics 2
		/// </summary>
		/// <returns></returns>
		public async Task<List<ObdPid>> GetSupportedPids()
		{
			List<ObdPid> supported = new List<ObdPid>();

			foreach (var pid in Enum.GetNames(typeof(ObdPid)))
			{
				var p = (ObdPid)Enum.Parse(typeof(ObdPid), pid);
				if (p.StringValue().StartsWith("01"))
				{
					var result = await Run(p, (data) => { return data; }) as string;
					if (result != null && !result.Contains("NO DATA"))
						supported.Add(p);
				}
			}

			return supported.ToList();
		}
	}
}
