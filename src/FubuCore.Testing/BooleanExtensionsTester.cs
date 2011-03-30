using System;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class BooleanExtensionsTester
    {
        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void If_should_also_return_an_ArgumentException()
        {
            var test = new TestObject
            {
                Boolean = true
            };

            "test".If(() => test.Boolean && test.Boolean);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void If_should_return_an_ArgumentException()
        {
            var test = new TestObject
            {
                Value = 1
            };

            "test".If(() => test.Value == 1);
        }

        [Test]
        public void If_should_return_empty_string_value()
        {
            var test = new TestObject
            {
                Boolean = false
            };

            "test".If(() => test.Boolean).ShouldEqual("");
        }

        [Test]
        public void If_should_return_original_value()
        {
            var test = new TestObject
            {
                Boolean = true
            };

            "test".If(() => test.Boolean).ShouldEqual("test");
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void IfNot_should_also_return_an_ArgumentException()
        {
            var test = new TestObject
            {
                Boolean = true
            };

            "test".IfNot(() => test.Boolean && test.Boolean);
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void IfNot_should_return_an_ArgumentException()
        {
            var test = new TestObject
            {
                Value = 1
            };

            "test".IfNot(() => test.Value == 1);
        }

        [Test]
        public void IfNot_should_return_empty_string_value()
        {
            var test = new TestObject
            {
                Boolean = false
            };

            "test".IfNot(() => test.Boolean).ShouldEqual("test");
        }

        [Test]
        public void IfNot_should_return_original_value()
        {
            var test = new TestObject
            {
                Boolean = true
            };

            "test".IfNot(() => test.Boolean).ShouldEqual("");
        }
    }
}