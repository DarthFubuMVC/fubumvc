using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class BindingResultTester
    {
        private BindResult result;

        private class PropertyHolder{
            public string SomeProperty { get; set; }}

        [SetUp]
        public void SetUp()
        {
            result = new BindResult {Value = "some value"};
        }

        [Test]
        public void to_string_shows_problems_count()
        {
            result.ToString().ShouldEqual("BindResult: some value, Problems:  0");
        }

        [Test]
        public void should_throw_bind_result_assertion_exception_on_problems()
        {
            var problem = new ConvertProblem{Properties = new List<PropertyInfo> {
                  typeof(PropertyHolder).GetProperty("SomeProperty") }, Value = "some value"};
            result.Problems.Add(problem);

            var ex = typeof (BindResultAssertionException).ShouldBeThrownBy(
                () => result.AssertNoProblems(typeof (string))) as BindResultAssertionException;
            
            ex.ShouldNotBeNull();
            ex.Message.ShouldEqual("Failure while trying to bind object of type '{0}'".ToFormat(ex.Type) +
                "Property: {0}, Value: '{1}', Exception:{2}{3}{2}".ToFormat(problem.PropertyName()
                , problem.Value, Environment.NewLine, problem.ExceptionText));
            ex.Type.ShouldEqual(typeof (string));
            ex.Problems.ShouldContain(problem);
        }

        [Test]
        public void exception_should_serialize_properly()
        {
            var firstProblem = new ConvertProblem { Properties = new[]{ReflectionHelper.GetProperty<DateTime>(d => d.Month)} };
            var secondProblem = new ConvertProblem { Properties = new[] { ReflectionHelper.GetProperty<DateTime>(d => d.Day) } };
            var originalException = new BindResultAssertionException(typeof(string), new[] { firstProblem, secondProblem });
            
            var deserializedException = originalException.ShouldTransferViaSerialization();
            deserializedException.Type.ShouldEqual(originalException.Type);
            deserializedException.Problems.ShouldHaveCount(originalException.Problems.Count);
        }
    }
}