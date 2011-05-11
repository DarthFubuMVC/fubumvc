using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.UI;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using HtmlTags;
using Spark;
using Spark.Bindings;
using Spark.FileSystem;

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
	
	// Temporary : Going to be removed soon.
	public class FubuBindingProvider : BindingProvider
	{	
		private readonly ISparkTemplates _sparkTemplates;

		public FubuBindingProvider (ISparkTemplates sparkTemplates)
		{
			_sparkTemplates = sparkTemplates;
		}

		public override IEnumerable<Binding> GetBindings (IViewFolder viewFolder)
		{					
			var bindings = new List<Binding> ();
			
			foreach (var binding in _sparkTemplates.ByName("bindings").Where(i => i.IsXml())) {
				var file = viewFolder.GetViewSource (binding.ViewPath);
				using (var stream = file.OpenViewStream())
				using (var reader = new StreamReader(stream)) {
					bindings.AddRange (LoadStandardMarkup (reader));
				}					
			}		
			
			return bindings;
		}
	}
}