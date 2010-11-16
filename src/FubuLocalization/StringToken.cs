using System;

namespace FubuLocalization
{
    public class StringToken
    {
        private readonly string _key;
        private readonly string _defaultValue;

        public static StringToken FromKeyString(string key)
        {
            return new StringToken(key, null);
        }

        public static StringToken FromKeyString(string key, string defaultValue)
        {
            return new StringToken(key, defaultValue);
        }

        protected StringToken(string key, string defaultValue)
        {
            _key = key;
            _defaultValue = defaultValue;
        }

        public string Key
        {
            get { return _key; }
        }

        public string DefaultValue { get { return _defaultValue; } }

        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary>
        /// Conditionally render the string based on a condition. Convenient if you want to avoid a bunch of messy script tags in the views.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public string ToString(bool condition)
        {
            return condition ? LocalizationManager.GetTextForKey(this) : string.Empty;
        }

        public string ToFormat(params object[] args)
        {
            return string.Format(ToString(), args);
        }

        public bool Equals(StringToken obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj._key, _key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(StringToken)) return false;
            return Equals((StringToken)obj);
        }

        public override int GetHashCode()
        {
            return (_key != null ? _key.GetHashCode() : 0);
        }
    }
}