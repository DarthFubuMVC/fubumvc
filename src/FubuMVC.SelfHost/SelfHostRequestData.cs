using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Web.Routing;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Util;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;


namespace FubuMVC.SelfHost
{
    public class SelfHostRequestData : RequestData
    {
        public SelfHostRequestData(RouteData routeData, HttpRequestMessage request,
                                   SelfHostCurrentHttpRequest httpRequest)
        {
            AddValues(new RouteDataValues(routeData));


            NameValueCollection querystring = request.RequestUri.ParseQueryString();
            AddValues("Querystring", new NamedKeyValues(querystring));

            NameValueCollection formData = request.Content.IsFormData()
                                               ? request.Content.ReadAsFormDataAsync().Result
                                               : new NameValueCollection();

            AddValues(RequestDataSource.Request.ToString(), new NamedKeyValues(formData));

            AddValues(new CookieValueSource( new Cookies(httpRequest)));
            AddValues(new HeaderValueSource(httpRequest));
        }
    }

    public class AggregateKeyValues : IKeyValues
    {
        private readonly IEnumerable<IKeyValues> _values;

        public AggregateKeyValues(IEnumerable<IKeyValues> values)
        {
            _values = values;
        }

        public bool Has(string key)
        {
            return _values.Any(x => x.Has(key));
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
            IKeyValues holder = findHolder(key);
            return holder == null ? false : holder.ForValue(key, callback);
        }

        public static AggregateKeyValues For(params IKeyValues[] values)
        {
            return new AggregateKeyValues(values);
        }

        private IKeyValues findHolder(string key)
        {
            return _values.FirstOrDefault(x => x.Has(key));
        }
    }
}