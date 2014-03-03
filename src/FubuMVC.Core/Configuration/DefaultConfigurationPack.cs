using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Resources.PathBased;

namespace FubuMVC.Core.Configuration
{
    public class DefaultConfigurationPack : ConfigurationPack
    {
        public DefaultConfigurationPack()
        {
            For(ConfigurationType.Attributes);
            Add<UrlPatternAttributeOnViewModelPolicy>();
            Add<ModifyChainAttributeConvention>();

            For(ConfigurationType.ModifyRoutes);
            Add<ResourcePathRoutePolicy>();
            Add<MissingRouteInputPolicy>();

            For(ConfigurationType.InjectNodes);
            Add<ContinuationHandlerConvention>();
            Add<AsyncContinueWithHandlerConvention>();
            Add<CachedPartialConvention>();
            Add<CacheAttributePolicy>();


            For(ConfigurationType.Reordering);
            Add<OutputBeforeAjaxContinuationPolicy>();

            Add(new ReorderBehaviorsPolicy
            {
                WhatMustBeBefore = node => node.Category == BehaviorCategory.Authentication,
                WhatMustBeAfter = node => node.Category == BehaviorCategory.Authorization
            });

            Add(new ReorderBehaviorsPolicy
            {
                WhatMustBeBefore = node => node is OutputCachingNode,
                WhatMustBeAfter = node => node is OutputNode
            });
        }
    }
}