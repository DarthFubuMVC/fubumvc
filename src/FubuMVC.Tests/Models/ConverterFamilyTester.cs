using System.Reflection;
using FubuCore.Binding;
using FubuCore.Reflection;
using NUnit.Framework;

namespace FubuMVC.Tests.Models
{
    [TestFixture]
    public class ConverterFamilyTester
    {
        public class ConverterTarget
        {
            public int Integer { get; set; }
            public bool Boolean { get; set; }
        }

        [Test]
        public void match_uses_the_predicate_in_the_ctor()
        {
            PropertyInfo intProp = ReflectionHelper.GetProperty<ConverterTarget>(c => c.Integer);
            PropertyInfo boolProp = ReflectionHelper.GetProperty<ConverterTarget>(c => c.Boolean);

            var family = new ConverterFamily(p => p.PropertyType == typeof (int), (r, t) => null);
            family.Matches(intProp).ShouldBeTrue();
            family.Matches(boolProp).ShouldBeFalse();
        }

        [Test]
        public void use_the_builder_func_from_the_constructor_to_build_the_string_converter()
        {
            ValueConverter converter = x => 1;

            var family = new ConverterFamily(null, (r, t) => converter);
            family.Build(null, null).ShouldBeTheSameAs(converter);
        }
    }
}