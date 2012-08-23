using System.Collections.Generic;
using System.Net;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Http
{
    public interface IResponse
    {
        int StatusCode { get; }
        string StatusDescription { get; }
        string HeaderValueFor(HttpResponseHeader key);
        string HeaderValueFor(string headerKey);

        IEnumerable<Header> AllHeaders();
    }
}