using System;
using System.Collections.Generic;
using FubuCore.Util;

namespace FubuCore.Binding
{
    public class InMemoryRequestData : IRequestData
    {
        private readonly Cache<string, object> _values = new Cache<string, object>();

        public InMemoryRequestData()
        {
        }

        public InMemoryRequestData(IDictionary<string, object> values)
        {
            _values = new Cache<string, object>(values);
        }

        public object this[string key] { get { return _values[key]; } set { _values[key] = value; } }

        public object Value(string key)
        {
            return _values.Has(key) ? _values[key] : null;
        }

        public bool Value(string key, Action<object> callback)
        {
            return _values.WithValue(key, callback);
        }
    }
}