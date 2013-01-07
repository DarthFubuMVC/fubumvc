using FubuCore.Descriptions;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Services;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Configuration
{
    public class SystemServicesPack : ConfigurationPack
    {
        public SystemServicesPack()
        {
            For(ConfigurationType.Explicit);
            Add<FileRegistration>();

            Services<ModelBindingServicesRegistry>();
            Services<SecurityServicesRegistry>();
            Services<HttpStandInServiceRegistry>();
            Services<CoreServiceRegistry>();
            Services<CachingServiceRegistry>();
        }

        [Title("Registers the IFubuApplicationFiles service")]
        internal class FileRegistration : IConfigurationAction
        {
            public void Configure(BehaviorGraph graph)
            {
                graph.Services.Clear(typeof(IFubuApplicationFiles));
                graph.Services.AddService(graph.Files);
            }
        }
    }
}