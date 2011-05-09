using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.StructureMap;
using FubuTestingSupport;

namespace FubuCore.Testing.Binding
{
    public abstract class PropertyBinderTester
    {
        protected InMemoryStructureMapBindingContext context;
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

    public class AddressViewModel
    {
        public Address Address { get; set; }
        public bool ShouldShow { get; set; }
        public IList<LocalityViewModel> Localities { get; set; }
    }

    public class LocalityViewModel
    {
        public string ZipCode { get; set; }
        public string CountyName { get; set; }
    }
}