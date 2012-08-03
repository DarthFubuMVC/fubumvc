using System;
using System.Linq;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions
{
    [TestFixture]
    public class RouteDefinitionResolverTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            observer = new RecordingConfigurationObserver();
            graph = new BehaviorGraph(observer);
            chain = new BehaviorChain();
            lastCall = null;

            resolver = new RouteDefinitionResolver();
        }

        #endregion

        private BehaviorGraph graph;
        private BehaviorChain chain;
        private RouteDefinitionResolver resolver;
        private RecordingConfigurationObserver observer;
        private ActionCall lastCall;

        private IRouteDefinition buildRoute(Expression<Action<RouteResolverController>> expression,
                                            Action<BehaviorChain> modifyChain)
        {
            return buildRoute<RouteResolverController>(expression, modifyChain);
        }

        private IRouteDefinition buildRoute<T>(Expression<Action<T>> expression, Action<BehaviorChain> modifyChain)
        {
            var method = ReflectionHelper.GetMethod(expression);
            lastCall = new ActionCall(typeof(T), method);

            chain.AddToEnd(lastCall);
            modifyChain(chain);

            resolver.Apply(graph, chain);

            return chain.Route;
        }


        public class RouteResolverController
        {
            public void SomeMethod(RouteInputModel model)
            {
            }

            [UrlPattern("special/{From}/to/{To}")]
            public void OverrideMethod(RouteInputModel model)
            {
            }

            [UrlPattern("override/noargs")]
            public void OverrideWithNoArgs()
            {
            }

            [FubuPartial]
            public void PartialAction(RouteInputModel model)
            {
            }

            public void Querystring(ModelWithQueryStrings query)
            {
            }

            public void NoArgMethod()
            {
            }

            public void Index()
            {
            }

            public void Generic<T>()
            {
                // should be ignored
            }
        }

        [UrlFolder("contracts")]
        public class OverridenResolverController : RouteResolverController
        {
        }


        public class ModelWithQueryStrings
        {
            [QueryString]
            public string Name { get; set; }

            [QueryString]
            public int Age { get; set; }

            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }

        public class RouteInputModel
        {
            [RouteInput("Jeremy")]
            public string Name { get; set; }

            [RouteInput]
            public int Age { get; set; }

            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }

        public class CaseInputModel : RouteInputModel
        {
        }

        public class Case : Entity
        {
        }

        public class Entity
        {
            public long Id { get; set; }
        }

        [Test]
        public void build_route_for_partial_action()
        {
            buildRoute(x => x.PartialAction(null), c => c.IsPartialOnly = true).ShouldBeNull();
        }

        [Test]
        public void build_route_when_ignoring_controller_names_entirely()
        {
            resolver.DefaultUrlPolicy.IgnoreControllerNamesEntirely = true;
            var route = buildRoute(x => x.SomeMethod(null), c => { });
            route.Input.ShouldBeOfType<RouteInput<RouteInputModel>>();

            route.Pattern.ShouldEqual("fubumvc/tests/registration/conventions/somemethod/{Name}/{Age}");
        }

        [Test]
        public void build_route_when_ignoring_suffix_of_controller_name()
        {
            resolver.DefaultUrlPolicy.IgnoreClassSuffix("Tester");
            var route = buildRoute(x => x.SomeMethod(null), c => { });
            route.Input.ShouldBeOfType<RouteInput<RouteInputModel>>();

            route.Pattern.ShouldEqual("fubumvc/tests/registration/conventions/routeresolver/somemethod/{Name}/{Age}");
        }

        [Test]
        public void build_route_when_ignoring_the_controller_namespace()
        {
            resolver.DefaultUrlPolicy.IgnoreControllerFolderName = true;
            var route = buildRoute(x => x.SomeMethod(null), c => { });

            route.Input.ShouldBeOfType<RouteInput<RouteInputModel>>();
            route.Pattern.ShouldEqual("fubumvc/tests/registration/routeresolver/somemethod/{Name}/{Age}");
        }

        [Test]
        public void build_route_when_ignoring_the_controller_namespace_entirely()
        {
            resolver.DefaultUrlPolicy.IgnoreControllerNamespaceEntirely = true;
            var route = buildRoute(x => x.SomeMethod(null), c => { });

            route.Input.ShouldBeOfType<RouteInput<RouteInputModel>>();

            route.Pattern.ShouldEqual("routeresolver/somemethod/{Name}/{Age}");
        }

        [Test]
        public void build_route_with_a_namespace_ignore()
        {
            resolver.DefaultUrlPolicy.IgnoreNamespace(GetType().Namespace);
            var route = buildRoute(x => x.SomeMethod(null), c => { });

            route.Input.ShouldBeOfType<RouteInput<RouteInputModel>>();

            route.Pattern.ShouldEqual("routeresolver/somemethod/{Name}/{Age}");

            route.Input.RouteParameters.Count.ShouldEqual(2);
            route.Input.RouteParameters.Select(x => x.Name).ShouldHaveTheSameElementsAs("Name", "Age");
        }

        [Test]
        public void build_route_with_a_registered_modification()
        {
            resolver.DefaultUrlPolicy.IgnoreControllerFolderName = true;
            resolver.DefaultUrlPolicy.RegisterRouteModification(x => true, r => r.Prepend("prepend-something"));
            var route = buildRoute(x => x.SomeMethod(null), c => { });

            route.Input.ShouldBeOfType<RouteInput<RouteInputModel>>();

            route.Pattern.ShouldEqual(
                "prepend-something/fubumvc/tests/registration/routeresolver/somemethod/{Name}/{Age}");
        }

        [Test]
        public void build_route_with_all_simple_inputs_and_default_conventions()
        {
            var route = buildRoute(x => x.SomeMethod(null), c => { });
            route.Pattern.ShouldEqual(
                "fubumvc/tests/registration/conventions/routeresolver/somemethod/{Name}/{Age}");

            route.Input.RouteParameters.Count.ShouldEqual(2);
            route.Input.RouteParameters.Select(x => x.Name).ShouldHaveTheSameElementsAs("Name", "Age");
        }

        [Test]
        public void build_route_with_an_appended_class_suffix()
        {
            resolver.DefaultUrlPolicy.AppendClassesWith(x => true, ".aspx");
            var route = buildRoute(x => x.SomeMethod(null), c => { });
            route.Pattern.ShouldEqual(
                "fubumvc/tests/registration/conventions/routeresolver.aspx/somemethod/{Name}/{Age}");

            route.Input.RouteParameters.Count.ShouldEqual(2);
            route.Input.RouteParameters.Select(x => x.Name).ShouldHaveTheSameElementsAs("Name", "Age");
        }

        [Test]
        public void build_route_with_no_inputs_and_default_conventions()
        {
            var route = buildRoute(x => x.NoArgMethod(), c => { }).ShouldBeOfType<RouteDefinition>();

            route.Pattern.ShouldEqual("FubuMVC/Tests/Registration/Conventions/RouteResolver/NoArgMethod".ToLower());
        }

        [Test]
        public void pick_up_UrlFolder_attribute_for_class_name()
        {
            var route =
                buildRoute<OverridenResolverController>(x => x.NoArgMethod(), c => { }).ShouldBeOfType<RouteDefinition>();

            route.Pattern.ShouldEqual("FubuMVC/Tests/Registration/Conventions/contracts/NoArgMethod".ToLower());
        }

        [Test]
        public void pick_up_default_value_on_the_RouteInputAttribute()
        {
            resolver.DefaultUrlPolicy.IgnoreNamespace(GetType().Namespace);
            var route =
                buildRoute(x => x.SomeMethod(null), c => { }).Input.ShouldBeOfType<RouteInput<RouteInputModel>>();

            route.RouteInputFor("Name").DefaultValue.ShouldEqual("Jeremy");
        }

        [Test]
        public void pick_up_querystrings()
        {
            var route = buildRoute(x => x.Querystring(null), c => { });
            route.Input.ShouldBeOfType<RouteInput<ModelWithQueryStrings>>();
            route.Input.QueryParameters.Select(x => x.Name).ShouldHaveTheSameElementsAs("Name", "Age");
        }

        [Test]
        public void should_log_policy_decision_if_url_policy_is_loggable()
        {
            buildRoute(x => x.SomeMethod(null), c => { });
            var log = observer.GetLog(lastCall);
        }

        [Test]
        public void should_log_the_defined_route_pattern()
        {
            buildRoute(x => x.SomeMethod(null), c => { });

            var log = observer.GetLog(lastCall);

            log.Where(s => s.Contains("ubumvc/tests/registration/conventions/routeresolver/somemethod/{Name}/{Age}")).
                Any().ShouldBeTrue();
        }

        [Test]
        public void should_log_the_first_url_policy_that_matches()
        {
            buildRoute(x => x.SomeMethod(null), c => { });

            var log = observer.GetLog(lastCall);

            log.Where(s => s.Contains(": UrlPolicy")).Any().ShouldBeTrue();
        }

        [Test]
        public void use_the_override_url_pattern_from_the_UrlPattern_attribute()
        {
            var route = buildRoute(x => x.OverrideMethod(null), c => { });

            route.Pattern.ShouldEqual("special/{From}/to/{To}");
            route.Input.RouteParameters.Select(x => x.Name).ShouldHaveTheSameElementsAs("From", "To");
        }

        [Test]
        public void use_the_override_url_pattern_from_the_UrlPattern_on_a_method_with_no_input_type()
        {
            var route = buildRoute(x => x.OverrideWithNoArgs(), c => { }).ShouldBeOfType<IRouteDefinition>();

            route.Pattern.ShouldEqual("override/noargs");
        }

        [Test]
        public void adds_convention_based_home_policy_if_not_specified()
        {
            resolver.Configure(graph);
            var route = buildRoute<HomeEndpoint>(x => x.Index(), c => { });
            route.Pattern.ShouldEqual("");
        }

        [Test]
        public void leaves_home_policy_alone_if_already_specified()
        {
            resolver.DefaultUrlPolicy.IgnoreClassSuffix("endpoint");
            resolver.DefaultUrlPolicy.IgnoreControllerNamespaceEntirely = true;
            resolver.RegisterUrlPolicy(new DefaultRouteMethodBasedUrlPolicy(ReflectionHelper.GetMethod<RouteResolverController>(x => x.Index())));

            resolver.Configure(graph);

            var route = buildRoute<HomeEndpoint>(x => x.Index(), c => { });
            route.Pattern.ShouldEqual("home/index");
        }

        private class HomeEndpoint
        {
            public void Index() { }
        }
    }
}