using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Resources.PathBased
{
    public class ResourcePathRoutePolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(x => x.InputType().CanBeCastTo<ResourcePath>())
                .Each(x => x.Route.Append(ResourcePath.UrlSuffix));
        }
    }
}