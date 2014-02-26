using System.Collections.Generic;

using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class FormatterReaderTester : InteractionContext<FormatterReader<Address>>
    {
        private MockedFubuRequestContext theContext;

        protected override void beforeEach()
        {
            theContext = new MockedFubuRequestContext(Services.Container);
            Services.Inject<IFubuRequestContext>(theContext);
        }

        [Test]
        public void delegates_to_its_formatter_for_mimetypes()
        {
            MockFor<IFormatter>().Stub(x => x.MatchingMimetypes)
                .Return(new[]{"text/json", "application/json"});

            ClassUnderTest.Mimetypes.ShouldHaveTheSameElementsAs("text/json", "application/json");
        }

        [Test]
        public void delegates_to_its_formatter_when_it_reads()
        {
            var address = new Address();

            MockFor<IFormatter>().Stub(x => x.Read<Address>(theContext))
                .Return(address);

            ClassUnderTest.Read("anything", theContext).ShouldBeTheSameAs(address);
        }


    }
}