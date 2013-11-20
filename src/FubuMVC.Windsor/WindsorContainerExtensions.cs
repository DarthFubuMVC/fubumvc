using System;
using Castle.Windsor;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Windsor
{
    public static  class WindsorContainerExtensions
    {
        /// <summary>
        /// Converts ObjecttDef to the array of IRegistrations and registers them all within container
        /// </summary>
        public static IWindsorContainer Register(this IWindsorContainer windsorContainer, Type serviceType, ObjectDef def)
        {
            var v = new WindsorDependencyVisitor(serviceType, def, false);
            windsorContainer.Register(v.Registrations());
            return windsorContainer;
        }
    }
}