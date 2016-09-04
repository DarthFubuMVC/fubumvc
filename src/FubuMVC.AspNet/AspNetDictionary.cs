using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FubuMVC.AspNet
{
    public class AspNetDictionary : IDictionary<string, object>
    {
        private static Dictionary<string, Func<HttpContext, object>> _sources = new Dictionary<string, Func<HttpContext, object>>();

        static AspNetDictionary()
        {
            _sources["owin.RequestHeaders"] = c => c.Request.Headers.AllKeys.ToDictionary(x => x, x => new []{c.Request.Headers[x]});
            _sources["owin.RequestMethod"] = c => c.Request.HttpMethod;
            _sources["owin.RequestPath"] = c => c.Request.Path;
            _sources["owin.RequestPathBase"] = c => c.Request.ApplicationPath;
            _sources["owin.RequestScheme"] = c => c.Request.Url.Scheme;
            _sources["owin.RequestQueryString"] = c => c.Request.Url.Query;
            _sources["owin.ResponseHeaders"] = c => c.Response.Headers.AllKeys.ToDictionary(x => x, x => new []{c.Response.Headers[x]});
            _sources["owin.ResponseStatusCode"] = c => c.Response.StatusCode;
            _sources["owin.ResponseReasonPhrase"] = c => c.Response.StatusDescription;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            yield break;
        }

        public void Add(KeyValuePair<string, object> item)
        {
        }

        public void Clear()
        {
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return false;
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return false;
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }

        public bool ContainsKey(string key)
        {
            return _sources.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
        }

        public bool Remove(string key)
        {
            return false;
        }

        public bool TryGetValue(string key, out object value)
        {
            value = null;
            return false;
        }

        public object this[string key]
        {
            get
            {
                Func<HttpContext, object> value;
                if (_sources.TryGetValue(key, out value))
                {
                    return value(HttpContext.Current);
                }
                return null;
            }
            set { }
        }

        public ICollection<string> Keys { get; private set; }
        public ICollection<object> Values { get; private set; }
    }
}
