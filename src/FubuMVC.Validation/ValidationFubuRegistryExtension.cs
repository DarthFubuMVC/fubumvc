using FubuMVC.Core;

namespace FubuMVC.Validation
{
    public class ValidationFubuRegistryExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.WithValidationDefaults();
        }
    }
}