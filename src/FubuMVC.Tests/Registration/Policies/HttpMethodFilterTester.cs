using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using FubuMVC.Core.Registration.Routes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Policies
{
    [TestFixture]
    public class HttpMethodFilterTester
    {
        [Test]
        public void description()
        {
            var filter = new HttpMethodFilter("POST");

            Description.For(filter).Title.ShouldEqual("Responds to Http POST's");
        }

        [Test]
        public void matches_positive()
        {
            var filter = new HttpMethodFilter("POST");
            var chain = new RoutedChain(new RouteDefinition("something"));
            chain.Route.AddHttpMethodConstraint("POST");

            filter.Matches(chain).ShouldBeTrue();
        }

        [Test]
        public void matches_positive_because_route_definition_does_not_have_any_constraints()
        {
            var filter = new HttpMethodFilter("POST");
            var chain = new RoutedChain(new RouteDefinition("something"));

            filter.Matches(chain).ShouldBeTrue();
        }

        [Test]
        public void matches_positive_is_not_case_sensitive()
        {
            var filter = new HttpMethodFilter("POST");
            var chain = new RoutedChain(new RouteDefinition("something"));
            chain.Route.AddHttpMethodConstraint("post");

            filter.Matches(chain).ShouldBeTrue();
        }

        [Test]
        public void matches_negative_because_of_methods()
        {
            var filter = new HttpMethodFilter("POST");
            var chain = new RoutedChain(new RouteDefinition("something"));
            chain.Route.AddHttpMethodConstraint("GET");

            filter.Matches(chain).ShouldBeFalse();
        }

        [Test]
        public void matches_negative_because_it_is_a_partial()
        {
            var filter = new HttpMethodFilter("POST");
            var chain = new BehaviorChain();

            filter.Matches(chain).ShouldBeFalse();

            chain.IsPartialOnly = true;

            filter.Matches(chain).ShouldBeFalse();

            
        }


        [Test]
        public void use_inside_policy()
        {
            var graph = BehaviorGraph.BuildFrom(x => {
                x.Policies.Local.Add(policy => {
                    policy.Where.RespondsToHttpMethod("PUT", "POST", "DELETE");
                    policy.Wrap.WithNode<FakeNode>();
                });
            });

            graph.BehaviorFor<HttpMethodEndpoint>(x => x.put_something(null)).Any(x => x is FakeNode).ShouldBeTrue();
            graph.BehaviorFor<HttpMethodEndpoint>(x => x.post_something(null)).Any(x => x is FakeNode).ShouldBeTrue();
            graph.BehaviorFor<HttpMethodEndpoint>(x => x.delete_something(null)).Any(x => x is FakeNode).ShouldBeTrue();
            graph.BehaviorFor<HttpMethodEndpoint>(x => x.get_something()).Any(x => x is FakeNode).ShouldBeFalse();
        }

    }


    public class HttpMethodEndpoint
    {
        public void put_something(InputModel model){}
        public void delete_something(InputModel model){}
        public void post_something(InputModel model){}
        public string get_something()
        {
            return "this is something";
        }
    }
}