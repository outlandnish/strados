using Strados.Obd.Extensions;

namespace Strados.Obd.Specification
{
	public enum ObdStandard
	{
		[StringValue("OBD-II (CARB)")]
		OBD2_CARB = 1,
		[StringValue("OBD-II (EPA)")]
		OBD_EPA = 2,
		[StringValue("OBD-I and OBD-II")]
		OBD_OBD2 = 3,
		[StringValue("OBD-I")]
		OBD = 4,
		[StringValue("Not OBD compliant")]
		NOT_OBD_COMPLIANT = 5,
		[StringValue("EOBD (Europe)")]
		EOBD = 6,
		[StringValue("EOBD and OBD-II")]
		EOBD_OBD2 = 7,
		[StringValue("EOBD and OBD")]
		EOBD_OBD = 8,
		[StringValue("EOBD, OBD and OBD II")]
		EOBD_OBD_OBD2 = 9,
		[StringValue("JOBD (Japan)")]
		JOBD = 10,
		[StringValue("JOBD and OBD II")]
		JOBD_OBD2 = 11,
		[StringValue("JOBD and EOBD")]
		JOBD_EOBD = 12,
		[StringValue("JOBD, EOBD, and OBD II")]
		JOBD_EOBD_OBD2 = 13,
		[StringValue("Reserved")]
		Reserved = 14,
		[StringValue("Reserved")]
		Reserved_1 = 15,
		[StringValue("Reserved")]
		Reserved_2 = 16,
		[StringValue("Engine Manufacturer Diagnostics (EMD)")]
		EMD = 17,
		[StringValue("Engine Manufacturer Diagnostics Enhanced (EMD+)")]
		EMD_Enhanced = 18,
		[StringValue("Heavy Duty OBD (Child/Partial) (HD OBD-C)")]
		HD_OBD_C = 19,
		[StringValue("Heavy Duty OBD (HD OBD)")]
		HD_OBD = 20,
		[StringValue("World Wide Harmonized OBD (WWH OBD)")]
		WWH_OBD = 21,
		[StringValue("Reserved")]
		Reserved_3 = 22,
		[StringValue("Heavy Duty Euro OBD Stage I without NOx control (HD EOBD-I)")]
		HD_EOBD_Stage1 = 23,
		[StringValue("Heavy Duty Euro OBD Stage I with NOx control (HD EOBD-I N)")]
		HD_EOBD_Stage1_Nox = 24,
		[StringValue("Heavy Duty Euro OBD Stage II without NOx control (HD EOBD-II)")]
		HD_EOBD_Stage2 = 25,
		[StringValue("Heavy Duty Euro OBD Stage II with NOx control (HD EOBD-II N)")]
		HD_EOBD_Stage2_Nox = 26,
		[StringValue("Reserved")]
		Reserved_4 = 27,
		[StringValue("Brazil OBD Phase 1 (OBDBr-1)")]
		OBDBr_Phase1 = 28,
		[StringValue("Brazil OBD Phase 2 (OBDBr-2)")]
		OBDBr_Phase2 = 29,
		[StringValue("Korean OBD (KOBD)")]
		KOBD = 30,
		[StringValue("India OBD I (IOBD I)")]
		IOBD = 31,
		[StringValue("India OBD II (IOBD II)")]
		IOBD2 = 32,
		[StringValue("Heavy Duty Euro OBD Stage VI (HD EOBD-IV)")]
		HD_EOBD_Stage4 = 33,
	}
}

