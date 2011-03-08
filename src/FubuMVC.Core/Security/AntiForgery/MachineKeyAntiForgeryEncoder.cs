using System;
using System.Text;
using System.Web.Security;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class MachineKeyAntiForgeryEncoder : IAntiForgeryEncoder
    {
        public byte[] Decode(string value)
        {
            return MachineKey.Decode(Base64ToHex(value), MachineKeyProtection.All);
        }

        public string Encode(byte[] bytes)
        {
            return HexToBase64(MachineKey.Encode(bytes, MachineKeyProtection.All).ToUpperInvariant());
        }

        private static string Base64ToHex(string base64)
        {
            var builder = new StringBuilder(base64.Length*4);
            foreach (byte b in Convert.FromBase64String(base64))
            {
                builder.Append(HexDigit(b >> 4));
                builder.Append(HexDigit(b & 0x0F));
            }
            string result = builder.ToString();
            return result;
        }

        private static char HexDigit(int value)
        {
            return (char) (value > 9 ? value + '7' : value + '0');
        }

        private static int HexValue(char digit)
        {
            return digit > '9' ? digit - '7' : digit - '0';
        }

        private static string HexToBase64(string hex)
        {
            int size = hex.Length/2;
            var bytes = new byte[size];
            for (int idx = 0; idx < size; idx++)
            {
                bytes[idx] = (byte) ((HexValue(hex[idx*2]) << 4) + HexValue(hex[idx*2 + 1]));
            }
            string result = Convert.ToBase64String(bytes);
            return result;
        }
    }
}