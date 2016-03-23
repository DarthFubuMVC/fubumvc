using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using FubuCore;
using FubuMVC.Core.Security.Authentication.Saml2.Xml;

namespace FubuMVC.Core.Security.Authentication.Saml2.Encryption
{
    public class AssertionXmlDecryptor : ReadsSamlXml, IAssertionXmlDecryptor
    {
        public void Decrypt(XmlDocument document, X509Certificate2 encryptionCert)
        {
            var assertion = document.FindChild(EncryptedAssertion);
            if (assertion == null) return; // Not encrypted, shame on them.

            var data = document.EncryptedChild("EncryptedData");
            var keyElement = assertion.EncryptedChild("EncryptedKey");

            var encryptedData = new EncryptedData();
            encryptedData.LoadXml(data);

            var encryptedKey = new EncryptedKey();
            encryptedKey.LoadXml(keyElement);

            var encryptedXml = new EncryptedXml(document);

            // Get encryption secret key used by decrypting with the encryption certificate's private key
            var secretKey = GetSecretKey(encryptedKey, encryptionCert.PrivateKey);

            // Seed the decryption algorithm with secret key and then decrypt
            var algorithm = GetSymmetricBlockEncryptionAlgorithm(encryptedData.EncryptionMethod.KeyAlgorithm);
            algorithm.Key = secretKey;
            var decryptedBytes = encryptedXml.DecryptData(encryptedData, algorithm);

            // Put decrypted xml elements back into the document in place of the encrypted data
            encryptedXml.ReplaceData(assertion, decryptedBytes);
        }

        public static SymmetricAlgorithm GetSymmetricBlockEncryptionAlgorithm(string algorithmUri)
        {
            switch (algorithmUri)
            {
                case EncryptedXml.XmlEncTripleDESUrl:
                    return new TripleDESCryptoServiceProvider();
                case EncryptedXml.XmlEncDESUrl:
                    return new DESCryptoServiceProvider();
                case EncryptedXml.XmlEncAES128Url:
                    return new RijndaelManaged { KeySize = 128 };
                case EncryptedXml.XmlEncAES192Url:
                    return new RijndaelManaged { KeySize = 192 };
                case EncryptedXml.XmlEncAES256Url:
                    return new RijndaelManaged();
                default:
                    throw new Exception("Unrecognized symmetric encryption algorithm URI '{0}'".ToFormat(algorithmUri));
            }
        }

        public static byte[] GetSecretKey(EncryptedKey encryptedKey, AsymmetricAlgorithm privateKey)
        {
            var keyAlgorithm = encryptedKey.EncryptionMethod.KeyAlgorithm;
            var asymmetricAlgorithm = GetAsymmetricKeyTransportAlgorithm(keyAlgorithm);
            asymmetricAlgorithm.FromXmlString(privateKey.ToXmlString(true));
            
            var useOaep = keyAlgorithm == EncryptedXml.XmlEncRSAOAEPUrl;
            return asymmetricAlgorithm.Decrypt(encryptedKey.CipherData.CipherValue, useOaep);
        }

        public static RSACryptoServiceProvider GetAsymmetricKeyTransportAlgorithm(string algorithmUri)
        {
            switch (algorithmUri)
            {
                case EncryptedXml.XmlEncRSA15Url:
                case EncryptedXml.XmlEncRSAOAEPUrl:
                    return new RSACryptoServiceProvider();
                default:
                    throw new Exception("Unrecognized asymmetric encryption algorithm URI '{0}'".ToFormat(algorithmUri));
            }
        }
    }
}