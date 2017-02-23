using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Registration.Conventions
{
    
    public class DefaultRouteConventionBasedUrlPolicyTester
    {
        private MethodInfo _method;
        private DefaultRouteConventionBasedUrlPolicy _policy;


        public DefaultRouteConventionBasedUrlPolicyTester()
        {
            _method = ReflectionHelper.GetMethod<TestController>(c => c.SomeAction(null));
            _policy = new DefaultRouteConventionBasedUrlPolicy();
        }

        [Fact]
        public void integrated_test()
        {
            var graph = BehaviorGraph.BuildFrom(r =>
            {
                r.Actions.IncludeType<HomeEndpoint>();
            });

            graph.ChainFor<HomeEndpoint>(x => x.Index()).As<RoutedChain>().Route.Pattern.ShouldBe("");
        }

        [Fact]
        public void should_build_a_route_definition_from_the_action_call()
        {
            var call = new ActionCall(typeof(HomeEndpoint), _method);
            _policy.Build(call).ShouldNotBeNull();
        }

        [Fact]
        public void should_match_the_home_endpoint_index_method()
        {
            var index = ReflectionHelper.GetMethod<HomeEndpoint>(c => c.Index());
            var call = new ActionCall(typeof(HomeEndpoint), index);
            _policy.Matches(call).ShouldBeTrue();
        }

        [Fact]
        public void should_not_match_another_method()
        {
            var anotherMethod = ReflectionHelper.GetMethod<HomeEndpoint>(c => c.Action());
            var call = new ActionCall(typeof(HomeEndpoint), anotherMethod);
            _policy.Matches(call).ShouldBeFalse();
        }

        [Fact]
        public void should_not_match_another_controller()
        {
            var anotherMethod = ReflectionHelper.GetMethod<AnotherEndpoint>(c => c.Index());
            var call = new ActionCall(typeof(HomeEndpoint), anotherMethod);
            _policy.Matches(call).ShouldBeFalse();
        } 

        private class HomeEndpoint
        {
            public void Index(){}
            public void Action(){}
        }

        private class AnotherEndpoint
        {
            public void Index() { }
        }
    }
}