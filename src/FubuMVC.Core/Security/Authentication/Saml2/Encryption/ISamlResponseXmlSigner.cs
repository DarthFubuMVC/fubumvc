using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace FubuMVC.Core.Security.Authentication.Saml2.Encryption
{
    public interface ISamlResponseXmlSigner
    {
        void ApplySignature(SamlResponse response ,X509Certificate2 certificate, XmlDocument document);
    }
}