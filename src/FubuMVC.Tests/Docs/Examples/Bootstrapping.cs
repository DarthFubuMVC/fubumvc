using System;
using System.Collections.Generic;
using System.Web;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.StructureMap;

namespace FubuMVC.Tests.Docs.Examples
{
    
    public static class Bootstrapper
    {
        public static void Bootstrap()
        {
            // SAMPLE: bootstrapping
            FubuApplication
                // Only going to use the default
                // conventions and policies
                .DefaultPolicies()

                // Starting with a brand new
                // StructureMap container.
                .StructureMap()

                // Build it up!
                .Bootstrap();

            // ENDSAMPLE
        }
    }



    // SAMPLE: bootstrapping-simplest-possible
    public class SimpleApplicationSource : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication
                .DefaultPolicies()
                .StructureMap();
        }
    }
    // ENDSAMPLE


    // SAMPLE: bootstrapping-with-asp-net
    public class Global : HttpApplication
    {
        private FubuRuntime _runtime;

        protected void Application_Start(object sender, EventArgs e)
        {
            // Start the runtime at application start
            _runtime = FubuApplication
                .BootstrapApplication<SimpleApplicationSource>();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Clean up after yourself when the application is
            // unloading!!!
            _runtime.Dispose();
        }
    }
    // ENDSAMPLE

    // SAMPLE: bootstrapping-custom-policies
    public class CustomApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication
                .For<CustomFubuRegistry>()
                .StructureMap();
        }
    }


    public class CustomFubuRegistry : FubuRegistry
    {
        public CustomFubuRegistry()
        {
            // I want to name my action classes with the
            // "Controller" suffix
            Actions.IncludeClassesSuffixedWithController();

            // Registering a custom policy
            Policies.Add<MyCustomPolicy>();

            // You *can* also declare IoC service
            // registrations in your FubuRegistry
            Services(x => {
                x.AddService<IActivator, MyCustomActivator>();
            });
        }
    }

    public class MyCustomPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            // insert new behaviors for some sort of
            // cross cutting concern
        }
    }

    public class MyCustomActivator : IActivator
    {
        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            // do something as the application starts up, 
            // but after the IoC container registrations are
            // completely baked in
        }
    }
    // ENDSAMPLE
}