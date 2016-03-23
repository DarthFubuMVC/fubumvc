using System.Xml;
using FubuMVC.Core.Security.Authentication.Saml2.Xml;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public class SamlName : ReadsSamlXml
    {
        public SamlName()
        {
            Type = SamlNameType.NameID;
            Format = NameFormat.Unspecified;
        }

        public SamlName(XmlElement element)
        {
            // TODO -- read BaseID
            // TODO -- read EncryptedID


            // TODO -- add NameQualifier as URI
            // TODO -- add SPNameQualifier as URI

            var name = element.FindChild(NameID, AssertionXsd);
            if (name != null)
            {
                Type = SamlNameType.NameID;
                Value = name.InnerText;
                Format = NameFormat.Get(name.GetAttribute(FormatAtt)) ?? NameFormat.Unspecified;
            }
        }

        public NameFormat Format { get; set; }
        public SamlNameType Type { get; set; }
        public string Value { get; set; }

        protected bool Equals(SamlName other)
        {
            return Equals(Format, other.Format) && Type == other.Type && string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SamlName) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Format != null ? Format.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Type;
                hashCode = (hashCode*397) ^ (Value != null ? Value.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Format: {0}, Type: {1}, Value: {2}", Format, Type, Value);
        }
    }
}