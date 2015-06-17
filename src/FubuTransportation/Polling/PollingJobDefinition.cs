using System;
using System.Linq.Expressions;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuTransportation.Polling
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

        public ObjectDef ToObjectDef()
        {
            var def = new ObjectDef(typeof(PollingJob<,>), JobType, SettingType);
            def.DependencyByValue(this);

            return def;
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
