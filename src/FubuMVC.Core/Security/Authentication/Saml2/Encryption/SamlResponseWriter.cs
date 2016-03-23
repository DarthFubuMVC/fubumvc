using System;
using System.Text;
using FubuMVC.Core.Security.Authentication.Saml2.Certificates;
using FubuMVC.Core.Security.Authentication.Saml2.Xml;

namespace FubuMVC.Core.Security.Authentication.Saml2.Encryption
{
    public interface ISamlResponseWriter
    {
        string Write(SamlResponse response);
    }

    public class SamlResponseWriter : ReadsSamlXml, ISamlResponseWriter
    {
        private readonly ICertificateService _certificates;
        private readonly ISamlResponseXmlSigner _xmlSigner;
        private readonly IAssertionXmlEncryptor _encryptor;

        public SamlResponseWriter(ICertificateService certificates, ISamlResponseXmlSigner xmlSigner, IAssertionXmlEncryptor encryptor)
        {
            _certificates = certificates;
            _xmlSigner = xmlSigner;
            _encryptor = encryptor;
        }

        public string Write(SamlResponse response)
        {
            if (response.Status == null) throw new ArgumentOutOfRangeException("SamlResponse must have a Status");

            var xml = new SamlResponseXmlWriter(response).Write();
            var certificate = _certificates.LoadCertificate(response.Issuer);

            _xmlSigner.ApplySignature(response, certificate, xml);
            _encryptor.Encrypt(xml, certificate);

            var rawXml = xml.OuterXml;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(rawXml));
        }
    
    }

    // TODO -- need some tests around this
}