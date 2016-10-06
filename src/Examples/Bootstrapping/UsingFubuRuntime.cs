using System.Security.Cryptography.X509Certificates;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;

namespace Examples.Bootstrapping
{
    public class UsingFubuRuntime
    {
        public void simplest_possible()
        {
            // SAMPLE: simplest-possible-bootstrapping
            // Start a simple FubuMVC application
            var runtime = FubuRuntime.Basic();

            // When you want to shut it down
            runtime.Dispose();
            // ENDSAMPLE
        }

        public void simple_bus()
        {
            // SAMPLE: simple-bus-bootstrapping
            var runtime = FubuRuntime.BasicBus();
            // ENDSAMPLE

        }

        public void configure_on_the_fly()
        {
            // SAMPLE: configure-app-with-basic
            var runtime = FubuRuntime.Basic(_ =>
            {
                _.Features.Diagnostics.Enable(TraceLevel.Verbose);
            });
            // ENDSAMPLE
        }

        public void using_fubu_registry()
        {
            // SAMPLE: using-fuburuntime
            // Bootstrap your application as is
            var runtime = FubuRuntime.For<MyApplication>();

            // or bootstrap your application as defined in
            // your FubuRegistry, but override some settings
            var runtime2 = FubuRuntime.For<MyApplication>(_ =>
            {
                _.Features.Diagnostics.Enable(TraceLevel.Production);
            });

            // This is an alternative syntax for the sample above
            // you may prefer instead
            var registry = new MyApplication();
            registry.Features.Diagnostics.Enable(TraceLevel.Production);
            var runtime3 = registry.ToRuntime();
            // ENDSAMPLE
        }
    }

    public class MyCustomActivator : IActivator
    {
        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            throw new System.NotImplementedException();
        }
    }

    // SAMPLE: Bootstrapping-MyApplication
    public class MyApplication : FubuRegistry
    {
        public MyApplication()
        {
            // If you don't like fubu's "Endpoint" naming
            // convention, use "Controller" to find http
            // action candidates instead
            Actions.DisableDefaultActionSource();
            Actions.IncludeClassesSuffixedWithController();

            Services.AddService<IActivator, MyCustomActivator>();
        }
    }
    // ENDSAMPLE
}