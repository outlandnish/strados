using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strados.Obd
{
    public class ObdResult
    {
        public int Mode { get; set; }
        public int Command { get; set; }
        public object Value { get; set; }
        public string Name { get; set; }
    }
}
