# Strados
Core library in Strados that transforms car data (On-Board Diagnostics 2) into human readable data.

## Usage
Using the Translator library is super simple. The ObdParser class provides a static function for passing hex data from an OBD2 sensor. Based off of the mode and command of the hex string (first two bytes), the library uses reflection to find an appropriate transformation to get the value.

```C#
using Strados.Obd;

string data = "41 OD FF"; 			//hex data from OBD2
var result = ObdParser.Parse(data);

Console.WriteLine(result.Mode);		//mode: 1
Console.WriteLine(result.Command);  //command: 13 (speed)
Console.WriteLine(result.Value);	//value: 255 (kph)
```

A list of OBD2 PIDS can be found at [Wikipedia](http://en.wikipedia.org/wiki/OBD-II_PIDs). If a PID is not currently supported, you can write an extension method for the ObdParser class. The name of the method should match an enum value in ObdPid. 