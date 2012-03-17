using System.Collections.Generic;
using System.Web;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Extensibility;
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

		public SparkActivator (ISparkTemplateRegistry templateRegistry, ISparkViewEngine engine)
		{
			_templateRegistry = templateRegistry;
			_engine = engine;
		}

		public void Activate (IEnumerable<IPackageInfo> packages, IPackageLog log)
		{
            log.Trace("Running {0}".ToFormat(GetType().Name));
			
            configureSparkSettings(log);
            setEngineDependencies(log);
		}

        // We need to get these settings from DSL and defaults
		private void configureSparkSettings (IPackageLog log)
		{
			var settings = (SparkSettings)_engine.Settings;

            settings.SetAutomaticEncoding(true);

			settings.AddAssembly (typeof(HtmlTag).Assembly)
                .AddAssembly (typeof(FubuPageExtensions).Assembly)
                .AddNamespace (typeof(VirtualPathUtility).Namespace) // System.Web
                .AddNamespace (typeof(FubuRegistryExtensions).Namespace) // FubuMVC.Spark
                .AddNamespace (typeof(FubuPageExtensions).Namespace) // FubuMVC.Core.UI
                .AddNamespace(typeof(ContentExtensions).Namespace) // FubuMVC.Core.UI.Extensibility
                .AddNamespace (typeof(HtmlTag).Namespace); // HtmlTags   

            log.Trace("Adding assemblies to SparkSettings:");
		    settings.UseAssemblies.Each(x => log.Trace("  - {0}".ToFormat(x)));

            log.Trace("Adding namespaces to SparkSettings:");
            settings.UseNamespaces.Each(x => log.Trace("  - {0}".ToFormat(x)));
		}

        private void setEngineDependencies(IPackageLog log)
        {
            var engine = (SparkViewEngine) _engine;

            engine.ViewFolder = new TemplateViewFolder(_templateRegistry.AllTemplates());
            log.Trace("Setting viewfolder [{0}] for view engine", _engine.ViewFolder.GetType().FullName);
            
            engine.DefaultPageBaseType = typeof(FubuSparkView).FullName;
            log.Trace("Setting page base type [{0}] for views", _engine.DefaultPageBaseType);

            engine.BindingProvider = new FubuBindingProvider(_templateRegistry);
            log.Trace("Setting binding provider [{0}] for view engine", engine.BindingProvider.GetType().FullName);
        }
	}
}