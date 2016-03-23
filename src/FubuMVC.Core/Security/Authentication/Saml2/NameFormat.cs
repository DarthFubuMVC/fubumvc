namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public class NameFormat : UriEnum<NameFormat>
    {
        public readonly static NameFormat Persistent = new NameFormat("urn:oasis:names:tc:SAML:2.0:nameid-format:persistent");
        public readonly static NameFormat Transient = new NameFormat("urn:oasis:names:tc:SAML:2.0:nameid-format:transient");
        public readonly static NameFormat EmailAddress = new NameFormat("urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress");
        public readonly static NameFormat EntityIdentifier = new NameFormat("urn:oasis:names:tc:SAML:2.0:nameid-format:entity");
        public readonly static NameFormat Kerboros = new NameFormat("urn:oasis:names:tc:SAML:2.0:nameid-format:kerberos");
        public readonly static NameFormat WindowsDomainQualifiedName = new NameFormat("urn:oasis:names:tc:SAML:1.1:nameid-format:WindowsDomainQualifiedName");
        public readonly static NameFormat X509SubjectName = new NameFormat("urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName");
        public readonly static NameFormat Unspecified = new NameFormat("urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified");

        private NameFormat(string uri, string description = null) : base(uri, description)
        {
        }

        public static NameFormat Get(string uriString)
        {
            return get(uriString);
        }
    }
}