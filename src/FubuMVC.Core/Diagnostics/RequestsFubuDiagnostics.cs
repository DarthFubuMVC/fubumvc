using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.Diagnostics
{
    public class RequestsFubuDiagnostics
    {
        private readonly IChainExecutionHistory _history;

        public RequestsFubuDiagnostics(IChainExecutionHistory history)
        {
            _history = history;
        }

        public HttpRequestSummaryItems get_requests()
        {
            var logs = _history.RecentReports().Where(x => x.RootChain is RoutedChain).OrderByDescending(x => x.Time);
            return new HttpRequestSummaryItems(logs);
        }

        public Dictionary<string, object>[] get_messages()
        {
            var logs = _history.RecentReports().Where(x => x.RootChain is HandlerChain)
                .Where(x => !x.RootChain.As<HandlerChain>().IsPollingJob())
                .OrderByDescending(x => x.Time);

            return logs.Select(log =>
            {
                var dict = log.ToHeaderDictionary();

                dict.Add("message", log.RootChain.InputType().Name);

                return dict;
            }).ToArray();
        } 

        public Dictionary<string, object> get_request_Id(ChainExecutionLog query)
        {
            var log = _history.Find(query.Id);

            var dict = new Dictionary<string, object>();

            if (log != null)
            {
                dict.Add("log", log.ToDictionary());
                if (log.RootChain != null) dict.Add("type", log.RootChain.GetType().Name);
            }

            return dict;
        }

    }

    public class HttpRequestSummaryItems
    {
        public HttpRequestSummaryItems()
        {
        }

        public HttpRequestSummaryItems(IEnumerable<ChainExecutionLog> logs)
        {
            requests = logs.Select(x => new HttpRequestSummary(x)).ToArray();
        }

        public HttpRequestSummary[] requests { get; set; }
    }
}