using System;
using System.Linq.Expressions;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class PollingJobDefinition
    {
        public Type JobType { get; private set; }
        public Type SettingType { get; private set; }
        public Expression IntervalSource { get; private set; }
        public ScheduledExecution ScheduledExecution { get; set; }

        public PollingJobDefinition(Type jobType, Type settingType, Expression intervalSource)
        {
            if (jobType == null) throw new ArgumentNullException("jobType");
            if (settingType == null) throw new ArgumentNullException("settingType");
            if (intervalSource == null) throw new ArgumentNullException("intervalSource");

            ScheduledExecution = ScheduledExecution.WaitUntilInterval;
            JobType = jobType;
            SettingType = settingType;
            IntervalSource = intervalSource;
        }

        public Instance ToInstance()
        {
            var instance = new ConfiguredInstance(typeof(PollingJob<,>), JobType, SettingType);
            instance.Dependencies.Add(typeof(PollingJobDefinition), this);

            return instance;
        }

        public static PollingJobDefinition For<TJob, TSettings>(Expression<Func<TSettings, double>> intervalSource) where TJob : IJob
        {
            return new PollingJobDefinition(typeof (TJob), typeof (TSettings), intervalSource);
        }
    }

    public enum ScheduledExecution
    {
        WaitUntilInterval,
        RunImmediately,
        Disabled
    }
}
