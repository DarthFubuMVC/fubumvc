using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Util;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetRequestData : RequestData
    {
        public AspNetRequestData(RequestContext context)
        {
            /*
             *  1.) Route
             *  2.) Request
             *  3.) File
             *  4.) Header
             *  5.) RequestProperty
             */

            AddValues(new RouteDataValues(context.RouteData));

            var request = context.HttpContext.Request;

            addValues(RequestDataSource.Request, key => request[key], () => keysForRequest(request));

            var files = request.Files;
            addValues(RequestDataSource.File, key => files[key], () => files.AllKeys);

            addValues(RequestDataSource.Header, key => request.Headers[key], () => request.Headers.AllKeys);

            AddValues(new RequestPropertyValueSource(context.HttpContext.Request));


        }

        private void addValues(RequestDataSource source, Func<string, object> finder, Func<IEnumerable<string>> findKeys)
        {
            var values = new SimpleKeyValues(finder, findKeys);
            var valueSource = new FlatValueSource<object>(values, source.ToString());

            //var valueSource = new GenericValueSource(source.ToString(), finder, findKeys);
            AddValues(valueSource);
        }

        private static IEnumerable<string> keysForRequest(HttpRequestBase request)
        {
            foreach (var key in request.QueryString.AllKeys)
            {
                yield return key;
            }

            foreach (var key in request.Form.AllKeys.Where(x => x != null))
            {
                yield return key;
            }
        }
    }

    public class SimpleKeyValues : IKeyValues<object>
    {
        private readonly Func<string, object> _source;
        private readonly Func<IEnumerable<string>> _allKeys;

        public SimpleKeyValues(Func<string, object> source, Func<IEnumerable<string>> allKeys)
        {
            _source = source;
            _allKeys = allKeys;
        }

        public bool Has(string key)
        {
            var comparer = StringComparer.InvariantCultureIgnoreCase;
            return _allKeys().Contains(key, comparer);
        }

        public object Get(string key)
        {
            return _source(key);
        }

        public IEnumerable<string> GetKeys()
        {
            return _allKeys();
        }

        public bool ForValue(string key, Action<string, object> callback)
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