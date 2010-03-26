using FubuCore.Util;
using NUnit.Framework;

namespace FubuCore.Testing.Util
{
    [TestFixture]
    public class CacheTester
    {
        private Cache<string, int> cache;

        [SetUp]
        public void SetUp()
        {
            cache = new Cache<string, int>();
        }

        [Test]
        public void store_and_fetch()
        {
            cache["a"] = 1;
            cache["a"].ShouldEqual(1);

            cache["a"] = 2;
            cache["a"].ShouldEqual(2);
        }

        [Test]
        public void test_the_on_missing()
        {
            int count = 0;
            cache.OnMissing = key => ++count;


            cache["a"].ShouldEqual(1);
            cache["b"].ShouldEqual(2);
            cache["c"].ShouldEqual(3);

            cache["a"].ShouldEqual(1);
            cache["b"].ShouldEqual(2);
            cache["c"].ShouldEqual(3);

            cache.Count.ShouldEqual(3);
        }

        [Test]
        public void fill_only_writes_if_there_is_not_previous_value()
        {
            cache.Fill("a", 1);
            cache["a"].ShouldEqual(1);

            cache.Fill("a", 2);
            cache["a"].ShouldEqual(1); // did not overwrite
        }

        [Test]
        public void WithValue_positive()
        {
            cache["b"] = 2;

            int number = 0;

            cache.WithValue("b", i => number = i);

            number.ShouldEqual(2);
        }

        [Test]
        public void WithValue_negative()
        {
            cache.WithValue("b", i => Assert.Fail("Should not be called"));
        }
    }
}