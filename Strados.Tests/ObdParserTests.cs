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
        public void TestRPM()
        {
            string min = "410C0000";
            string max = "410CFFFF";

            var minResult = ObdParser.Parse(min);
            var maxResult = ObdParser.Parse(max);

            Assert.Equal(0.0, (double)minResult.Value);
            Assert.Equal(16383.75, (double)maxResult.Value);
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
