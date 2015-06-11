using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;

namespace FubuMVC.Tests.Docs.Topics.Authorization
{

    // SAMPLE: allowrole-rolemodel
    public class RoleModelAuthorizationPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions()
                 .Where(x => x.HasInput && x.InputType().Name.EndsWith("RoleModel"))
                 .Each(x =>
                 {
                     var roleName = x.InputType().Name.Replace("RoleModel", string.Empty);
                     x.ParentChain().Authorization.AddRole(roleName);
                 });
        }
    }
    // ENDSAMPLE
}