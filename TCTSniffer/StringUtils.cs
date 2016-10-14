using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCTSniffer
{
    public static class StringUtils
    {
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length / 2;
            byte[] bytes = new byte[NumberChars];
            using (var sr = new StringReader(hex))
            {
                for (int i = 0; i < NumberChars; i++)
                    bytes[i] =
                      Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            return bytes;
        }

        public static int Hex4BStringToInt(string hex)
        {
            var sb = new StringBuilder();
            for (int i = 8 - 2; i >= 0; i -= 2)
            {
                sb.Append(hex[i]);
                sb.Append(hex[i + 1]);
            }
            var result = Convert.ToInt32(sb.ToString(), 16);
            return result;
        }
        public static int Hex2BStringToInt(string hex)
        {
            var sb = new StringBuilder();
            for (int i = 4 - 2; i >= 0; i -= 2)
            {
                sb.Append(hex[i]);
                sb.Append(hex[i + 1]);
            }
            var result = Convert.ToInt32(sb.ToString(), 16);
            return result;
        }

        public static int GetStringEnd(string s, int startIndex, string terminator)
        {
            int endIndex = startIndex;
            bool zeroes = false;
            while (!zeroes)
            {
                string test = s[endIndex + 0].ToString() +
                              s[endIndex + 1].ToString() +
                              s[endIndex + 2].ToString() +
                              s[endIndex + 3].ToString();

                if (test == terminator)
                {
                    zeroes = true;
                }
                else
                {
                    zeroes = false;
                    endIndex += 4;
                }
            }
            return endIndex;
        }

        public static string GetStringFromHex(string hex, int startIndex, string terminator)
        {
            StringBuilder b = new StringBuilder();
            for (int i = startIndex; i < GetStringEnd(hex, startIndex, terminator); i += 2)
            {
                b.Append(hex[i].ToString() + hex[i + 1].ToString());
            }
            b.Replace("00", "");
            var B = StringToByteArray(b.ToString());
            return Encoding.UTF7.GetString(B);
        }

    }
}
