using System;
using System.Collections;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;

namespace FubuMVC.Windsor
{
    /// <summary>
    /// Lazy component loader allows to resolve concrete class from windsor container withoout explicit registration
    /// </summary>
    public class WindsorLazyComponentLoader : ILazyComponentLoader
    {
        public IRegistration Load(string name, Type service, IDictionary arguments)
        {
            if (!service.IsClass || service.IsAbstract)
            {
                return null;
            }
            return Component.For(service).ImplementedBy(service).Named(name).DependsOn(arguments);
        }
    }
}