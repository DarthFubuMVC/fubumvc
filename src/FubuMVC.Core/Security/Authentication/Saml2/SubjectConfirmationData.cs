using System;
using System.Xml;
using FubuMVC.Core.Security.Authentication.Saml2.Xml;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public class SubjectConfirmationData : ReadsSamlXml
    {
        public SubjectConfirmationData()
        {
        }

        public SubjectConfirmationData(XmlElement element)
        {
            NotOnOrAfter = element.ReadAttribute<DateTimeOffset>(NotOnOrAfterAtt);
            Recipient = element.ReadAttribute<Uri>(RecipientAtt);
        }

        //public DateTimeOffset? NotBefore { get; set; }
        public DateTimeOffset? NotOnOrAfter { get; set; }
        public Uri Recipient { get; set; }
        //public string InResponseTo { get; set; }
        //public string Address { get; set; }

        // Change this to IValueSource I think.  You could have custom attributes
        // or custom elements
        //public IDictionary<string, string> Attributes { get; set; }

    }
}