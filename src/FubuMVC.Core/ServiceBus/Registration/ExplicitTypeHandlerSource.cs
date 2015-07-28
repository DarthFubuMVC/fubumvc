using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

        public Task<HandlerCall[]> FindCalls(Assembly applicationAssembly)
        {
            return Task.Factory.StartNew(() =>
            {
                return _types.SelectMany(type =>
                {
                    return type.GetMethods().Where(HandlerCall.IsCandidate)
                        .Select(method => new HandlerCall(type, method));
                }).ToArray();
            });
        }
    }
}