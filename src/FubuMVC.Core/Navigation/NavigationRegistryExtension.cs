namespace FubuMVC.Core.Navigation
{
    public class NavigationRegistryExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Policies.Global.Add<MenuItemAttributeConfigurator>();

            registry.Services.IncludeRegistry<NavigationServiceRegistry>();
        }
    }
}