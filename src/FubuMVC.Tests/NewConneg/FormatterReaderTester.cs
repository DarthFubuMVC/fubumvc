using System.Collections.Generic;

using FubuCore.Binding;

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

            MockFor<IFormatter>().Stub(x => x.Read<Address>())
                .Return(address);

            ClassUnderTest.Read("anything").ShouldBeTheSameAs(address);
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

            MockFor<IFormatter>().Stub(x => x.Read<Address>())
               .Return(address);

            MockFor<IObjectResolver>()
                .Stub(x => x.BindProperties((Address)null, null))
                .IgnoreArguments()
                .Callback<Address, IBindingContext>((model, context) =>
                {
                    model.ZipCode = ExpectedBoundZipCode;
                    return true;
                });

            var result = ClassUnderTest.Read("anything");
            result.ZipCode.ShouldEqual(ExpectedBoundZipCode);
        }

        [Test]
        public void binds_properties_after_reading()
        {
            var calls = new List<string>();

            MockFor<IFormatter>().Stub(x => x.Read<Address>()).Callback(() =>
            {
                calls.Add("Formatter");
                return true;
            });

            MockFor<IObjectResolver>()
                .Stub(x => x.BindProperties((Address)null, null))
                .IgnoreArguments()
                .Callback<Address, IBindingContext>((model, context) =>
                {
                    calls.Add("Binder");
                    return true;
                });

            ClassUnderTest.Read("anything");
            calls.ShouldHaveTheSameElementsAs("Formatter", "Binder");
        }
    }
}