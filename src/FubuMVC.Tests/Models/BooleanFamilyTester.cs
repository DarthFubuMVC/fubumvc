using System.Reflection;
using FubuMVC.Core.Models;
using FubuMVC.Core.Util;
using NUnit.Framework;

namespace FubuMVC.Tests.Models
{
    [TestFixture]
    public class BooleanFamilyTester
    {
        public class DummyClass
        {
            public bool Hungry { get; set; }
            public bool? Thirsty { get; set; }
        }

        [Test]
        public void can_accept_the_property_name_and_treat_it_as_true()
        {
            PropertyInfo property = ReflectionHelper.GetProperty<DummyClass>(c => c.Hungry);
            object convertedValue = new BooleanFamily().Build(null, property)(new RawValue
            {
                Property = property,
                Value = "Hungry"
            });
            ((bool) convertedValue).ShouldBeTrue();
        }

        [Test]
        public void can_convert_boolean_values()
        {
            new BooleanFamily().Matches(ReflectionHelper.GetProperty<DummyClass>(c => c.Hungry)).ShouldBeTrue();
        }

        [Test]
        public void can_convert_nullable_boolean_values()
        {
            new BooleanFamily().Matches(ReflectionHelper.GetProperty<DummyClass>(c => c.Thirsty)).ShouldBeTrue();
        }

        [Test]
        public void treats_false_string_as_false()
        {
            PropertyInfo property = ReflectionHelper.GetProperty<DummyClass>(c => c.Hungry);
            object convertedValue = new BooleanFamily().Build(null, property)(new RawValue
            {
                Property = property,
                Value = "false"
            });
            ((bool) convertedValue).ShouldBeFalse();
        }

        [Test]
        public void treats_true_string_as_true()
        {
            PropertyInfo property = ReflectionHelper.GetProperty<DummyClass>(c => c.Hungry);
            object convertedValue = new BooleanFamily().Build(null, property)(new RawValue
            {
                Property = property,
                Value = "true"
            });
            ((bool) convertedValue).ShouldBeTrue();
        }
    }
}