using System.Reflection;
using FubuMVC.Core.Models;
using FubuMVC.Core.Util;
using FubuMVC.Tests.Diagnostics;
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
            var context = new InMemoryBindingContext();
            context["Hungry"] = "Hungry";

            ValueConverter converter = new BooleanFamily().Build(null, property);
            context.ForProperty(property, () =>
            {
                converter(context).As<bool>().ShouldBeTrue();
            });
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
            var context = new InMemoryBindingContext();
            context["Hungry"] = "false";

            ValueConverter converter = new BooleanFamily().Build(null, property);
            context.ForProperty(property, () =>
            {
                converter(context).As<bool>().ShouldBeFalse();
            });
        }

        [Test]
        public void treats_true_string_as_true()
        {
            PropertyInfo property = ReflectionHelper.GetProperty<DummyClass>(c => c.Hungry);
            var context = new InMemoryBindingContext();
            context["Hungry"] = "true";

            ValueConverter converter = new BooleanFamily().Build(null, property);
            context.ForProperty(property, () =>
            {
                converter(context).As<bool>().ShouldBeTrue();
            });
        }
    }
}