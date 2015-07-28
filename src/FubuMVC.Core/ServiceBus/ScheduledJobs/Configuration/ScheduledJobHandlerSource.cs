using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Registration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Messages;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration
{
    public class ScheduledJobHandlerSource : IHandlerSource
    {
        private readonly Type[] _jobTypes;

        public ScheduledJobHandlerSource(Type[] jobTypes)
        {
            _jobTypes = jobTypes;
        }

        public Task<HandlerCall[]> FindCalls(Assembly applicationAssembly)
        {
            return Task.Factory.StartNew(() => _jobTypes.SelectMany(handlersForJob).ToArray());
        }

        private IEnumerable<HandlerCall> handlersForJob(Type jobType)
        {
            yield return typeof (ScheduledJobHandlerCall<>).CloseAndBuildAs<HandlerCall>(jobType);
            yield return HandlerCall.For(typeof (SchedulingHandler<>), jobType, "Reschedule");
        } 
    }
}