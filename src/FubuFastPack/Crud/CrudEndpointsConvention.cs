using System.Collections.Generic;
using FubuMVC.Core.Registration;

namespace FubuFastPack.Crud
{
    public class CrudEndpointsConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var handlerSets = graph.HandlerSetsFor(t => t.IsCrudController());
            handlerSets.Each(@set =>
            {
                var modifier = new CrudActionModifier(@set.HandlerType);
                modifier.ModifyChains(@set, graph);
            });
        }
    }
}