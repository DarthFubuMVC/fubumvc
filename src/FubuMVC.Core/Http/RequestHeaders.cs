using System;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Conversion;

namespace FubuMVC.Core.Http
{


    public class RequestHeaders : IRequestHeaders
    {
        private readonly IObjectConverter _converter;
        private readonly IObjectResolver _resolver;
        private readonly IValueSource _values;

        public RequestHeaders(IObjectConverter converter, IObjectResolver resolver, ICurrentHttpRequest request)
        {
            _converter = converter;
            _resolver = resolver;

            _values = new HeaderValueSource(request);
        }

        public void Value<T>(string header, Action<T> callback)
        {
            _values.Value(header, value =>
            {
                var converted = _converter.FromString<T>(value.RawValue.ToString());
                callback(converted);
            });
        }

        public T BindToHeaders<T>()
        {
            var bindResult = _resolver.BindModel<T>(_values);

            bindResult.AssertNoProblems(typeof (T));

            return (T) bindResult.Value;
        }

        public bool HasHeader(string header)
        {
            return _values.Has(header);
        }

        public bool IsAjaxRequest()
        {
            bool isAjax = false;

            Value<string>(AjaxExtensions.XRequestedWithHeader, val => isAjax = val == AjaxExtensions.XmlHttpRequestValue);

            return isAjax;
        }
    }

    // TODO -- move this to FubuCore
    public static class ObjectResolverExtensions
    {
        public static BindResult BindModel<T>(this IObjectResolver resolver, IValueSource values)
        {
            var requestData = new RequestData(values);
            return resolver.BindModel(typeof (T), requestData);
        }
    }
}