using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Diagnostics
{
    // needs to be a singleton
    public interface IRequestHistoryService
    {
        void AddReport(IDebugReport report);
        IEnumerable<IDebugReport> RecentReports();
    }

    public class RequestHistoryService : IRequestHistoryService
    {
        private readonly Queue<IDebugReport> _reports = new Queue<IDebugReport>();

        public void AddReport(IDebugReport report)
        {
            _reports.Enqueue(report);
            while (_reports.Count > 50)
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