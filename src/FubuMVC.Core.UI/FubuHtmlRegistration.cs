using FubuMVC.Core.UI.ViewEngine;
using FubuMVC.Core.View;

namespace FubuMVC.Core.UI
{
    public class FubuHtmlRegistration : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services<UIServiceRegistry>();

            registry.ViewFacility(new HtmlDocumentViewFacility());
        }
    }
}