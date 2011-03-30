using System;
using FubuFastPack.Binding;
using FubuFastPack.Persistence;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuFastPack.Testing.Binding
{
    [TestFixture]
    public class DomainEntityConverterFamilyTester : InteractionContext<DomainEntityConverterFamily>
    {
        [Test]
        public void should_match_an_entity()
        {
            ClassUnderTest.Matches(typeof(Case), null).ShouldBeTrue();
            ClassUnderTest.Matches(typeof(Site), null).ShouldBeTrue();
            ClassUnderTest.Matches(typeof(Solution), null).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_primitives()
        {
            ClassUnderTest.Matches(typeof(string), null).ShouldBeFalse();
            ClassUnderTest.Matches(typeof(int), null).ShouldBeFalse();
            ClassUnderTest.Matches(typeof(DateTime), null).ShouldBeFalse();
            ClassUnderTest.Matches(typeof(long), null).ShouldBeFalse();
        }

        [Test]
        public void can_return_a_finder_for_an_entity()
        {
            var id = Guid.NewGuid();

            var func = ClassUnderTest.CreateConverter(typeof (Case), null);

            var @case = new Case{Id = id};
            MockFor<IRepository>().Stub(x => x.Find<Case>(id)).Return(@case);

            func(id.ToString()).ShouldBeTheSameAs(@case);
        }
    }
}