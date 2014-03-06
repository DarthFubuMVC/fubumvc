using System.Web;
using Bottles;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Model.Sharing;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;
using Spark.Caching;

namespace FubuMVC.Spark
{
    public class SparkEngine : IFubuRegistryExtension
    {
        private readonly Parsings _parsings = new Parsings();
        private readonly SparkTemplateRegistry _templateRegistry = new SparkTemplateRegistry();

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            var engine = new SparkViewEngine();

            registry.AlterSettings<ViewEngines>(x => x.AddFacility(new SparkViewFacility(_templateRegistry, _parsings, engine)));

            registry.Services(configureServices);
        }

        private void configureServices(ServiceRegistry services)
        {
            services.ReplaceService<ISparkTemplateRegistry>(_templateRegistry);
            services.ReplaceService<ITemplateRegistry<ISparkTemplate>>(_templateRegistry);
            services.ReplaceService<IParsingRegistrations<ISparkTemplate>>(_parsings);
            
            var graph = new SharingGraph();
            services.SetServiceIfNone(graph);
            services.SetServiceIfNone<ISharingGraph>(graph);

            services.SetServiceIfNone<ISparkViewEngine>(new SparkViewEngine());
            services.SetServiceIfNone<ICacheService>(new DefaultCacheService(HttpRuntime.Cache));

            services.SetServiceIfNone(new SharingLogsCache());

            services.FillType<IActivator, SharingConfigActivator>();
            services.FillType<IActivator, SharingPolicyActivator>();
            services.FillType<IActivator, SharingAttacherActivator<ISparkTemplate>>();
            services.FillType<IActivator, SparkActivator>();
            services.FillType<IActivator, SparkPrecompiler>();

            services.FillType<ISharingAttacher<ISparkTemplate>, MasterAttacher<ISparkTemplate>>();
            services.FillType<ISharingAttacher<ISparkTemplate>, BindingsAttacher>();

            services.SetServiceIfNone<ISharedPathBuilder>(new SharedPathBuilder());
            services.SetServiceIfNone<ITemplateDirectoryProvider<ISparkTemplate>, TemplateDirectoryProvider<ISparkTemplate>>();
            services.SetServiceIfNone<ISharedTemplateLocator, SharedTemplateLocator>();
            services.FillType<ISharedTemplateLocator<ISparkTemplate>, SharedTemplateLocator>();

            services.FillType<ITemplateSelector<ISparkTemplate>, SparkTemplateSelector>();


            services.SetServiceIfNone<IHtmlEncoder, DefaultHtmlEncoder>();

        }
    }
}