using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics
{
    public interface IRequestHistoryCache
    {
        void AddReport(IDebugReport report, CurrentRequest request);
        IEnumerable<IDebugReport> RecentReports();
    }
}