using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using NUnit.Framework;
using System.Linq;
using Shouldly;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class ActionMethodFilterTester
    {
        private List<MethodInfo> methods;

        [SetUp]
        public void SetUp()
        {
            var filter = new ActionMethodFilter();
            methods = typeof(ActionMethodTarget).PublicInstanceMethods().Where(filter.Matches).ToList();
        }

        private bool contains(Expression<Action<ActionMethodTarget>> expression)
        {
            var method = ReflectionHelper.GetMethod(expression);
            return methods.Any(x => x.Name == method.Name); // Never count on equality between reflection members
        }

        [Test]
        public void does_not_include_object_methods()
        {
            contains(x => x.ToString()).ShouldBeFalse();
            contains(x => x.GetHashCode()).ShouldBeFalse();
            contains(x => x.Equals(null)).ShouldBeFalse();
        }

        [Test]
        public void does_not_include_methods_from_marshal_by_ref_object()
        {
            contains(x => x.InitializeLifetimeService()).ShouldBeFalse();
        }

        [Test]
        public void does_not_contain_the_dispose_method()
        {
            contains(x => x.Dispose()).ShouldBeFalse();
        }

        [Test]
        public void does_not_contain_property_accessors()
        {
            methods.Any(x => x.Name.Contains("Name")).ShouldBeFalse();
        }

        [Test]
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