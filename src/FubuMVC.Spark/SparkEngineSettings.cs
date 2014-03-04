using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark
{
    public class SparkEngineSettings
    {
        private CompositeAction<TemplateComposer<ITemplate>> _configurations = new CompositeAction<TemplateComposer<ITemplate>>();
        private Func<bool> _precompileViews; 

        public SparkEngineSettings()
        {
            defaultSearch();
            defaultComposer();

            _precompileViews = () => !FubuMode.InDevelopment();
        }

        private void defaultSearch()
        {
            Search = new FileSet { DeepSearch = true };
            Search.AppendInclude("*{0}".ToFormat(Constants.DotSpark));
            Search.AppendInclude("*{0}".ToFormat(Constants.DotShade));
            Search.AppendInclude("bindings.xml");
            Search.AppendExclude("bin/*.*");
            Search.AppendExclude("obj/*.*");
        }

        private void defaultComposer()
        {
            Register(composer => composer
                .AddBinder<ViewDescriptorBinder>()
                .AddBinder<GenericViewModelBinder<ITemplate>>()
                .AddBinder<ViewModelBinder<ITemplate>>()
                .Apply<NamespacePolicy>()
                .Apply<ViewPathPolicy<ITemplate>>());            
        }

        public void Register(Action<TemplateComposer<ITemplate>> alteration)
        {
            _configurations += alteration;
        }

        public void ResetComposerConfiguration()
        {
            _configurations = new CompositeAction<TemplateComposer<ITemplate>>();
        }

        public void Configure(TemplateComposer<ITemplate> composer)
        {
            _configurations.Do(composer);
        }

        /// <summary>
        /// List of namespaces that will be applied as global namespaces to the SparkViewEngine
        /// </summary>
        public readonly IList<string> UseNamespaces = new List<string>();

        /// <summary>
        /// Adds a namespace to UseNamespaces
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UseNamespaceIncludingType<T>()
        {
            UseNamespaces.Add(typeof(T).Name);
        }

        public FileSet Search { get; private set; }

        /// <summary>
        /// By default, FubuMVC.Spark will precompile views if FubuMode.InDevelopment() is false.
        /// </summary>
        public bool PrecompileViews
        {
            get { return _precompileViews(); }
            set { _precompileViews = () => value; }
        }
    }
}