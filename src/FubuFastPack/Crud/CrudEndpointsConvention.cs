using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuFastPack.Crud
{
    public class CrudEndpointsConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var crudActions = graph.Behaviors.Select(x => x.FirstCall()).Where(x => x != null)
                .Where(x => x.HandlerType.IsCrudController())
                .GroupBy(x => x.HandlerType);


            crudActions
                .Each(group =>
                {
                    var actions = new ActionCallSet(group);
                    var modifier = new CrudActionModifier(CrudTypeExtensions.GetEntityType(group.Key), CrudTypeExtensions.GetEditEntityModelType(group.Key));
                    modifier.ModifyChains(actions, graph);
                });
        }
    }
}