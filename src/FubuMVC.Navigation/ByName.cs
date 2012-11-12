using FubuLocalization;

namespace FubuMVC.Navigation
{
    public class ByName : IStringTokenMatcher
    {
        private readonly string _name;

        public ByName(string name)
        {
            _name = name;
        }

        public bool Matches(StringToken token)
        {
            return token.ToLocalizationKey().ToString() == _name || token.Key == _name;
        }

        public StringToken DefaultKey()
        {
            return new NavigationKey(_name);
        }

        public string Description
        {
            get { return ToString(); }
        }

        public bool Equals(ByName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ByName)) return false;
            return Equals((ByName) obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Match by name: {0}", _name);
        }
    }
}