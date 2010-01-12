using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace FubuMVC.Core.Runtime
{
    public enum RequestDataSource
    {
        Route,
        Request,
        File,
        Header,
        Other
    }


    public class AggregateDictionary
    {
        private readonly IList<Locator> _locators = new List<Locator>();

        public AggregateDictionary()
        {
        }

        public AggregateDictionary(RequestContext context)
        {
            AddLocator(RequestDataSource.Route, key =>
            {
                object found;
                context.RouteData.Values.TryGetValue(key, out found);
                return found;
            });

            HttpContextBase @base = context.HttpContext;

            configureForRequest(@base);
        }

        public static AggregateDictionary ForHttpContext(HttpContextWrapper context)
        {
            var dict = new AggregateDictionary();
            dict.configureForRequest(context);

            return dict;
        }

        private void configureForRequest(HttpContextBase @base)
        {
            HttpRequestBase request = @base.Request;

            AddLocator(RequestDataSource.Request, key => request[key]);
            AddLocator(RequestDataSource.File, key => request.Files[key]);
            AddLocator(RequestDataSource.Header, key => request.Headers[key]);
        }


        public void Value(string key, Action<RequestDataSource, object> callback)
        {
            _locators.Any(x => x.Locate(key, callback));
        }

        public AggregateDictionary AddLocator(RequestDataSource source, Func<string, object> locator)
        {
            _locators.Add(new Locator
            {
                Getter = locator,
                Source = source
            });

            return this;
        }

        public AggregateDictionary AddDictionary(IDictionary<string, object> dictionary)
        {
            AddLocator(RequestDataSource.Other, key => dictionary.ContainsKey(key) ? dictionary[key] : null);
            return this;
        }

        #region Nested type: Locator

        public class Locator
        {
            public RequestDataSource Source { get; set; }
            public Func<string, object> Getter { get; set; }

            public bool Locate(string key, Action<RequestDataSource, object> callback)
            {
                object value = Getter(key);
                if (value != null)
                {
                    callback(Source, value);
                    return true;
                }

                return false;
            }
        }

        #endregion
    }
}