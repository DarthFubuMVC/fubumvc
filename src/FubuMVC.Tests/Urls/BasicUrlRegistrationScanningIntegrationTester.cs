using System;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Urls;
using NUnit.Framework;

namespace FubuMVC.Tests.Urls
{
    [TestFixture]
    public class BasicUrlRegistrationScanningIntegrationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            UrlContext.Reset();

            graph = new FubuRegistry(x => x.Actions.IncludeClassesSuffixedWithController()).BuildGraph();

            registry = new UrlRegistry(new ChainResolver(new TypeResolver(), graph));
        }

        #endregion

        private BehaviorGraph graph;
        private IUrlRegistry registry;


        public class SpecialModel : RouteInputModel
        {
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

            [UrlPattern("override/noargs")]
            public void OverrideWithNoArgs()
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
            }).ShouldEqual("special/Jeremy/is/35");
        }

        [Test]
        public void has_action_calls_for_actions_with_no_input_args()
        {
            registry.UrlFor<SpecialController>(x => x.NoArgMethod()).ShouldEqual(
                "fubumvc/tests/urls/special/noargmethod");
            registry.UrlFor<SpecialController>(x => x.Index()).ShouldEqual("fubumvc/tests/urls/special/index");
        }
    }
}