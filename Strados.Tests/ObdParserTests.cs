using Strados.Obd;
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
        }

        [Fact]
        public void TestMonitorStatus()
        {
            
        }

        [Fact]
        public void TestFuelSystemStatus()
        {

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

            Assert.Equal(0, (int)minResult.Value);
            Assert.Equal(255, (int)maxResult.Value);
        }
    }
}
