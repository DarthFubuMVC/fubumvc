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
            log.SessionTag = CurrentSessionTag;

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

        public string CurrentSessionTag { get; set; }

        public RequestLog Find(Guid id)
        {
            return _reports.FirstOrDefault(x => x.Id == id);
        }

        public void Clear()
        {
            while (_reports.Count > 0)
            {
                RequestLog _;
                _reports.TryDequeue(out _);
            }
        }
    }
}