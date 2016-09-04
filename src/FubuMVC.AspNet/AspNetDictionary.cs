using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace FubuMVC.AspNet
{
    public class AspNetDictionary : IDictionary<string, object>
    {
        private readonly HttpContext _context;
        private IDictionary<string, object> _contextInfo;

        private IDictionary<string, object> ContextInfo => _contextInfo
                                                           ??
                                                           (_contextInfo = PopulateFromContext());

        private IDictionary<string, object> PopulateFromContext()
        {
            var dictionary = new Dictionary<string, object>();
            var requestHeaders = new Dictionary<string, string[]>();

            var originalRequestHeaders = _context.Request.Headers;
            foreach (var key in originalRequestHeaders.AllKeys)
            {
                requestHeaders.Add(key, new[] {originalRequestHeaders[key]});
            }
            dictionary["owin.RequestHeaders"] = requestHeaders;

            dictionary["owin.RequestMethod"] = _context.Request.HttpMethod;
            dictionary["owin.RequestPath"] = _context.Request.Path;
            dictionary["owin.RequestPathBase"] = _context.Request.ApplicationPath;
            dictionary["owin.RequestScheme"] = _context.Request.Url.Scheme;
            dictionary["owin.RequestQueryString"] = _context.Request.Url.Query;
            var responseHeaders = new Dictionary<string, string[]>();
            var originalResponseHeaders = _context.Response.Headers;
            foreach (var key in originalResponseHeaders.AllKeys)
            {
                responseHeaders.Add(key, new [] {originalResponseHeaders[key]});
            }
            dictionary["owin.ResponseHeaders"] = responseHeaders;

            dictionary["owin.ResponseStatusCode"] = _context.Response.StatusCode;
            dictionary["owin.ResponseReasonPhrase"] = _context.Response.StatusDescription;
            return dictionary;
        }

        public AspNetDictionary(HttpContext context)
        {
            _context = context;
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
            return ContextInfo.ContainsKey(key);
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
            get { return ContextInfo[key]; }
            set { }
        }

        public ICollection<string> Keys { get; private set; }
        public ICollection<object> Values { get; private set; }
    }
}
