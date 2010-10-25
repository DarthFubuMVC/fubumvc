using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace FubuLocalization.Tests
{
    [TestFixture]
    public class ThreadSafeLocaleCacheTester
    {
        private ThreadSafeLocaleCache cache;

        [SetUp]
        public void SetUp()
        {
            var dictionary = new Dictionary<LocalizationKey, string>()
            {
                {new LocalizationKey("a"), "a1"},
                {new LocalizationKey("b"), "b1"},
                {new LocalizationKey("c"), "c1"},
            };

            cache = new ThreadSafeLocaleCache(new CultureInfo("en-US"), dictionary);
        }

        [Test]
        public void retrieve_a_key_that_can_be_found()
        {
            cache.Retrieve(new LocalizationKey("a"), () =>
            {
                throw new AccessViolationException("Shouldn't call this");
            })
            .ShouldEqual("a1");
        }

        [Test]
        public void append_and_retrieve()
        {
            cache.Append(new LocalizationKey("d"), "d1");
            cache.Retrieve(new LocalizationKey("d"), () =>
            {
                throw new AccessViolationException("Shouldn't call this");
            })
            .ShouldEqual("d1");
        }

        [Test]
        public void call_the_missing_function_if_the_key_cannot_be_found()
        {
            cache.Retrieve(new LocalizationKey("d"), () => "d1").ShouldEqual("d1");

            // call again should hit the cache
            cache.Retrieve(new LocalizationKey("d"), () =>
            {
                throw new AccessViolationException("Shouldn't call this");
            })
            .ShouldEqual("d1");
        }
    }
}