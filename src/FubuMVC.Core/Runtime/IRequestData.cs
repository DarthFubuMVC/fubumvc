using System;

namespace FubuMVC.Core.Runtime
{
    public interface IRequestData
    {
        void Value(string key, Action<object> callback);
    }
}