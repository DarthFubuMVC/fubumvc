using System.Linq;
using AutoImportTarget;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class AutoImport_of_configuration_actions_and_extensions_Tester
    {
        private BehaviorGraph theGraph;

        [SetUp]
        public void SetUp()
        {
            theGraph = BehaviorGraph.BuildFrom<AutoImportRegistry>();
        }

        [Test]
        public void should_have_evidence_that_an_auto_import_extension_was_applied()
        {
            theGraph.BehaviorFor<FooHandler>(x => x.get_hello())
                .ShouldNotBeNull();
        }

        [Test]
        public void should_have_evidence_That_an_auto_import_policy_was_applied()
        {
            theGraph.BehaviorFor<FooHandler>(x => x.get_hello())
                .OfType<Wrapper>()
                .Single()
                .Type
                .ShouldEqual(typeof (FooWrapper));
        }
    }
}