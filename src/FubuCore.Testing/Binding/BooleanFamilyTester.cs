using System.Reflection;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
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
            context.ForProperty(property, x =>
            {
                converter.Convert(context).As<bool>().ShouldBeTrue();
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
    }

    [TestFixture]
    public class BooleanFamily_can_convert_various_string_inputs_to_boolean
    {
        private bool WithValue(string value)
        {
            PropertyInfo property = ReflectionHelper.GetProperty<BooleanFamilyTester.DummyClass>(c => c.Hungry);
            var context = new InMemoryBindingContext();
            context["Hungry"] = value;

            var convertedValue = false;

            ValueConverter converter = new BooleanFamily().Build(null, property);
            context.ForProperty(property, x =>
            {
                convertedValue = converter.Convert(context).As<bool>();
            });

            return convertedValue;
        }

        [Test]
        public void treats_false_string_as_false()
        {
            WithValue("false").ShouldBeFalse();
            WithValue("False").ShouldBeFalse();
            WithValue("FALSE").ShouldBeFalse();
        }

        [Test]
        public void treats_true_string_as_true()
        {
            WithValue("true").ShouldBeTrue();
            WithValue("True").ShouldBeTrue();
            WithValue("TRUE").ShouldBeTrue();
        }

        [Test]
        public void treats_on_string_as_true()
        {
            WithValue("on").ShouldBeTrue();
            WithValue("ON").ShouldBeTrue();
        }

        [Test]
        public void treats_empty_string_as_false()
        {
            WithValue("").ShouldBeFalse();
        }
    }
}