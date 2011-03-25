using System.Linq;
using FubuCore;
using FubuFastPack.Domain;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using System.Collections.Generic;

namespace FubuFastPack.Crud
{
    public class EnsureEntityExistsConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(x => x.InputType().CanBeCastTo<DomainEntity>()).Each(endpoint =>
            {
                var actionCall = endpoint.FirstCall();
                var entityExistsType = typeof(EnsureEntityExistsBehavior<>).MakeGenericType(actionCall.InputType());
                actionCall.AddBefore(new Wrapper(entityExistsType));
            });
        }
    }
}