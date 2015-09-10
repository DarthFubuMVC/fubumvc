using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus.ErrorHandling;

namespace FubuMVC.AspNet
{
    public class AspNetDictionary : IDictionary<string, object>
    {
        public static Cache<string, Func<HttpContext, object>> _sources = new Cache<string, Func<HttpContext, object>>(key => null);

        static AspNetDictionary()
        {
            _sources["owin.RequestHeaders"] = c =>
            {
                var dict = new Dictionary<string, string[]>();

                c.Request.Headers.AllKeys.Each(key =>
                {
                    dict.Add(key, new string[]{c.Request.Headers[key]});
                });

                return dict;
            };

            _sources["owin.RequestMethod"] = c => c.Request.HttpMethod;
            _sources["owin.RequestPath"] = c => c.Request.Path;
            _sources["owin.RequestPathBase"] = c => c.Request.ApplicationPath;
            _sources["owin.RequestScheme"] = c => c.Request.Url.Scheme;
            _sources["owin.RequestQueryString"] = c => c.Request.Url.Query;
            _sources["owin.ResponseHeaders"] = c =>
            {
                var dict = new Dictionary<string, string[]>();
                c.Response.Headers.AllKeys.Each(key =>
                {
                    dict.Add(key, new string[] { c.Response.Headers[key] });
                });

                return dict;
            };

            _sources["owin.ResponseStatusCode"] = c => c.Response.StatusCode;
            _sources["owin.ResponseReasonPhrase"] = c => c.Response.StatusDescription;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }
        public bool ContainsKey(string key)
        {
            return _sources.Has(key);
        }

        public void Add(string key, object value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out object value)
        {
            throw new NotImplementedException();
        }

        public object this[string key]
        {
            get { return _sources[key](HttpContext.Current); }
            set { throw new NotImplementedException(); }
        }

        public ICollection<string> Keys { get; private set; }
        public ICollection<object> Values { get; private set; }
    }
}