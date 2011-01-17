using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.StructureMap;
using FubuMVC.Tests.UI;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Models
{
    [TestFixture]
    public class ConversionPropertyBinderTester : PropertyBinderTester
    {
        [SetUp]
        public void SetUp()
        {
            context = new InMemoryBindingContext();
            propertyBinder = new ConversionPropertyBinder(new ValueConverterRegistry(new IConverterFamily[0]));
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
        public void converter_should_not_match_non_primitive_properties_out_of_the_box()
        {
            shouldNotMatch(x => x.Address);
        }

        [Test]
        public void set_a_property_correctly_against_a_binding_context()
        {
            var address = new Address();
            context.WithData("Address1", "2035 Ozark");
            context.StartObject(address);

            var property = ReflectionHelper.GetProperty<Address>(x => x.Address1);

            propertyBinder.Bind(property, context);

            address.Address1.ShouldEqual("2035 Ozark");
        }
    }

    [TestFixture]
    public class IgnorePropertyBinderTester
    {
        private IgnorePropertyBinder _binder;
        private IBindingContext _context;
        private PropertyInfo _property;

        private class SomeObject{public object SomeProperty { get; set; }}

        [SetUp]
        public void SetUp()
        {
            _binder = new IgnorePropertyBinder(info => info.Name == "SomeProperty");
            _context = MockRepository.GenerateMock<IBindingContext>();
        }

        [Test]
        public void should_match()
        {
            _property = ReflectionHelper.GetProperty<SomeObject>(o=>o.SomeProperty);
            _binder.Matches(_property).ShouldBeTrue();
        }

        [Test]
        public void should_ignore_binding()
        {
            //NOTE: Nothing to test, Bind method is empty
            _binder.Bind(_property, _context);
        }
    }
}