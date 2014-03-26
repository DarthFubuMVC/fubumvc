using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.Diagnostics.Model;
using HtmlTags;

namespace FubuMVC.Diagnostics
{
    public static class FubuPageExtensions
    {
        public static HtmlTag DiagnosticMenu(this IFubuPage page)
        {
            return page.Get<DiagnosticMenuTag>();
        }

        public static string DiagnosticsTitle(this IFubuPage page)
        {
            return page.Get<IDiagnosticContext>().Title();
        }

        public static HtmlTag OtherGroupMenus(this IFubuPage page)
        {
            return page.Get<DiagnosticGroupsMenuTag>();
        }
    }

    public class DiagnosticGroupsMenuTag : HtmlTag
    {
        public DiagnosticGroupsMenuTag(DiagnosticGraph groups, IHttpRequest httpRequest) : base("ul")
        {
            AddClass("dropdown-menu");
            Attr("role", "menu");
            Attr("aria-labelledby", "groups");

            groups.Groups().OrderBy(x => x.Title).Each(group => {
                var li = Add("li").Attr("role", "presentation");
                li.Add("a")
                    .Attr("role", "menuitem")
                    .Attr("tabindex", "-1")
                    .Attr("href", httpRequest.ToFullUrl(group.GetDefaultUrl()))
                    .Text(group.Title).Attr("title", group.Description);
            });
        }
    }

    public class DiagnosticMenuTag : HtmlTag
    {
        public DiagnosticMenuTag(IDiagnosticContext context, IHttpRequest currentHttpRequest, IUrlRegistry urls) : base("ul")
        {
            AddClass("nav");

            var group = context.CurrentGroup();

            var index = group.Index();
            if (index != null)
            {
                addLink(index, context, currentHttpRequest);
            }
            else
            {
                var url = urls.UrlFor(new GroupRequest {Name = group.Name});
                var li = Add("li");
                li.Add("a").Attr("href", url).Text(group.Name).Attr("title", group.Description);

                if (context.CurrentChain() == null)
                {
                    li.AddClass("active");
                }
            }

            group.Links().Each(x => addLink(x, context, currentHttpRequest));
        }

        private void addLink(DiagnosticChain diagnosticChain, IDiagnosticContext context, IHttpRequest currentHttpRequest)
        {
            var url = currentHttpRequest.ToFullUrl(diagnosticChain.GetRoutePattern());
            var li = Add("li");
            li.Add("a").Attr("href", url).Text(diagnosticChain.Title).Attr("title", diagnosticChain.Description);

            if (context.CurrentChain() == diagnosticChain)
            {
                li.AddClass("active");
            }
        }
    }

   
}