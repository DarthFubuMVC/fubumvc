using System;
using System.Collections.Generic;
using System.Globalization;
using FubuMVC.Core.Localization;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Localization
{
    
    public class ThreadSafeLocaleCacheTester
    {
        private ThreadSafeLocaleCache cache;

        public ThreadSafeLocaleCacheTester()
        {
            var dictionary = new Dictionary<LocalizationKey, string>()
            {
                {new LocalizationKey("a"), "a1"},
                {new LocalizationKey("b"), "b1"},
                {new LocalizationKey("c"), "c1"},
            };

            cache = new ThreadSafeLocaleCache(new CultureInfo("en-US"), dictionary);
        }

        [Fact]
        public void retrieve_a_key_that_can_be_found()
        {
            cache.Retrieve(new LocalizationKey("a"), () =>
            {
                throw new AccessViolationException("Shouldn't call this");
            })
            .ShouldBe("a1");
        }

        [Fact]
        public void append_and_retrieve()
        {
            cache.Append(new LocalizationKey("d"), "d1");
            cache.Retrieve(new LocalizationKey("d"), () =>
            {
                throw new AccessViolationException("Shouldn't call this");
            })
            .ShouldBe("d1");
        }

        [Fact]
        public void call_the_missing_function_if_the_key_cannot_be_found()
        {
            cache.Retrieve(new LocalizationKey("d"), () => "d1").ShouldBe("d1");

            // call again should hit the cache
            cache.Retrieve(new LocalizationKey("d"), () =>
            {
                throw new AccessViolationException("Shouldn't call this");
            })
            .ShouldBe("d1");
        }

		[Fact]
		public void initializing_via_localstrings_with_duplicate_keys_should_throw()
		{
			const string duplicatedkey = "duplicatedkey";
			var localStrings = new[]
            {
                new LocalString(duplicatedkey, "a1"),
                new LocalString(duplicatedkey, "a2")
            };

		    Exception<ArgumentException>.ShouldBeThrownBy(() =>
		    {
                new ThreadSafeLocaleCache(new CultureInfo("en-US"), localStrings);
            }).Message.ShouldContain(duplicatedkey);


		}

    }
}