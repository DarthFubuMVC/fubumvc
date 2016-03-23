namespace FubuMVC.Core.Security.Authentication.Saml2.Xml
{
    public class ReadsSamlXml
    {
        public const string AssertionXsd = "urn:oasis:names:tc:SAML:2.0:assertion";
        public const string AssertionPrefix = "saml2";
        public const string ProtocolXsd = "urn:oasis:names:tc:SAML:2.0:protocol";
        public const string ProtocolPrefix = "samlp";
        public const string EncryptedXsd = "http://www.w3.org/2001/04/xmlenc#";

        protected const string ID = "ID";
        protected const string ResponseIdPrefix = "SamlResponse-";
        protected const string AssertionIdPrefix = "SamlAssertion-";
        protected const string Destination = "Destination";
        protected const string IssueInstant = "IssueInstant";
        protected const string StatusCode = "StatusCode";
        protected const string ValueAtt = "Value";
        protected const string AttributeStatement = "AttributeStatement";
        protected const string NameAtt = "Name";
        protected const string AttributeValue = "AttributeValue";
        protected const string Attribute = "Attribute";
        protected const string Signature = "Signature";
        protected const string Issuer = "Issuer";
        protected const string ConditionsElem = "Conditions";
        protected const string Subject = "Subject";
        protected const string NameID = "NameID";
        protected const string SubjectConfirmation = "SubjectConfirmation";
        protected const string SubjectConfirmationData = "SubjectConfirmationData";
        protected const string MethodAtt = "Method";

        protected const string NotOnOrAfterAtt = "NotOnOrAfter";
        protected const string RecipientAtt = "Recipient";
        protected const string NotBeforeAtt = "NotBefore";
        protected const string AudienceRestriction = "AudienceRestriction";
        protected const string Audience = "Audience";
        protected const string FormatAtt = "Format";

        protected const string AuthnStatement = "AuthnStatement";
        protected const string AuthnInstant = "AuthnInstant";
        protected const string SessionIndexAtt = "SessionIndex";
        protected const string SessionNotOnOrAfterAtt = "SessionNotOnOrAfter";
        protected const string AuthnContext = "AuthnContext";
        protected const string AuthnContextDeclRef = "AuthnContextDeclRef";
        protected const string AuthnContextClassRef = "AuthnContextClassRef";
        protected const string NameFormatAtt = "NameFormat";
        protected const string AssertionElem = "Assertion";
        protected const string EncryptedAssertion = "EncryptedAssertion";
    }
}