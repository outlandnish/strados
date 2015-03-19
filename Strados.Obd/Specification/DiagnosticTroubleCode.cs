using System;

namespace Strados.Obd.Specification
{
    public class DiagnosticTroubleCode
    {
        public static string Parse(string binary)
        {
            var code = "";
            code += firstCharacter(Convert.ToByte(binary.Substring(0, 2), 2));
            code += Convert.ToByte(binary.Substring(2, 2), 2);
            code += Convert.ToInt32(binary.Substring(4), 2).ToString("X");
            return code;
        }

        private static string firstCharacter(byte binary)
        {
            switch (binary)
            {
                case 0:
                    return "P";
                case 1:
                    return "C";
                case 2:
                    return "B";
                case 3:
                    return "U";
                default:
                    return "UNK";
            }
        }
    }
}
