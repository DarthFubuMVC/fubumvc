using System.Collections.Generic;
using NUnit.Framework;

namespace FubuLocalization.Tests
{
    [TestFixture]
    public class LocalStringTester
    {
        [Test]
        public void two_instances_with_the_same_key_should_equal_each_other()
        {
            var x = new LocalString { value = "foo" };
            var y = new LocalString { value = "foo" };

            x.ShouldEqual(y);
            y.ShouldEqual(x);
        }

        [Test]
        public void two_instances_with_the_same_key_should_be_considered_the_same_for_hashing_purposes()
        {
            var x = new LocalString { value = "foo" };
            var y = new LocalString { value = "foo" };

            var dict = new Dictionary<LocalString, int> { { x, 0 } };

            dict.ContainsKey(y).ShouldBeTrue();
        }
    }
}