using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;

namespace FubuMVC.Razor
{
    public class RazorEngineRegistry : IFubuRegistryExtension
    {

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.AlterSettings<ViewEngines>(x => {
                var facility = new RazorViewFacility();
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
            //services.SetServiceIfNone<IRazorTemplateGenerator, RazorTemplateGenerator>();
            //services.SetServiceIfNone<ITemplateCompiler, TemplateCompiler>();
            //services.SetServiceIfNone<ITemplateFactory, TemplateFactoryCache>();
            //services.SetServiceIfNone<ITemplateDirectoryProvider<RazorTemplate>, TemplateDirectoryProvider<RazorTemplate>>();
            //services.SetServiceIfNone<ISharedPathBuilder>(new SharedPathBuilder());
            //services.SetServiceIfNone<IPartialRenderer, PartialRenderer>();

            //var graph = new SharingGraph();
            //services.SetServiceIfNone(graph);
            //services.SetServiceIfNone<ISharingGraph>(graph);

            //services.FillType<ISharedTemplateLocator<RazorTemplate>, SharedTemplateLocator<RazorTemplate>>();
            //services.FillType<ISharingAttacher<RazorTemplate>, MasterAttacher<RazorTemplate>>();
            //services.FillType<Bottles.IActivator, SharingAttacherActivator<RazorTemplate>>();


        }
    }

}