using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.ServiceBus.Registration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Messages;

namespace FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration
{
    public class ScheduledJobHandlerSource : IHandlerSource
    {
        public readonly IList<Type> JobTypes = new List<Type>(); 

        public IEnumerable<HandlerCall> FindCalls(Assembly applicationAssembly)
        {
            return JobTypes.SelectMany(handlersForJob).ToArray();

        }

        private IEnumerable<HandlerCall> handlersForJob(Type jobType)
        {
            yield return typeof (ScheduledJobHandlerCall<>).CloseAndBuildAs<HandlerCall>(jobType);
            yield return HandlerCall.For(typeof (SchedulingHandler<>), jobType, "Reschedule");
        } 
    }
}