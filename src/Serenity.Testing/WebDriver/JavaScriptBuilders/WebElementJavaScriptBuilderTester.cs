using System.Collections.ObjectModel;
using System.Drawing;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.WebDriver.JavaScriptBuilders;
using System.Collections.Generic;

namespace Serenity.Testing.WebDriver.JavaScriptBuilders
{
    public class WebElementJavaScriptBuilderTester : InteractionContext<WebElementJavaScriptBuilder>
    {
        [TestCaseSource("MatchTestCases")]
        public bool Matches(object obj)
        {
            return ClassUnderTest.Matches(obj);
        }

        public IEnumerable<TestCaseData> MatchTestCases()
        {
            yield return new TestCaseData(null).Returns(false);
            yield return new TestCaseData("string").Returns(false);
            yield return new TestCaseData("").Returns(false);
            yield return new TestCaseData(1).Returns(false);
            yield return new TestCaseData(new object()).Returns(false);
            yield return new TestCaseData(new FakeWebElement()).Returns(true);
        }

        [Test]
        public void ReturnsNullAsString()
        {
            ClassUnderTest.Build(MockFor<IWebElement>()).ShouldBe(WebElementJavaScriptBuilder.Marker);
        }

        public class FakeWebElement : IWebElement
        {
            public IWebElement FindElement(By @by)
            {
                throw new System.NotSupportedException();
            }

            public ReadOnlyCollection<IWebElement> FindElements(By @by)
            {
                throw new System.NotSupportedException();
            }

            public void Clear()
            {
                throw new System.NotSupportedException();
            }

            public void SendKeys(string text)
            {
                throw new System.NotSupportedException();
            }

            public void Submit()
            {
                throw new System.NotSupportedException();
            }

            public void Click()
            {
                throw new System.NotSupportedException();
            }

            public string GetAttribute(string attributeName)
            {
                throw new System.NotSupportedException();
            }

            public string GetCssValue(string propertyName)
            {
                throw new System.NotSupportedException();
            }

            public string TagName { get; private set; }
            public string Text { get; private set; }
            public bool Enabled { get; private set; }
            public bool Selected { get; private set; }
            public Point Location { get; private set; }
            public Size Size { get; private set; }
            public bool Displayed { get; private set; }
        }
    }
}