using System;
using System.Linq.Expressions;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Registration.DSL
{
    public class UrlRegistryExpression
    {
        private readonly FubuRegistry _registry;

        public UrlRegistryExpression(FubuRegistry registry)
        {
            _registry = registry;
        }

        public void Forward<TInput>(Type type, string category, Expression<Func<TInput, IUrlRegistry, string>> forward)
        {
            throw new NotImplementedException();    
        }
        
        public void Forward<TInput>(string category, Expression<Func<TInput, IUrlRegistry, string>> forward)
        {
            throw new NotImplementedException();    
        }

        public void Forward<TInput>(Expression<Func<TInput, IUrlRegistry, string>> forward)
        {
            throw new NotImplementedException();    
        }
    }
}