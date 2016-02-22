using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Localization;

namespace FubuMVC.Core.Validation
{
    public class Template
    {
        private readonly StringToken _template;
        private readonly TemplateValueCollection _values;

        public Template(StringToken template, params TemplateValue[] values)
        {
            _template = template;
            _values = new TemplateValueCollection(values);
        }

        public TemplateValueCollection Values { get { return _values; } }

        public string Render()
        {
            return TemplateParser.Parse(_template, _values.ToDictionary());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Template)) return false;
            return Equals((Template) obj);
        }

        public bool Equals(Template other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._template, _template) && Equals(other._values, _values);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_template.GetHashCode()*397) ^ _values.GetHashCode();
            }
        }
    }

    public class TemplateValue
    {
        private readonly string _key;
        private readonly string _value;

        public TemplateValue(string key, string value)
        {
            _key = key;
            _value = value;
        }

        public string Key
        {
            get { return _key; }
        }

        public string Value
        {
            get { return _value; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TemplateValue)) return false;
            return Equals((TemplateValue) obj);
        }

        public bool Equals(TemplateValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._key, _key) && Equals(other._value, _value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_key.GetHashCode()*397) ^ _value.GetHashCode();
            }
        }

        public static TemplateValue For(string key, string value)
        {
            return new TemplateValue(key, value);
        }

        public static TemplateValue For(string key, object value)
        {
            return For(key, value.ToString());
        }
    }

    public class TemplateValueCollection : IEnumerable<TemplateValue>
    {
        private readonly IList<TemplateValue> _values = new List<TemplateValue>();

        public TemplateValueCollection()
        {
        }

        public TemplateValueCollection(IEnumerable<TemplateValue> values)
        {
            _values.AddRange(values);
        }

        public void Add(TemplateValue value)
        {
            _values.Fill(value);
        }

        public IEnumerator<TemplateValue> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TemplateValueCollection)) return false;
            return Equals((TemplateValueCollection) obj);
        }

        public bool Equals(TemplateValueCollection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return _values.SequenceEqual(other._values);
        }

        public override int GetHashCode()
        {
            return _values.GetHashCode();
        }

        public static TemplateValueCollection For(params TemplateValue[] values)
        {
            return new TemplateValueCollection(values);
        }

        public IDictionary<string, string> ToDictionary()
        {
            var values = new Dictionary<string, string>();
            _values.Each(x => values.Fill(x.Key, x.Value));

            return values;
        }
    }
}