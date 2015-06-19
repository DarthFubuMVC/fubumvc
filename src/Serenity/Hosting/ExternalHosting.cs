using FubuMVC.Core;

namespace Serenity.Hosting
{
    public class ExternalHosting : ISerenityHosting
    {
        public IApplicationUnderTest Start(ApplicationSettings settings, FubuRuntime runtime, IBrowserLifecycle lifecycle)
        {
            var application = new ApplicationUnderTest(runtime, settings, lifecycle);
            application.Ping();

            return application;
        }

        public void Shutdown()
        {
            // Nothing
        }
    }
}