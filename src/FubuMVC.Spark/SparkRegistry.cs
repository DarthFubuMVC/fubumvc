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

        // NOTE: not so sure about this
        // NOTE: Remember that we will need to support different policies on the "matching" from each package
        ///      not saying that this will not allow for it, but let us give this an extra thought.
        private readonly Builder<ActionCall, SparkFile> _matcher;

        public SparkRegistry()
        {
            _sources = new List<IScanSource>();
            _sparkFiles = new SparkFiles();
            _scanner = new SparkScanner(new FileSystem());
            _matcher = new Builder<ActionCall, SparkFile>(call => null);

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

                _viewFacility = new SparkViewFacility(_matcher.Build);
                s.AddService(_viewFacility);
            });
        }


        public void UsingDefaultSources()
        {
            AddSource<WebRootSource>();
            AddSource(new PackagesSource(PackageRegistry.Packages));
        }

        public void AddSource<T>() where T : IScanSource, new()
        {
            _sources.Add(new T());
        }
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