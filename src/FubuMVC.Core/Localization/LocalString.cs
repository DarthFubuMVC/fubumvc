using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Localization
{
    public class LocalString : IComparable<LocalString>, IComparable
    {
        private string _value;

        public static LocalString ReadFrom(string line)
        {
            var parts = line.Trim().Split('=');
            if (parts.Length != 2)
            {
                throw new ArgumentException("LocalString must be expressed as 'value=display', '{0}' is invalid".ToFormat(line));
            }

            return new LocalString(parts.First(), parts.Last());
        }

        public static IEnumerable<LocalString> ReadAllFrom(string text)
        {
            return text.ReadLines()
                .Select(x => x.Trim())
                .Where(x => x.IsNotEmpty())
                .Select(ReadFrom);
        }

        public LocalString(string value)
        {
            this.value = value;
            display = value;
        }

        public LocalString(string value, string display)
        {
            _value = value;
            this.display = display;
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
            return Equals(obj.value, value) && Equals(obj.display, display);
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

        public override string ToString()
        {
            return string.Format("{0}={1}", _value, display);
        }
    }
}