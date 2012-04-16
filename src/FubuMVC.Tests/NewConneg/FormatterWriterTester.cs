using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class FormatterWriterTester : InteractionContext<FormatterWriter<Address, IFormatter>>
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

            ClassUnderTest.Write("something", address);

            MockFor<IFormatter>().AssertWasCalled(x => x.Write(address, "something"));
        }
    }
}