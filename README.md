# Strados [![Build Status](https://travis-ci.org/nsamala/strados.svg?branch=master)](https://travis-ci.org/nsamala/strados)
Core library in Strados that transforms car data (On-Board Diagnostics 2) into human readable data.

## The Details
### Strados OBD Library
Contains the code that parses OBD2 based on a given OBD2 PID. A list of OBD2 PIDS can be found at [Wikipedia](http://en.wikipedia.org/wiki/OBD-II_PIDs). 

#### Usage
Using the Strados OBD library is super simple. The ObdParser class provides a static function for passing hex data from an OBD2 sensor. Based off of the mode and command of the hex string (first two bytes), the library uses reflection to find an appropriate transformation to get the value.

```C#
using Strados.Obd;

string data = "41 0D FF"; 			//hex data from OBD2
var result = ObdParser.Parse(data);

Console.WriteLine(result.Mode);		//mode: 1
Console.WriteLine(result.Command);  //command: 13
Console.WriteLine(result.Name);		//name: VehicleSpeed
Console.WriteLine(result.Value);	//value: 255 (kph)
```

If a PID is not currently supported, you can write an extension method for the ObdParser class. The name of the method should match an enum value in ObdPid. 

#### Installation (from NuGet)
`Install-Package Strados.Obd`

### Strados Vehicle Library
The Vehicle Library provides a service interface `IVehicleService` to interact with an OBD2 reader. An abstract implementation `Elm327VehicleServiceBase` for the ELM327 (and it's variants like the STN11xx chipsets) is provided. You'll need to implement a child class on your platform to interact with the vehicle service.

#### Usage
[TODO]

#### Installation (from NuGet)
`Install-Package Strados.Vehicle`