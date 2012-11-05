using FubuMVC.Core.Registration.Diagnostics;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Diagnostics
{
    [TestFixture]
    public class ProvenanceChainTester
    {
        private Provenance p1;
        private Provenance p2;
        private Provenance p3;
        private Provenance p4;

        [SetUp]
        public void SetUp()
        {
            p1 = MockRepository.GenerateMock<Provenance>();
            p2 = MockRepository.GenerateMock<Provenance>();
            p3 = MockRepository.GenerateMock<Provenance>();
            p4 = MockRepository.GenerateMock<Provenance>();
        }

        [Test]
        public void equals_method_is_predictable()
        {
            var chain1 = new ProvenanceChain(new Provenance[] {p1, p2, p3});
            var chain2 = new ProvenanceChain(new Provenance[] {p1, p2, p3});

            chain1.ShouldEqual(chain2);
            chain2.ShouldEqual(chain1);
        }

        [Test]
        public void prepend()
        {
            var chain1 = new ProvenanceChain(new Provenance[] { p1, p2 });
            chain1.Prepend(new Provenance[]{p3, p4});

            chain1.Chain.ShouldHaveTheSameElementsAs(p3, p4, p1, p2);
        }

        [Test]
        public void push()
        {
            var chain1 = new ProvenanceChain(new Provenance[] { p1, p2, p3 });
            var chain2 = chain1.Push(p4);

            chain2.Chain.ShouldHaveTheSameElementsAs(p1, p2, p3, p4);
        }

        [Test]
        public void pop()
        {
            var chain1 = new ProvenanceChain(new Provenance[] { p1, p2, p3 });
            var chain2 = chain1.Push(p4);

            chain2.Pop().ShouldBeTheSameAs(chain1);
        }
    }
}