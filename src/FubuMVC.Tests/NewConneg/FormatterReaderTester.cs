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
    public class FormatterReaderTester : InteractionContext<FormatterReader<Address, IFormatter>>
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

        [Test]
        public void binds_properties_when_it_reads()
        {
            const string ExpectedBoundZipCode = "Bound Zip Code";
            var address = new Address
            {
                Line1 = "Line 1",
                Line2 = "Line 2",
                City = "City",
                State = "State",
                ZipCode = null
            };

            MockFor<IFormatter>().Stub(x => x.Read<Address>(theContext))
               .Return(address);

            MockFor<IObjectResolver>()
                .Stub(x => x.BindProperties((Address)null, null))
                .IgnoreArguments()
                .Callback<Address, IBindingContext>((model, context) =>
                {
                    model.ZipCode = ExpectedBoundZipCode;
                    return true;
                });

            var result = ClassUnderTest.Read("anything", theContext);
            result.ZipCode.ShouldEqual(ExpectedBoundZipCode);
        }

    }
}