using System;
using System.Reflection;
using FubuCore.Binding;
using FubuMVC.Core;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.Registration.Expressions
{
    [TestFixture]
    public class ModelExpressionTester
    {
        public class ExampleConverter : IConverterFamily
        {
            public bool Matches(PropertyInfo prop)
            {
                return false;
            }

            public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo prop)
            {
                throw new NotImplementedException();
            }
        }

        public class ExamplePropertyBinder : IPropertyBinder
        {
            public bool Matches(PropertyInfo property)
            {
                return false;
            }

            public void Bind(PropertyInfo property, IBindingContext context)
            {
                throw new NotImplementedException();
            }
        }

        public class ExampleModelBinder : IModelBinder
        {
            public bool Matches(Type type)
            {
                return false;
            }

            public void Bind(Type type, object instance, IBindingContext context)
            {
                throw new NotImplementedException();
            }

            public object Bind(Type type, IBindingContext context)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void can_register_custom_binding_services()
        {
            var registry = new FubuRegistry();
            registry.Models
                .BindModelsWith<ExampleModelBinder>()
                .BindPropertiesWith<ExamplePropertyBinder>()
                .ConvertUsing<ExampleConverter>();


            using (var runtime = registry.ToRuntime())
            {
                var container = runtime.Get<IContainer>();
                container.ShouldHaveRegistration<IConverterFamily, ExampleConverter>();
                container.ShouldHaveRegistration<IPropertyBinder, ExamplePropertyBinder>();
                container.ShouldHaveRegistration<IModelBinder, ExampleModelBinder>();
            }
        }
    }
}