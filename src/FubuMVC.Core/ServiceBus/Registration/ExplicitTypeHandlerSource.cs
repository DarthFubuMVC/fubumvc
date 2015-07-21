using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.ServiceBus.Registration.Nodes;

namespace FubuMVC.Core.ServiceBus.Registration
{
    public class ExplicitTypeHandlerSource : IHandlerSource
    {
        private readonly IList<Type> _types = new List<Type>();

        public ExplicitTypeHandlerSource(params Type[] types)
        {
            _types.Fill(types);
        }

        public IEnumerable<HandlerCall> FindCalls(Assembly applicationAssembly)
        {
            foreach (var type in _types)
            {
                foreach (var method in type.GetMethods().Where(HandlerCall.IsCandidate))
                {
                    yield return new HandlerCall(type, method);
                }
            }
        }
    }
}