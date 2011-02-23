using System;
using FubuCore;
using FubuCore.Binding;

namespace FubuMVC.Core.Runtime
{
    public class SmartRequest : ISmartRequest
    {
        private readonly IRequestData _data;
        private readonly IObjectConverter _converter;
        private readonly IFubuRequest _request;

        public SmartRequest(IRequestData data, IObjectConverter converter, IFubuRequest request)
        {
            _data = data;
            _converter = converter;
            _request = request;
        }

        public object Value(Type type, string key)
        {
            object returnValue = null;
            _data.Value(key, o =>
            {
                returnValue = convertValue(o, type);
            });

            if (returnValue == null)
            {
                returnValue = _request.Get(type);
            }

            return returnValue;
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