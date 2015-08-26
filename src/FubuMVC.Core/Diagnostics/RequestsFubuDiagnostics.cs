using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Diagnostics.Instrumentation;

namespace FubuMVC.Core.Diagnostics
{
    public class RequestsFubuDiagnostics
    {
        private readonly IChainExecutionHistory _history;

        public RequestsFubuDiagnostics(IChainExecutionHistory history)
        {
            _history = history;
        }

        public Dictionary<string, object> get_requests()
        {
            return new Dictionary<string, object>
            {
                {"requests", _history.RecentReports().OrderByDescending(x => x.Time).ToArray()}
            };
        }

        public ChainExecutionLog get_request_Id(ChainExecutionLog query)
        {
            var log = _history.Find(query.Id);

            return log ?? query;
        }

    }
}