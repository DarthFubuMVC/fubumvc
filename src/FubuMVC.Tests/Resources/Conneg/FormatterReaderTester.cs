using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.Resources.Conneg
{
    
    public class FormatterReaderTester : InteractionContext<FormatterReader<Address>>
    {
        private MockedFubuRequestContext theContext;

        protected override void beforeEach()
        {
            theContext = new MockedFubuRequestContext(Services.Container);
            Services.Inject<IFubuRequestContext>(theContext);
        }

        [Fact]
        public void delegates_to_its_formatter_for_mimetypes()
        {
            MockFor<IFormatter>().Stub(x => x.MatchingMimetypes)
                .Return(new[]{"text/json", "application/json"});

            ClassUnderTest.Mimetypes.ShouldHaveTheSameElementsAs("text/json", "application/json");
        }

        [Fact]
        public void delegates_to_its_formatter_when_it_reads()
        {
            var address = new Address();

            MockFor<IFormatter>().Stub(x => x.Read<Address>(theContext))
                .Return(address);

            ClassUnderTest.Read("anything", theContext).ShouldBeTheSameAs(address);
        }


    }
}