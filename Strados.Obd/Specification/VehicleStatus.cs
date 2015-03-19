using System;
using Strados.Obd.Specification;

namespace Strados.Obd.Specification
{
	public class VehicleStatus
	{
		public bool CheckEngineLightOn { get; set; }
		public int DTCCount { get; set; }
		public bool[] OnBoardTests { get; set; }

		public bool Test(OnBoardTest test, out bool incomplete){
			if ((int)test <= 8)
				incomplete = OnBoardTests [(int)test + 4];
			else
				incomplete = false;
			return OnBoardTests [(int)test];
		}
	}
}

