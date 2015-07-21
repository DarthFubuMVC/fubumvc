using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.ServiceBus.Polling
{
    [ApplicationLevel]
    public class PollingJobSettings
    {
        private readonly Cache<Type, PollingJobDefinition> _jobs =
            new Cache<Type, PollingJobDefinition>(type => new PollingJobDefinition {JobType = type});

        public IEnumerable<PollingJobDefinition> Jobs
        {
            get { return _jobs; }
        }

        public PollingJobDefinition JobFor<T>() where T : IJob
        {
            return _jobs[typeof (T)];
        }
    }
}