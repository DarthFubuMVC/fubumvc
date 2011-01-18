using FubuMVC.Core;
using FubuMVC.Core.UI.Tags;
using FubuMVC.StructureMap;
using FubuMVC.StructureMap.Bootstrap;
using Microsoft.Practices.ServiceLocation;
using Spark.Web.FubuMVC.ViewCreation;
using StructureMap;

namespace Spark.Web.FubuMVC.Registration
{
    public class SparkStructureMapApplication : FubuStructureMapApplication
    {
    	private static SparkViewFactory _factory;

        protected virtual SparkSettings GetSparkSettings()
        {
            return new SparkSettings()
                .AddAssembly(typeof(PartialTagFactory).Assembly)
                .AddNamespace("Spark.Web.FubuMVC")
                .AddNamespace("FubuMVC.Core.UI")
                .AddNamespace("HtmlTags");
        }

		protected virtual SparkViewFactory GetSparkFactory()
		{
			return _factory ?? (_factory = new SparkViewFactory(GetSparkSettings()));
		}

    	protected override void InitializeStructureMap(IInitializationExpression ex)
        {
            ex.For<SparkViewFactory>().Use(_factory);
            ex.For<IServiceLocator>().Use<StructureMapServiceLocator>();
            ex.For<ISparkSettings>().Use(_factory.Settings);
            ex.For(typeof(ISparkViewRenderer<>)).Use(typeof(SparkViewRenderer<>));
        }

        public override FubuRegistry GetMyRegistry()
        {
			return new SparkFubuRegistry(_factory);
        }
    }

}