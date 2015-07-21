using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Runtime.Routing;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration
{
    public class ScheduledJobRoutingRule<T> : IRoutingRule where T : IJob
    {
        public bool Matches(Type type)
        {
            return type == typeof (ExecuteScheduledJob<T>) || type == typeof(RescheduleRequest<T>);
        }

        public string Describe()
        {
            return "Executes Scheduled job: " + typeof (T).GetFullName();
        }
    }
}