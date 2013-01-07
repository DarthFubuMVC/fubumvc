using System.Collections.Generic;
using System.Net;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Http
{
    /// <summary>
    /// A diagnostic service to examine the Http response
    /// </summary>
    public interface IResponse
    {
        int StatusCode { get; }
        string StatusDescription { get; }
        string HeaderValueFor(HttpResponseHeader key);
        string HeaderValueFor(string headerKey);

        IEnumerable<Header> AllHeaders();
    }
}