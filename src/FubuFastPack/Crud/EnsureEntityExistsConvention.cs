using System.Linq;
using FubuCore;
using FubuFastPack.Domain;
using FubuMVC.Core.Registration;
using System.Collections.Generic;

namespace FubuFastPack.Crud
{
    public class EnsureEntityExistsConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.FirstActions().Where(x => x.InputType().CanBeCastTo<DomainEntity>()).Each(call =>
            {
                call.WrapWith(typeof(EnsureEntityExistsBehavior<>), call.InputType());
            });
        }
    }
}