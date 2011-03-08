using System;
using System.Security.Cryptography;
using System.Text;

namespace FubuMVC.Core.Security.AntiForgery
{
    public class AntiForgeryTokenProvider : IAntiForgeryTokenProvider
    {
        private const string AntiForgeryTokenFieldName = "__RequestVerificationToken";
        private const int TokenLength = 0x10;
        private readonly RNGCryptoServiceProvider _rngc = new RNGCryptoServiceProvider();

        public string GetTokenName()
        {
            return GetTokenName(null);
        }

        public string GetTokenName(string appPath)
        {
            if (String.IsNullOrEmpty(appPath))
            {
                return AntiForgeryTokenFieldName;
            }
            return AntiForgeryTokenFieldName + "_" + Base64EncodedPath(appPath);
        }

        public AntiForgeryData GenerateToken()
        {
            string tokenString = GenerateRandomTokenString();
            return new AntiForgeryData
            {
                Value = tokenString
            };
        }

        public string Base64EncodedPath(string appPath)
        {
            byte[] rawBytes = Encoding.UTF8.GetBytes(appPath);
            string base64String = Convert.ToBase64String(rawBytes);

            return base64String.Replace('+', '.').Replace('/', '-').Replace('=', '_');
        }

        private string GenerateRandomTokenString()
        {
            var tokenBytes = new byte[TokenLength];
            _rngc.GetBytes(tokenBytes);

            string token = Convert.ToBase64String(tokenBytes);
            return token;
        }
    }
}