using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using Serenity.WebDriver;
using By = OpenQA.Selenium.By;

namespace Serenity.Testing.WebDriver
{
    public class SearchContextToJavaScriptExecutorTester : InteractionContext<SearchContextToJavaScriptExecutor>
    {
        [Test]
        public void SearchContextIsJavaScriptExecutor()
        {
            var context = new FakeJavaScriptExecutor();
            var convertedContext = ClassUnderTest.Convert(context);
            convertedContext.ShouldNotBeNull();
            convertedContext.ShouldBeOfType<FakeJavaScriptExecutor>();
            convertedContext.ShouldBeTheSameAs(context);
        }

        [Test]
        public void SearchContextIsWrappedDriver()
        {
            var executor = new FakeJavaScriptExecutor();
            var context = new FakeWrapsDriver(executor);
            var convertedContext = ClassUnderTest.Convert(context);
            convertedContext.ShouldNotBeNull();
            convertedContext.ShouldBeOfType<FakeJavaScriptExecutor>();
            convertedContext.ShouldBeTheSameAs(executor);
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void ThrowsIfCannotConvertToJavaScriptExecutor()
        {
            ClassUnderTest.Convert(new FakeSearchContextOnly());
        }

        public class FakeSearchContextOnly : ISearchContext
        {
            public IWebElement FindElement(By @by)
            {
                throw new NotSupportedException();
            }

            public ReadOnlyCollection<IWebElement> FindElements(By @by)
            {
                throw new NotSupportedException();
            }
        }

        public class FakeJavaScriptExecutor : IJavaScriptExecutor, IWebDriver
        {
            public IWebElement FindElement(By @by)
            {
                throw new NotSupportedException();
            }

            public ReadOnlyCollection<IWebElement> FindElements(By @by)
            {
                throw new NotSupportedException();
            }

            public object ExecuteScript(string script, params object[] args)
            {
                throw new NotSupportedException();
            }

            public object ExecuteAsyncScript(string script, params object[] args)
            {
                throw new NotSupportedException();
            }

            public void Dispose()
            {
                throw new NotSupportedException();
            }

            public void Close()
            {
                throw new NotSupportedException();
            }

            public void Quit()
            {
                throw new NotSupportedException();
            }

            public IOptions Manage()
            {
                throw new NotSupportedException();
            }

            public INavigation Navigate()
            {
                throw new NotSupportedException();
            }

            public ITargetLocator SwitchTo()
            {
                throw new NotSupportedException();
            }

            public string Url { get; set; }
            public string Title { get; private set; }
            public string PageSource { get; private set; }
            public string CurrentWindowHandle { get; private set; }
            public ReadOnlyCollection<string> WindowHandles { get; private set; }
        }

        public class FakeWrapsDriver : ISearchContext, IWrapsDriver
        {
            public FakeWrapsDriver(FakeJavaScriptExecutor executor)
            {
                WrappedDriver = executor;
            }

            public IWebElement FindElement(By @by)
            {
                throw new NotSupportedException();
            }

            public ReadOnlyCollection<IWebElement> FindElements(By @by)
            {
                throw new NotSupportedException();
            }

            public IWebDriver WrappedDriver { get; private set; }
        }
    }
}
