using System;

namespace Strados.Obd.Exceptions
{
    public class ObdNoDataException : Exception
    {
        public ObdNoDataException() : base("No Data") { }
    }
}

