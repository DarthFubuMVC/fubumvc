using System;
using System.Reflection;

namespace FubuLocalization
{
    public class LocalizationKey
    {
        private readonly string _key1;
        private readonly string _key2;

        public LocalizationKey(string key1)
            : this(key1, string.Empty)
        {
        }

        public LocalizationKey(string key1, string key2)
        {
            _key1 = key1;
            _key2 = key2 ?? string.Empty;
        }

        public LocalizationKey(PropertyInfo property)
        {
            _key1 = property.Name;
            _key2 = property.DeclaringType.FullName;
        }

        public bool Equals(LocalizationKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(_key1, other._key1, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(_key2, other._key2, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(LocalizationKey)) return false;
            return Equals((LocalizationKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_key1.ToLower().GetHashCode() * 397) ^ _key2.ToLower().GetHashCode();
            }
        }

        public override string ToString()
        {
            return _key1 + "-" + _key2;
        }

        public string Key1 { get { return _key1; } }
    }
}