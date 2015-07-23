using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FubuCore.Reflection;
using FubuMVC.Core;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class ReflectionExtensionsTester
    {
        private MethodInfo _method1;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            _method1 = ReflectionHelper.GetMethod<PureTestPurposes>(x => x.Bind(5));
        }

        [Test]
        public void methods_from_different_types_should_not_match()
        {
            var method2 = ReflectionHelper.GetMethod<AnotherTestCase>(x => x.Bind(12));
            _method1.Matches(method2).ShouldBeFalse();
        }

        [Test]
        public void methods_with_different_names_should_not_match()
        {
            var method2 = ReflectionHelper.GetMethod<PureTestPurposes>(x => x.Bind2(20));
            _method1.Matches(method2).ShouldBeFalse();
        }

        [Test]
        public void methods_with_different_parameter_count_should_not_match()
        {
            var method2 = ReflectionHelper.GetMethod<PureTestPurposes>(x => x.SmartBind(20, "Some format"));
            _method1.Matches(method2).ShouldBeFalse();
        }

        [Test]
        public void method_should_match_itself()
        {
            _method1.Matches(_method1).ShouldBeTrue();
        }

        [Test]
        public void parameters_of_different_types_should_not_match()
        {
            var method = ReflectionHelper.GetMethod<PureTestPurposes>(x => x.SmartBind(20, "Some format"));
            var intParameter = method.GetParameters().SingleOrDefault(p => p.Name == "input");
            var stringParameter = method.GetParameters().SingleOrDefault(p => p.Name == "format");
            intParameter.Matches(stringParameter).ShouldBeFalse();
        }

        [Test]
        public void parameter_should_match_itself()
        {
            var parameter = _method1.GetParameters().SingleOrDefault(p => p.Name == "input");
            parameter.Matches(parameter).ShouldBeTrue();
        }
    }

    class PureTestPurposes
    {
        public void Bind(int input)
        {
            
        }

        public void Bind2(int input)
        {
            
        }

        public SomeResult SmartBind(int input, string format)
        {
            return new SomeResult();
        }
    }

    class SomeResult
    {
         
    }

    class AnotherTestCase
    {
         public void Bind(int input)
         {
             
         }
    }
}
