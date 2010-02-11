using System;
using System.Linq.Expressions;
using FubuMVC.Core.Models;
using FubuMVC.Core.Util;
using FubuMVC.StructureMap;
using FubuMVC.Tests.UI;
using NUnit.Framework;

namespace FubuMVC.Tests.Models
{
    [TestFixture]
    public class ConversionPropertyBinderTester
    {
        private InMemoryBindingContext context;
        private ConversionPropertyBinder converter;

        [SetUp]
        public void SetUp()
        {
            context = new InMemoryBindingContext();
            converter = new ConversionPropertyBinder(new ValueConverterRegistry(new IConverterFamily[0]));
        }

        private bool matches(Expression<Func<AddressViewModel, object>> expression)
        {
            var property = ReflectionHelper.GetProperty(expression);
            return converter.Matches(property);
        }

        private void shouldMatch(Expression<Func<AddressViewModel, object>> expression)
        {
            matches(expression).ShouldBeTrue();
        }

        private void shouldNotMatch(Expression<Func<AddressViewModel, object>> expression)
        {
            matches(expression).ShouldBeFalse();
        }

        [Test]
        public void converter_matches_primitive_properties_out_of_the_box()
        {
            shouldMatch(x => x.Address.Address1);
            shouldMatch(x => x.Address.Order);
            shouldMatch(x => x.Address.IsActive);
            shouldMatch(x => x.Address.DateEntered);
            shouldMatch(x => x.Address.Color); // enum value
            shouldMatch(x => x.Address.Guid);
        }

        [Test]
        public void set_a_property_correctly_against_a_binding_context()
        {
            var address = new Address();
            context.WithData("Address1", "2035 Ozark");
            context.StartObject(address);

            var property = ReflectionHelper.GetProperty<Address>(x => x.Address1);

            converter.Bind(property, context);

            address.Address1.ShouldEqual("2035 Ozark");
        }
    }
}