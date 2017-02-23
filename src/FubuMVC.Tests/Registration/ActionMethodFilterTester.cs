using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using Xunit;
using System.Linq;
using Shouldly;

namespace FubuMVC.Tests.Registration
{
    
    public class ActionMethodFilterTester
    {
        private List<MethodInfo> methods;

        public ActionMethodFilterTester()
        {
            var filter = new ActionMethodFilter();
            methods = typeof(ActionMethodTarget).PublicInstanceMethods().Where(filter.Matches).ToList();
        }

        private bool contains(Expression<Action<ActionMethodTarget>> expression)
        {
            var method = ReflectionHelper.GetMethod(expression);
            return methods.Any(x => x.Name == method.Name); // Never count on equality between reflection members
        }

        [Fact]
        public void does_not_include_object_methods()
        {
            contains(x => x.ToString()).ShouldBeFalse();
            contains(x => x.GetHashCode()).ShouldBeFalse();
            contains(x => x.Equals(null)).ShouldBeFalse();
        }

        [Fact]
        public void does_not_include_methods_from_marshal_by_ref_object()
        {
            contains(x => x.InitializeLifetimeService()).ShouldBeFalse();
        }

        [Fact]
        public void does_not_contain_the_dispose_method()
        {
            contains(x => x.Dispose()).ShouldBeFalse();
        }

        [Fact]
        public void does_not_contain_property_accessors()
        {
            methods.Any(x => x.Name.Contains("Name")).ShouldBeFalse();
        }

        [Fact]
        public void does_contain_other_methods()
        {
            methods.Single().Name.ShouldBe("Good");
        }


        public class ActionMethodTarget : MarshalByRefObject, IDisposable
        {
            public string Name { get; set; }
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public string Good() {
                return "hello"; }
        }
    }
}