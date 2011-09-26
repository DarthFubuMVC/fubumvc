using System.Collections.Generic;
using System.Web.Routing;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Runtime;
using FubuMVC.Tests.Registration;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;
using FubuMVC.StructureMap;
using System.Linq;

namespace FubuMVC.Tests.StructureMapIoC
{
    [TestFixture]
    public class when_applying_diagnostics_from_FubuApplication
    {
        private Container theContainer;
        private IList<RouteBase> theRoutes;

        [SetUp]
        public void SetUp()
        {
            theContainer = new Container();
            theRoutes = FubuApplication.For(() => new FubuRegistry(x =>
            {
                x.Actions.IncludeType<DiagnosticController>();
                x.IncludeDiagnostics(true);
            }))
                .StructureMap(() => theContainer)
                .Bootstrap();            
        }


        [Test]
        public void building_out_a_behavior_chain_should_put_behavior_tracers_around_behaviors()
        {
            theContainer.GetAllInstances<IActionBehavior>().All(x => x is BehaviorTracer).ShouldBeTrue();
        }
    }

    public class DiagnosticController
    {
        public void Go(Model1 input){}
        public void G2(Model1 input) { }
        public void G3(Model1 input) { }
    }
}