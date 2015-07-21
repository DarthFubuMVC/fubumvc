using System.Collections.Generic;
using System.ComponentModel;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration
{
    [Description("Registers scheduled job services to the application service container")]
    public class RegisterScheduledJobs : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var scheduledJobs = graph.Settings.Get<ScheduledJobGraph>();
            scheduledJobs.Jobs.Each(x => {
                graph.Services.SetServiceIfNone(typeof(IScheduledJob<>).MakeGenericType(x.JobType), ObjectDef.ForValue(x));
            });
        }
    }
}