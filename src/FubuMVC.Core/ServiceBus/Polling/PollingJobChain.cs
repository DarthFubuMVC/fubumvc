using System;
using System.Linq.Expressions;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.Polling
{
    public class PollingJobChain : HandlerChain
    {
        public Type JobType { get; private set; }
        public Type SettingType { get; private set; }
        public Expression IntervalSource { get; private set; }
        public ScheduledExecution ScheduledExecution { get; set; }

        public PollingJobChain(Type jobType, Type settingType, Expression intervalSource)
        {
            if (jobType == null) throw new ArgumentNullException("jobType");
            if (settingType == null) throw new ArgumentNullException("settingType");
            if (intervalSource == null) throw new ArgumentNullException("intervalSource");

            ScheduledExecution = ScheduledExecution.WaitUntilInterval;
            JobType = jobType;
            SettingType = settingType;
            IntervalSource = intervalSource;

            Tags.Add(NoTracing);

            var handlerType = typeof (JobRunner<>).MakeGenericType(JobType);
            var method = handlerType.GetMethod("Run");
            AddToEnd(new HandlerCall(handlerType, method));
        }

        public override string Title()
        {
            return "Polling Job: " + JobType.Name;
        }

        public override bool IsPollingJob()
        {
            return true;
        }

        public Instance ToInstance()
        {
            var instance = new ConfiguredInstance(typeof (PollingJob<,>), JobType, SettingType);
            instance.Dependencies.Add(typeof (PollingJobChain), this);

            return instance;
        }

        public static PollingJobChain For<TJob, TSettings>(Expression<Func<TSettings, double>> intervalSource)
            where TJob : IJob
        {
            return new PollingJobChain(typeof (TJob), typeof (TSettings), intervalSource);
        }
    }

    public enum ScheduledExecution
    {
        WaitUntilInterval,
        RunImmediately,
        Disabled
    }
}