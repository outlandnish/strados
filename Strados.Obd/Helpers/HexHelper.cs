using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strados.Obd.Helpers
{
    public class HexHelper
    {
        private static readonly Dictionary<char, string> hexCharacterToBinary = new Dictionary<char, string> {
            { '0', "0000" },
            { '1', "0001" },
            { '2', "0010" },
            { '3', "0011" },
            { '4', "0100" },
            { '5', "0101" },
            { '6', "0110" },
            { '7', "0111" },
            { '8', "1000" },
            { '9', "1001" },
            { 'a', "1010" },
            { 'b', "1011" },
            { 'c', "1100" },
            { 'd', "1101" },
            { 'e', "1110" },
            { 'f', "1111" }
        };

        public static string HexStringToBinary(string hex)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in hex)
                result.Append(hexCharacterToBinary[char.ToLower(c)]);

            return result.ToString();
        }

        public static string HexStringToAscii(string hex)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hex.Length; i += 2)
            {
                var hs = hex.Substring(i, 2);
                result.Append(Convert.ToChar(Convert.ToInt32(hs, 16)));
            }
            return result.ToString();
        }

        public static string Sanitize(string data)
        {
            data = data.Replace(" ", "").Substring(4).Replace("\r", "");
            return HexHelper.HexStringToBinary(data);
        }
    }
}
