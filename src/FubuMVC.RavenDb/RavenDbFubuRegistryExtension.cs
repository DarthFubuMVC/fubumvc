using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.RavenDb.RavenDb;
using StructureMap.Configuration.DSL;

namespace FubuMVC.RavenDb
{
    public class RavenDbFubuRegistryExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Services<RavenDbRegistry>();
        }
    }

    public class TransactionalBehaviorPolicy : Policy
    {
        public TransactionalBehaviorPolicy()
        {
            Where.RespondsToHttpMethod("POST", "PUT", "DELETE");
            Wrap.WithBehavior<TransactionalBehavior>();
        }
    }
}