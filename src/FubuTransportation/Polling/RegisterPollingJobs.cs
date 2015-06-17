using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuTransportation.Polling
{
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