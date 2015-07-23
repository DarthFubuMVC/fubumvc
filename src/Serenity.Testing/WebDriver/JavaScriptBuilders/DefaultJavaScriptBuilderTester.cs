using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Serenity.WebDriver.JavaScriptBuilders;

namespace Serenity.Testing.WebDriver.JavaScriptBuilders
{
    public class DefaultJavaScriptBuilderTester : InteractionContext<DefaultJavaScriptBuilder>
    {
        [TestCaseSource("TestCases")]
        public void MatchesAnything(object obj)
        {
            ClassUnderTest.Matches(obj).ShouldBeTrue();
        }

        public IEnumerable TestCases()
        {
            yield return null;
            yield return "some string";
            yield return 1;
            yield return HttpStatusCode.OK;
            yield return new Object();
            yield return new ToStringOverride();
        }

        [TestCaseSource("BuildTestCases")]
        public string ReturnsToStringOfObjectPassedIn(object obj)
        {
            return ClassUnderTest.Build(obj);
        }

        public IEnumerable<TestCaseData> BuildTestCases()
        {
            return TestCases().Cast<object>().Select<object, TestCaseData>(x => new TestCaseData(x).Returns(x == null ? "null" : x.ToString()));
        }

        public class ToStringOverride
        {
            public override string ToString()
            {
                return "Simple to string method";
            }
        }
    }
}