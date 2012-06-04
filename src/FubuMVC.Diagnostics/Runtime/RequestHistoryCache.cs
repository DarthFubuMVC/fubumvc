using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuCore;
using FubuMVC.Diagnostics.Runtime.Tracing;

namespace FubuMVC.Diagnostics.Runtime
{
    public class RequestHistoryCache : IRequestHistoryCache
    {
        private readonly Queue<IDebugReport> _reports = new Queue<IDebugReport>();
        private readonly IEnumerable<IRequestHistoryCacheFilter> _filters;
        private readonly DiagnosticsSettings _settings;

        public RequestHistoryCache(IEnumerable<IRequestHistoryCacheFilter> filters, DiagnosticsSettings settings)
        {
            _filters = filters;
            _settings = settings;
        }

        // TODO -- let's thin this down from CurrentRequest
        public void AddReport(IDebugReport report, CurrentRequest request)
        {
            if(_filters.Any(f => f.Exclude(request)))
            {
                return;
            }

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

    public interface IRequestHistoryCacheFilter
    {
        bool Exclude(CurrentRequest request);
    }

    public class DiagnosticRequestHistoryCacheFilter : IRequestHistoryCacheFilter
    {
        public bool Exclude(CurrentRequest request)
        {
            return (request.Path.IsNotEmpty() && request.Path.StartsWith("/" + DiagnosticUrlPolicy.DIAGNOSTICS_URL_ROOT));
        }
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