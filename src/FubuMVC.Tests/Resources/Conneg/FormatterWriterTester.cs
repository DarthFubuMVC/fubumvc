using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Formatters;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Resources.Conneg
{
    
    public class FormatterWriterTester : InteractionContext<FormatterWriter<Address>>
    {
        [Fact]
        public void delegates_to_its_formatter_for_mimetypes()
        {
            MockFor<IFormatter>().Stub(x => x.MatchingMimetypes)
                .Return(new[]{"text/json", "application/json"});

            ClassUnderTest.Mimetypes.ShouldHaveTheSameElementsAs("text/json", "application/json");
        }

        [Fact]
        public void delegates_to_its_formatter_when_it_writes()
        {
            var address = new Address();

            var context = new MockedFubuRequestContext();
            ClassUnderTest.Write("something", context, address);

            MockFor<IFormatter>().AssertWasCalled(x => x.Write(context, address, "something"));
        }
    }
}