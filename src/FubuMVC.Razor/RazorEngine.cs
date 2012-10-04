using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Model.Sharing;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.Rendering;
using FubuMVC.Razor.RazorModel;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using IActivator = Bottles.IActivator;

namespace FubuMVC.Razor
{
    public class RazorEngineRegistry : IFubuRegistryExtension
    {
        private readonly RazorParsings _parsings = new RazorParsings();
        private readonly TemplateRegistry<IRazorTemplate> _templateRegistry = new TemplateRegistry<IRazorTemplate>();

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.ViewFacility(new RazorViewFacility(_templateRegistry, _parsings));
            registry.Services(configureServices);
        }

        private void configureServices(IServiceRegistry services)
        {
            var configuration = new TemplateServiceConfiguration {BaseTemplateType = typeof (FubuRazorView)};

            services.ReplaceService<ITemplateRegistry<IRazorTemplate>>(_templateRegistry);
            services.ReplaceService<IFubuTemplateService>(new FubuTemplateService(_templateRegistry, new TemplateService(configuration), new FileSystem()));
            services.ReplaceService<ITemplateServiceConfiguration>(configuration);
            services.ReplaceService<IParsingRegistrations<IRazorTemplate>>(_parsings);
            services.SetServiceIfNone<ITemplateDirectoryProvider<IRazorTemplate>, TemplateDirectoryProvider<IRazorTemplate>>();
            services.SetServiceIfNone<ISharedPathBuilder>(new SharedPathBuilder());

            var graph = new SharingGraph();
            services.SetServiceIfNone(graph);
            services.SetServiceIfNone<ISharingGraph>(graph);


            services.FillType<IActivator, RazorActivator>();

            services.FillType<ISharedTemplateLocator<IRazorTemplate>, SharedTemplateLocator<IRazorTemplate>>();
            services.FillType<ISharingAttacher<IRazorTemplate>, MasterAttacher<IRazorTemplate>>();
            services.FillType<ITemplateSelector<IRazorTemplate>, RazorTemplateSelector>();
            services.FillType<IActivator, SharingAttacherActivator<IRazorTemplate>>();
            services.FillType<IRenderStrategy, AjaxRenderStrategy>();
            services.FillType<IRenderStrategy, DefaultRenderStrategy>();

            services.SetServiceIfNone<IViewModifierService<IFubuRazorView>, ViewModifierService<IFubuRazorView>>();

            services.FillType<IViewModifier<IFubuRazorView>, LayoutActivation>();
            services.FillType<IViewModifier<IFubuRazorView>, PartialRendering>();
            services.FillType<IViewModifier<IFubuRazorView>, FubuPartialRendering>();
        }
    }
}