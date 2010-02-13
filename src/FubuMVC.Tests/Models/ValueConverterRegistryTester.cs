using System.Reflection;
using FubuMVC.Core.Models;
using FubuMVC.Core.Util;
using FubuMVC.StructureMap;
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

        public class TargetHolder
        {
            public Target Target { get; set; }
        }

        [Test]
        public void return_a_null_converter()
        {
            var property = ReflectionHelper.GetProperty<TargetHolder>(x => x.Target);
            new ValueConverterRegistry(new IConverterFamily[0]).FindConverter(property).ShouldBeNull();
        }

        [Test]
        public void should_convert_nonnull_values_for_nullable_types()
        {
            PropertyInfo nullIntProp = ReflectionHelper.GetProperty<Target>(x => x.NullInt);
            var reg = new ValueConverterRegistry(new IConverterFamily[0]);
            var value = new InMemoryBindingContext().WithPropertyValue("99");
            reg.FindConverter(nullIntProp)(value).ShouldEqual(99);
        }

        [Test]
        public void should_convert_null_values_for_nullable_types()
        {
            PropertyInfo nullIntProp = ReflectionHelper.GetProperty<Target>(x => x.NullInt);
            var reg = new ValueConverterRegistry(new IConverterFamily[0]);

            var value = new InMemoryBindingContext().WithPropertyValue(null);
            reg.FindConverter(nullIntProp)(value).ShouldEqual(null);
        }
    }
}