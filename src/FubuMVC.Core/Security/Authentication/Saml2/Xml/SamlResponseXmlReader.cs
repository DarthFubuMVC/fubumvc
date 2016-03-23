using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using FubuMVC.Core.Security.Authentication.Saml2.Certificates;

namespace FubuMVC.Core.Security.Authentication.Saml2.Xml
{
    public class SamlResponseXmlReader : ReadsSamlXml
    {
        private readonly XmlDocument _document;
        private readonly XmlElement _root;

        public SamlResponseXmlReader(string xml)
        {
            _document = new XmlDocument();
            _document.LoadXml(xml);
            _root = _document.DocumentElement;
        }

        public SamlResponseXmlReader(XmlDocument document)
        {
            _document = document;
            _root = _document.DocumentElement;
        }

        private XmlElement find(string elementName, string xsd)
        {
            var elements = _document.GetElementsByTagName(elementName, xsd);
            return (XmlElement) (elements.Count > 0 ? elements[0] : null);
        }

        private string findText(string elementName, string xsd)
        {
            var element = find(elementName, xsd);
            return element == null ? null : element.InnerText;
        }

        // This assumes signing cert is embedded in the signature
        private void readSignaturesAndCertificates(SamlResponse response)
        {
            var element = find(Signature, "http://www.w3.org/2000/09/xmldsig#");
            if (element == null)
            {
                response.Signed = SignatureStatus.NotSigned;
                return;
            }

            var signedXml = new SignedXml(_document);
            signedXml.LoadXml(element);

            response.Signed = signedXml.CheckSignature()
                                  ? SignatureStatus.Signed
                                  : SignatureStatus.InvalidSignature;

            response.Certificates = signedXml
                .KeyInfo.OfType<KeyInfoX509Data>()
                .SelectMany(x => {
                    return x.Certificates.OfType<X509Certificate2>()
                    .Select(cert => new X509CertificateWrapper(cert));
                });
        }

        public string ReadIssuer()
        {
            return findText(Issuer, AssertionXsd);
        }

        public SamlResponse Read()
        {
            var response = new SamlResponse
            {
                Id = _root.GetAttribute(ID),
                Destination = _root.GetAttribute(Destination).ToUri(),
                IssueInstant = _root.ReadAttribute<DateTimeOffset>(IssueInstant),
                Issuer = ReadIssuer(),
                Status = readStatusCode(),
                Conditions = new ConditionGroup(find(ConditionsElem, AssertionXsd)),
                Subject = new Subject(find(Subject, AssertionXsd)),
                Authentication = new AuthenticationStatement(_document)
            };

            readSignaturesAndCertificates(response);
            readAttributes(response);

            return response;
        }

        // TODO -- test payload w/o attributes
        private void readAttributes(SamlResponse response)
        {
            XmlElement attributes = find(AttributeStatement, AssertionXsd);
            if (attributes == null) return;

            foreach (XmlElement attElement in attributes.GetElementsByTagName(Attribute, AssertionXsd))
            {
                string key = attElement.GetAttribute(NameAtt);
                foreach (XmlElement valueElement in attElement.GetElementsByTagName(AttributeValue, AssertionXsd))
                {
                    response.AddAttribute(key, valueElement.InnerText);
                }
            }
        }


        private SamlStatus readStatusCode()
        {
            return find(StatusCode, ProtocolXsd).GetAttribute(ValueAtt).ToSamlStatus();
        }
    }
}