using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Model.Sharing;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.Rendering;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor
{
    public class RazorEngineRegistry : IFubuRegistryExtension
    {
        private readonly RazorParsings _razorParsings = new RazorParsings();

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.AlterSettings<ViewEngines>(x => {
                var facility = new RazorViewFacility(_razorParsings);
                x.AddFacility(facility);
            });

            registry.Services(configureServices);

            registry.AlterSettings<CommonViewNamespaces>(x =>
            {
                x.AddForType<RazorViewFacility>(); // FubuMVC.Razor
                x.AddForType<IPartialInvoker>(); // FubuMVC.Core.UI
            });
        }

        private void configureServices(ServiceRegistry services)
        {
            services.SetServiceIfNone<IRazorTemplateGenerator, RazorTemplateGenerator>();
            services.SetServiceIfNone<ITemplateCompiler, TemplateCompiler>();
            services.SetServiceIfNone<ITemplateFactory, TemplateFactoryCache>();
            services.ReplaceService<IParsingRegistrations<IRazorTemplate>>(_razorParsings);
            services.SetServiceIfNone<ITemplateDirectoryProvider<IRazorTemplate>, TemplateDirectoryProvider<IRazorTemplate>>();
            services.SetServiceIfNone<ISharedPathBuilder>(new SharedPathBuilder());
            services.SetServiceIfNone<IPartialRenderer, PartialRenderer>();

            var graph = new SharingGraph();
            services.SetServiceIfNone(graph);
            services.SetServiceIfNone<ISharingGraph>(graph);

            services.FillType<ISharedTemplateLocator<IRazorTemplate>, SharedTemplateLocator<IRazorTemplate>>();
            services.FillType<ISharingAttacher<IRazorTemplate>, MasterAttacher<IRazorTemplate>>();
            services.FillType<ITemplateSelector<IRazorTemplate>, RazorTemplateSelector>();
            services.FillType<Bottles.IActivator, SharingAttacherActivator<IRazorTemplate>>();


        }
    }

}