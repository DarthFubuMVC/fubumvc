using System;
using FubuMVC.Core;
using FubuMVC.Core.View;
using HtmlTags;

namespace FubuMVC.UI
{
    public static class AuthorizedLinkExpressions
    {
        public static HtmlTag AuthorizedLink(this IFubuPage page, Func<IEndpointService, Endpoint> finder)
        {
            var endpoints = page.Get<IEndpointService>();
            var endpoint = finder(endpoints);

            return new HtmlTag("a")
                .Attr("href", endpoint.Url)
                .Authorized(endpoint.IsAuthorized);
        }
    }
}