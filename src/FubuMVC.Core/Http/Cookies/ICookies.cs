using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Linq;

namespace FubuMVC.Core.Http.Cookies
{
    public interface ICookies
    {
        bool Has(string name);
        Cookie Get(string name);

        IEnumerable<Cookie> Request { get; }
    }

    // TODO -- need to register this
    public class Cookies : ICookies
    {
        private readonly Lazy<IEnumerable<Cookie>> _cookies;

        public Cookies(ICurrentHttpRequest request)
        {
            _cookies = new Lazy<IEnumerable<Cookie>>(() => {
                return request.GetHeader(HttpRequestHeader.Cookie).Select(x => CookieParser.ToCookie(x)).ToArray();
            });
        }

        public bool Has(string name)
        {
            return _cookies.Value.Any(x => x.Matches(name));
        }

        public Cookie Get(string name)
        {
            return _cookies.Value.FirstOrDefault(x => x.Matches(name));
        }

        public IEnumerable<Cookie> Request
        {
            get { return _cookies.Value; }
        }
    }
}