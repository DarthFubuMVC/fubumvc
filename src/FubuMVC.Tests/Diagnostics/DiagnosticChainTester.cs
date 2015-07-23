using System.Linq;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Registration.Nodes;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class DiagnosticChainTester
    {
        [Test]
        public void build_the_route()
        {
            DiagnosticChain.For<FakeFubuDiagnostics>(x => x.get_link())
                .GetRoutePattern().ShouldBe("_fubu/fake/link");
        }

        [Test]
        public void chain_includes_the_call()
        {
            DiagnosticChain.For<FakeFubuDiagnostics>(x => x.get_link())
                .Last()
                .ShouldBeOfType<ActionCall>()
                .Description
                .ShouldBe("FakeFubuDiagnostics.get_link() : String");
        }
    }

    public class FakeQuery
    {
        public string Name { get; set; }
    }
}