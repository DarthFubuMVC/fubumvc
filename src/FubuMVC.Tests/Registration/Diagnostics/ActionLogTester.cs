using FubuMVC.Core.Registration.Diagnostics;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Diagnostics
{
    [TestFixture]
    public class ActionLogTester
    {
        [Test]
        public void prepend_provenance()
        {
            var action = new UniquePolicy();
            var p1 = MockRepository.GenerateMock<Provenance>();
            var p2 = MockRepository.GenerateMock<Provenance>();
            var p3 = MockRepository.GenerateMock<Provenance>();
            var p4 = MockRepository.GenerateMock<Provenance>();

            var chain = new Provenance[] {p1, p2};

            var log = new ActionLog(action, chain);

            log.PrependProvenance(new Provenance[]{p3, p4});

            log.ProvenanceChain.ShouldHaveTheSameElementsAs(p3, p4, p1, p2);
        }
    }
}