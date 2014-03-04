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
            registry.ViewFacility(new SparkViewFacility(_templateRegistry, _parsings));
            registry.Services(configureServices);
        }

        private void configureServices(ServiceRegistry services)
        {
            services.ReplaceService<ISparkTemplateRegistry>(_templateRegistry);
            services.ReplaceService<ITemplateRegistry<ITemplate>>(_templateRegistry);
            services.ReplaceService<IParsingRegistrations<ITemplate>>(_parsings);
            
            var graph = new SharingGraph();
            services.SetServiceIfNone(graph);
            services.SetServiceIfNone<ISharingGraph>(graph);

            services.SetServiceIfNone<ISparkViewEngine>(new SparkViewEngine());
            services.SetServiceIfNone<ICacheService>(new DefaultCacheService(HttpRuntime.Cache));

            services.SetServiceIfNone(new SharingLogsCache());

            services.FillType<IActivator, SharingConfigActivator>();
            services.FillType<IActivator, SharingPolicyActivator>();
            services.FillType<IActivator, SharingAttacherActivator<ITemplate>>();
            services.FillType<IActivator, SparkActivator>();
            services.FillType<IActivator, SparkPrecompiler>();

            services.FillType<ISharingAttacher<ITemplate>, MasterAttacher<ITemplate>>();
            services.FillType<ISharingAttacher<ITemplate>, BindingsAttacher>();

            services.SetServiceIfNone<ISharedPathBuilder>(new SharedPathBuilder());
            services.SetServiceIfNone<ITemplateDirectoryProvider<ITemplate>, TemplateDirectoryProvider<ITemplate>>();
            services.SetServiceIfNone<ISharedTemplateLocator, SharedTemplateLocator>();
            services.FillType<ISharedTemplateLocator<ITemplate>, SharedTemplateLocator>();

            services.FillType<IRenderStrategy, NestedRenderStrategy>();
            services.FillType<IRenderStrategy, AjaxRenderStrategy>();
            services.FillType<IRenderStrategy, DefaultRenderStrategy>();

            services.FillType<ITemplateSelector<ITemplate>, SparkTemplateSelector>();

            services.SetServiceIfNone<IViewEntryProviderCache, ViewEntryProviderCache>();
            services.SetServiceIfNone<IViewModifierService<IFubuSparkView>, ViewModifierService<IFubuSparkView>>();

            services.FillType<IViewModifier<IFubuSparkView>, SiteResourceAttacher>();
            services.FillType<IViewModifier<IFubuSparkView>, ContentActivation>();
            services.FillType<IViewModifier<IFubuSparkView>, OnceTableActivation>();
            services.FillType<IViewModifier<IFubuSparkView>, OuterViewOutputActivator>();
            services.FillType<IViewModifier<IFubuSparkView>, NestedViewOutputActivator>();
            services.FillType<IViewModifier<IFubuSparkView>, ViewContentDisposer>();
            services.FillType<IViewModifier<IFubuSparkView>, NestedOutputActivation>();

            services.SetServiceIfNone<IHtmlEncoder, DefaultHtmlEncoder>();

            services.SetServiceIfNone(new DefaultViewDefinitionPolicy());
            services.SetServiceIfNone<IViewDefinitionResolver, ViewDefinitionResolver>();
        }
    }
}