using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Spark.Scanning;

namespace FubuMVC.Spark
{
    public class SparkRegistry : FubuRegistry, IFubuRegistryExtension
    {
        private IViewFacility _viewFacility;

        private readonly SparkFiles _sparkFiles;
        private readonly IList<IScanSource> _sources;
        private readonly ISparkScanner _scanner;

        public SparkRegistry()
        {
            _sources = new List<IScanSource>();
            _sparkFiles = new SparkFiles();
            _scanner = new SparkScanner(new FileSystem(), new SparkFileComposer(Enumerable.Empty<ISparkFileAlteration>()));

            services();
        }

        public void Configure(FubuRegistry registry)
        {
            registry.Policies.Add(new SparkConfiguration(_sparkFiles));
            registry.Import(this, string.Empty);
        }

        private void services()
        {
            Services(s =>
            {
                s.SetServiceIfNone(_sparkFiles);

                //_viewFacility = new SparkViewFacility(...);
                s.AddService(_viewFacility);
            });
        }

        // TBD
        public void UsingDefaultSources()
        {
            AddSource<WebRootSource>();
            AddSource(new PackagesSource(PackageRegistry.Packages));
        }
        // TBD
        public void AddSource<T>() where T : IScanSource, new()
        {
            _sources.Add(new T());
        }
        // TBD
        public void AddSource(IScanSource source)
        {
            _sources.Add(source);
        }
    }

    public class SparkConfiguration : IConfigurationAction
    {
        private readonly SparkFiles _sparkFiles;
        public SparkConfiguration(SparkFiles sparkFiles)
        {
            _sparkFiles = sparkFiles;
        }

        public void Configure(BehaviorGraph graph)
        {
        }
    }
}