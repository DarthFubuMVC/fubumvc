using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Diagnostics.Model;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Model
{
    [TestFixture]
    public class Diagnostic_Chain_Scanning_Integrated_Tester
    {
        [SetUp]
        public void SetUp()
        {
            theRuntime = FubuApplication.DefaultPolicies().StructureMap().Bootstrap();
        }

        [TearDown]
        public void TearDown()
        {
            theRuntime.Dispose();
        }

        private readonly BehaviorGraph theGraph = FubuApplication.DefaultPolicies()
            .StructureMap().Bootstrap().Factory.Get<BehaviorGraph>();

        private FubuRuntime theRuntime;

        [Test]
        public void has_the_diagnostic_chains_from_the_application_assembly()
        {
            var group = theRuntime.Factory.Get<DiagnosticGraph>()
                .FindGroup(Assembly.GetExecutingAssembly().GetName().Name);

            group.Links().Select(x => x.GetRoutePattern())
                .ShouldHaveTheSameElementsAs("_fubu/fake/link", "_fubu/fake/else", "_fubu/fake/simple");
            
        }

        [Test]
        public void has_the_diagnostic_group_for_FubuMVC_Diagnostics_itself()
        {
            var group = theRuntime.Factory.Get<DiagnosticGraph>()
                .FindGroup(typeof(DiagnosticGraph).Assembly.GetName().Name);

            var links = @group.Links().ToArray();
            links.Select(x => x.GetRoutePattern())
                .Each(x => Debug.WriteLine(x))
                .ShouldHaveTheSameElementsAs("_fubu/package/logs", "_fubu/conventions", "_fubu/endpoints", "_fubu/binding/all", "_fubu/requests");
        }
    }
}