using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;

namespace FubuMVC.Diagnostics.Runtime
{
    public class RequestHistoryCache : IRequestHistoryCache
    {
        private readonly Queue<IDebugReport> _reports = new Queue<IDebugReport>();
        private readonly DiagnosticsSettings _settings;

        public RequestHistoryCache(DiagnosticsSettings settings)
        {
            _settings = settings;
        }

        public void AddReport(IDebugReport report)
        {
            _reports.Enqueue(report);
            while (_reports.Count > _settings.MaxRequests)
            {
                _reports.Dequeue();
            }
        }

        public IEnumerable<IDebugReport> RecentReports()
        {
            return _reports.ToList();
        }
    }
}