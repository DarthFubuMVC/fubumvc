using System;
using System.Collections.Generic;
using System.Diagnostics;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Urls
{
    [TestFixture]
    public class UrlRegistryIntegrationTester
    {
        private BehaviorGraph graph;
        private UrlRegistry urls;
        private OwinHttpRequest theHttpRequest;

        [TestFixtureSetUp]
        public void SetUp()
        {
            theHttpRequest = OwinHttpRequest.ForTesting();
            theHttpRequest.FullUrl("http://server/fubu");


            var registry = new FubuRegistry();
            registry.Actions.IncludeType<OneController>();
            registry.Actions.IncludeType<TwoController>();
            registry.Actions.IncludeType<QueryStringTestController>();
            registry.Actions.IncludeType<OnlyOneActionController>();


            registry.Configure(x => { x.TypeResolver.AddStrategy<UrlModelForwarder>(); });

            //registry.Routes.HomeIs<DefaultModel>();

            graph = BehaviorGraph.BuildFrom(registry);

            var resolver = graph.Services.DefaultServiceFor<ITypeResolver>().Value;
            var urlResolver = new ChainUrlResolver(theHttpRequest);

            urls = new UrlRegistry(new ChainResolutionCache((ITypeResolver) resolver, graph), urlResolver,
                new JQueryUrlTemplate(), theHttpRequest);
        }

        [Test]
        public void find_by_handler_type_if_only_one_method()
        {

            urls.UrlFor<OnlyOneActionController>()
                .ShouldEqual("/onlyoneaction/go");
        }

        [Test]
        public void retrieve_by_controller_action_even_if_it_has_an_input_model()
        {
            urls.UrlFor<OneController>(x => x.M1(null), null).ShouldEqual("/one/m1");
        }

        [Test]
        public void retrieve_a_url_for_a_model_simple_case()
        {
            urls.UrlFor(new Model1()).ShouldEqual("/one/m1");
        }

        [Test]
        public void retrieve_a_url_for_a_inferred_model_simple_case()
        {
            urls.UrlFor<Model1>((string) null).ShouldEqual("/one/m1");
        }

        [Test]
        public void retrieve_a_url_for_a_model_that_does_not_exist()
        {
            Exception<FubuException>.ShouldBeThrownBy(() => { urls.UrlFor(new ModelWithNoChain()); });
        }


        [Test]
        public void retrieve_a_url_for_a_model_that_has_route_inputs()
        {
            urls.UrlFor(new ModelWithInputs
            {
                Name = "Jeremy"
            }).ShouldEqual("/find/Jeremy");
        }

        [Test]
        public void retrieve_a_url_for_a_model_that_has_querystring_inputs()
        {
            var model = new ModelWithQueryStringInput() {Param = 42};

            urls.UrlFor(model).ShouldEqual("/qs/test?Param=42");
        }

        [Test]
        public void retrieve_a_url_for_a_model_that_has_mixed_inputs()
        {
            var model = new ModelWithQueryStringAndRouteInput() {Param = 42, RouteParam = 23};

            urls.UrlFor(model).ShouldEqual("/qsandroute/test/23?Param=42");
        }

        [Test]
        public void retrieve_url_by_input_type_with_parameters()
        {
            var parameters = new RouteParameters<ModelWithInputs>();
            parameters[x => x.Name] = "Max";

            urls.UrlFor<ModelWithInputs>(parameters).ShouldEqual("/find/Max");
        }

        [Test]
        public void retrieve_a_url_for_a_model_and_category()
        {
            urls.UrlFor(new UrlModel(), "different").ShouldEqual("/one/m4");
        }

        [Test]
        public void retrieve_a_url_by_action()
        {
            urls.UrlFor<OneController>(x => x.M2(), null).ShouldEqual("/one/m2");
        }

        [Test]
        public void retrieve_a_url_by_action_negative_case()
        {

            Exception<FubuException>.ShouldBeThrownBy(() => { urls.UrlFor<RandomClass>(x => x.Ignored(), null); });
        }

        [Test]
        public void url_for_handler_type_and_method_positive()
        {
            var method = ReflectionHelper.GetMethod<OneController>(x => x.M3());

            urls.UrlFor(typeof (OneController), method, null).ShouldEqual("/one/m3");
        }

        [Test]
        public void url_for_handler_type_and_method_negative_case_should_throw_204()
        {
            Exception<FubuException>.ShouldBeThrownBy(() => {
                var method = ReflectionHelper.GetMethod<RandomClass>(x => x.Ignored());
                urls.UrlFor(typeof (OneController), method, null);
            }).ErrorCode.ShouldEqual(2104);
        }

        [Test]
        public void url_for_new_positive_case()
        {
            urls.UrlForNew<UrlModel>().ShouldEqual("/two/m2");
            urls.UrlForNew(typeof (UrlModel)).ShouldEqual("/two/m2");
        }

        [Test]
        public void url_for_new_negative_case_should_throw_2109()
        {
            Exception<FubuException>.ShouldBeThrownBy(() => { urls.UrlForNew<ModelWithoutNewUrl>(); })
                .ErrorCode.ShouldEqual(2109);
        }

        [Test]
        public void has_new_url_positive()
        {
            urls.HasNewUrl(typeof (UrlModel)).ShouldBeTrue();
        }

        [Test]
        public void has_new_url_negative()
        {
            urls.HasNewUrl(typeof (ModelWithoutNewUrl)).ShouldBeFalse();
        }

        [Test]
        public void retrieve_a_url_for_a_model_when_it_trips_off_type_resolver_rules()
        {
            urls.UrlFor(new SubclassUrlModel()).ShouldEqual("/two/m4");
        }


        [Test]
        public void forward_with_a_category()
        {
            graph.Forward<Model4>(m => new Model6(), "A");
            graph.Forward<Model4>(m => new Model7(), "B");

            urls.UrlFor(new Model4(), "A").ShouldEqual("/one/a");
            urls.UrlFor(new Model4(), "B").ShouldEqual("/one/b");
        }

        [Test]
        public void forward_with_route_inputs()
        {
            graph.Forward<Model4>(m => new ModelWithInputs() {Name = "chiefs"});
            urls.UrlFor(new Model4()).ShouldEqual("/find/chiefs");
        }

        [Test]
        public void forward_respects_the_type_resolution()
        {
            graph.Forward<UrlModel>(m => new Model6(), "new");
            urls.UrlFor(new SubclassUrlModel(), "new").ShouldEqual("/one/a");
        }

        [Test]
        public void template_for_model_will_respects_the_absolute_pathing()
        {
            urls.TemplateFor(new ModelWithInputs())
                .ShouldEqual("/find/${Name}");
        }


        [Test]
        public void url_for_route_parameter_by_type_respects_the_absolute_path()
        {
            urls.UrlFor<Model6>(new RouteParameters())
                .ShouldEqual("/one/a");
        }

        [Test]
        public void url_for_route_parameter_by_type_and_category_respects_absolute_path()
        {
            urls.UrlFor<UrlModel>(new RouteParameters(), "different")
                .ShouldEqual("/one/m4");
        }
    }

    [TestFixture]
    public class Forwarding_tests
    {
        private BehaviorGraph graph;
        private UrlRegistry urls;
        private OwinHttpRequest theHttpRequest;

        [TestFixtureSetUp]
        public void SetUp()
        {
            theHttpRequest = OwinHttpRequest.ForTesting();
            theHttpRequest.FullUrl("http://server/fubu");


            var registry = new FubuRegistry();
            registry.Actions.IncludeType<OneController>();
            registry.Actions.IncludeType<TwoController>();
            registry.Actions.IncludeType<QueryStringTestController>();
            registry.Actions.IncludeType<OnlyOneActionController>();


            registry.Configure(x => { x.TypeResolver.AddStrategy<UrlModelForwarder>(); });

            //registry.Routes.HomeIs<DefaultModel>();

            graph = BehaviorGraph.BuildFrom(registry);

            var resolver = graph.Services.DefaultServiceFor<ITypeResolver>().Value;
            var urlResolver = new ChainUrlResolver(theHttpRequest);

            urls = new UrlRegistry(new ChainResolutionCache((ITypeResolver)resolver, graph), urlResolver,
                new JQueryUrlTemplate(), theHttpRequest);
        }


        [Test]
        public void forward_without_a_category()
        {
            graph.Forward<Model4>(m => new Model3());
            urls.UrlFor(new Model4()).ShouldEqual("/one/m5");
        }
    }

    public class RandomClass
    {
        public void Ignored()
        {
        }
    }

    public class OneController
    {
        [UrlPattern("find/{Name}")]
        public void MethodWithPattern(ModelWithInputs inputs)
        {
        }

        public void A(Model6 input)
        {
        }

        public void B(Model7 input)
        {
        }


        public void M1(Model1 input)
        {
        }

        public void M2()
        {
        }

        public void M3()
        {
        }

        public void M5(Model3 input)
        {
        }

        [UrlRegistryCategory("different")]
        public void M4(UrlModel model)
        {
        }

        public string Default(DefaultModel model)
        {
            return "welcome to the default view";
        }
    }

    public class TwoController
    {
        public void M1()
        {
        }

        [UrlForNew(typeof (UrlModel))]
        public void M2()
        {
        }

        public void M3()
        {
        }

        public void M4(UrlModel model)
        {
        }
    }

    public class OnlyOneActionController
    {
        public void Go(Model8 input)
        {
        }
    }

    public class QueryStringTestController
    {
        public void get_qs_test(ModelWithQueryStringInput input)
        {
        }

        public void get_qsandroute_test_RouteParam(ModelWithQueryStringAndRouteInput input)
        {
        }
    }

    public class ModelWithInputs
    {
        public string Name { get; set; }
    }

    public class Model1
    {
    }

    public class Model2
    {
    }

    public class Model3
    {
    }

    public class Model4
    {
    }

    public class Model5
    {
    }

    public class Model6
    {
    }

    public class Model7
    {
    }

    public class Model8
    {
    }

    public class DefaultModel
    {
    }

    public class ModelWithNoChain
    {
    }

    public class ModelWithoutNewUrl
    {
    }

    public class UrlModelForwarder : ITypeResolverStrategy
    {
        public bool Matches(object model)
        {
            return model is SubclassUrlModel;
        }

        public Type ResolveType(object model)
        {
            return typeof (UrlModel);
        }
    }

    public class UrlModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class SubclassUrlModel : UrlModel
    {
    }

    public class ModelWithQueryStringInput
    {
        [QueryString]
        public int Param { get; set; }
    }

    public class ModelWithQueryStringAndRouteInput
    {
        [QueryString]
        public int Param { get; set; }

        public int RouteParam { get; set; }
    }
}