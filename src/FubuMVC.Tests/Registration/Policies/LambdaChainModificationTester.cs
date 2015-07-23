using System;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using NUnit.Framework;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration.Policies
{
    [TestFixture]
    public class LambdaChainModificationTester
    {
        [Test]
        public void describes_itself_with_user_given_description()
        {
            var modification = new LambdaChainModification(c => { });

            modification.Title = "some title";
            modification.Description = "some description";

            var description = Description.For(modification);

            description.Title.ShouldBe(modification.Title);
            description.ShortDescription.ShouldBe(modification.Description);
        }

        [Test]
        public void modify_is_just_a_straight_delegation()
        {
            var action = MockRepository.GenerateMock<Action<BehaviorChain>>();

            var modification = new LambdaChainModification(action);

            var chain = new BehaviorChain();

            modification.Modify(chain);

            action.AssertWasCalled(x => x.Invoke(chain));
        }
    }
}