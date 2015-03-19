using System;

namespace Strados.Vehicle.Log
{
    public class Reading<T>
    {
        public DateTimeOffset Timestamp { get; set; }
        public T Value { get; set; }

        public Reading(T val)
        {
            Value = val;
            Timestamp = DateTimeOffset.Now;
        }
    }
}
