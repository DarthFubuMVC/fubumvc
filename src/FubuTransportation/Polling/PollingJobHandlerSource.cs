using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuTransportation.Registration;
using FubuTransportation.Registration.Nodes;

namespace FubuTransportation.Polling
{
    // Tested through integration tests
    public class PollingJobHandlerSource : IHandlerSource
    {
        private readonly IList<Type> _jobTypes = new List<Type>();
 
        public void AddJobType(Type type)
        {
            _jobTypes.Add(type);
        }

        public IEnumerable<HandlerCall> FindCalls(Assembly applicationAssembly)
        {
            return _jobTypes.Select(x => {
                var handlerType = typeof (JobRunner<>).MakeGenericType(x);
                var method = handlerType.GetMethod("Run");

                return new HandlerCall(handlerType, method);
            });
        }

        public bool HasAny()
        {
            return _jobTypes.Any();
        }


    }
}