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

            
            var forwarder = new ChainForwarder<Model1>(m1 => model2);

            var resolver = MockRepository.GenerateMock<IChainResolver>();
            var chain = new BehaviorChain();
            
            
            resolver.Expect(x => x.FindUnique(model2)).Return(chain);

            forwarder.FindChain(resolver, new Model1()).ShouldBeTheSameAs(chain);
        }

        [Test]
        public void input_model_flows_from_the_generic_parameter()
        {
            new ChainForwarder<Model1>(m1 => null).InputType.ShouldEqual(typeof (Model1));
        }
    }

    public class Model1{}
    public class Model2{}
}