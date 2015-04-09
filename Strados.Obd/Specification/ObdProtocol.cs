using Strados.Obd.Extensions;

namespace Strados.Obd.Specification
{
    public enum ObdProtocol : int
    {
        [StringValue("Auto")]
        Auto = 0x00,
        [StringValue("SAE J1850 PWM (41.6 kbps)")]
        SAE_J1850_PWM = 0x01,
        [StringValue("SAE J1850 VPW (10.4 kbps)")]
        SAE_J1850_VPW = 0x02,
        [StringValue("ISO 9141-2 (5 baud init)")]
        ISO_9141_2 = 0x03,
        [StringValue("ISO 14230-4 KWP (5 baud init)")]
        ISO_14230_4_KWP = 0x04,
        [StringValue("ISO 14230-4 KWP (fast init)")]
        ISO_14230_4_KWP_FAST = 0x05,
        [StringValue("ISO 15765-4 CAN (11 bit ID, 500 kbps)")]
        ISO_15765_4_CAN = 0x06,
        [StringValue("ISO 15765-4 CAN (29 bit ID, 500 kbps)")]
        ISO_15765_4_CAN_B = 0x07,
        [StringValue("ISO 15765-4 CAN (11 bit ID, 250 kbps)")]
        ISO_15765_4_CAN_C = 0x08,
        [StringValue("ISO 15765-4 CAN (29 bit ID, 250 kbps)")]
        ISO_15765_4_CAN_D = 0x09,
        [StringValue("SAE J1939 CAN (29  bit ID, {0} kbps)")]
        SAE_J1939_CAN = 0x0A,
        [StringValue("User1 CAN ({0} bit ID, {1} kbps)")]
        USER1_CAN = 0x0B,
        [StringValue("User2 CAN ({0} bit ID, {1} kbps)")]
        USER2_CAN = 0x0C,
        [StringValue("No Protocol")]
        NOT_SET = -1,
    }
}

