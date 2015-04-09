using System;

namespace Strados.Obd.Exceptions
{
    public class ObdException : Exception
    {
        public ObdException(string message) : base(message) { }
    }
}

