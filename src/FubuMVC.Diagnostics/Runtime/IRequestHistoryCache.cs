using System.Collections.Generic;
using FubuMVC.Core;

namespace FubuMVC.Diagnostics.Runtime
{
    public interface IRequestHistoryCache
    {
        void AddReport(IDebugReport report, CurrentRequest request);
        IEnumerable<IDebugReport> RecentReports();
    }
}