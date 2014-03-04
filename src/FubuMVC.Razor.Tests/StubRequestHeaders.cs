using System;
using FubuCore;
using FubuCore.Binding.Values;
using FubuMVC.Core.Http;

namespace FubuMVC.Tests.Assets.Http
{
    public class StubHeaders : IRequestHeaders
    {
        public KeyValues Data = new KeyValues();

        public void Value<T>(string header, Action<T> callback)
        {
            Data.ForValue(header, (key, value) => callback(value.As<T>()));
        }

        public T BindToHeaders<T>()
        {
            throw new NotImplementedException();
        }

        public bool HasHeader(string header)
        {
            return Data.Has(header);
        }

        public bool IsAjaxRequest()
        {
            throw new NotImplementedException();
        }
    }
}