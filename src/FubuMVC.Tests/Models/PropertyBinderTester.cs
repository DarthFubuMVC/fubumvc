using System;
using System.Linq.Expressions;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.StructureMap;
using FubuMVC.Tests.UI;

namespace FubuMVC.Tests.Models
{
    public abstract class PropertyBinderTester
    {
        protected InMemoryBindingContext context;
        protected IPropertyBinder propertyBinder;

        protected bool matches(Expression<Func<AddressViewModel, object>> expression)
        {
            var property = ReflectionHelper.GetProperty(expression);
            return propertyBinder.Matches(property);
        }

        protected void shouldMatch(Expression<Func<AddressViewModel, object>> expression)
        {
            matches(expression).ShouldBeTrue();
        }

        protected void shouldNotMatch(Expression<Func<AddressViewModel, object>> expression)
        {
            matches(expression).ShouldBeFalse();
        }
    }
}