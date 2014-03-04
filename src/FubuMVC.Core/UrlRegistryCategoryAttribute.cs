using System;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core
{
    /// <summary>
    /// Explicitly adds a category to the Route of this endpoint.  Used to resolve or match
    /// url's or endpoints in usages like IUrlRegistry.UrlFor(model, category)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class UrlRegistryCategoryAttribute : ModifyChainAttribute
    {
        private readonly string _category;

        public UrlRegistryCategoryAttribute(string category)
        {
            _category = category;
        }

        public string Category
        {
            get { return _category; }
        }

        public override void Alter(ActionCall call)
        {
            call.ParentChain().As<RoutedChain>().UrlCategory.Category = Category;
        }
    }
}