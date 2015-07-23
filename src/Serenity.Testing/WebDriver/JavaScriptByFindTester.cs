using System.Collections.Generic;
using System.Collections.ObjectModel;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using NUnit.Framework;
using OpenQA.Selenium;
using Rhino.Mocks;
using Serenity.WebDriver;
using By = Serenity.WebDriver.By;

namespace Serenity.Testing.WebDriver
{
    public abstract class JavaScriptByFindTester<TExpected, TActual> : InteractionContext<JavaScriptBy>
    {
        private ISearchContext _searchContext;

        protected abstract TExpected ExpectedResult { get; }
        protected TActual FoundResult { get; private set; }
        protected JavaScriptBy Selector { get; set; }

        protected bool ExecuteFind = true;

        protected override void beforeEach()
        {
            var javascriptExecutor = MockFor<IJavaScriptExecutor>();
            javascriptExecutor.Stub(x => x.ExecuteScript(Arg<string>.Is.Anything, Arg<object[]>.Is.Anything)).Return(ExpectedResult);
            javascriptExecutor.Stub(x => x.ExecuteAsyncScript(Arg<string>.Is.Anything, Arg<object[]>.Is.Anything)).Return(ExpectedResult);

            _searchContext = MockFor<ISearchContext>();

            var converter = MockFor<ISearchContextToJavaScriptExecutor>();
            converter.Stub(x => x.Convert(_searchContext)).Return(javascriptExecutor);
            JavaScriptBy.SearchContextConverter = converter;

            Selector = By.jQuery(".test");

            if (ExecuteFind)
            {
                Execute();
            }
        }

        protected void Execute()
        {
            FoundResult = Find(_searchContext);
        }

        protected abstract TActual Find(ISearchContext context);

        [TestFixtureTearDown]
        public void ResetConverter()
        {
            JavaScriptBy.ResetConverter();
        }
    }

    public abstract class JavaScriptByFindTester<T> : JavaScriptByFindTester<T, T> { }

    public class JQueryByFindElementTester : JavaScriptByFindTester<IWebElement>
    {
        private IWebElement _expectedResult;
        protected override IWebElement ExpectedResult
        {
            get { return _expectedResult ?? (_expectedResult = MockFor<IWebElement>()); }
        }

        protected override IWebElement Find(ISearchContext context)
        {
            return Selector.FindElement(context);
        }

        [Test]
        public void ReturnsAnElement()
        {
            FoundResult.ShouldNotBeNull();
        }

        [Test]
        public void ReturnsTheExpectedElement()
        {
            FoundResult.ShouldBeTheSameAs(ExpectedResult);
        }
    }

    public class JQueryByFindElementNull : JavaScriptByFindTester<IWebElement>
    {
        protected override IWebElement ExpectedResult
        {
            get { return null; }
        }

        protected override void beforeEach()
        {
            ExecuteFind = false;
            base.beforeEach();
        }

        protected override IWebElement Find(ISearchContext context)
        {
            return Selector.FindElement(context);
        }

        [Test, ExpectedException(typeof(NoSuchElementException))]
        public void ThrowsNoSuchElementException()
        {
            Execute();
        }
    }

    public class JQueryByFindElementsNone : JavaScriptByFindTester<ReadOnlyCollection<object>, ReadOnlyCollection<IWebElement>>
    {
        protected override ReadOnlyCollection<object> ExpectedResult
        {
            get { return new ReadOnlyCollection<object>(new List<object>()); }
        }

        protected override ReadOnlyCollection<IWebElement> Find(ISearchContext context)
        {
            return Selector.FindElements(context);
        }

        [Test]
        public void ReturnsCollection()
        {
            FoundResult.ShouldNotBeNull();
        }

        [Test]
        public void ReturnedCollectionIsEmpty()
        {
            FoundResult.ShouldBeEmpty();
        }
    }

    public class JQueryByFindElementsWithResults : JavaScriptByFindTester<ReadOnlyCollection<IWebElement>, ReadOnlyCollection<IWebElement>>
    {
        private ReadOnlyCollection<IWebElement> _expectedResult; 
        protected override ReadOnlyCollection<IWebElement> ExpectedResult
        {
            get
            {
                return _expectedResult ?? (_expectedResult = new ReadOnlyCollection<IWebElement>(new []
                {
                    Services.AddAdditionalMockFor<IWebElement>(),
                    Services.AddAdditionalMockFor<IWebElement>()
                }));
            }
        }

        protected override ReadOnlyCollection<IWebElement> Find(ISearchContext context)
        {
            return Selector.FindElements(context);
        }

        [Test]
        public void ReturnsCollection()
        {
            FoundResult.ShouldNotBeNull();
        }

        [Test]
        public void ReturnedCollectionHasItems()
        {
            FoundResult.Count.ShouldBe(2);
        }

        [Test]
        public void CollectionContainsExpectedElements()
        {
            FoundResult.ShouldHaveTheSameElementsAs(ExpectedResult[0], ExpectedResult[1]);
        }
    }
}
