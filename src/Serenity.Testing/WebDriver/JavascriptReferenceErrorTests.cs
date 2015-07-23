using System;
using Shouldly;
using NUnit.Framework;
using OpenQA.Selenium;
using Rhino.Mocks;
using Serenity.WebDriver;

namespace Serenity.Testing.WebDriver
{
    [TestFixture]
    public class JavascriptReferenceErrorTests
    {
        private JavaScript ClassUnderTest;
        private IJavaScriptExecutor _executor;

        [SetUp]
        public void Setup()
        {
            ClassUnderTest = JavaScript.CreateJQuery("$(.test)");
            _executor = MockRepository.GenerateMock<IJavaScriptExecutor>();
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowsInvalidOperationExceptionForReferenceError()
        {
            _executor.Stub(x => x.ExecuteAsyncScript(Arg<string>.Is.Anything, Arg<object[]>.Is.Anything)).Return("ReferenceError");
            ClassUnderTest.ExecuteAndGet(_executor);
        }

        [Test]
        public void DoesNotThrowInvalidOperationExceptionForNull()
        {
            _executor.Stub(x => x.ExecuteAsyncScript(Arg<string>.Is.Anything, Arg<object[]>.Is.Anything)).Return(null);
            ClassUnderTest.ExecuteAndGet(_executor).ShouldBeNull();
        }

        [Test]
        public void DoesNotThrowInvalidOperationExceptionForEmptyString()
        {
            _executor.Stub(x => x.ExecuteAsyncScript(Arg<string>.Is.Anything, Arg<object[]>.Is.Anything)).Return(string.Empty);
            ClassUnderTest.ExecuteAndGet(_executor).ShouldBe(string.Empty);
        }
    }
}