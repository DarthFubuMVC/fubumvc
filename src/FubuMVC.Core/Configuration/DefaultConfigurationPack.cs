using FubuMVC.Core.Ajax;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Resources.PathBased;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Configuration
{
    public class DefaultConfigurationPack : ConfigurationPack
    {
        public DefaultConfigurationPack()
        {
            For(ConfigurationType.Discovery);
            if (FubuMode.InDevelopment())
            {
                Add<RegisterAbout>();
            }

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

            For(ConfigurationType.Conneg);
            Add<JsonMessageInputConvention>();
            Add<AjaxContinuationPolicy>();
            Add<DictionaryOutputConvention>();
            Add<HtmlStringOutputPolicy>();
            Add<StringOutputPolicy>();
            Add<HtmlTagOutputPolicy>();

            For(ConfigurationType.Attachment);
            Add<DefaultOutputPolicy>();
            Add<AttachAuthorizationPolicy>();
            Add<AttachInputPolicy>();
            Add<AttachOutputPolicy>();

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