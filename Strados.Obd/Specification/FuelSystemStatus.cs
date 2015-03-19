using System;

namespace Strados.Obd.Specification
{
    public class FuelSystemStatus
    {
        public bool Open { get; set; }
        public string Status { get; set; }

        public FuelSystemStatus(int value)
        {
            switch (value)
            {
                case 1:
                    Open = true;
                    Status = "Insufficient engine temperature";
                    break;
                case 2:
                    Open = false;
                    Status = "Fuel mix by O2 sensor";
                    break;
                case 4:
                    Open = true;
                    Status = "Engine load/deceleration";
                    break;
                case 8:
                    Open = true;
                    Status = "System failure";
                    break;
                case 15:
                    Open = false;
                    Status = "Fault in feedback system";
                    break;
                default:
                    throw new Exception("Invalid fuel system status");
            }
        }
    }
}

