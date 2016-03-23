using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Security.Authentication.Saml2
{
    public class SamlError
    {
        public SamlError()
        {
        }

        public SamlError(StringToken token)
        {
            Key = token.Key;
            Message = token.ToString();
        }

        public string Key;
        public string Message;

        protected bool Equals(SamlError other)
        {
            return string.Equals(Key, other.Key) && string.Equals(Message, other.Message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SamlError) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0)*397) ^ (Message != null ? Message.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Key: {0}, Message: {1}", Key, Message);
        }
    }
}