using Strados.Obd.Specification;
using Strados.Vehicle.Obd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strados.Obd.Extensions;

namespace Strados.Vehicle
{
	public class Elm327VehicleService
	{
		ObdService service;

		public void StartService()
		{
			//TODO: this is where you'd initialize the obd service
		}

		public async Task<bool> ConnectToVehicle(bool reset = true)
		{
			if (service == null)
				throw new InvalidOperationException("The OBD Service must be initialized before connecting to a car");

			//initialize the ELM327 / STN11xx variant chipset
			await resetCar();
			await turnOffHeaders();

			//store the car protocol somewhere...
			var protocol = await determineCarProtocol();

			//find out some info about the car (including PIDs it supports, VIN, etc.)
			getVehicleDetails(true);

			//default to true for now. TODO: change based on task completion
			return true;
		}

		private async Task resetCar()
		{
			//reset the ELM327 based firmware
			await service.Run(ObdPid.Elm327Initialize, (data) =>	{ return data; });

			//request the version of the chipset to confirm we're good to go
			await service.Run(ObdPid.Elm327Info, (data) => { return data; });
		}

		private async Task turnOffHeaders()
		{
			//turn off command echo
			await service.Run(string.Format(ObdPid.Elm327Echo.StringValue(), 0), (data) => { return data; });

			//turn off line feeds
			await service.Run(string.Format(ObdPid.Elm327LineFeed.StringValue(), 0), (data) => { return data; });

			//turn off headers
			await service.Run(string.Format(ObdPid.Elm327Headers.StringValue(), 0), (data) => { return data; });
		}

		private async Task<ObdProtocol> determineCarProtocol()
		{
			//try to set to automatic protocol detection
			await service.Run(string.Format(ObdPid.Elm327Protocol.StringValue(), Convert.ToString((int)ObdProtocol.Auto, 16)), (data) => { return data; });
			var received = await service.Run(ObdPid.EngineRPM) != null;

			if (!received)
			{
				//get a list of all the protocols
				var protocols = Enum.GetNames(typeof(ObdProtocol));

				//detect protocol manually by cycling through each one
				for (int i = 0; i < protocols.Length - 1; i++)
				{
					var protocol = (ObdProtocol)Enum.Parse(typeof(ObdProtocol), protocols[i]);
					await service.Run(string.Format(ObdPid.Elm327Protocol.StringValue(), Convert.ToString((int)ObdProtocol.Auto, 16)), (data) => { return data; });
					//var data = await Run (ObdCommands.ModeSupport (ObdPid.PidSupport_01_20));
					var dataRecieved = service.Run(ObdPid.EngineRPM) != null;

					if (dataRecieved)
					{
						//dummy data
						await service.Run(ObdPid.EngineRPM);
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

		private async Task getVehicleDetails(bool closeOnCompletion)
		{
			throw new NotImplementedException();
		}
	}
}
