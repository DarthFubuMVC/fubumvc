using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace FubuMVC.Core.Security.Authentication.Saml2.Encryption
{
    public interface IAssertionXmlDecryptor
    {
        void Decrypt(XmlDocument document, X509Certificate2 encryptionCert);
    }
}