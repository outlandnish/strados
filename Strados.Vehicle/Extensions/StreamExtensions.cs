using System;
using System.IO;
using System.Text;

namespace Strados.Vehicle.Extensions
{
    public static class StreamExtensions
    {
        static TimeSpan REQUEST_TIMEOUT = TimeSpan.FromMilliseconds(200);

        public static void WriteLine(this Stream stream, string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text.ToCharArray());
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        public static string ReadLine(this Stream stream, char terminator)
        {
            string text = "";
            var data = stream.ReadByte();
            while (data != -1 && (char)data != terminator)
            {
                text += (char)data;
                data = stream.ReadByte();
            }
            stream.Flush();

            if (text.Contains("DATA") || text.Contains("SEARCHING"))
                text = "NO DATA";
            return text;
        }
    }
}

