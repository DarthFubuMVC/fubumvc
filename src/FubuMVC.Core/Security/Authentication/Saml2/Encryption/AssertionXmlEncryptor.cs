using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using FubuMVC.Core.Security.Authentication.Saml2.Xml;

namespace FubuMVC.Core.Security.Authentication.Saml2.Encryption
{
    public class AssertionXmlEncryptor : ReadsSamlXml, IAssertionXmlEncryptor
    {
        public void Encrypt(XmlDocument document, X509Certificate2 certificate)
        {
            var element = document.FindChild(AssertionElem);
            var encryptedXml = new EncryptedXml {Encoding = Encoding.UTF8};
            
            // TODO -- make this pluggable with settings?
            //Create the Symmetric Key for encrypting
            var key = new RijndaelManaged
            {
                KeySize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None
            };

            var encryptedData = ToEncryptedData(encryptedXml, element, key);
            var encryptedKey = ToEncryptedKey(certificate, key);

            var wrapper = document.CreateElement(EncryptedAssertion, AssertionXsd);
            wrapper.AppendChild(document.ImportNode(encryptedData.GetXml(), true));
            wrapper.AppendChild(document.ImportNode(encryptedKey.GetXml(), true));

            element.ParentNode.ReplaceChild(wrapper, element);
        }

        private static EncryptedData ToEncryptedData(EncryptedXml encryptedXml, XmlElement element, RijndaelManaged key)
        {
            var encryptedElement = encryptedXml.EncryptData(element, key, false);

            var encryptedData = new EncryptedData
            {
                Type = EncryptedXml.XmlEncElementUrl,
                EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES128Url),
                Id = null,
                CipherData = new CipherData(encryptedElement)
            };
            return encryptedData;
        }

        public static EncryptedKey ToEncryptedKey(X509Certificate2 certificate, SymmetricAlgorithm key)
        {
            var provider = new RSACryptoServiceProvider();
            provider.FromXmlString(certificate.PublicKey.Key.ToXmlString(false));
            var secretKey = provider.Encrypt(key.Key, false);
            
            var encryptedKey = new EncryptedKey
            {
                EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncRSA15Url),
                CipherData = new CipherData(secretKey)
            };

            encryptedKey.KeyInfo.AddClause(new KeyInfoName("encryption"));
            
            return encryptedKey;
        }


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
            var secretKey = AssertionXmlDecryptor.GetSecretKey(encryptedKey, encryptionCert.PrivateKey);

            // Seed the decryption algorithm with secret key and then decrypt
            var algorithm = AssertionXmlDecryptor.GetSymmetricBlockEncryptionAlgorithm(encryptedData.EncryptionMethod.KeyAlgorithm);
            algorithm.Key = secretKey;
            var decryptedBytes = encryptedXml.DecryptData(encryptedData, algorithm);

            // Put decrypted xml elements back into the document in place of the encrypted data
            encryptedXml.ReplaceData(assertion, decryptedBytes);
        }
    }
}