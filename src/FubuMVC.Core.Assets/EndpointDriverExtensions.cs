using System;
using System.Net;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Endpoints;

namespace FubuMVC.Core.Assets
{
    public static class EndpointDriverExtensions
    {
        public static HttpResponse GetAsset(this EndpointDriver endpoints, AssetFolder folder, string name, string etag = null)
        {
            var path = new AssetPath(name, folder);

            return endpoints.GetByInput(path, configure:request => {
                request.Headers.Add(HttpRequestHeader.IfNoneMatch, etag);
            });
        }
    }
}