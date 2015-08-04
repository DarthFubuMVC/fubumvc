using System;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authentication;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
    public class ApplyAuthenticationPolicyTester
    {
        [Test]
        public void prepends_the_authentication_node()
        {
            var thePolicy = new ApplyAuthenticationPolicy();
            
            var chain = chainFor<TestAuthenticationEndpoint<AuthenticatedModel>>(x => x.get_something(null));
            var graph = new BehaviorGraph();

            graph.AddChain(chain);

            thePolicy.Configure(graph);

            chain.First().ShouldBeOfType<AuthenticationFilterNode>();
        }

        [Test]
        public void uses_the_filter()
        {
            var thePolicy = new ApplyAuthenticationPolicy();

            var chain = chainFor<TestAuthenticationEndpoint<AuthenticatedModel>>(x => x.get_something(null));
            var graph = new BehaviorGraph();
            graph.AddChain(chain);

            // This is the filter set up that will explicitly exclude this chain
            graph.Settings.Get<AuthenticationSettings>()
                .ExcludeChains = x => typeof (AuthenticatedModel) == x.InputType();

            thePolicy.Configure(graph);

            chain.ShouldHaveTheSameElementsAs(chain.FirstCall());
        }

        private static BehaviorChain chainFor<T>(Expression<Func<T, object>> expression)
        {
            var chain = new RoutedChain("something");
            chain.AddToEnd(ActionCall.For(expression));
            return chain;
        }

        private static BehaviorChain chainFor<T>(Expression<Action<T>> expression)
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For(expression));
            return chain;
        }
    }
}