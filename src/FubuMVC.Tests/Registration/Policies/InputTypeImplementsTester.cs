using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Policies
{
    [TestFixture]
    public class InputTypeImplementsTester
    {
        [Test]
        public void matches_positive()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<SomeEndpoints>(x => x.post_interface(null)));

            new InputTypeImplements<ISomeInterface>()
                .Matches(chain).ShouldBeTrue();
        }

        [Test]
        public void matches_negative()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<SomeEndpoints>(x => x.post_no_interface(null)));

            new InputTypeImplements<ISomeInterface>()
                .Matches(chain).ShouldBeFalse();
        }

        [Test]
        public void does_not_blow_up_if_there_is_no_input_type()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<SomeEndpoints>(x => x.get_interface()));

            new InputTypeImplements<ISomeInterface>()
                .Matches(chain).ShouldBeFalse();
        }
    }
}