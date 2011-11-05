using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Core.Configuration
{
    public class RemoveBasicDiagnostics : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(chain =>
            {
                var call = chain.FirstCall();

                return call != null 
                    && call.IsInternalFubuAction()
                    && call.HandlerType != typeof(AssetWriter);
                
            }).ToList()
            .Each(graph.RemoveChain);
        }
    }
}