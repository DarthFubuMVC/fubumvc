using System;
using FubuCore;
using FubuTransportation.Polling;
using FubuTransportation.Runtime.Routing;
using FubuTransportation.ScheduledJobs.Execution;

namespace FubuTransportation.ScheduledJobs.Configuration
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