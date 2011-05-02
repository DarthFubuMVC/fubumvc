using System;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Urls;
using FubuMVC.Tests.View.FakeViews;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Urls
{
    [TestFixture]
    public class UrlRegistryIntegrationTester
    {
        private BehaviorGraph graph;
        private UrlRegistry urls;
        private string _appBaseUrl;

        [SetUp]
        public void SetUp()
        {
            UrlContext.Reset();
            _appBaseUrl = "Fubu";
            UrlContext.Stub(_appBaseUrl);
            var registry = new FubuRegistry();
            registry.Actions.IncludeType<OneController>();
            registry.Actions.IncludeType<TwoController>();
            registry.Actions.ExcludeMethods(x => x.Method.Name.Contains("Ignore"));

            registry.Routes
                .IgnoreControllerFolderName()
                .IgnoreNamespaceForUrlFrom<UrlRegistryIntegrationTester>()
                .IgnoreClassSuffix("Controller");


            registry.ResolveTypes(x => x.AddStrategy<UrlModelForwarder>());

            // need to do forwards



            graph = registry.BuildGraph();

            var resolver = graph.Services.DefaultServiceFor<ITypeResolver>().Value;

            urls = new UrlRegistry(new ChainResolver((ITypeResolver) resolver, graph), new JQueryUrlTemplate());
        }

        [Test]
        public void retrieve_by_controller_action_even_if_it_has_an_input_model()
        {
            urls.UrlFor<OneController>(x => x.M1(null)).ShouldEqual("Fubu/one/m1");
        }

        [Test]
        public void retrieve_a_url_for_a_model_simple_case()
        {
            urls.UrlFor(new Model1()).ShouldEqual("Fubu/one/m1");
        }



        [Test]
        public void retrieve_a_url_for_a_model_that_does_not_exist()
        {
            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                urls.UrlFor(new ModelWithNoChain());
            });
        }



        [Test]
        public void retrieve_a_url_for_a_model_that_has_route_inputs()
        {
            urls.UrlFor(new ModelWithInputs{
                Name = "Jeremy"
            }).ShouldEqual("Fubu/find/Jeremy");
        }

        [Test]
        public void retrieve_url_by_input_type_with_parameters()
        {
            var parameters = new RouteParameters<ModelWithInputs>();
            parameters[x => x.Name] = "Max";

            urls.UrlFor<ModelWithInputs>(parameters).ShouldEqual("Fubu/find/Max");
        }

        [Test]
        public void retrieve_a_url_for_a_model_and_category()
        {
            urls.UrlFor(new UrlModel(), "different").ShouldEqual("Fubu/one/m4");
        }

        [Test]
        public void retrieve_a_url_for_a_model_and_category_negative_case()
        {
            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                urls.UrlFor(new Model3(), "not a real category");
            });
        }

        [Test]
        public void retrieve_a_url_by_action()
        {
            urls.UrlFor<OneController>(x => x.M2()).ShouldEqual("Fubu/one/m2");
        }

        [Test]
        public void retrieve_a_url_by_action_negative_case()
        {
            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                urls.UrlFor<OneController>(x => x.Ignored());
            });
        }

        [Test]
        public void url_for_handler_type_and_method_positive()
        {
            var method = ReflectionHelper.GetMethod<OneController>(x => x.M3());

            urls.UrlFor(typeof(OneController), method).ShouldEqual("Fubu/one/m3");
        }

        [Test]
        public void url_for_handler_type_and_method_negative_case_should_throw_208()
        {
            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                var method = ReflectionHelper.GetMethod<OneController>(x => x.Ignored());
                urls.UrlFor(typeof (OneController), method);
            }).ErrorCode.ShouldEqual(2108);
        }

        [Test]
        public void url_for_new_positive_case()
        {
            urls.UrlForNew<UrlModel>().ShouldEqual("Fubu/two/m2");
            urls.UrlForNew(typeof(UrlModel)).ShouldEqual("Fubu/two/m2");
        }

        [Test]
        public void url_for_new_negative_case_should_throw_2109()
        {
            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                urls.UrlForNew<ModelWithoutNewUrl>();
            }).ErrorCode.ShouldEqual(2109);
        }

        [Test]
        public void has_new_url_positive()
        {
            urls.HasNewUrl(typeof(UrlModel)).ShouldBeTrue();
        }

        [Test]
        public void has_new_url_negative()
        {
            urls.HasNewUrl(typeof(ModelWithoutNewUrl)).ShouldBeFalse();
        }

        [Test]
        public void retrieve_a_url_for_a_model_when_it_trips_off_type_resolver_rules()
        {
            urls.UrlFor(new SubclassUrlModel()).ShouldEqual("Fubu/two/m4");
        }

        [Test]
        public void forward_without_a_category()
        {
            graph.Forward<Model4>(m => new Model3());
            urls.UrlFor(new Model4()).ShouldEqual("Fubu/one/m5");
        }

        [Test]
        public void forward_with_a_category()
        {
            graph.Forward<Model4>(m => new Model6(), "A");
            graph.Forward<Model4>(m => new Model7(), "B");

            urls.UrlFor(new Model4(), "A").ShouldEqual("Fubu/one/a");
            urls.UrlFor(new Model4(), "B").ShouldEqual("Fubu/one/b");
        }

        [Test]
        public void forward_with_route_inputs()
        {
            graph.Forward<Model4>(m => new ModelWithInputs(){Name = "chiefs"});
            urls.UrlFor(new Model4()).ShouldEqual("Fubu/find/chiefs");
        }

        [Test]
        public void forward_respects_the_type_resolution()
        {
            graph.Forward<UrlModel>(m => new Model6(), "new");
            urls.UrlFor(new SubclassUrlModel(), "new").ShouldEqual("Fubu/one/a");
        }
    }


    public class OneController
    {
        [UrlPattern("find/{Name}")]
        public void MethodWithPattern(ModelWithInputs inputs)
        {
            
        }

        public void A(Model6 input){}
        public void B(Model7 input){}

        public void Ignored()
        {
            
        }

        public void M1(Model1 input){}
        public void M2(){}
        public void M3(){}

        public void M5(Model3 input)
        {
        }

        [UrlRegistryCategory("different")]
        public void M4(UrlModel model) { }
    }

    public class TwoController
    {
        public void M1() { }

        [UrlForNew(typeof(UrlModel))]
        public void M2() { }
        public void M3() { }
        public void M4(UrlModel model) { }
    }

    public class ModelWithInputs
    {
        public string Name { get; set; }
    }
    public class Model1{}
    public class Model2{}
    public class Model3{}
    public class Model4{}
    public class Model5{}
    public class Model6{}
    public class Model7{}
    public class ModelWithNoChain{}
    public class ModelWithoutNewUrl{}

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


}