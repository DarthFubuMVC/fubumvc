using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.Caching;

namespace FubuMVC.Core.Registration.Conventions
{
    [Title("Applies caching rules from the [Cache] attribute")]
    public class CacheAttributePolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(x => x.ResourceType() != null).Each(chain =>
            {
                chain.ResourceType().ForAttribute<CacheAttribute>(att => att.Alter(chain));
            });
        }
    }
}