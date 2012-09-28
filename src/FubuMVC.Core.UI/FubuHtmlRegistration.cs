using FubuMVC.Core;
using FubuMVC.Core.UI;

namespace FubuHtml
{
    public class FubuHtmlRegistration : IFubuRegistryExtension
    {
        #region IFubuRegistryExtension Members

        public void Configure(FubuRegistry registry)
        {
            registry.Services<UIServiceRegistry>();
        }

        #endregion
    }
}