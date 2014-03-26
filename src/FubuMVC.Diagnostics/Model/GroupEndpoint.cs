using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors.Chrome;
using FubuMVC.Core.Http;
using FubuMVC.Diagnostics.Chrome;
using HtmlTags;

namespace FubuMVC.Diagnostics.Model
{
    public class GroupEndpoint
    {
        private readonly DiagnosticGraph _graph;
        private readonly IHttpRequest _httpRequest;

        public GroupEndpoint(DiagnosticGraph graph, IHttpRequest httpRequest)
        {
            _graph = graph;
            _httpRequest = httpRequest;
        }

        [Chrome(typeof(DashboardChrome), Title = "Diagnostics")]
        [UrlPattern("_fubu/{Name}")]
        public HtmlTag Group(GroupRequest request)
        {
            var group = _graph.FindGroup(request.Name);

            var tag = new HtmlTag("div");
            tag.Add("h1").Text(group.Name);
            if (group.Description.IsNotEmpty())
            {
                tag.Add("p/i").Text(group.Description);
            }

            var ul = tag.Add("ul");
            group.Links().Each(x => {
                ul.Add("li/a")
                    .Text(x.Title)
                    .Attr("title", x.Description)
                    .Attr("href", _httpRequest.ToFullUrl(x.GetRoutePattern()));
            });


            return tag;
        }
    }

    public class GroupRequest
    {
        public string Name { get; set; }
    }
}