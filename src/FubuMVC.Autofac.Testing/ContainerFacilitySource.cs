using System;

using Autofac;

using FubuCore;
using FubuCore.Binding;

using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;


namespace FubuMVC.Autofac.Testing
{
    public static class ContainerFacilitySource
    {
        public static IServiceFactory New(Action<IContainerFacility> configure)
        {
            var builder = new ContainerBuilder();
            var facility = new AutofacContainerFacility(builder.Build());
            configure(facility);

            // A ContainerFacility cannot be considered "ready" for business until BuildFactory() has been called.
            return facility.BuildFactory();
        }

        public static IServiceLocator Services(Action<IContainerFacility> configure)
        {
            var builder = new ContainerBuilder();
            var facility = new AutofacContainerFacility(builder.Build());
            configure(facility);

            // A ContainerFacility cannot be considered "ready" for business until BuildFactory() has been called.
            return facility.BuildFactory().Get<IServiceLocator>();
        }

        public static IActionBehavior BuildBehavior(ServiceArguments arguments, ObjectDef behaviorDef, Action<IContainerFacility> configuration)
        {
            var id = Guid.NewGuid();
            behaviorDef.Name = id.ToString();

            var facility = New(
                x =>
                {
                    configuration(x);

                    x.Register(typeof(IActionBehavior), behaviorDef);
                });

            var behavior = facility.BuildBehavior(arguments, id);

            // StartInnerBehavior() is not part of the core interface, but I had to have something to get at the real top level
            // behavior within the context of an Autofac nested container.
            return behavior.As<NestedAutofacContainerBehavior>().StartInnerBehavior();
        }
    }
}