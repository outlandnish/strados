using System;

namespace Strados.Obd.Exceptions
{
	public class ObdBadCommandException : Exception
	{
		public ObdBadCommandException() : base("Unknown command") { }

        public ObdBadCommandException(string message) : base(message) { }
	}
}

