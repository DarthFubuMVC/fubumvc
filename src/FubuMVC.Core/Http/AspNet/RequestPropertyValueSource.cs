using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuCore;

namespace FubuMVC.Core.Http.AspNet
{
    public class RequestPropertyValueSource : IValueSource
    {
        private static readonly Cache<string, Func<HttpRequestBase, object>> _requestProperties =
            new Cache<string, Func<HttpRequestBase, object>>();

        private static readonly IList<PropertyInfo> _systemProperties = new List<PropertyInfo>();
        private readonly HttpRequestBase _request;

        static RequestPropertyValueSource()
        {
            AddRequestProperty(r => r.AcceptTypes);
            AddRequestProperty(r => r.ApplicationPath);
            AddRequestProperty(r => r.AppRelativeCurrentExecutionFilePath);
            AddRequestProperty(r => r.Browser);
            AddRequestProperty(r => r.ClientCertificate);
            AddRequestProperty(r => r.ContentEncoding);
            AddRequestProperty(r => r.ContentLength);
            AddRequestProperty(r => r.ContentType);
            AddRequestProperty(r => r.Cookies);
            AddRequestProperty(r => r.CurrentExecutionFilePath);
            AddRequestProperty(r => r.FilePath);
            AddRequestProperty(r => r.Files);
            AddRequestProperty(r => r.Filter);
            AddRequestProperty(r => r.Form);
            AddRequestProperty(r => r.Headers);
            AddRequestProperty(r => r.HttpMethod);
            AddRequestProperty(r => r.IsAuthenticated);
            AddRequestProperty(r => r.IsLocal);
            AddRequestProperty(r => r.IsSecureConnection);
            AddRequestProperty(r => r.LogonUserIdentity);
            AddRequestProperty(r => r.Params);
            AddRequestProperty(r => r.Path);
            AddRequestProperty(r => r.PathInfo);
            AddRequestProperty(r => r.PhysicalApplicationPath);
            AddRequestProperty(r => r.PhysicalPath);
            AddRequestProperty(r => r.QueryString);
            AddRequestProperty(r => r.RawUrl);
            AddRequestProperty(r => r.RequestType);
            AddRequestProperty(r => r.ServerVariables);
            AddRequestProperty(r => r.TotalBytes);
            AddRequestProperty(r => r.UrlReferrer);
            AddRequestProperty(r => r.UserAgent);
            AddRequestProperty(r => r.UserHostAddress);
            AddRequestProperty(r => r.UserHostName);
            AddRequestProperty(r => r.UserLanguages);
        }

        public static bool IsSystemProperty(PropertyInfo property)
        {
            return
                _systemProperties.Any(
                    x => property.PropertyType.IsAssignableFrom(x.PropertyType) && x.Name == property.Name);
        }

        public static void AddRequestProperty(Expression<Func<HttpRequestBase, object>> expression)
        {
            var property = ReflectionHelper.GetProperty(expression);
            _systemProperties.Add(property);

            _requestProperties[property.Name] = expression.Compile();
        }

        public RequestPropertyValueSource(HttpRequestBase request)
        {
            _request = request;
        }

        //AddLocator(RequestDataSource.RequestProperty, key => GetRequestProperty(request, key),
        //               () => _requestProperties.GetAllKeys());

        public bool Has(string key)
        {
            return _systemProperties.Any(x => x.Name == key);
        }

        public object Get(string key)
        {
            return GetRequestProperty(_request, key);
        }

        public bool HasChild(string key)
        {
            return false;
        }

        public IValueSource GetChild(string key)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<IValueSource> GetChildren(string key)
        {
            return Enumerable.Empty<IValueSource>();
        }

        public void WriteReport(IValueReport report)
        {
            _systemProperties.Each(prop => report.Value(prop.Name, Get(prop.Name)));
        }

        public bool Value(string key, Action<BindingValue> callback)
        {
            if (!Has(key)) return false;

            callback(new BindingValue{
                RawKey = key,
                RawValue = Get(key),
                Source = Provenance
            });

            return true;
        }

        public string Provenance
        {
            get { return RequestDataSource.RequestProperty.ToString(); }
        }

        private static object GetRequestProperty(HttpRequestBase request, string key)
        {
            return _requestProperties.Has(key) ? _requestProperties[key](request) : null;
        }
    }
}