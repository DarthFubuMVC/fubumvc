using System;
using System.Reflection;
using FubuCore.Binding;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class ConverterExpressionTester
    {
        private IRegistrar _registrar;
        private ConverterExpression _expression;
        private Func<IValueConverterRegistry, PropertyInfo, ValueConverter> _builder;
        private Predicate<PropertyInfo> _matches;
        private PropertyInfo _property;

        public interface IRegistrar
        {
            void Register(ConverterFamily family);
        }

        private class PropertyHolder{public string SomeProperty { get; set; }}

        [SetUp]
        public void SetUp()
        {
            _registrar = MockRepository.GenerateStub<IRegistrar>();
            _builder = ((registry, info) => ctx => ctx.PropertyValue);
            _matches = (info => info.Name == "SomeProperty");
            _property = typeof(PropertyHolder).GetProperty("SomeProperty");
            _registrar.Stub(r => r.Register(Arg<ConverterFamily>.Matches(f=>f.Matches(_property))));
            _expression = new ConverterExpression(_matches, _registrar.Register);
        }

        [Test]
        public void should_set()
        {
            _expression.Use(_builder );
            _registrar.AssertWasCalled(r => r.Register(Arg<ConverterFamily>.Matches(f => f.Matches(_property))));
        }
    }
}