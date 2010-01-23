using System;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Registration.DSL
{
    public class UrlRegistryExpression
    {
        private readonly Action<IUrlRegistrationConvention> _register;
        private readonly IUrlRegistration _registration;

        public UrlRegistryExpression(Action<IUrlRegistrationConvention> register, IUrlRegistration registration)
        {
            _register = register;
            _registration = registration;
        }

        public UrlRegistryExpression AddConvention<T>() where T : IUrlRegistrationConvention, new()
        {
            return AddConvention(new T());
        }

        public UrlRegistryExpression AddConvention(IUrlRegistrationConvention convention)
        {
            _register(convention);
            return this;
        }

        public UrlRegistryExpression Register(Action<IUrlRegistration> configure)
        {
            configure(_registration);
            return this;
        }
    }
}