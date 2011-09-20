using System;
using FubuMVC.Core.Rest.Media;
using FubuMVC.Core.Rest.Media.Formatters;
using FubuMVC.Tests.UI;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Rest.Media
{
    [TestFixture]
    public class FormatterMediaChoiceTester : InteractionContext<FormatterMediaChoice<Address>>
    {
        protected override void beforeEach()
        {
            MockFor<IFormatter>().Stub(x => x.MatchingMimetypes).Return(new string[]{"application/json", "text/json"});
        }

        [Test]
        public void matches_positive()
        {
            ClassUnderTest.Matches("application/json").ShouldBeTrue();
            ClassUnderTest.Matches("text/json").ShouldBeTrue();
        }

        [Test]
        public void matches_negative()
        {
            ClassUnderTest.Matches("something").ShouldBeFalse();
        }

        [Test]
        public void write_should_delegate_to_the_formatter()
        {
            var address = new Address();
            ClassUnderTest.Write(address);

            MockFor<IFormatter>().AssertWasCalled(x => x.Write(address));
        }
    }
}