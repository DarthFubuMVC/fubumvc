using System;
using FubuCore.Util;

namespace FubuMVC.Core.Json
{
    public class ValueProjection<T> : IValueProjection<T>
    {
        private readonly string _propertyName;
        private readonly Func<T, object> _valueSource;

        public ValueProjection(string propertyName, Func<T, object> valueSource)
        {
            _propertyName = propertyName;
            _valueSource = valueSource;
        }

        public void Map(T target, Cache<string, object> props)
        {
            props[_propertyName] = _valueSource(target);
        }
    }
}