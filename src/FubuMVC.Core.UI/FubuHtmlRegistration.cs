using FubuMVC.Core.UI.ViewEngine;

namespace FubuMVC.Core.UI
{
    public class FubuHtmlRegistration : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services<UIServiceRegistry>();

            registry.WithTypes(types => registry.Views.Facility(new HtmlDocumentViewFacility(types)));
        }
    }
}