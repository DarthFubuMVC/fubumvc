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
		private readonly ISparkTemplates _sparkTemplates;
		private readonly ISparkViewEngine _engine;

		public SparkActivator (ISparkTemplates sparkTemplates, ISparkViewEngine engine)
		{
			_sparkTemplates = sparkTemplates;
			_engine = engine;
		}

        // Reconsider
		public void Activate (IEnumerable<IPackageInfo> packages, IPackageLog log)
		{
			sparkSettings ();

			_engine.ViewFolder = new TemplateViewFolder (_sparkTemplates);
			_engine.DefaultPageBaseType = typeof(FubuSparkView).FullName;
			((SparkViewEngine)_engine).BindingProvider = new FubuBindingProvider (_sparkTemplates);
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