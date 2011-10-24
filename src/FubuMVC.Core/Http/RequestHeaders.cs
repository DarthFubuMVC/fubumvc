using System;
using FubuCore;
using FubuCore.Binding;

namespace FubuMVC.Core.Http
{
    public class RequestHeaders : IRequestHeaders
    {
        private readonly IObjectConverter _converter;
        private readonly AggregateDictionary _dictionary;
        private readonly IObjectResolver _resolver;

        public RequestHeaders(IObjectConverter converter, IObjectResolver resolver, AggregateDictionary dictionary)
        {
            _converter = converter;
            _resolver = resolver;
            _dictionary = dictionary;
        }

        public void Value<T>(string header, Action<T> callback)
        {
            _dictionary.Value(RequestDataSource.Header.ToString(), header, (name, value) =>
            {
                if (value == null)
                {
                    callback(default(T));
                }
                else
                {
                    var converted = _converter.FromString<T>(value.ToString());
                    callback(converted);
                }
            });
        }

        public T BindToHeaders<T>()
        {
            var data = _dictionary.DataFor(RequestDataSource.Header.ToString());
            var bindResult = _resolver.BindModel(typeof (T), data);

            bindResult.AssertNoProblems(typeof (T));

            return (T) bindResult.Value;
        }
    }
}