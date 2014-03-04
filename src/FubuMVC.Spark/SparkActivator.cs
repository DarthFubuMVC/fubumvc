using System.Collections.Generic;
using System.Web;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using HtmlTags;
using Spark;

namespace FubuMVC.Spark
{
    public class SparkActivator : IActivator
    {
        private readonly ISparkTemplateRegistry _templateRegistry;
        private readonly ISparkViewEngine _engine;
        private readonly CommonViewNamespaces _namespaces;
        readonly ITemplateDirectoryProvider<ITemplate> _directoryProvider;
        private readonly SparkEngineSettings _settings;

        public SparkActivator (ISparkTemplateRegistry templateRegistry, ISparkViewEngine engine, CommonViewNamespaces namespaces, ITemplateDirectoryProvider<ITemplate> directoryProvider, SparkEngineSettings settings)
        {
            _templateRegistry = templateRegistry;
            _engine = engine;
            _namespaces = namespaces;
            _directoryProvider = directoryProvider;
            _settings = settings;
        }

        public void Activate (IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            log.Trace("Running {0}".ToFormat(GetType().Name));
            
            configureSparkSettings(log);
            setEngineDependencies(log);
        }

        private void configureSparkSettings (IPackageLog log)
        {
            var settings = (SparkSettings)_engine.Settings;

            settings.SetAutomaticEncoding(true);

            settings.AddAssembly (typeof(HtmlTag).Assembly)
                .AddAssembly(typeof(IPartialInvoker).Assembly)
                .AddNamespace (typeof(IPartialInvoker).Namespace)
                .AddNamespace (typeof(VirtualPathUtility).Namespace) // System.Web
                .AddNamespace (typeof(SparkViewFacility).Namespace) // FubuMVC.Spark
                .AddNamespace (typeof(HtmlTag).Namespace); // HtmlTags   

            _namespaces.Namespaces.Each(x => settings.AddNamespace(x));

            _settings.UseNamespaces.Each(ns => settings.AddNamespace(ns));

            log.Trace("Adding assemblies to SparkSettings:");
            settings.UseAssemblies.Each(x => log.Trace("  - {0}".ToFormat(x)));

            log.Trace("Adding namespaces to SparkSettings:");
            settings.UseNamespaces.Each(x => log.Trace("  - {0}".ToFormat(x)));
        }

        private void setEngineDependencies(IPackageLog log)
        {
            var engine = (SparkViewEngine) _engine;

            engine.ViewFolder = new TemplateViewFolder(_templateRegistry);
            log.Trace("Setting viewfolder [{0}] for view engine", _engine.ViewFolder.GetType().FullName);
            
            engine.DefaultPageBaseType = typeof(FubuSparkView).FullName;
            log.Trace("Setting page base type [{0}] for views", _engine.DefaultPageBaseType);

            engine.BindingProvider = new FubuBindingProvider(_templateRegistry);
            log.Trace("Setting binding provider [{0}] for view engine", engine.BindingProvider.GetType().FullName);

            engine.PartialProvider = new FubuPartialProvider(_directoryProvider);
            log.Trace("Setting partial provider [{0}] for view engine", engine.PartialProvider.GetType().FullName);
        }
    }
}