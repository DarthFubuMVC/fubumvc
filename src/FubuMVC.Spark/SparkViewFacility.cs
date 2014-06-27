using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using HtmlTags;
using Spark;
using Spark.Caching;

namespace FubuMVC.Spark
{
    public class SparkViewFacility : ViewFacility<SparkTemplate>
    {
        private readonly SparkViewEngine _engine;
        private readonly SparkSettings _sparkSettings;

        public SparkViewFacility()
        {
            _engine = new SparkViewEngine();
            _sparkSettings = _engine.Settings.As<SparkSettings>();
        }

        protected override void registerWatchSettings(AssetSettings settings)
        {
            settings.ContentMatches.Add("*.spark");
            settings.ContentMatches.Add("bindings.xml");
        }

        protected override void addNamespacesForViews(CommonViewNamespaces namespaces)
        {
            
        }

        protected override void precompile(BehaviorGraph graph)
        {
            graph.Settings.Alter<SparkEngineSettings>(spark => {
                if (spark.PrecompileViews)
                {
                    AllTemplates().Each(x => x.Precompile());
                }
            });
        }

        public override Func<IFubuFile, SparkTemplate> CreateBuilder(SettingsCollection settings)
        {
            return file => new SparkTemplate(file, _engine);
        }

        public override FileSet FindMatching(SettingsCollection settings)
        {
            return settings.Get<SparkEngineSettings>().Search;
        }

        public override void Fill(ViewEngineSettings settings, BehaviorGraph graph)
        {
            configureNamespaces(graph);



            base.Fill(settings, graph);

            var bindingTemplates = graph.Files
                .FindFiles(FileSet.Shallow("Shared/bindings.xml"))
                .Select(x => new SparkTemplate(x, _engine)).ToArray();

            _engine.ViewFolder = new TemplateViewFolder(AllTemplates());
            _engine.DefaultPageBaseType = typeof (FubuSparkView).FullName;
            _engine.BindingProvider = new FubuBindingProvider(bindingTemplates);
            _engine.PartialProvider = new FubuPartialProvider(this);
        }

        private void configureNamespaces(BehaviorGraph graph)
        {
            _sparkSettings.SetAutomaticEncoding(true);

            _sparkSettings.AddAssembly(typeof (HtmlTag).Assembly)
                .AddAssembly(typeof (IPartialInvoker).Assembly)
                .AddNamespace(typeof (IPartialInvoker).Namespace)
                .AddNamespace(typeof (VirtualPathUtility).Namespace) // System.Web
                .AddNamespace(typeof (SparkViewFacility).Namespace) // FubuMVC.Spark
                .AddNamespace(typeof (HtmlTag).Namespace); // HtmlTags 

            var engineSettings = graph.Settings.Get<SparkEngineSettings>();
            engineSettings.UseNamespaces.Each(ns => _sparkSettings.AddNamespace(ns));
        }

        public override void ReadSharedNamespaces(CommonViewNamespaces namespaces)
        {
            namespaces.Namespaces.Each(x => _sparkSettings.AddNamespace(x));
        }

        protected override void registerServices(ServiceRegistry services)
        {
            // TODO -- this needs to change at some point
            services.SetServiceIfNone<ICacheService>(new DefaultCacheService(HttpRuntime.Cache));

            services.SetServiceIfNone<IHtmlEncoder, DefaultHtmlEncoder>();
        }
    }
}