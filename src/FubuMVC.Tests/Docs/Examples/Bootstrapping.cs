using System;
using System.Web;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;

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
                

                // Build it up!
                .Bootstrap();

            // ENDSAMPLE
        }
    }


    // SAMPLE: bootstrapping-simplest-possible
    public class SimpleApplicationSource : IApplicationSource
    {
        public FubuApplication BuildApplication(string directory)
        {
            return FubuApplication
                .DefaultPolicies()
                ;
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
        public FubuApplication BuildApplication(string directory)
        {
            return FubuApplication
                .For<CustomFubuRegistry>()
                ;
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
            Policies.Local.Add<MyCustomPolicy>();

            // You *can* also declare IoC service
            // registrations in your FubuRegistry
            Services.AddService<IActivator, MyCustomActivator>();
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
        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            // do something as the application starts up, 
            // but after the IoC container registrations are
            // completely baked in
        }
    }

    // ENDSAMPLE
}