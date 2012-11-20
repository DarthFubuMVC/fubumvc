using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Resources.PathBased
{
    [Title("Builds out the url pattern and route inputs for chains where the Input Type is derived from ResourceHash")]
    public class ResourcePathRoutePolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(x => x.InputType().CanBeCastTo<ResourcePath>())
                .Each(x =>
                {
                    if (!x.Route.Pattern.Contains(ResourcePath.UrlSuffix))
                    {
                        x.Route.Append(ResourcePath.UrlSuffix);
                    
                    
                        x.Route.RegisterRouteCustomization(r =>
                        {
                            r.Defaults.Add("Part0", null);
                            r.Defaults.Add("Part1", null);
                            r.Defaults.Add("Part2", null);
                            r.Defaults.Add("Part3", null);
                            r.Defaults.Add("Part4", null);
                            r.Defaults.Add("Part5", null);
                            r.Defaults.Add("Part6", null);
                            r.Defaults.Add("Part7", null);
                            r.Defaults.Add("Part8", null);
                            r.Defaults.Add("Part9", null);
                        });
                    }
                    
                });
        }
    }
}