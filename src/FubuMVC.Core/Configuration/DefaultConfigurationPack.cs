using FubuMVC.Core.Caching;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Resources.PathBased;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Attachment;

namespace FubuMVC.Core.Configuration
{
    public class DefaultConfigurationPack : ConfigurationPack
    {
        public DefaultConfigurationPack()
        {
            For(ConfigurationType.Policy);
            Add<ViewAttacher>();
            Add<ActionlessViewConvention>();
            Add<AutoImportModelNamespacesConvention>();

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