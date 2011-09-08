using FubuMVC.Core;
using FubuMVC.Diagnostics.Core.Configuration;

namespace FubuMVC.Diagnostics.Core
{
    public class AdvancedDiagnosticsFubuExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry
                .Policies
                .Add<RemoveBasicDiagnostics>();
        }
    }
}