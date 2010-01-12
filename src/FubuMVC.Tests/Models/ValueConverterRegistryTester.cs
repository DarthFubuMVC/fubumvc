using System.Reflection;
using FubuMVC.Core.Models;
using FubuMVC.Core.Util;
using NUnit.Framework;

namespace FubuMVC.Tests.Models
{
    [TestFixture]
    public class ValueConverterRegistryTester
    {
        public class Target
        {
            public int Integer { get; set; }
            public bool Boolean { get; set; }
            public int? NullInt { get; set; }
        }

        [Test]
        public void basic_convert_an_object_that_is_already_the_right_type()
        {
            ValueConverterRegistry.BasicConvert(typeof (string), "abc").ShouldEqual("abc");
            ValueConverterRegistry.BasicConvert(typeof (int), 123).ShouldEqual(123);
        }

        [Test]
        public void basic_convert_an_object_that_is_not_the_correct_type()
        {
            ValueConverterRegistry.BasicConvert(typeof (int), "123").ShouldEqual(123);
        }

        [Test]
        public void should_convert_nonnull_values_for_nullable_types()
        {
            PropertyInfo nullIntProp = ReflectionHelper.GetProperty<Target>(x => x.NullInt);
            var reg = new ValueConverterRegistry(new IConverterFamily[0]);
            var value = new RawValue
            {
                Property = nullIntProp,
                Value = "99"
            };
            reg.FindConverter(nullIntProp)(value).ShouldEqual(99);
        }

        [Test]
        public void should_convert_null_values_for_nullable_types()
        {
            PropertyInfo nullIntProp = ReflectionHelper.GetProperty<Target>(x => x.NullInt);
            var reg = new ValueConverterRegistry(new IConverterFamily[0]);
            var value = new RawValue
            {
                Property = nullIntProp,
                Value = null
            };
            reg.FindConverter(nullIntProp)(value).ShouldEqual(null);
        }
    }
}