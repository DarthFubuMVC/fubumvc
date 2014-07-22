using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuCore.Util;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Visualization;
using HtmlTags;


namespace FubuMVC.Diagnostics
{
    public class RequestsFubuDiagnostics
    {
        private readonly IRequestHistoryCache _cache;
        private readonly BehaviorGraph _graph;
        private readonly IVisualizer _visualizer;

        public RequestsFubuDiagnostics(IRequestHistoryCache cache, BehaviorGraph graph, IVisualizer visualizer)
        {
            _cache = cache;
            _graph = graph;
            _visualizer = visualizer;
        }

        public Dictionary<string, object> get_requests()
        {
            return new Dictionary<string, object>
            {
                {"requests", _cache.RecentReports().OrderByDescending(x => x.Time).Select(x => x.ToDictionary()).ToArray()}
            };
        }

        public HtmlTag VisualizeException(ExceptionReport report)
        {
            var tag = new HtmlTag("div");
            tag.Add("p").Add("b").Text("Exception: " + report.ExceptionType);
            tag.Add("pre").AddClass("text-warning").Text(report.ExceptionText);

            return tag;
        }

        public Dictionary<string, object> get_request_Id(RequestLog query)
        {
            var dict = new Dictionary<string, object>();

            var log = _cache.Find(query.Id);

            if (log == null)
            {
                return dict;
            }

            var request = log.ToDictionary();
            dict.Add("request", request);

            
            request.AddHeaders("request-headers", log.RequestHeaders);
            request.AddHeaders("response-headers", log.ResponseHeaders);

            var chain = _graph.Behaviors.FirstOrDefault(x => x.GetHashCode() == log.Hash);
            request.Add("title", chain.Title());

            request.AddNameValues("querystring", log.QueryString);
            request.AddNameValues("form", log.FormData);

            request.Add("logs", buildLogs(log).ToArray());

            return dict;
        }

        private IEnumerable<LogItem> buildLogs(RequestLog log)
        {
            var stack = new Stack<BehaviorNode>();
            var titles = new Cache<BehaviorNode, string>(node => Description.For(node).Title);

            var steps = log.AllSteps().ToArray();
            foreach (var step in steps)
            {
                (step.Log as BehaviorStart).CallIfNotNull(x => stack.Push(x.Correlation.Node));




                var node = findNode(step, stack, steps);
                

                yield return new LogItem
                {
                    behavior = node == null ? "Unknown" : titles[node],
                    time = step.RequestTimeInMilliseconds,
                    html = determineHtml(step.Log)
                };

                (step.Log as BehaviorFinish).CallIfNotNull(x => stack.Pop());
            }
        }

        private BehaviorNode findNode(RequestStep step, Stack<BehaviorNode> stack, RequestStep[] steps)
        {
            if (stack.Any()) return stack.Peek();

            var index = Array.IndexOf(steps, step);

            var last = steps.Select(x => x.Log).Take(index).OfType<BehaviorFinish>().LastOrDefault();
            return last == null ? null : last.Correlation.Node;
        }


        private string determineHtml(object log)
        {
            if (log is BehaviorStart) return "Started";
            if (log is BehaviorFinish) return "Finished";

            return _visualizer.Visualize(log).ToString();
        }

        public class LogItem
        {
            public double time;
            public string behavior;
            public string html;
        }
    }
}