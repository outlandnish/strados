using Strados.Obd.Helpers;

namespace Strados.Obd.Specification
{
    public enum SecondaryAirStatus : int
    {
        Upstream = 1,
        Downstream = 2,
        OutsideAtmosphereOrOff = 4,
        PumpCommandedOnDiagnostics = 8
    }
}
