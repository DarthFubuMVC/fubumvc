using System;
using System.Text;
using System.Xml;
using FubuMVC.Core.Security.Authentication.Saml2.Certificates;
using FubuMVC.Core.Security.Authentication.Saml2.Xml;

namespace FubuMVC.Core.Security.Authentication.Saml2.Encryption
{
    public interface ISamlResponseReader
    {
        SamlResponse Read(string responseText);
    }

    public class SamlResponseReader : ReadsSamlXml, ISamlResponseReader
    {
        private readonly ICertificateService _certificates;
        private readonly IAssertionXmlDecryptor _decryptor;

        public SamlResponseReader(ICertificateService certificates, IAssertionXmlDecryptor decryptor)
        {
            _certificates = certificates;
            _decryptor = decryptor;
        }

        public SamlResponse Read(string responseText)
        {
            var bytes = Convert.FromBase64String(responseText);
            var xml = Encoding.UTF8.GetString(bytes);
            var document = new XmlDocument();
            document.LoadXml(xml);

            var reader = new SamlResponseXmlReader(document);
            var certificate = _certificates.LoadCertificate(reader.ReadIssuer());

            _decryptor.Decrypt(document, certificate);

            return reader.Read();
        }
    }
}