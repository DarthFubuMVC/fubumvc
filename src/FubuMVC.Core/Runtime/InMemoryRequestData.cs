using System;
using FubuMVC.Core.Util;

namespace FubuMVC.Core.Runtime
{
    public class InMemoryRequestData : IRequestData
    {
        private readonly Cache<string, object> _values = new Cache<string, object>();

        public object this[string key] { get { return _values[key]; } set { _values[key] = value; } }

        public void Value(string key, Action<object> callback)
        {
            _values.WithValue(key, callback);
        }
    }
}