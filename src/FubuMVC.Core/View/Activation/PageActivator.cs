using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Core.View.Activation
{
    public class PageActivator : IPageActivator
    {
        private readonly IServiceLocator _services;
        private readonly PageActivationRules _rules;

        public PageActivator(IServiceLocator services, PageActivationRules rules)
        {
            _services = services;
            _rules = rules;
        }

        public void Activate(IFubuPage page)
        {
            page.ServiceLocator = _services;

            _rules.ActivationsFor(page.GetType()).Each(rule => rule.Activate(_services, page));
        }
    }
}