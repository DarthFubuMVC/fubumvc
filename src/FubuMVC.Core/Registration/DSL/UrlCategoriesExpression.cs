using System;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Registration.DSL
{
    public class UrlCategoriesExpression
    {
        private readonly Action<IUrlCategoryConvention> _register;
        private readonly IUrlRegistration _registration;

        public UrlCategoriesExpression(Action<IUrlCategoryConvention> register, IUrlRegistration registration)
        {
            _register = register;
            _registration = registration;
        }

        public UrlCategoriesExpression Add<T>() where T : IUrlCategoryConvention, new()
        {
            return Add(new T());
        }

        public UrlCategoriesExpression Add(IUrlCategoryConvention convention)
        {
            _register(convention);
            return this;
        }

        public UrlCategoriesExpression Register(Action<IUrlRegistration> configure)
        {
            configure(_registration);
            return this;
        }
    }
}