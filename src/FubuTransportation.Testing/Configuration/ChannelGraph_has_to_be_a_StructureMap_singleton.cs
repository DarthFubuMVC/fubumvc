using FubuTestingSupport;
using FubuTransportation.Configuration;
using NUnit.Framework;
using StructureMap;
using StructureMap.Pipeline;

namespace FubuTransportation.Testing.Configuration
{
    [TestFixture]
    public class ChannelGraph_has_to_be_a_StructureMap_singleton
    {
        [Test]
        public void must_be_a_singleton()
        {
            using (var runtime = FubuTransport.For(x => x.EnableInMemoryTransport()).Bootstrap()
                )
            {
                runtime.Factory.Get<IContainer>().Model.For<ChannelGraph>().Lifecycle.ShouldBeOfType<SingletonLifecycle>();
            }

           
        }
    }
}