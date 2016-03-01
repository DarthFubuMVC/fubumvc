using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace FubuMVC.Core.ServiceBus.Runtime.Headers
{
    [Serializable]
    public class NameValueHeaders : IHeaders
    {
        private readonly NameValueCollection _inner;

        public NameValueHeaders() : this(new NameValueCollection())
        {
        }

        public NameValueHeaders(NameValueCollection inner)
        {
            _inner = inner;
        }

        public string this[string key]
        {
            get { return _inner[key]; }
            set { _inner[key] = value; }
        }

        public IEnumerable<string> Keys()
        {
            return _inner.AllKeys;
        }

        public NameValueCollection ToNameValues()
        {
            return _inner;
        }

        public IDictionary<string, string> ToDictionary()
        {
            return _inner.AllKeys.ToDictionary(key => key, key => _inner[key]);
        }

        public bool Has(string key)
        {
            return _inner.AllKeys.Contains(key);
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