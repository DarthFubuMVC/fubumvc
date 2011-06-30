using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Diagnostics.Configuration;
using FubuMVC.Diagnostics.Configuration.Policies;

namespace FubuMVC.Diagnostics
{
    public class AdvancedDiagnosticsFubuExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry
                .Policies
                .Add<RemoveBasicDiagnostics>();

            registry
                .Services(x => x.AddService(typeof(IRequestHistoryCacheFilter), new ObjectDef
                                                                                    {
                                                                                        Type = typeof(LambdaRequestHistoryCacheFilter),
                                                                                        Value = new LambdaRequestHistoryCacheFilter(r => r.Path.ToLower().StartsWith("/" + DiagnosticsUrls.ROOT))
                                                                                    }));
        }
    }
}