using FubuMVC.Core.Registration;
using System.Collections.Generic;

namespace FubuMVC.Core.Security
{
    public class AttachAuthorizationPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Each(x =>
            {
                if (!x.Authorization.HasRules()) return;
                x.Prepend(x.Authorization);
                x.Calls.Each(call => graph.Observer.RecordCallStatus(call, "Protected by authorization rules"));
            });
        }
    }
}