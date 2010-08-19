using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration.Querying
{
    [TestFixture]
    public class ChainForwarderTester
    {
        [Test]
        public void forward_the_chain()
        {
            var model2 = new Model2();

            
            var forwarder = new ChainForwarder(typeof (Model1), (o, r) =>
            {
                return r.Find(model2);
            });

            var resolver = MockRepository.GenerateMock<IChainResolver>();
            var chains = new BehaviorChain[0];
            
            resolver.Expect(x => x.Find(model2)).Return(chains);

            forwarder.Find(resolver, new Model1()).ShouldBeTheSameAs(chains);
        }
    }

    public class Model1{}
    public class Model2{}
}