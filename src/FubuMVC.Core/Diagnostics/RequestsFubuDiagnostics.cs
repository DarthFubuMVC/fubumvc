using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Registration.Nodes;

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

        public ChainExecutionLog get_request_Id(ChainExecutionLog query)
        {
            var log = _history.Find(query.Id);

            return log ?? query;
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