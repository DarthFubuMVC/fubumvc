using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FubuMVC.Core.Security.Authentication.Tickets
{
    public class EncryptionSettings
    {
        public EncryptionSettings()
        {
            Password = "something";
            
        
        }

        public string Password { get; set; }
    }

    public interface IEncryptor
    {
        string Decrypt(string cipherText);
        string Encrypt(string clearText);
    }

    public class Encryptor : IEncryptor
    {
        private readonly EncryptionSettings _settings;

        public Encryptor(EncryptionSettings settings)
        {
            _settings = settings;
        }


        private static readonly byte[] _salt = new byte[]{
            0x45, 0xF1, 0x61, 0x6e, 0x20, 0x00, 0x65, 0x64, 0x76, 0x65, 0x64, 0x03,
            0x76
        };

        public string Decrypt(string cipherText)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);
            var pdb = new Rfc2898DeriveBytes(_settings.Password, _salt);
            var decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
            return Encoding.Unicode.GetString(decryptedData);
        }

        public static byte[] Decrypt(byte[] cipherData, byte[] key, byte[] iv)
        {
            var stream = new MemoryStream();
            CryptoStream cryptoStream = null;
            try
            {
                var rijndael = Rijndael.Create();
                rijndael.Key = key;
                rijndael.IV = iv;
                cryptoStream = new CryptoStream(stream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
                cryptoStream.Write(cipherData, 0, cipherData.Length);
                cryptoStream.FlushFinalBlock();
                return stream.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            {
                if (cryptoStream != null) cryptoStream.Close();
            }
        }

        public string Encrypt(string clearText)
        {
            var clearBytes = Encoding.Unicode.GetBytes(clearText);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(_settings.Password, _salt);
            var encryptedData = encrypt(clearBytes, rfc2898DeriveBytes.GetBytes(32), rfc2898DeriveBytes.GetBytes(16));
            return Convert.ToBase64String(encryptedData);
        }


        private static byte[] encrypt(byte[] clearData, byte[] key, byte[] iv)
        {
            var ms = new MemoryStream();
            CryptoStream cs = null;
            try
            {
                var alg = Rijndael.Create();
                alg.Key = key;
                alg.IV = iv;
                cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(clearData, 0, clearData.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            {
                cs.Close();
            }
        }
    }
}