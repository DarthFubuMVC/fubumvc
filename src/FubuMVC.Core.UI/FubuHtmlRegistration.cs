namespace FubuMVC.Core.UI
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