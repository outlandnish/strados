using System.Diagnostics;
using Xunit;

namespace Strados.Obd
{
    public class ObdParserTests
    {
        [Fact]
        public void PidSupportTest()
        {
            string hex = "4100BE1FA813";
            var result = ObdParser.Parse(hex);

            Assert.Equal(1, result.mode);
            Assert.Equal(0, result.command);
        }

        [Fact]
        public void TestRPM()
        {
            string min = "410C0000";
            string max = "410CFFFE";

            var minResult = ObdParser.Parse(min);
            var maxResult = ObdParser.Parse(max);

            Assert.Equal(0.0, minResult.value);
            Assert.Equal(16383.75, maxResult.value);
        }
    }
}
