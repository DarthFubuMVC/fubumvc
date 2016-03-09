using FubuMVC.Core;
using Marten;

namespace FubuMVC.Marten
{
    public class MartenExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services.IncludeRegistry<MartenStructureMapServices>();

            if (registry.Mode.InDevelopment())
            {
                registry.Services.For<IMartenSessionLogger>().Use<CommandRecordingLogger>();
            }


        }
    }
}