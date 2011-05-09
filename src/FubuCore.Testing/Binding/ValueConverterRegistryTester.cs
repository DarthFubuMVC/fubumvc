using System.Reflection;
using System.Web;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
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
            value.ForProperty(nullIntProp, c =>
            {
                reg.FindConverter(nullIntProp).Convert(c).ShouldEqual(99);
            });

            
        }

        [Test]
        public void should_convert_null_values_for_nullable_types()
        {
            PropertyInfo nullIntProp = ReflectionHelper.GetProperty<Target>(x => x.NullInt);
            var reg = new ValueConverterRegistry(new IConverterFamily[0]);

            var value = new InMemoryStructureMapBindingContext().WithPropertyValue(null);
            value.ForProperty(nullIntProp, c =>
            {
                reg.FindConverter(nullIntProp).Convert(c).ShouldEqual(null);
            });

            
        }

        [Test]
        public void can_find_a_converter_for_HttpPostedFileBase()
        {
            var registry = new ValueConverterRegistry(new IConverterFamily[0]);

            var property = ReflectionHelper.GetProperty<ModelWithHttpPostedFileBase>(x => x.File);
            registry.FindConverter(property).ShouldNotBeNull();
        }

        [Test]
        public void can_find_a_converter_for_HttpCookie()
        {
            var registry = new ValueConverterRegistry(new IConverterFamily[0]);

            var property = ReflectionHelper.GetProperty<ModelWithHttpPostedFileBase>(x => x.MyCookie);
            registry.FindConverter(property).ShouldNotBeNull();
        }


        [Test]
        public void can_find_a_converter_for_a_system_property()
        {
            var registry = new ValueConverterRegistry(new IConverterFamily[0]);

            var property = ReflectionHelper.GetProperty<ModelWithHttpPostedFileBase>(x => x.AcceptTypes);
            registry.FindConverter(property).ShouldNotBeNull();
        }

        public class ModelWithHttpPostedFileBase
        {
            public HttpPostedFileBase File { get; set; }
            public string File2 { get; set; }
            public HttpCookie MyCookie { get; set; }

            // this is a "system" property defined in AggregateDictionary
            public string[] AcceptTypes { get; set; }
        }
    }
}