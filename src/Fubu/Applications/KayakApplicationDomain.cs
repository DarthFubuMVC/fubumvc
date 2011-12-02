using System;
using FubuMVC.Core;

namespace Fubu.Applications
{
    public class KayakApplicationDomain : IApplicationDomain
    {
        private AppDomain _domain;
        private ApplicationRunner _runner;
        private readonly FubuMvcApplicationFileWatcher _watcher;
        private ApplicationSettings _currentSettings;

        public KayakApplicationDomain()
        {
            _watcher = new FubuMvcApplicationFileWatcher(this);
        }

        public ApplicationStartResponse Start(ApplicationSettings settings)
        {
            _currentSettings = settings;


            return startAppDomain(settings);
        }

        private ApplicationStartResponse startAppDomain(ApplicationSettings settings)
        {
            var setup = new AppDomainSetup
                        {
                            ApplicationName = "FubuMVC-Kayak-" + settings.Name + "-" + Guid.NewGuid(),
                            ConfigurationFile = "Web.config",
                            ShadowCopyFiles = "true",
                            PrivateBinPath = "bin",
                            ApplicationBase = settings.PhysicalPath
                        };

            Console.WriteLine("Starting a new AppDomain at " + setup.ApplicationBase);

            _domain = AppDomain.CreateDomain(setup.ApplicationName, null, setup);

            Type proxyType = typeof(ApplicationRunner);
            _runner =
                (ApplicationRunner)
                _domain.CreateInstanceAndUnwrap(proxyType.Assembly.FullName, proxyType.FullName);

            var response = _runner.StartApplication(settings);

            setupWatchers(settings, response);

            return response;
        }

        private void setupWatchers(ApplicationSettings settings, ApplicationStartResponse response)
        {
            _watcher.StartWatching(settings, response.BottleDirectories);
        }

        public void RecycleContent()
        {
            Console.WriteLine("Restarting the FubuMVC application");
            var response =  _runner.Recycle();
        
            explainResponse(response);

        }

        private static void explainResponse(RecycleResponse response)
        {
            if (response.Success)
            {
                Console.WriteLine("  Success!");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("  Failed!");
                Console.WriteLine(response.Message);
            }
        }

        public void RecycleDomain()
        {
            Teardown();
            var response = startAppDomain(_currentSettings);
            response.WriteReport(_currentSettings);
        }

        public void Teardown()
        {
            try
            {
                if (_runner != null) _runner.Dispose();
            }
            catch (Exception)
            {
            }

            _runner = null;
            if (_domain != null)
            {
                AppDomain.Unload(_domain);
                _domain = null;
            }
        }
    }
}