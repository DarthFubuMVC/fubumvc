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
    // NOTE: Nice thing about activation is that we get IoC - 
    //       So we can move tasks that not necessarily need to happen
    //       during bootstrap to activators.
    public class SparkActivator : IActivator
    {
        private readonly SparkItems _sparkItems;
        private readonly ISparkViewEngine _engine;
        public SparkActivator(SparkItems sparkItems, ISparkViewEngine engine)
        {
            _sparkItems = sparkItems;
            _engine = engine;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            var settings = (SparkSettings) _engine.Settings;
            settings.AddAssembly(typeof(HtmlTag).Assembly)
				.AddAssembly(typeof(FubuPageExtensions).Assembly)
                .AddNamespace(typeof(VirtualPathUtility).Namespace) // System.Web
				.AddNamespace(typeof(FubuRegistryExtensions).Namespace) // FubuMVC.Spark
				.AddNamespace(typeof(FubuPageExtensions).Namespace) // FubuMVC.Core.UI
				.AddNamespace(typeof(HtmlTag).Namespace); // HtmlTags
            _engine.ViewFolder = new SparkItemViewFolder(_sparkItems);
            _engine.DefaultPageBaseType = typeof (FubuSparkView).FullName;

        }
    }
}