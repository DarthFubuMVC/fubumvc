using System;

namespace FubuCore.Binding
{
    public class SmartRequest : ISmartRequest
    {
        private readonly IRequestData _data;
        private readonly IObjectConverter _converter;

        public SmartRequest(IRequestData data, IObjectConverter converter)
        {
            _data = data;
            _converter = converter;
        }

        public object Value(Type type, string key)
        {
            var rawValue = _data.Value(key);
            return convertValue(rawValue, type);
        }

        private object convertValue(object rawValue, Type type)
        {
            if (rawValue == null) return null;

            if (rawValue.GetType().CanBeCastTo(type)) return rawValue;

            return _converter.FromString(rawValue.ToString(), type);
        }

        public T Value<T>(string key)
        {
            return (T) Value(typeof (T), key);
        }

        public bool Value<T>(string key, Action<T> callback)
        {
            return _data.Value(key, raw =>
            {
                var value = (T)convertValue(raw, typeof (T));
                callback(value);
            });
        }
    }
}