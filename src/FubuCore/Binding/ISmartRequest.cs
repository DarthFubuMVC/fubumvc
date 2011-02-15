using System;

namespace FubuCore.Binding
{
    public interface ISmartRequest
    {
        object Value(Type type, string key);
        T Value<T>(string key);
        bool Value<T>(string key, Action<T> callback);
    }
}