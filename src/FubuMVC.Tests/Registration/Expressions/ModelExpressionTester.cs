using System;
using System.Reflection;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.Registration.Expressions
{
    [TestFixture]
    public class ModelExpressionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Models
                    .BindModelsWith<ExampleModelBinder>()
                    .BindPropertiesWith<ExamplePropertyBinder>()
                    .ConvertUsing<ExampleConverter>();
            });
        }

        #endregion

        private BehaviorGraph _graph;

        public class ExampleConverter : IConverterFamily
        {
            public bool Matches(PropertyInfo prop)
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
        public void should_register_the_converter_in_the_graph()
        {
            _graph.Services.ServicesFor<IConverterFamily>().Select(x => x.Type).ShouldContain(typeof(ExampleConverter));
        }

        [Test]
        public void should_register_the_property_binder_in_the_graph()
        {
            _graph.Services.ServicesFor<IPropertyBinder>().Select(x => x.Type).ShouldContain(typeof(ExamplePropertyBinder));
        }
        [Test]
        public void should_register_the_model_binder_in_the_graph()
        {
            _graph.Services.ServicesFor<IModelBinder>().Where(x => x.Type != null).Select(x => x.Type).ShouldContain(typeof(ExampleModelBinder));
        }
    }

    
}