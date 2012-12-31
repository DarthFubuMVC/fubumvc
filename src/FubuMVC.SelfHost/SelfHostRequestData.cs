using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Routing;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Util;
using FubuMVC.Core.Http;
using System.Linq;
using FubuMVC.Core.Http.AspNet;
using FubuMVC.Core.Http.Cookies;

namespace FubuMVC.SelfHost
{
    public class SelfHostRequestData : RequestData
    {
        public SelfHostRequestData(RouteData routeData, HttpRequestMessage request, ICookies cookies)
        {
            AddValues(new RouteDataValues(routeData));

            

            var querystring = request.RequestUri.ParseQueryString();
            AddValues("Querystring", new NamedKeyValues(querystring));

            var formData = request.Content.IsFormData() ? request.Content.ReadAsFormDataAsync().Result : new NameValueCollection();

            AddValues(RequestDataSource.Request.ToString(), new NamedKeyValues(formData));

            var headers = AggregateKeyValues.For(new HeaderKeyValues(request.Headers),
                                                 new HeaderKeyValues(request.Content.Headers));
            AddValues(RequestDataSource.Header.ToString(), headers);

            Func<string, IEnumerable<string>, bool> ignoreCaseKeyFinder = (key, keys) => keys.Contains(key, StringComparer.InvariantCultureIgnoreCase);
            var values = new SimpleKeyValues(key => cookies.Get(key).Value, () => cookies.Request.Select(x => x.Name), ignoreCaseKeyFinder);
            var valueSource = new FlatValueSource<object>(values, RequestDataSource.Cookie.ToString());

            AddValues(valueSource);
        }
    }

    public class AggregateKeyValues : IKeyValues
    {
        private readonly IEnumerable<IKeyValues> _values;

        public static AggregateKeyValues For(params IKeyValues[] values)
        {
            return new AggregateKeyValues(values);
        }

        public AggregateKeyValues(IEnumerable<IKeyValues> values)
        {
            _values = values;
        }

        public bool Has(string key)
        {
            return _values.Any(x => x.Has(key));
        }

        private IKeyValues findHolder(string key)
        {
            return _values.FirstOrDefault(x => x.Has(key));
        }

        public string Get(string key)
        {
            return findHolder(key).Get(key);
        }

        public IEnumerable<string> GetKeys()
        {
            return _values.SelectMany(x => x.GetKeys());
        }

        public bool ForValue(string key, Action<string, string> callback)
        {
            var holder = findHolder(key);
            return holder == null ? false : holder.ForValue(key, callback);
        }
    }

    public class HeaderKeyValues : IKeyValues
    {
        private readonly HttpHeaders _headers;
        private readonly Lazy<IList<string>> _keys;

        public HeaderKeyValues(HttpHeaders headers)
        {
            _headers = headers;
            _keys = new Lazy<IList<string>>(() => headers.Select(x => x.Key).ToList());
        }

        public bool Has(string key)
        {
            return _keys.Value.Contains(key);
        }

        public string Get(string key)
        {
            return _headers.GetValues(key).Join(", ");
        }

        public IEnumerable<string> GetKeys()
        {
            return _keys.Value;
        }

        public bool ForValue(string key, Action<string, string> callback)
        {
            if (Has(key))
            {
                callback(key, Get(key));
                return true;
            }

            return false;
        }
    }
}