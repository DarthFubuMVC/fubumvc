using System;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.ErrorHandling
{
    [TestFixture]
    public class ExceptionTypeMatchTester
    {
        [Test]
        public void matches_by_type()
        {
            var match = new ExceptionTypeMatch<NotImplementedException>();

            // Hey, it's important that this code actually works
            match.Matches(null, new NotImplementedException()).ShouldBeTrue();
            match.Matches(null, new Exception()).ShouldBeFalse();
            match.Matches(null, new NotSupportedException()).ShouldBeFalse();
            match.Matches(null, new ApplicationException()).ShouldBeFalse();
        }
    }
}