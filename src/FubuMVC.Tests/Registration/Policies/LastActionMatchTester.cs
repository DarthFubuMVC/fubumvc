using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Policies
{
    [TestFixture]
    public class LastActionMatchTester
    {
        [Test]
        public void description_from_an_expression()
        {
            var match = new LastActionMatch(call => call.Method.Name.EndsWith("HTML"));

            var description = Description.For(match);

            description.ShortDescription.ShouldEqual("call => call.Method.Name.EndsWith(\"HTML\")");
        }

        [Test]
        public void description_from_a_func()
        {
            var match = new LastActionMatch(call => call.Method.Name.EndsWith("HTML"), "Action method is suffixed with HTML");

            var description = Description.For(match);

            description.ShortDescription.ShouldEqual("Action method is suffixed with HTML");
        }

        [Test]
        public void does_not_blow_up_with_no_chains()
        {
            var match = new LastActionMatch(call => call.Method.Name.EndsWith("HTML"));
            match.Matches(new BehaviorChain()).ShouldBeFalse();
        }

        

        [Test]
        public void matches_positive_with_one_action()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<ActionEndpoint>(x => x.SaySomethingHTML()));

            var match = new LastActionMatch(call => call.Method.Name.EndsWith("HTML"));
            match.Matches(chain).ShouldBeTrue();
        }

        [Test]
        public void matches_negative_with_one_action()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<ActionEndpoint>(x => x.get_hello()));

            var match = new LastActionMatch(call => call.Method.Name.EndsWith("HTML"));
            match.Matches(chain).ShouldBeFalse();
        }

        [Test]
        public void only_matches_the_last_action_call()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<ActionEndpoint>(x => x.SaySomethingHTML()));
            chain.AddToEnd(ActionCall.For<ActionEndpoint>(x => x.get_hello()));

            var match = new LastActionMatch(call => call.Method.Name.EndsWith("HTML"));
            match.Matches(chain).ShouldBeFalse();
        }

        [Test]
        public void but_does_match_the_last_call()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<ActionEndpoint>(x => x.get_hello()));
            chain.AddToEnd(ActionCall.For<ActionEndpoint>(x => x.SaySomethingHTML()));
            

            var match = new LastActionMatch(call => call.Method.Name.EndsWith("HTML"));
            match.Matches(chain).ShouldBeTrue();
        }
    }

    public class ActionEndpoint
    {
        public string get_hello()
        {
            return "hello";
        }

        public string SaySomethingHTML()
        {
            return "hello";
        }
    }
}