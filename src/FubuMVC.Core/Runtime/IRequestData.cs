using System;

namespace FubuMVC.Core.Runtime
{
    public interface IRequestData
    {
        object Value(string key);
        bool Value(string key, Action<object> callback);
    }
}