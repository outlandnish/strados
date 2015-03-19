using Strados.Obd;
using Strados.Obd.Specification;
using System;
using System.Collections.Generic;
using Xunit;

namespace Strados.Tests
{
    public class ObdParserTests
    {
        [Fact]
        public void PidSupportTest()
        {
            string hex = "4100BE1FA813";
            var result = ObdParser.Parse(hex);

            Assert.Equal(1, result.Mode);
            Assert.Equal(0, result.Command);

            Dictionary<ObdPid, bool> supported = new Dictionary<ObdPid, bool>()
            {
                { ObdPid.MonitorStatus,                     true },
                { ObdPid.FreezeDTC,                         false },
                { ObdPid.FuelSystemStatus,                  true },
                { ObdPid.CalcEngineLoad,                    true },
                { ObdPid.EngineCoolantTemp,                 true },
                { ObdPid.ShortTermFuelPercentTrimBankOne,   true },
                { ObdPid.LongTermFuelPercentTrimBankOne,    true },
                { ObdPid.ShortTermFuelPercentTrimBankTwo,   false },
                { ObdPid.LongTermFuelPercentTrimBankTwo,    false },
                { ObdPid.FuelPressure,                      false },
                { ObdPid.IntakeManifoldAbsolutePressure,    false },
                { ObdPid.EngineRPM,                         true },
                { ObdPid.VehicleSpeed,                      true },
                { ObdPid.TimingAdvance,                     true },
                { ObdPid.IntakeAirTemperature,              true },
                { ObdPid.MAFRate,                           true },
                { ObdPid.ThrottlePosition,                  true },
                { ObdPid.CommandedSecondaryAirStatus,       false },
                { ObdPid.OxygenSensorsPresent,              true },
                { ObdPid.Bank1_Sensor1,                     false },
                { ObdPid.Bank1_Sensor2,                     true },
                { ObdPid.Bank1_Sensor3,                     false },
                { ObdPid.Bank1_Sensor4,                     false },
                { ObdPid.Bank2_Sensor1,                     false },
                { ObdPid.Bank2_Sensor2,                     false },
                { ObdPid.Bank2_Sensor3,                     false },
                { ObdPid.Bank2_Sensor4,                     false },
                { ObdPid.OBDStandard,                       true },
                { ObdPid.OxygenSensorsPresent_1,            false },
                { ObdPid.AuxilaryInputStatus,               false },
                { ObdPid.RunTimeSinceEngineStart,           true },
                { ObdPid.PidSupport_21_40,                  true }
            };

            Assert.Equal(supported, result.Value);
        }

        [Fact]
        public void TestMonitorStatus()
        {
            
        }

        [Fact]
        public void TestFuelSystemStatus()
        {
            string first = "41031F";

            var firstExpected = new FuelSystemStatus(1);
            var secondExpected = new FuelSystemStatus(15);
            var result = ObdParser.Parse(first).Value as List<FuelSystemStatus>;

            Assert.Equal(2, result.Count);

            Assert.Equal(firstExpected.Open, result[0].Open);
            Assert.Equal(firstExpected.Status,  result[0].Status);

            Assert.Equal(secondExpected.Open, result[1].Open);
            Assert.Equal(secondExpected.Status, result[1].Status);
        }

        [Fact]
        public void TestCalculatedEngineLoad()
        {
            string min = "410400";
            string max = "4104FF";

            var minResult = ObdParser.Parse(min);
            var maxResult = ObdParser.Parse(max);

            Assert.Equal(0.0, minResult.Value);
            Assert.Equal(100.0, maxResult.Value);
        }

        [Fact]
        public void TestEngineCoolantTemperature()
        {
            string min = "410500";
            string max = "4105FF";

            var minResult = ObdParser.Parse(min);
            var maxResult = ObdParser.Parse(max);

            Assert.Equal(-40, minResult.Value);
            Assert.Equal(215, maxResult.Value);
        }

        [Fact]
        public void TestFuelTrims()
        {
            string[] mins = { "410600", "410700", "410800", "410900" };
            string[] maxs = { "4106FF", "4107FF", "4108FF", "4109FF" };

            for (int i = 0; i < mins.Length; i++)
            {
                var minResult = ObdParser.Parse(mins[i]);
                var maxResult = ObdParser.Parse(maxs[i]);

                Assert.Equal(-100.0, minResult.Value);
                Assert.Equal(99.22, maxResult.Value);
            }
        }

        [Fact]
        public void TestFuelPressure()
        {
            string min = "410A00";
            string max = "410AFF";

            var minResult = ObdParser.Parse(min);
            var maxResult = ObdParser.Parse(max);

            Assert.Equal(0, minResult.Value);
            Assert.Equal(765, maxResult.Value);
        }

        [Fact]
        public void TestIntakeMAP()
        {
            string min = "410B00";
            string max = "410BFF";

            var minResult = ObdParser.Parse(min);
            var maxResult = ObdParser.Parse(max);

            Assert.Equal(0, minResult.Value);
            Assert.Equal(255, maxResult.Value);
        }

        [Fact]
        public void TestRPM()
        {
            string min = "410C0000";
            string max = "410CFFFF";

            var minResult = ObdParser.Parse(min);
            var maxResult = ObdParser.Parse(max);

            Assert.Equal(0.0, minResult.Value);
            Assert.Equal(16383.75, maxResult.Value);
        }

        [Fact]
        public void TestSpeed()
        {
            string min = "410D00";
            string max = "410DFF";

            var minResult = ObdParser.Parse(min);
            var maxResult = ObdParser.Parse(max);

            Assert.Equal(0, minResult.Value);
            Assert.Equal(255, maxResult.Value);
        }

        [Fact]
        public void TestTimingAdvance()
        {
            string min = "410E00";
            string max = "410EFF";

            var minResult = ObdParser.Parse(min);
            var maxResult = ObdParser.Parse(max);

            Assert.Equal(-64.0, minResult.Value);
            Assert.Equal(63.5, maxResult.Value);
        }

        [Fact]
        public void TestIntakeAirTemperature()
        {
            string min = "410F00";
            string max = "410FFF";

            var minResult = ObdParser.Parse(min);
            var maxResult = ObdParser.Parse(max);

            Assert.Equal(-40, minResult.Value);
            Assert.Equal(215, maxResult.Value);
        }

        [Fact]
        public void TestMAFRate()
        {
            string min = "41100000";
            string max = "4110FFFF";

            var minResult = ObdParser.Parse(min);
            var maxResult = ObdParser.Parse(max);

            Assert.Equal(0.0, minResult.Value);
            Assert.Equal(655.35, maxResult.Value);
        }

        [Fact]
        public void TestThrottlePosition()
        {
            string min = "411100";
            string max = "4111FF";

            var minResult = ObdParser.Parse(min);
            var maxResult = ObdParser.Parse(max);

            Assert.Equal(0.0, minResult.Value);
            Assert.Equal(100.0, maxResult.Value);
        }

        [Fact]
        public void TestCommandedSecondaryAirStatus()
        {
            string hex = "411210";

            var result = ObdParser.Parse(hex);

            Assert.Equal(SecondaryAirStatus.Upstream, result.Value);
        }

        [Fact]
        public void TestOxygenSensorsPresent()
        {
            string hex = "411310";

            var expected = new bool[] { false, false, false, true, false, false, false, false };
            var result = ObdParser.Parse(hex);

            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void TestOxySensorVoltageTrim()
        {
            string min = "41140000";
            string max = "4114FFFF";

            var minResult = ObdParser.Parse(min).Value as double[];
            var maxResult = ObdParser.Parse(max).Value as double[];

            Assert.Equal(2, minResult.Length);
            Assert.Equal(2, maxResult.Length);

            Assert.Equal(0.0, minResult[0]);
            Assert.Equal(-100.0, minResult[1]);

            Assert.Equal(1.275, maxResult[0]);
            Assert.Equal(99.2, maxResult[1]);
        }

        [Fact]
        public void TestObdStandard()
        {
            var hex = "411C01";

            var result = ObdParser.Parse(hex).Value;

            Assert.Equal(ObdStandard.OBD2_CARB, result);
        }

        [Fact]
        public void TestAuxilaryInputStatus()
        {

        }

        [Fact]
        public void TestRunTimeSinceEngineStart()
        {
            var min = "411F0000";
            var max = "411FFFFF";

            var minResult = ObdParser.Parse(min).Value;
            var maxResult = ObdParser.Parse(max).Value;

            Assert.Equal(TimeSpan.FromSeconds(0), minResult);
            Assert.Equal(TimeSpan.FromSeconds(65535), maxResult);
        }
    }
}
