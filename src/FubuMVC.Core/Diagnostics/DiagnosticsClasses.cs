using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Diagnostics
{
    public interface IRequestHistoryCache
    {
        void AddReport(IDebugReport report);
        IEnumerable<IDebugReport> RecentReports();
    }

    public class RequestHistoryCache : IRequestHistoryCache
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

    public class DiagnosticBehaviorFactory : IBehaviorFactory
    {
        public DiagnosticBehaviorFactory(IBehaviorFactory inner)
        {
        }

        public IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
        {
            throw new NotImplementedException();
        }
    }
}