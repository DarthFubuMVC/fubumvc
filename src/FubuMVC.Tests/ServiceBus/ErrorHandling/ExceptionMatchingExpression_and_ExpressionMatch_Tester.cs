using System;
using FubuCore;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    [TestFixture]
    public class ExceptionMatchingExpression_and_ExpressionMatch_Tester
    {
        private ExceptionMatch theMatch;
        private ExceptionMatchExpression theExpression;

        [SetUp]
        public void SetUp()
        {
            theExpression = new ExceptionMatchExpression(m => theMatch = m.As<ExceptionMatch>());
        }

        [Test]
        public void message_contains()
        {
            var exception1 = new NotImplementedException("I don't like you");
            var exception2 = new NotImplementedException("It's all good");

            theExpression.MessageContains("like you");

            theMatch.Description.ShouldBe("Exception message contains 'like you'");

            theMatch.Matches(null, exception1).ShouldBeTrue();
            theMatch.Matches(null, exception2).ShouldBeFalse();
        }

        [Test]
        public void exception_type()
        {
            var exception1 = new NotImplementedException();
            var exception2 = new NotSupportedException();

            theExpression.IsType<NotImplementedException>();

            theMatch.Description.ShouldBe("Exception type is " + typeof (NotImplementedException).FullName);

            theMatch.Matches(null, exception1).ShouldBeTrue();
            theMatch.Matches(null, exception2).ShouldBeFalse();
        }
    }
}