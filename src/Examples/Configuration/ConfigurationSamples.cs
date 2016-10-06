using System;
using FubuMVC.Core;
using StructureMap;

namespace Examples.Configuration
{
    // SAMPLE: MyAppSettings
    public class MyAppSettings
    {
        public Uri Incoming { get; set; }

        public string FileShare { get; set; }

        public int PauseOnFailuresInMilliseconds { get; set; }
            = 2000;
    }
    // ENDSAMPLE

    public class UseConfiguration
    {
        // SAMPLE: retrieve-settings
        public void retrieve_settings()
        {
            var runtime = FubuRuntime.Basic();

            // retrieve the settings from the
            // underlying IoC container
            var settings = runtime.Get<MyAppSettings>();
        }
        // ENDSAMPLE



        public void override_at_runtime()
        {
            var runtime = FubuRuntime.Basic();
            runtime.Get<MyAppSettings>()
                .FileShare = "~/otherplace";

            // or

            var settings = new MyAppSettings
            {
                FileShare = "~/otherplace"
            };

            runtime.Get<IContainer>().Inject(settings);
        }
    }

    // SAMPLE: injecting-settings
    public class HomeEndpoint
    {
        private readonly MyAppSettings _settings;

        public HomeEndpoint(MyAppSettings settings)
        {
            _settings = settings;
        }

        public string Index()
        {
            return $"The file share is at {_settings.FileShare}";
        }
    }
    // ENDSAMPLE

    // SAMPLE: using-alter-settings
    public class MyApp : FubuRegistry
    {
        public MyApp()
        {
            AlterSettings<MyAppSettings>(_ =>
            {
                // Override whatever the default value
                // and any possible app.config values
                // for this property
                _.PauseOnFailuresInMilliseconds = 5000;
            });

            // Or
            ReplaceSettings(new MyAppSettings
            {
                PauseOnFailuresInMilliseconds = 1000
            });
        }
    }
    // ENDSAMPLE


}