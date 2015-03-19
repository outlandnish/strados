using Strados.Obd.Extensions;

namespace Strados.Obd.Specification
{
	public enum ObdPid : int
	{
		[StringValue("atspa{0}")]
		Elm327AutoProtocol = -11,
		[StringValue("atz")]
		Elm327Initialize = -10,
		[StringValue("ati")]
		Elm327Info = -9,
		[StringValue("ate{0}")]
		Elm327Echo = -8,
		[StringValue("ath{0}")]
		Elm327Headers = -7,
		[StringValue("atl{0}")]
		Elm327LineFeed = -6,
		[StringValue("atsp{0}")]
		Elm327Protocol = -5,
		[StringValue("atst{0}")]
		Elm327Timeout = -4,
		[StringValue("atws")]
		Elm327WarmReset = -3,
		[StringValue("atrv")]
		Elm327ReadVoltage = -2,
		[StringValue("atign")]
		Elm327IgnMonInputLevel = -1,
		[StringValue("0100")]
		PidSupport_01_20 = 0,
		[StringValue("0101")]
		MonitorStatus = 1,
		[StringValue("0102")]
		FreezeDTC = 2,
		[StringValue("0103")]
		FuelSystemStatus = 3,
		[StringValue("0104")]
		CalcEngineLoad = 4,
		[StringValue("0105")]
		EngineCoolantTemp = 5,
		[StringValue("0106")]
		ShortTermFuelPercentTrimBankOne = 6,
		[StringValue("0107")]
		LongTermFuelPercentTrimBankOne = 7,
		[StringValue("0108")]
		ShortTermFuelPercentTrimBankTwo = 8,
		[StringValue("0109")]
		LongTermFuelPercentTrimBankTwo = 9,
		[StringValue("010A")]
		FuelPressure = 10,
		[StringValue("010B")]
		IntakeManifoldAbsolutePressure = 11,
		[StringValue("010C")]
		EngineRPM = 12,
		[StringValue("010D")]
		VehicleSpeed = 13,
		[StringValue("010E")]
		TimingAdvance = 14,
		[StringValue("010F")]
		IntakeAirTemperature = 15,
		[StringValue("0110")]
		MAFRate = 16,
		[StringValue("0111")]
		ThrottlePosition = 17,
		[StringValue("0112")]   
		CommandedSecondaryAirStatus = 18,
		[StringValue("0113")]
		OxygenSensorsPresent = 19,
		[StringValue("0114")]
		Bank1_Sensor1 = 20,
		[StringValue("0115")]
		Bank1_Sensor2 = 21,
		[StringValue("0116")]
		Bank1_Sensor3 = 22,
		[StringValue("0117")]
		Bank1_Sensor4 = 23,
		[StringValue("0118")]
		Bank2_Sensor1 = 24,
		[StringValue("0119")]
		Bank2_Sensor2 = 25,
		[StringValue("011A")]
		Bank2_Sensor3 = 26,
		[StringValue("011B")]
		Bank2_Sensor4 = 27,
		[StringValue("011C")]
		OBDStandard = 28,
		[StringValue("011D")]
		OxygenSensorsPresent_1 = 29,
		[StringValue("011E")]
		AuxilaryInputStatus = 30,
		[StringValue("011F")]
		RunTimeSinceEngineStart = 31,
		[StringValue("0120")]
		PidSupport_21_40 = 32,
		[StringValue("0121")]
		DistanceTraveledWithMILOn = 33,
		[StringValue("0122")]
		FuelRailPressure_RelativeToManifoldVacuum = 34,
		[StringValue("0123")]
		FuelRailPressure_Diesel = 35,
		[StringValue("0124")]
		O2S1_WR_Lambda_EquivRatio = 36,
		[StringValue("0125")]
		O2S2_WR_Lambda_EquivRatio = 37,
		[StringValue("0126")]
		O2S3_WR_Lambda_EquivRatio = 38,
		[StringValue("0127")]
		O2S4_WR_Lambda_EquivRatio = 39,
		[StringValue("0128")]
		O2S5_WR_Lambda_EquivRatio = 40,
		[StringValue("0129")]
		O2S6_WR_Lambda_EquivRatio = 41,
		[StringValue("012A")]
		O2S7_WR_Lambda_EquivRatio = 42,
		[StringValue("012B")]
		O2S8_WR_Lambda_EquivRatio = 43,
		[StringValue("012C")]
		CommandedEGR = 44,
		[StringValue("012D")]
		EGRError = 45,
		[StringValue("012E")]
		CommandedEvapPurge = 46,
		[StringValue("012F")]
		FuelLevelInput = 47,
		[StringValue("0130")]
		NumberWarmUpsSinceCodeCleared = 48,
		[StringValue("0131")]
		FuelLevelInput_2 = 49,
		[StringValue("0132")]
		EvaporationSystemVaporPressure = 50,
		[StringValue("0133")]
		BarometricPressure = 51,
		[StringValue("0134")]
		O2S1_WR_Lambda_EquivRatio_1 = 52,
		[StringValue("0135")]
		O2S2_WR_Lambda_EquivRatio_1 = 53,
		[StringValue("0136")]
		O2S3_WR_Lambda_EquivRatio_1 = 54,
		[StringValue("0137")]
		O2S4_WR_Lambda_EquivRatio_1 = 55,
		[StringValue("0138")]
		O2S5_WR_Lambda_EquivRatio_1 = 56,
		[StringValue("0139")]
		O2S6_WR_Lambda_EquivRatio_1 = 57,
		[StringValue("013A")]
		O2S7_WR_Lambda_EquivRatio_1 = 58,
		[StringValue("013B")]
		O2S8_WR_Lambda_EquivRatio_1 = 59,
		[StringValue("013C")]
		CatalystTemperature_Bank1_Sensor1 = 60,
		[StringValue("013D")]
		CatalystTemperature_Bank2_Sensor1 = 61,
		[StringValue("013E")]
		CatalystTemperature_Bank1_Sensor2 = 62,
		[StringValue("013F")]
		CatalystTemperature_Bank2_Sensor2 = 63,
		[StringValue("0140")]
		PidSupport_41_60 = 64,
		[StringValue("0141")]
		MonitorStatusThisDriveCycle = 65,
		[StringValue("0142")]
		ControlModuleVoltage = 66,
		[StringValue("0143")]
		AbsoluteLoadValue = 67,
		[StringValue("0144")]
		CommandEquivalenceRatio = 68,
		[StringValue("0145")]
		RelativeThrottlePosition = 69,
		[StringValue("0146")]
		AmbientAirTemperature = 70,
		[StringValue("0147")]
		AbsoluteThrottlePosB = 71,
		[StringValue("0148")]
		AbsoluteThrottlePosC = 72,
		[StringValue("0149")]
		AbsoluteThrottlePosD = 73,
		[StringValue("014A")]
		AbsoluteThrottlePosE = 74,
		[StringValue("014B")]
		AbsoluteThrottlePosF = 75,
		[StringValue("014C")]
		CommandedThrottleActuator = 76,
		[StringValue("014D")]
		TimeRunWithMILOn = 77,
		[StringValue("014E")]
		TimeSinceTroubleCodesCleared = 78,
		[StringValue("014F")]
		MaxEquivRatioO2SensorIMAP = 79,
		[StringValue("0150")]
		MaxMAF = 80,
		[StringValue("0151")]
		FuelRate = 81,
		[StringValue("0152")]
		EthanolFuelPercent = 82,
		[StringValue("0153")]
		AbsoluteEvapVaporPressure = 83,
		[StringValue("0154")]
		EvapVaporPressure = 84,
		[StringValue("0155")]
		STSecondaryO2SensorTrim_Bank_1_3 = 85,
		[StringValue("0156")]
		LTSecondaryO2SensorTrim_Bank_1_3 = 86,
		[StringValue("0157")]
		STSecondaryO2SensorTrim_Bank_2_4  = 87,
		[StringValue("0158")]
		LTSecondaryO2SensorTrim_Bank_2_4 = 88,
		[StringValue("0159")]
		FuelRailPressure = 89,
		[StringValue("015A")]
		RelativeAcceleratorPedalPosition = 90,
		[StringValue("015B")]
		HybridBatteryPackRemainingLife = 91,
		[StringValue("015C")]
		EngineOilTemperature = 92,
		[StringValue("015D")]
		FuelInjectionTiming = 93,
		[StringValue("015E")]
		EngineFuelRate = 94,
		[StringValue("015F")]
		EmissionRequirements = 95,
		[StringValue("0160")]
		PidSupport_61_80 = 96,
		[StringValue("0161")]
		DriverDemandEnginePercentTorque = 97,
		[StringValue("0162")]
		ActualEnginePercentTorque = 98,
		[StringValue("0163")]
		EngineReferenceTorque = 99,
		[StringValue("0164")]
		EnginePercentTorqueData = 100,
		[StringValue("0165")]
		AuxilaryIOSupported = 101,
		[StringValue("0166")]
		MassAirFlowSensor = 102,
		[StringValue("0167")]
		EngineCoolantTemperature = 103,
		[StringValue("0168")]
		IntakeAirTemperatureSensor = 104,
		[StringValue("0169")]
		CommandedEGRAndError = 105,
		[StringValue("016A")]
		CommandedDieselIntakeAirFlowControl = 106,
		[StringValue("016B")]
		FuelPressureControlSystem = 107,
		[StringValue("016E")]
		InjectionPressureControlSystem = 108,
		[StringValue("016F")]
		TurbochargerCompressorInletPressure = 109,
		[StringValue("0170")]
		BoostPressureControl = 110,
		[StringValue("0171")]
		VariableGeometryTurboControl = 111,
		[StringValue("0172")]
		WastegateControl = 112,
		[StringValue("0173")]
		TurbochargerRPM = 113,
		[StringValue("0174")]
		TurbochargerTemperature = 114,
		[StringValue("0175")]
		TurbochargerTemperature_2 = 115,
		[StringValue("0176")]
		ChargeAirCoolerTemperature = 116,
		[StringValue("0177")]
		ExhaustGasTemperatureBank_1 = 117,
		[StringValue("0178")]
		ExhaustGasTemperatureBank_2 = 118,
		[StringValue("017F")]
		EngineRunTime = 119,
		[StringValue("0180")]
		PidSupport_81_A0 = 120,
		[StringValue("0181")]
		EngineRunTimeAECD = 121,
		[StringValue("0182")]
		EngineRunTimeAECD_2 = 122,
		[StringValue("0183")]
		NOxSensor = 123,
		[StringValue("0184")]
		ManifoldSurfaceTemperature = 124,
		[StringValue("0185")]
		NOxReagentSystem = 125,
		[StringValue("0186")]
		ParticulateMatterSensor = 126,
		[StringValue("0187")]
		IntakeManifoldAbsolutePressure_2 = 127,
		[StringValue("0202")]
		FreezeFrameTroubleCodes = 128,
		[StringValue("03")]
		RequestTroubleCodes = 129,
		[StringValue("04")]
		ClearTroubleCodes = 130,
		[StringValue("07")]
		PendingTroubleCodes = 131,
		[StringValue("0A")]
		PermanentTroubleCodes = 132,
		[StringValue("0901")]
		VinMessageCount = 133,
		[StringValue("0902")]
		VINNumber = 134,
		[StringValue("0903")]
		CalibrationID = 135,
		[StringValue("0904")]
		Calibration = 136,
	}
}

