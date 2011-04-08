using System.Collections.Generic;

namespace FubuMVC.Core.Diagnostics
{
    public interface IRequestHistoryCache
    {
        void AddReport(IDebugReport report);
        IEnumerable<IDebugReport> RecentReports();
    }
}