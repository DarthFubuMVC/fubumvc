using System.Collections.Generic;
using System.Web;

namespace FubuMVC.Core.Http.Cookies
{
    public interface ICookies
    {
        bool Has(string name);
        HttpCookie Get(string name);

        IEnumerable<HttpCookie> Request { get; }
        IEnumerable<HttpCookie> Response { get; }
    }
}