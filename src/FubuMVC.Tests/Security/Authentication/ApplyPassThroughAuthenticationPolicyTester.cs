using System;
using System.Linq;
using System.Linq.Expressions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authentication;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Security.Authentication
{
    [TestFixture]
	public class ApplyPassThroughAuthenticationPolicyTester
    {
        [Test]
        public void prepends_the_pass_through_authentication_node()
        {
            var thePolicy = new ApplyPassThroughAuthenticationPolicy();
            
            var chain = chainFor<TestAuthenticationEndpoint<PassThroughModel>>(x => x.get_something(null));
            var graph = new BehaviorGraph();

            graph.AddChain(chain);

            thePolicy.Configure(graph);

	        var filter = chain.OfType<ActionFilter>().Single();
	        filter.HandlerType.ShouldEqual(typeof (PassThroughAuthenticationFilter));
        }

        private static BehaviorChain chainFor<T>(Expression<Func<T, object>> expression)
        {
            var chain = new RoutedChain("something");
            chain.AddToEnd(ActionCall.For(expression));
            return chain;
        }
    }
}