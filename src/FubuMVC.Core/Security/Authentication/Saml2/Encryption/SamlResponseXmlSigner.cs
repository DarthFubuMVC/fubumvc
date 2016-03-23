using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using FubuMVC.Core.Security.Authentication.Saml2.Xml;

namespace FubuMVC.Core.Security.Authentication.Saml2.Encryption
{
    public class SamlResponseXmlSigner : ReadsSamlXml, ISamlResponseXmlSigner
    {
        public void ApplySignature(SamlResponse response, X509Certificate2 certificate, XmlDocument document)
        {
            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(certificate));

            var signedXml = new SignedXml(document)
            {
                SigningKey = certificate.PrivateKey,
                KeyInfo = keyInfo
            };

            var reference = new Reference(AssertionIdPrefix + response.Id);
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(reference);
            signedXml.ComputeSignature();

            var xml = signedXml.GetXml();

            document.FindChild(AssertionElem).AppendChild(xml);
        }
    }
}