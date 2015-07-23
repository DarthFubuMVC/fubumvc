using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Urls;
using Shouldly;
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

            registry = new UrlRegistry(new ChainResolutionCache(graph), urlResolver, request);
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
            }).ShouldBe("/special/Jeremy/is/35");
        }

        [Test]
        public void get_url_for_input_model_default_value()
        {
            registry.UrlFor(new SpecialModelForDefault()
            {
                Name = "Frank"
            }).ShouldBe("/defaultage/Frank/is/10");
        }

        [Test]
        public void get_url_nullable_ok()
        {
            registry.UrlFor(new SpecialModelForDefault()
            {
                Name = "Frank",
                PossiblyEmpty = 36
            }).ShouldBe("/defaultage/Frank/is/36");
        }

        [Test]
        public void has_action_calls_for_actions_with_no_input_args()
        {
            registry.UrlFor<SpecialController>(x => x.NoArgMethod(), null).ShouldBe(
                "/special/noargmethod");
            registry.UrlFor<SpecialController>(x => x.Index(), null).ShouldBe("/special/index");
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