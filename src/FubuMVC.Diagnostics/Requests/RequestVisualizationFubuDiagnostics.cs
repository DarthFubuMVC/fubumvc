using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuMVC.Diagnostics.Requests
{
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

        public Dictionary<string, object> get_request_details_Id(RequestLog query)
        {
            var dict = new Dictionary<string, object>();

            var log = _cache.Find(query.Id);

            if (log == null)
            {
                return dict;
            }

            var request = log.ToDictionary();
            dict.Add("request", request);

            // TODO -- get the request headers
            

            var responseHeaders = request.AddChild("response-headers");
            log.ResponseHeaders.Each(x => responseHeaders.Add(x.Name, x.Value));

            var chain = _graph.Behaviors.FirstOrDefault(x => x.GetHashCode() == log.Hash);
            
//            dict.Add("chain-trace", new BehaviorChainTraceTag(chain, log).ToString());
//            dict.Add("response-headers", new ResponseHeadersTag(log).ToString());

            //return new HttpRequestVisualization(log, chain, _urls);

            return dict;
        }

    }
}