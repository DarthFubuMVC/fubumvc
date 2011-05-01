using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View.Activation
{
    public class PageActivator : IPageActivator
    {
        private readonly IServiceLocator _services;
        private readonly PageActivationRuleCache _ruleCache;

        public PageActivator(IServiceLocator services, PageActivationRuleCache ruleCache)
        {
            _services = services;
            _ruleCache = ruleCache;
        }

        public void Activate(IFubuPage page)
        {
            page.ServiceLocator = _services;

            _ruleCache.ActivationsFor(page.GetType()).Each(rule => rule.Activate(_services, page));
        }
    }
}