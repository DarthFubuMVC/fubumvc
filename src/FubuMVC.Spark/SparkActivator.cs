using System.Collections.Generic;
using System.Web;
using Bottles;
using Bottles.Diagnostics;
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

		public SparkActivator (ITemplateRegistry templateRegistry, ISparkViewEngine engine)
		{
			_templateRegistry = templateRegistry;
			_engine = engine;
		}

        // Reconsider
		public void Activate (IEnumerable<IPackageInfo> packages, IPackageLog log)
		{
			sparkSettings ();

			_engine.ViewFolder = new TemplateViewFolder (_templateRegistry.AllTemplates());
			_engine.DefaultPageBaseType = typeof(FubuSparkView).FullName;
			((SparkViewEngine)_engine).BindingProvider = new FubuBindingProvider (_templateRegistry);
		}

		private void sparkSettings ()
		{
			var settings = (SparkSettings)_engine.Settings;
            
			settings.AddAssembly (typeof(HtmlTag).Assembly)
                .AddAssembly (typeof(FubuPageExtensions).Assembly)
                .AddNamespace (typeof(VirtualPathUtility).Namespace) // System.Web
                .AddNamespace (typeof(FubuRegistryExtensions).Namespace) // FubuMVC.Spark
                .AddNamespace (typeof(FubuPageExtensions).Namespace) // FubuMVC.Core.UI
                .AddNamespace (typeof(HtmlTag).Namespace); // HtmlTags   
		}
	}
}