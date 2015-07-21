using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;

namespace FubuMVC.Core.ServiceBus
{
    public static class HandlerExtensions
    {
        public static IEnumerable<HandlerCall> Handlers(this BehaviorGraph graph)
        {
            var handlerGraph = graph.Settings.Get<HandlerGraph>();
            return handlerGraph.SelectMany(behavior => behavior.OfType<HandlerCall>().Distinct());
        }
    }
}