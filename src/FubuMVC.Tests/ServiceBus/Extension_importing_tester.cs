using System.Linq;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using Shouldly;
using NUnit.Framework;

namespace FubuTransportation.Testing
{
    [TestFixture]
    public class Extension_importing_tester
    {
        [Test]
        public void import_an_extension()
        {
            var graph = FubuTransportRegistry.HandlerGraphFor(x => {
                x.Import<DoerExtension>();
            });

            graph.ChainFor(typeof(Message1)).OfType<HandlerCall>()
                .Any(x => x.HandlerType == typeof(OneDoer))
                .ShouldBeTrue();

            
        }
    }

    public class DoerExtension : IFubuTransportRegistryExtension
    {
        public void Configure(FubuTransportRegistry registry)
        {
            registry.Handlers.FindBy(x => {
                x.IncludeClassesSuffixedWith("Doer");
            });
        }
    }

    public class OneDoer
    {
        public void Handle(Message1 message)
        {
            
        }
    }
}