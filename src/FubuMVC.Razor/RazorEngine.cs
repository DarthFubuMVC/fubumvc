using FubuMVC.Core;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuMVC.Razor.Rendering;

namespace FubuMVC.Razor
{
    public class RazorEngineRegistry : IFubuRegistryExtension
    {

        void IFubuRegistryExtension.Configure(FubuRegistry registry)
        {
            registry.AlterSettings<ViewEngineSettings>(x => {
                var facility = new RazorViewFacility();
                x.AddFacility(facility);
            });

            registry.Services(services => {
                services.SetServiceIfNone<IPartialRenderer, PartialRenderer>();
            });

            registry.AlterSettings<CommonViewNamespaces>(x =>
            {
                x.AddForType<RazorViewFacility>(); // FubuMVC.Razor
                x.AddForType<IPartialInvoker>(); // FubuMVC.Core.UI
            });
        }
    }

}