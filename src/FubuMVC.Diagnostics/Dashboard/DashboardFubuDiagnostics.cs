using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Diagnostics.Model;
using HtmlTags;

namespace FubuMVC.Diagnostics.Dashboard
{
    public class DashboardFubuDiagnostics
    {
        private readonly DiagnosticGraph _graph;
        private readonly IHttpRequest _httpRequest;

        public DashboardFubuDiagnostics(DiagnosticGraph graph, IHttpRequest httpRequest)
        {
            _graph = graph;
            _httpRequest = httpRequest;
        }

        public DashboardModel Index(DashboardRequestModel request)
        {
            var tag = new HtmlTag("ul");
            var groups = _graph.Groups().OrderBy(x => x.Title).ToList();
            var fubumvc = _graph.FindGroup("FubuMVC.Diagnostics");
            groups.Remove(fubumvc);
            groups.Insert(0, fubumvc);

            groups.Each(group => {
                tag.Add("li/a")
                    .Text(group.Title)
                    .Attr("title", group.Description)
                    .Attr("href", _httpRequest.ToFullUrl(group.GetDefaultUrl()));
            });

            return new DashboardModel
            {
                Links = tag
            };
        }
    }
}