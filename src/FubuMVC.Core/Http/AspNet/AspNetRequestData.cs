using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Util;
using FubuMVC.Core.Http.Cookies;

namespace FubuMVC.Core.Http.AspNet
{
    public class AspNetRequestData : RequestData
    {
        public AspNetRequestData(RequestContext context, AspNetCurrentHttpRequest currentRequest)
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

            Func<string, IEnumerable<string>, bool> ignoreCaseKeyFinder = (key, keys) => keys.Contains(key, StringComparer.InvariantCultureIgnoreCase);

            addValues(RequestDataSource.Cookie, key => request.Cookies[key].Value, () => request.Cookies.AllKeys, ignoreCaseKeyFinder);

            AddValues(new CookieValueSource(new Cookies.Cookies(currentRequest)));
            AddValues(new HeaderValueSource(currentRequest));

            AddValues(new RequestPropertyValueSource(request));

            
        }


        private void addValues(RequestDataSource source, Func<string, object> finder, Func<IEnumerable<string>> findKeys, Func<string,IEnumerable<string>,bool> keyFinder = null)
        {
            Func<string, IEnumerable<string>, bool> defaultKeyFinder = (key, keys) => keys.Contains(key);

            var values = new SimpleKeyValues(finder, findKeys, keyFinder ?? defaultKeyFinder);
            var valueSource = new FlatValueSource<object>(values, source.ToString());

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
        private readonly Func<string,IEnumerable<string>, bool> _keyFinder;
        private readonly Func<IEnumerable<string>> _allKeys;

        public SimpleKeyValues(Func<string, object> source, Func<IEnumerable<string>> allKeys, Func<string,IEnumerable<string>, bool> keyFinder)
        {
            _source = source;
            _keyFinder = keyFinder;
            _allKeys = allKeys;
        }

        public SimpleKeyValues(Func<string,object> source,Func<IEnumerable<string>> allKeys) : this(source,allKeys, (key,keys) => keys.Contains(key))
        {
        }

        public bool Has(string key)
        {
            return _keyFinder(key,_allKeys());
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