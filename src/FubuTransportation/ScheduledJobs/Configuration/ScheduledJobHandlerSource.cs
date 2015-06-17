using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuTransportation.Registration;
using FubuTransportation.Registration.Nodes;
using FubuTransportation.ScheduledJobs.Messages;

namespace FubuTransportation.ScheduledJobs.Configuration
{
    public class ScheduledJobHandlerSource : IHandlerSource
    {
        public readonly IList<Type> JobTypes = new List<Type>(); 

        public IEnumerable<HandlerCall> FindCalls()
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