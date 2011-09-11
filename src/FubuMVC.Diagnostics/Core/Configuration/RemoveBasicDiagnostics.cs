using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Diagnostics.Core.Configuration
{
    public class RemoveBasicDiagnostics : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph
                .Behaviors
                .Where(chain =>
                           {
                               if (chain.Route != null && chain.Route.Pattern.ToLower().Contains("_fubu"))
                               {
                                   var call = chain.FirstCall();
                                   if (call != null)
                                   {
                                       return BehaviorExtensions.IsInternalFubuAction(call);
                                   }
                               }

                               return false;
                           })
                .Each(chain =>
                          {
                              chain.IsPartialOnly = true;
                              chain.Route = RouteDefinition.Empty();
                          });
        }
    }
}