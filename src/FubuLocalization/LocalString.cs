using System;

namespace FubuLocalization
{
    public class LocalString : IComparable<LocalString>, IComparable
    {
        private string _value;

        public LocalString(string value)
        {
            this.value = value;
            display = value;
        }

        public LocalString()
        {
        }

        public LocalString(Type type)
            : this(type.Name)
        {

        }

        public LocalString(StringToken token)
        {
            value = token.Key;
            display = token.ToString();
        }

        public string value
        {
            get { return _value; }
            set { _value = value; }
        }
        public string display { get; set; }

        public bool Equals(LocalString obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.value, value);
        }

        public int CompareTo(LocalString other)
        {
            if (other == null) return 1;
            if (other.display == null && display == null) return 0;
            if (display == null) return -1;

            return display.CompareTo(other.display);
        }

        public int CompareTo(object obj)
        {
            return display.CompareTo(((LocalString)obj).display);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(LocalString)) return false;
            return Equals((LocalString)obj);
        }

        public override int GetHashCode()
        {
            return (value != null ? value.GetHashCode() : 0);
        }

    }
}