using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Services
{
    /// <summary>
    /// Strictly used within the main Program
    /// </summary>
    public class JasperServiceRuntime
    {
        private readonly JasperServiceConfiguration _configuration;
        private readonly Lazy<IApplicationLoader> _runner;
        private IDisposable _shutdown;

        public JasperServiceRuntime(JasperServiceConfiguration configuration)
        {
            _configuration = configuration;
            _runner = new Lazy<IApplicationLoader>(bootstrap);
        }

        private IApplicationLoader Runner
        {
            get { return _runner.Value; }
        }

        private IApplicationLoader bootstrap()
        {
            return ApplicationLoaderFinder.FindLoader(_configuration.BootstrapperType);
        }

        public void Start()
        {
            Console.WriteLine("Starting application from " + Runner);
            _shutdown = Runner.Load(new Dictionary<string, string>());
        }

        public void Stop()
        {
            _shutdown.Dispose();
        }
    }


}