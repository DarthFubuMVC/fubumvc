using System;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    public class SparkEngineSettings
    {
        private CompositeAction<TemplateComposer<ITemplate>> _configurations = new CompositeAction<TemplateComposer<ITemplate>>(); 

        public SparkEngineSettings()
        {
            defaultSearch();
            defaultComposerConfiguration();
        }

        private void defaultSearch()
        {
            Search = new FileSet { DeepSearch = true };
            Search.AppendInclude("*{0}".ToFormat(Constants.DotSpark));
            Search.AppendInclude("*{0}".ToFormat(Constants.DotShade));
            Search.AppendInclude("bindings.xml");                        
        }

        private void defaultComposerConfiguration()
        {
            ConfigureComposer(composer => composer
                .AddBinder<ViewDescriptorBinder>()
                .AddBinder<GenericViewModelBinder<ITemplate>>()
                .AddBinder<ViewModelBinder<ITemplate>>()
                .Apply<NamespacePolicy>()
                .Apply<ViewPathPolicy<ITemplate>>());            
        }

        public void ConfigureComposer(Action<TemplateComposer<ITemplate>> config)
        {
            _configurations += config;
        }

        // TODO: Ask around
        public void ResetComposerConfiguration()
        {
            _configurations = new CompositeAction<TemplateComposer<ITemplate>>();
        }

        public FileSet Search { get; private set; }
        public Action<TemplateComposer<ITemplate>> ComposerConfiguration { get { return _configurations.Do; } }
    }
}