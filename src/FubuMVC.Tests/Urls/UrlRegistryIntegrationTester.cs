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
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Urls
{
    
    public class UrlRegistryIntegrationTester
    {
        private BehaviorGraph graph;
        private UrlRegistry urls;
        private OwinHttpRequest theHttpRequest;

        public UrlRegistryIntegrationTester()
        {
            theHttpRequest = OwinHttpRequest.ForTesting();
            theHttpRequest.FullUrl("http://server/fubu");


            var registry = new FubuRegistry();
            registry.Actions.IncludeType<OneController>();
            registry.Actions.IncludeType<TwoController>();
            registry.Actions.IncludeType<QueryStringTestController>();
            registry.Actions.IncludeType<OnlyOneActionController>();

            graph = BehaviorGraph.BuildFrom(registry);

            var urlResolver = new ChainUrlResolver(theHttpRequest);

            urls = new UrlRegistry(new ChainResolutionCache(graph), urlResolver, theHttpRequest);
        }


        [Fact]
        public void find_by_handler_type_if_only_one_method()
        {

            urls.UrlFor<OnlyOneActionController>()
                .ShouldBe("/onlyoneaction/go");
        }

        [Fact]
        public void retrieve_by_controller_action_even_if_it_has_an_input_model()
        {
            urls.UrlFor<OneController>(x => x.M1(null), null).ShouldBe("/one/m1");
        }

        [Fact]
        public void retrieve_a_url_for_a_model_simple_case()
        {
            urls.UrlFor(new Model1()).ShouldBe("/one/m1");
        }

        [Fact]
        public void retrieve_a_url_for_a_inferred_model_simple_case()
        {
            urls.UrlFor<Model1>((string) null).ShouldBe("/one/m1");
        }

        [Fact]
        public void retrieve_a_url_for_a_model_that_does_not_exist()
        {
            Exception<FubuException>.ShouldBeThrownBy(() => { urls.UrlFor(new ModelWithNoChain()); });
        }


        [Fact]
        public void retrieve_a_url_for_a_model_that_has_route_inputs()
        {
            urls.UrlFor(new ModelWithInputs
            {
                Name = "Jeremy"
            }).ShouldBe("/find/Jeremy");
        }

        [Fact]
        public void retrieve_a_url_for_a_model_that_has_querystring_inputs()
        {
            var model = new ModelWithQueryStringInput() {Param = 42};

            urls.UrlFor(model).ShouldBe("/qs/test?Param=42");
        }

        [Fact]
        public void retrieve_a_url_for_a_model_that_has_mixed_inputs()
        {
            var model = new ModelWithQueryStringAndRouteInput() {Param = 42, RouteParam = 23};

            urls.UrlFor(model).ShouldBe("/qsandroute/test/23?Param=42");
        }

        [Fact]
        public void retrieve_url_by_input_type_with_parameters()
        {
            var parameters = new RouteParameters<ModelWithInputs>();
            parameters[x => x.Name] = "Max";

            urls.UrlFor<ModelWithInputs>(parameters).ShouldBe("/find/Max");
        }

        [Fact]
        public void retrieve_a_url_for_a_model_and_category()
        {
            urls.UrlFor(new UrlModel(), "different").ShouldBe("/one/m4");
        }

        [Fact]
        public void retrieve_a_url_by_action()
        {
            urls.UrlFor<OneController>(x => x.M2(), null).ShouldBe("/one/m2");
        }

        [Fact]
        public void retrieve_a_url_by_action_negative_case()
        {

            Exception<FubuException>.ShouldBeThrownBy(() => { urls.UrlFor<RandomClass>(x => x.Ignored(), null); });
        }

        [Fact]
        public void url_for_handler_type_and_method_positive()
        {
            var method = ReflectionHelper.GetMethod<OneController>(x => x.M3());

            urls.UrlFor(typeof (OneController), method, null).ShouldBe("/one/m3");
        }

        [Fact]
        public void url_for_handler_type_and_method_negative_case_should_throw_204()
        {
            Exception<FubuException>.ShouldBeThrownBy(() => {
                var method = ReflectionHelper.GetMethod<RandomClass>(x => x.Ignored());
                urls.UrlFor(typeof (OneController), method, null);
            }).ErrorCode.ShouldBe(2104);
        }

        [Fact]
        public void url_for_new_positive_case()
        {
            urls.UrlForNew<UrlModel>().ShouldBe("/two/m2");
            urls.UrlForNew(typeof (UrlModel)).ShouldBe("/two/m2");
        }

        [Fact]
        public void url_for_new_negative_case_should_throw_2109()
        {
            Exception<FubuException>.ShouldBeThrownBy(() => { urls.UrlForNew<ModelWithoutNewUrl>(); })
                .ErrorCode.ShouldBe(2109);
        }

        [Fact]
        public void has_new_url_positive()
        {
            urls.HasNewUrl(typeof (UrlModel)).ShouldBeTrue();
        }

        [Fact]
        public void has_new_url_negative()
        {
            urls.HasNewUrl(typeof (ModelWithoutNewUrl)).ShouldBeFalse();
        }


        [Fact]
        public void url_for_route_parameter_by_type_respects_the_absolute_path()
        {
            urls.UrlFor<Model6>(new RouteParameters())
                .ShouldBe("/one/a");
        }

        [Fact]
        public void url_for_route_parameter_by_type_and_category_respects_absolute_path()
        {
            urls.UrlFor<UrlModel>(new RouteParameters(), "different")
                .ShouldBe("/one/m4");
        }
    }

    
    public class Forwarding_tests
    {
        private BehaviorGraph graph;
        private UrlRegistry urls;
        private OwinHttpRequest theHttpRequest;

        public Forwarding_tests()
        {
            theHttpRequest = OwinHttpRequest.ForTesting();
            theHttpRequest.FullUrl("http://server/fubu");


            var registry = new FubuRegistry();
            registry.Actions.IncludeType<OneController>();
            registry.Actions.IncludeType<TwoController>();
            registry.Actions.IncludeType<QueryStringTestController>();
            registry.Actions.IncludeType<OnlyOneActionController>();

            graph = BehaviorGraph.BuildFrom(registry);

            var urlResolver = new ChainUrlResolver(theHttpRequest);

            urls = new UrlRegistry(new ChainResolutionCache(graph), urlResolver, theHttpRequest);
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