using FubuMVC.Core;
using FubuMVC.Diagnostics.Configuration;

namespace FubuMVC.Diagnostics
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