using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FubuMVC.Core.Http
{
    public interface ICookies
    {
        bool Has(string name);
        HttpCookie Get(string name);

        IEnumerable<HttpCookie> Request { get; }
        IEnumerable<HttpCookie> Response { get; }
    }

    public class InMemoryCookies : ICookies
    {
        private readonly IList<HttpCookie> _request = new List<HttpCookie>(); 
        private readonly IList<HttpCookie> _response = new List<HttpCookie>();
 

        public bool Has(string name)
        {
            return Get(name) != null;
        }

        public HttpCookie Get(string name)
        {
            return _request.SingleOrDefault(x => x.Name == name);
        }

        public IEnumerable<HttpCookie> Request
        {
            get { return _request; }
        }

        public IEnumerable<HttpCookie> Response
        {
            get { return _response; }
        }

        // For testing
        public void AddRequestCookie(HttpCookie cookie)
        {
            _request.Add(cookie);
        }

        public void AddResponseCookie(HttpCookie cookie)
        {
            _response.Add(cookie);
        }
    }
}