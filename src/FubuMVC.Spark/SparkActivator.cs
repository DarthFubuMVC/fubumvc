using System.Collections.Generic;
using System.Web;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core.UI;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using HtmlTags;
using Spark;

namespace FubuMVC.Spark
{
	public class SparkActivator : IActivator
	{
		private readonly ITemplateRegistry _templateRegistry;
		private readonly ISparkViewEngine _engine;
	    private readonly IPackageLog _logger;

		public SparkActivator (ITemplateRegistry templateRegistry, ISparkViewEngine engine)
		{
			_templateRegistry = templateRegistry;
			_engine = engine;
            _logger = PackageRegistry.Diagnostics.LogFor(this);
		}

		public void Activate (IEnumerable<IPackageInfo> packages, IPackageLog log)
		{
			configureSparkSettings();
            setEngineDependencies();
		}

        // We need to get these settings from DSL and defaults
		private void configureSparkSettings ()
		{
			var settings = (SparkSettings)_engine.Settings;            
			settings.AddAssembly (typeof(HtmlTag).Assembly)
                .AddAssembly (typeof(FubuPageExtensions).Assembly)
                .AddNamespace (typeof(VirtualPathUtility).Namespace) // System.Web
                .AddNamespace (typeof(FubuRegistryExtensions).Namespace) // FubuMVC.Spark
                .AddNamespace (typeof(FubuPageExtensions).Namespace) // FubuMVC.Core.UI
                .AddNamespace (typeof(HtmlTag).Namespace); // HtmlTags   

            _logger.Trace("Adding assemblies to SparkSettings:");
		    settings.UseAssemblies.Each(x => _logger.Trace("  - {0}".ToFormat(x)));

            _logger.Trace("Adding namespaces to SparkSettings:");
            settings.UseNamespaces.Each(x => _logger.Trace("  - {0}".ToFormat(x)));
		}

        private void setEngineDependencies()
        {
            var engine = (SparkViewEngine) _engine;

            engine.ViewFolder = new TemplateViewFolder(_templateRegistry.AllTemplates());
            _logger.Trace("Setting viewfolder [{0}] for view engine", _engine.ViewFolder.GetType().FullName);
            
            engine.DefaultPageBaseType = typeof(FubuSparkView).FullName;
            _logger.Trace("Setting page base type [{0}] for views", _engine.DefaultPageBaseType);

            engine.BindingProvider = new FubuBindingProvider(_templateRegistry);
            _logger.Trace("Setting binding provider [{0}] for view engine", engine.BindingProvider.GetType().FullName);
        }
	}
}