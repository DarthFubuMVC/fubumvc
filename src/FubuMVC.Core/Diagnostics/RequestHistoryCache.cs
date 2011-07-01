using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Diagnostics
{
    public class RequestHistoryCache : IRequestHistoryCache
    {
        private readonly Queue<IDebugReport> _reports = new Queue<IDebugReport>();
        private readonly Func<CurrentRequest> _request;
        private readonly IEnumerable<IRequestHistoryCacheFilter> _filters;
        private readonly DiagnosticsConfiguration _configuration;

        public RequestHistoryCache(Func<CurrentRequest> request, IEnumerable<IRequestHistoryCacheFilter> filters, DiagnosticsConfiguration configuration)
        {
            _request = request;
            _filters = filters;
            _configuration = configuration;
        }

        public void AddReport(IDebugReport report)
        {
            if(_filters.Any(f => f.Exclude(_request())))
            {
                return;
            }

            _reports.Enqueue(report);
            while (_reports.Count > _configuration.MaxRequests)
            {
                _reports.Dequeue();
            }
        }

        public IEnumerable<IDebugReport> RecentReports()
        {
            return _reports.ToList();
        }
    }

    public interface IRequestHistoryCacheFilter
    {
        bool Exclude(CurrentRequest request);
    }

    public class LambdaRequestHistoryCacheFilter : IRequestHistoryCacheFilter
    {
        private readonly Func<CurrentRequest, bool> _shouldExclude;

        public LambdaRequestHistoryCacheFilter(Func<CurrentRequest, bool> shouldExclude)
        {
            _shouldExclude = shouldExclude;
        }

        public bool Exclude(CurrentRequest request)
        {
            return _shouldExclude(request);
        }
    }
}