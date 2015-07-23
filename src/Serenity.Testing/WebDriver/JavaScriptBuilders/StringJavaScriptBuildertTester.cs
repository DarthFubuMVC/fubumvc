using System.Collections.Generic;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using Serenity.WebDriver.JavaScriptBuilders;

namespace Serenity.Testing.WebDriver.JavaScriptBuilders
{
    public class StringJavaScriptBuilderTester : InteractionContext<StringJavaScriptBuilder>
    {
        [TestCaseSource("MatchTestCases")]
        public bool Matches(object obj)
        {
            return ClassUnderTest.Matches(obj);
        }

        public IEnumerable<TestCaseData> MatchTestCases()
        {
            yield return new TestCaseData(null).Returns(false);
            yield return new TestCaseData((string) null).Returns(false);
            yield return new TestCaseData("string").Returns(true);
            yield return new TestCaseData("").Returns(true);
            yield return new TestCaseData(1).Returns(false);
            yield return new TestCaseData(new object()).Returns(false);
        }

        [Test]
        public void WrapsStringWithQuotes()
        {
            ClassUnderTest.Build("some string").ShouldBe("\"some string\"");
        }
    }
}
