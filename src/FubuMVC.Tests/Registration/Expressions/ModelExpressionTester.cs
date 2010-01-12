using System;
using System.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Models;
using FubuMVC.Core.Registration;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Expressions
{
    [TestFixture]
    public class ModelExpressionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _graph = new FubuRegistry(x => x.Models.ConvertUsing<ExampleConverter>()).BuildGraph();
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

        [Test]
        public void should_register_the_converter_in_the_graph()
        {
            _graph.Services.DefaultServiceFor<IConverterFamily>().Type.ShouldEqual(typeof (ExampleConverter));
        }
    }
}