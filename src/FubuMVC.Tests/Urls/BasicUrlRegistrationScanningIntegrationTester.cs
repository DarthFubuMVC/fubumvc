using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Urls
{
    [TestFixture]
    public class BasicUrlRegistrationScanningIntegrationTester
    {
        #region Setup/Teardown

        [TestFixtureSetUp]
        public void SetUp()
        {
            graph = BehaviorGraph.BuildFrom(x => x.Actions.IncludeClassesSuffixedWithController());

            var request = OwinHttpRequest.ForTesting().FullUrl("http://server/cool");
	        var urlResolver = new ChainUrlResolver(request);

            registry = new UrlRegistry(new ChainResolutionCache(new TypeResolver(), graph), urlResolver, new JQueryUrlTemplate(), request);
        }

        #endregion

        private BehaviorGraph graph;
        private IUrlRegistry registry;


        public class SpecialModel : RouteInputModel
        {
        }

        public class SpecialModelForDefault : RouteInputModel
        {
            [RouteInput]
            public int? PossiblyEmpty { get; set; }
        }

        public class SpecialController
        {
            // A tiny controller class.  The [UrlPattern] att
            // is only for exception cases and, sigh, Dovetail
            // legacy code
            [UrlPattern("special/{Name}/is/{Age}")]
            public void OverrideMethod(SpecialModel model)
            {
            }

            [UrlPattern("defaultage/{Name}/is/{PossiblyEmpty:10}")]
            public void OverrideMethodDefaultValue(SpecialModelForDefault model)
            {
            }

            [UrlPattern("override/noargs")]
            public void OverrideWithNoArgs()
            {
            }

            public void Querystring(ModelWithQueryStrings query)
            {
            }

            public void InputWithGuid(ModelWithGuid model)
            {

            }

            public string NoArgMethod()
            {
                return "something";
            }

            public string Index()
            {
                return "the index";
            }

            public void Generic<T>()
            {
                // should be ignored
            }
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
        public void get_url_for_input_model()
        {
            registry.UrlFor(new SpecialModel
            {
                Name = "Jeremy",
                Age = 35
            }).ShouldEqual("/special/Jeremy/is/35");
        }

        [Test]
        public void get_url_for_input_model_default_value()
        {
            registry.UrlFor(new SpecialModelForDefault()
            {
                Name = "Frank"
            }).ShouldEqual("/defaultage/Frank/is/10");
        }

        [Test]
        public void get_url_nullable_ok()
        {
            registry.UrlFor(new SpecialModelForDefault()
            {
                Name = "Frank",
                PossiblyEmpty = 36
            }).ShouldEqual("/defaultage/Frank/is/36");
        }

        [Test]
        public void has_action_calls_for_actions_with_no_input_args()
        {
            registry.UrlFor<SpecialController>(x => x.NoArgMethod(), null).ShouldEqual(
                "/special/noargmethod");
            registry.UrlFor<SpecialController>(x => x.Index(), null).ShouldEqual("/special/index");
        }

        [Test]
        public void get_templated_url_for_input_model()
        {
            registry.TemplateFor(new SpecialModel()).ShouldEqual("/special/${Name}/is/${Age}");
        }

        [Test]
        public void get_partially_templated_url_for_input_model()
        {
            // this test is failing but not because the code is wrong... because int defaults to 0 and theres no way to 
            // tell whether it really *should* be 0
            registry.TemplateFor(new SpecialModel() { Name = "Ryan" }).ShouldEqual("/special/Ryan/is/${Age}");
        }

        [Test]
        public void get_templated_url_with_empty_guid()
        {
            registry.TemplateFor(new ModelWithGuid()).ShouldEqual("/special/inputwithguid/${OtherGuid}/${Id}");
        }

        [Test]
        public void get_partially_templated_url_with_empty_guid()
        {
            var otherguid = Guid.NewGuid();

            registry.TemplateFor(new ModelWithGuid()
                                 {
                                     OtherGuid = otherguid
                                 }).ShouldEqual("/special/inputwithguid/" + otherguid.ToString() + "/${Id}");
        }

        [Test]
        public void get_templated_url_with_explicit_params()
        {
            registry.TemplateFor<SpecialModel>(Age => 0).ShouldEqual("/special/${Name}/is/0");
        }

        [Test]
        public void get_templated_url_with_empty_explicit_params()
        {
            registry.TemplateFor<SpecialModel>().ShouldEqual("/special/${Name}/is/${Age}");
        }
    }

    public class ModelWithGuid
    {
        [RouteInput]
        public Guid OtherGuid { get; set; }

        [RouteInput]
        public Guid Id { get; set; }
    }
}