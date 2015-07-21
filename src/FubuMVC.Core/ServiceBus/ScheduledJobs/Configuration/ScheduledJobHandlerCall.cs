using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration
{
    public class ScheduledJobHandlerCall<T> : HandlerCall where T : IJob
    {
        private static MethodInfo method()
        {
            return ReflectionHelper.GetMethod<ScheduledJobRunner<T>>(x => x.Execute(null));
        }

        public ScheduledJobHandlerCall()
            : base(typeof(ScheduledJobRunner<T>), method())
        {
        }
    }

}