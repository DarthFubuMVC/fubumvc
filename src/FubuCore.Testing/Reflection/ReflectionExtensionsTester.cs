using System;
using System.Linq.Expressions;
using FubuTestingSupport;
using NUnit.Framework;
using FubuCore.Reflection;
using Rhino.Mocks;

namespace FubuCore.Testing.Reflection
{
    [TestFixture]
    public class ReflectionExtensionsTester
    {
        public class PropertyHolder{
            public int Age { get; set; }}
        public interface ICallback{void Callback();}

        private ICallback _callback;
        private Expression<Func<PropertyHolder, object>> _expression;
        private ICallback _uncalledCallback;

        [SetUp]
        public void SetUp()
        {
            _expression = ph => ph.Age;
            _callback = MockRepository.GenerateStub<ICallback>();
            _uncalledCallback = MockRepository.GenerateStub<ICallback>();
        }

        [Test]
        public void get_name_returns_expression_property_name()
        {
            _expression.GetName().ShouldEqual("Age");
        }

        [Test]
        public void ifPropertyTypeIs_invokes_method()
        {
            Accessor accessor = _expression.ToAccessor();
            accessor.IfPropertyTypeIs<int>(_callback.Callback);
            _callback.AssertWasCalled(c=>c.Callback());
            accessor.IfPropertyTypeIs<PropertyHolder>(_uncalledCallback.Callback);
            _uncalledCallback.AssertWasNotCalled(c=>c.Callback());
        }

        [Test]
        public void isInteger_returns_if_accessor_property_type_is_int()
        {
            Accessor accessor = _expression.ToAccessor();
            accessor.IsInteger().ShouldBeTrue();
        }
    }
}