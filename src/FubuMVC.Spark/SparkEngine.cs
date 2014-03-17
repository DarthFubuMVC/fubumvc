using System.Web;
using Bottles;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;
using Spark.Caching;

namespace FubuMVC.Spark
{
    public class SparkEngine : IFubuRegistryExtension
    {

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            var engine = new SparkViewEngine();

            registry.AlterSettings<ViewEngineSettings>(x => x.AddFacility(new SparkViewFacility(engine)));

            registry.Services(configureServices);
        }

        private void configureServices(ServiceRegistry services)
        {
            services.SetServiceIfNone<ISparkViewEngine>(new SparkViewEngine());
            services.SetServiceIfNone<ICacheService>(new DefaultCacheService(HttpRuntime.Cache));

            services.FillType<IActivator, SparkActivator>();

            services.SetServiceIfNone<IHtmlEncoder, DefaultHtmlEncoder>();

        }
    }
}