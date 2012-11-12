using FubuLocalization;

namespace FubuMVC.Navigation
{
    public class Literal : IStringTokenMatcher
    {
        private readonly StringToken _token;

        public Literal(StringToken token)
        {
            _token = token;
        }

        public bool Matches(StringToken token)
        {
            return _token.Equals(token);
        }

        public StringToken DefaultKey()
        {
            return _token;
        }

        public string Description
        {
            get { return ToString(); }
        }

        public bool Equals(Literal other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._token, _token);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Literal)) return false;
            return Equals((Literal) obj);
        }

        public override int GetHashCode()
        {
            return (_token != null ? _token.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("By token: {0}", _token.ToLocalizationKey().ToString());
        }
    }
}