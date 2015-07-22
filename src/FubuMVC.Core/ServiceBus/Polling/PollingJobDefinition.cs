using System;
using System.Linq.Expressions;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class PollingJobDefinition
    {
        public Type JobType { get; set; }
        public Type SettingType { get; set; }
        public Expression IntervalSource { get; set; }
        public ScheduledExecution ScheduledExecution { get; set; }

        public PollingJobDefinition()
        {
            ScheduledExecution = ScheduledExecution.WaitUntilInterval;
        }

        public Instance ToInstance()
        {
            var instance = new ConfiguredInstance(typeof(PollingJob<,>), JobType, SettingType);
            instance.Dependencies.Add(typeof(PollingJobDefinition), this);

            return instance;
        }

        public static PollingJobDefinition For<TJob, TSettings>(Expression<Func<TSettings, double>> intervalSource) where TJob : IJob
        {
            return new PollingJobDefinition
            {
                JobType = typeof(TJob),
                IntervalSource = intervalSource,
                SettingType = typeof(TSettings)
            };
        }
    }

    public enum ScheduledExecution
    {
        WaitUntilInterval,
        RunImmediately,
        Disabled
    }
}
