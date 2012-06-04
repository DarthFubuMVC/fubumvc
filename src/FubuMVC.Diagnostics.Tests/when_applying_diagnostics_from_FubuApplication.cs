using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Diagnostics.Runtime.Tracing;
using FubuMVC.StructureMap;
using FubuMVC.Tests;
using FubuMVC.Tests.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Model1 = FubuMVC.Tests.Registration.Model1;

namespace FubuMVC.Diagnostics.Tests
{
    [TestFixture]
    public class when_applying_diagnostics_from_FubuApplication
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            theContainer = new Container();
            theContainer.Configure(x =>
            {
                x.For<IHttpWriter>().Use(new NulloHttpWriter());

                x.For<ICurrentHttpRequest>().Use(new StubCurrentHttpRequest{
                    TheApplicationRoot = "http://server"
                });

                x.For<IStreamingData>().Use(MockRepository.GenerateMock<IStreamingData>());

                x.For<ICurrentChain>().Use(new CurrentChain(null, null));
            });

            theRoutes = FubuApplication.For(() => new FubuRegistry(x =>
            {
                x.Actions.IncludeType<DiagnosticController>();
                x.Import<DiagnosticsRegistration>();
            }))
                .StructureMap(() => theContainer)
                .Bootstrap().Routes;

            theContainer.Configure(x => x.For<HttpContextBase>().Use(new FakeHttpContext()));
        }

        #endregion

        private Container theContainer;
        private IList<RouteBase> theRoutes;

        [Test]
        public void build_out_the_behavior_should_have_a_BehaviorTracer_around_everything_real()
        {
            var behavior = theContainer.GetAllInstances<IActionBehavior>().FirstOrDefault();
            behavior.ShouldBeOfType<DiagnosticBehavior>()
                .Inner.ShouldBeOfType<BehaviorTracer>()
                .Inner.ShouldNotBeOfType<BehaviorTracer>();
        }

        [Test]
        public void building_out_a_behavior_chain_should_put_behavior_tracers_around_behaviors()
        {
            Debug.WriteLine(theContainer.WhatDoIHave());
            theContainer.GetAllInstances<IActionBehavior>().All(x => x is DiagnosticBehavior).ShouldBeTrue();
        }
    }

    public class DiagnosticController
    {
        public void Go(Model1 input)
        {
        }

        public void G2(Model1 input)
        {
        }

        public void G3(Model1 input)
        {
        }
    }
}