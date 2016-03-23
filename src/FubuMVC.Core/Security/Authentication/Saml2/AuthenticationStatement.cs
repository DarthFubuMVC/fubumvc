using System;
using System.Xml;
using FubuMVC.Core.Security.Authentication.Saml2.Xml;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public class AuthenticationStatement : ReadsSamlXml
    {
        public AuthenticationStatement()
        {

        }

        public AuthenticationStatement(XmlDocument document)
        {
            var element = document.FindChild(AuthnStatement);
            if (element == null) return;

            Instant = element.ReadAttribute<DateTimeOffset>(AuthnInstant);
            SessionIndex = element.ReadAttribute<string>(SessionIndexAtt);
            SessionNotOnOrAfter = element.ReadAttribute<DateTimeOffset>(SessionNotOnOrAfterAtt);

            var context = element.FindChild(AuthnContext, AssertionXsd);
            DeclarationReference = context.ReadChildText<Uri>(AuthnContextDeclRef);
            ClassReference = context.ReadChildText<Uri>(AuthnContextClassRef);
        }

        public DateTimeOffset Instant { get; set; }
        public string SessionIndex { get; set; }
        public DateTimeOffset? SessionNotOnOrAfter { get; set; }

        // TODO -- support AuthenticatingAuthority
        public Uri ClassReference { get; set; }
        public Uri DeclarationReference { get; set; }

        // TODO -- support declaration
        //public string Declaration { get; set; }
    }

}