using System;
using System.Collections.Generic;
using Bottles;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
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
            _scanner = new SparkScanner(new FileSystem());

            services();
        }

        public void Configure(FubuRegistry registry)
        {
            // Note: We could have just one entry configuration 
            //       and have our own policies that we put into that.
            
            registry.Policies.Add<SparkScannerConfiguration>();
            registry.Policies.Add(new SparkFilesConfiguration(_sparkFiles));

            registry.Import(this, string.Empty);
        }

        private void services()
        {
            Services(s =>
            {
                s.SetServiceIfNone(_sparkFiles);

                //_viewFacility = new SparkViewFacility();
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

    // NOTE: I think we could only use one IConfigurationAction to put on the parent registry.

    public class SparkScannerConfiguration : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            throw new NotImplementedException();
        }
    }

    public class SparkFilesConfiguration : IConfigurationAction
    {
        private readonly SparkFiles _sparkFiles;

        public SparkFilesConfiguration(SparkFiles sparkFiles)
        {
            _sparkFiles = sparkFiles;
        }

        public void Configure(BehaviorGraph graph)
        {
            throw new NotImplementedException();
        }
    }
}