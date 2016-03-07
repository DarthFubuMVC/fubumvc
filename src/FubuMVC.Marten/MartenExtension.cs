using FubuCore.Util;
using FubuMVC.Core;

namespace FubuMVC.Marten
{
    public class MartenExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services.IncludeRegistry<MartenStructureMapServices>();
        }
    }
}