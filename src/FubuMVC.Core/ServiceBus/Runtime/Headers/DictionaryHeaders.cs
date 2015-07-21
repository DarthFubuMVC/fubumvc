using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FubuMVC.Core.ServiceBus.Runtime.Headers
{
    [Serializable]
    public class DictionaryHeaders : IHeaders
    {
        private readonly IDictionary<string, string> _inner;

        public DictionaryHeaders() : this(new Dictionary<string, string>())
        {
        }

        public DictionaryHeaders(IDictionary<string, string> inner)
        {
            _inner = inner;
        }

        public string this[string key]
        {
            get { return _inner.ContainsKey(key) ? _inner[key] : null; }
            set
            {
                if (_inner.ContainsKey(key))
                {
                    _inner[key] = value;
                }
                else
                {
                    _inner.Add(key, value);
                }
            }
        }

        public IEnumerable<string> Keys()
        {
            return _inner.Keys;
        }

        public NameValueCollection ToNameValues()
        {
            var values = new NameValueCollection();
            _inner.Each(x => values[x.Key] = x.Value);

            return values;
        }

        public bool Has(string key)
        {
            return _inner.ContainsKey(key);
        }

        public void Remove(string key)
        {
            if (Has(key))
            {
                _inner.Remove(key);
            }
        }
    }
}