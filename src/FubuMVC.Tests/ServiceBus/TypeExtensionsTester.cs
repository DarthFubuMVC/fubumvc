using System;
using FubuMVC.Core.ServiceBus;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class TypeExtensionsTester
    {
        [Test]
        public void has_property_returns_true_if_property_exists()
        {
            typeof(TestSaga).HasProperty("CorrelationId").ShouldBeTrue();
        }

        [Test]
        public void has_property_returns_false_if_property_does_not_exists()
        {
            typeof(TestSaga).HasProperty("State").ShouldBeFalse();
        }

        [Test]
        public void simple_types_do_not_throw()
        {
            typeof(int).HasProperty("Blah").ShouldBeFalse();
        }
    }

    public class TestSaga
    {
        public Guid CorrelationId { get; set; }
    }
}