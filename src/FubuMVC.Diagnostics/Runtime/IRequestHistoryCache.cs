using System;
using System.Collections.Generic;
using FubuMVC.Core;

namespace FubuMVC.Diagnostics.Runtime
{
    public interface IRequestHistoryCache
    {
        void AddReport(IDebugReport report, CurrentRequest request);
        IEnumerable<IDebugReport> RecentReports();
    }

    public class RequestHistoryCache : IRequestHistoryCache
    {
        public RequestHistoryCache(DiagnosticsSettings settings)
        {
        }

        public void AddReport(IDebugReport report, CurrentRequest request)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDebugReport> RecentReports()
        {
            throw new NotImplementedException();
        }
    }
}