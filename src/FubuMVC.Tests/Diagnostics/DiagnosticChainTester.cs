using System.Linq;
using FubuMVC.Core.Behaviors.Chrome;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    public class FakeDiagnosticGroup : DiagnosticGroup
    {
        public FakeDiagnosticGroup() : base("fake")
        {
        }
    }

    [TestFixture]
    public class DiagnosticChainTester
    {
        private DiagnosticGroup theGroup = new FakeDiagnosticGroup();



        [Test]
        public void build_the_route()
        {
            DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.get_link())
                .GetRoutePattern().ShouldEqual("_fubu/fake/link");
        }

        [Test]
        public void chain_includes_the_call()
        {
            DiagnosticChain.For<FakeFubuDiagnostics>(theGroup, x => x.get_link())
                .Last()
                .ShouldBeOfType<ActionCall>()
                .Description
                .ShouldEqual("FakeFubuDiagnostics.get_link() : String");
        }


    }

    public class FakeQuery
    {
        public string Name { get; set; }
    }
}