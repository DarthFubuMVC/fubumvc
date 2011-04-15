using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Spark.Tokenization;
using FubuMVC.Spark.Tokenization.Model;
using FubuMVC.Spark.Tokenization.Scanning;

namespace FubuMVC.Spark
{
    public class SparkRegistry : FubuRegistry, IFubuRegistryExtension
    {
        private IViewFacility _viewFacility;

        private readonly SparkFiles _sparkFiles;
        private readonly IFileScanner _scanner;

        public SparkRegistry()
        {
            _sparkFiles = new SparkFiles();
            _scanner = null;

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

    // TODO: AAh.. Improve by a RegisterEnricher<> on tokenizer, and put it as default 
    public static class DefaultDependencies
    {
        public static IEnumerable<ISparkFileEnricher> Enrichers()
        {
            yield return new MasterPageEnricher();
            yield return new ViewModelEnricher();
            yield return new NamespaceEnricher();
        }
    }
}