using System.ComponentModel;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors.Chrome;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Chrome;
using FubuMVC.Diagnostics.Runtime;
using HtmlTags;

namespace FubuMVC.Diagnostics.Requests
{
    [Description("Request Visualization")]
    public class RequestVisualizationFubuDiagnostics
    {
        private readonly IRequestHistoryCache _cache;
        private readonly BehaviorGraph _graph;
        private readonly IUrlRegistry _urls;

        public RequestVisualizationFubuDiagnostics(IRequestHistoryCache cache, BehaviorGraph graph, IUrlRegistry urls)
        {
            _cache = cache;
            _graph = graph;
            _urls = urls;
        }

        public HttpRequestVisualization get_request_details_Id(RequestLog request)
        {
            RequestLog log = _cache.Find(request.Id);

            if (log == null)
            {
                return new HttpRequestVisualization(null, null, _urls)
                {
                    RedirectTo =
                        FubuContinuation.RedirectTo<RequestVisualizationFubuDiagnostics>(x => x.get_requests_missing())
                };
            }

            BehaviorChain chain = _graph.Behaviors.FirstOrDefault(x => x.UniqueId == log.ChainId);

            return new HttpRequestVisualization(log, chain, _urls);
        }

        [FubuPartial]
        public HtmlTag get_requests_missing()
        {
            return new HtmlTag("p", p => {
                p.Add("span").Text("This request is not longer in the request cache.  ");
                p.Add("a").Text("Return to the Request Explorer").Attr("href", _urls.UrlFor<RequestsFubuDiagnostics>());
            });
        }
    }
}