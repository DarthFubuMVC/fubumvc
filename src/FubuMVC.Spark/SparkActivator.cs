using System.Collections.Generic;
using System.Web;
using System.IO;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core.UI;
using FubuCore;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using HtmlTags;
using Spark;
using Spark.Bindings;
using Spark.FileSystem;
using S = Spark;
using System.Linq;

namespace FubuMVC.Spark
{
    public class SparkActivator : IActivator
    {
        private readonly ISparkItems _sparkItems;
        private readonly ISparkViewEngine _engine;

        public SparkActivator(ISparkItems sparkItems, ISparkViewEngine engine)
        {
            _sparkItems = sparkItems;
            _engine = engine;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            sparkSettings();

            _engine.ViewFolder = new SparkItemViewFolder(_sparkItems);
            _engine.DefaultPageBaseType = typeof(FubuSparkView).FullName;
			((SparkViewEngine)_engine).BindingProvider = new FubuBindingProvider(_sparkItems);
        }

        private void sparkSettings()
        {
            var settings = (SparkSettings)_engine.Settings;
            
			settings.AddAssembly(typeof(HtmlTag).Assembly)
                .AddAssembly(typeof(FubuPageExtensions).Assembly)
                .AddNamespace(typeof(VirtualPathUtility).Namespace) // System.Web
                .AddNamespace(typeof(FubuRegistryExtensions).Namespace) // FubuMVC.Spark
                .AddNamespace(typeof(FubuPageExtensions).Namespace) // FubuMVC.Core.UI
                .AddNamespace(typeof(HtmlTag).Namespace); // HtmlTags   
        }
    }
	
	// Temporary : Needs discussion + test coverage (if used)
	public class FubuBindingProvider : BindingProvider
    {	
		private readonly ISparkItems _sparkItems;
        public FubuBindingProvider(ISparkItems sparkItems)
        {
            _sparkItems = sparkItems;
        }
		
        public override IEnumerable<Binding> GetBindings(IViewFolder viewFolder)
        {					
			var bindings = new List<Binding>();
			
			foreach (var binding in _sparkItems.ByName("bindings")) 
			{
			    var file = viewFolder.GetViewSource(binding.ViewPath);
            	using (var stream = file.OpenViewStream())
				using (var reader = new StreamReader(stream))
                {
					bindings.AddRange(LoadStandardMarkup(reader));
                }					
			}		
			
			return bindings;
        }
    }
}