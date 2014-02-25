using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class FormatterWriterTester : InteractionContext<FormatterWriter<Address>>
    {
        [Test]
        public void delegates_to_its_formatter_for_mimetypes()
        {
            MockFor<IFormatter>().Stub(x => x.MatchingMimetypes)
                .Return(new[]{"text/json", "application/json"});

            ClassUnderTest.Mimetypes.ShouldHaveTheSameElementsAs("text/json", "application/json");
        }

        [Test]
        public void delegates_to_its_formatter_when_it_writes()
        {
            var address = new Address();

            var context = new MockedFubuRequestContext();
            ClassUnderTest.Write("something", context, address);

            MockFor<IFormatter>().AssertWasCalled(x => x.Write(context, address, "something"));
        }
    }
}