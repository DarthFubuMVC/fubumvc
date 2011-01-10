using System;
using System.Linq;
using FubuCore.Binding;
using FubuCore.Util;

namespace FubuMVC.Core.Runtime
{
    public class InMemoryRequestData : IRequestData
    {
        private readonly Cache<string, object> _values = new Cache<string, object>();

        public object this[string key] { get { return _values[key]; } set { _values[key] = value; } }

        public object Value(string key)
        {
            return _values.Has(key) ? _values[key] : null;
        }

        public bool Value(string key, Action<object> callback)
        {
            return _values.WithValue(key, callback);
        }

        public bool HasValueThatStartsWith(string key)
        {
            return _values.GetAllKeys().Any(x => x.StartsWith(key));
        }
    }
}