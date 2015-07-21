using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.ServiceBus.Polling
{
    [Description("Adds the configured polling jobs to the application services")]
    public class RegisterPollingJobs : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var jobs = graph.Settings.Get<PollingJobSettings>().Jobs;

            jobs.Select(x => x.ToObjectDef())
                .Each(x => graph.Services.AddService(typeof(IPollingJob), x));
        }
    }
}