using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    public class RequestHistoryCache : IRequestHistoryCache
    {
        private readonly ConcurrentQueue<RequestLog> _reports = new ConcurrentQueue<RequestLog>();
        private readonly DiagnosticsSettings _settings;

        public RequestHistoryCache(DiagnosticsSettings settings)
        {
            _settings = settings;
        }

        public void Store(RequestLog log)
        {
            _reports.Enqueue(log);
            while (_reports.Count > _settings.MaxRequests)
            {
                RequestLog _;
                _reports.TryDequeue(out _);
            }
        }

        public IEnumerable<RequestLog> RecentReports()
        {
            return _reports.ToList();
        }

        public RequestLog Find(Guid id)
        {
            return _reports.FirstOrDefault(x => x.Id == id);
        }
    }
}