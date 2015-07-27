using System.Collections.Generic;
using System.Collections.Specialized;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Runtime.Headers
{
    [TestFixture]
    public class Headers_testing
    {
        [Test]
        public void get_and_set_with_name_value_collection()
        {
            var values = new NameValueCollection();
            values["a"] = "1";
            values["b"] = "2";

            var headers = new NameValueHeaders(values);
            headers["a"].ShouldBe("1");
            headers["b"].ShouldBe("2");

            headers["c"] = "3";

            values["c"].ShouldBe("3");
        }

        [Test]
        public void name_value_has()
        {
            var values = new NameValueCollection();
            values["a"] = "1";
            values["b"] = "2";

            var headers = new NameValueHeaders(values);

            headers.Has("a").ShouldBeTrue();
            headers.Has("c").ShouldBeFalse();
        }

        [Test]
        public void get_keys_from_name_value_collection()
        {
            var values = new NameValueCollection();
            values["a"] = "1";
            values["b"] = "2";
            values["c"] = "3";

            var headers = new NameValueHeaders(values);
        
            headers.Keys().ShouldHaveTheSameElementsAs("a", "b", "c");
        }

        [Test]
        public void get_and_set_with_dictionary()
        {
            var values = new Dictionary<string, string>{{"a", "1"}, {"b", "2"}};

            var headers = new DictionaryHeaders(values);
            headers["a"].ShouldBe("1");
            headers["b"].ShouldBe("2");

            headers["c"] = "3";

            values["c"].ShouldBe("3");
        }

        [Test]
        public void dictionary_has()
        {
            var values = new Dictionary<string, string> { { "a", "1" }, { "b", "2" } };

            var headers = new DictionaryHeaders(values);

            headers.Has("a").ShouldBeTrue();
            headers.Has("c").ShouldBeFalse();
        }

        [Test]
        public void to_name_values_for_dictionary()
        {
            var values = new Dictionary<string, string> { { "a", "1" }, { "b", "2" } };

            var headers = new DictionaryHeaders(values);

            var collection = headers.ToNameValues();
            collection["a"].ShouldBe("1");
            collection["b"].ShouldBe("2");
        }

        [Test]
        public void get_keys_from_dictionary()
        {
            var values = new Dictionary<string, string> { { "a", "1" }, { "b", "2" }, {"c", "3"} };

            var headers = new DictionaryHeaders(values);

            headers.Keys().ShouldHaveTheSameElementsAs("a", "b", "c");
        }

    }
}