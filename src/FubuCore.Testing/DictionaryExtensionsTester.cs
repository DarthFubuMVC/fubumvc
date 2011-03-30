using System.Collections.Generic;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class DictionaryExtensionsTester
    {
        private Dictionary<string, int> dictionary;

        [SetUp]
        public void SetUp()
        {
            dictionary = new Dictionary<string, int>()
            {
                {"a", 1},
                {"b", 2},
                {"c", 3},
            };
        }

        [Test]
        public void get_a_dictionary_value_that_exists()
        {
            dictionary.Get("a").ShouldEqual(1);
        }

        [Test]
        public void get_a_dictionary_value_that_does_not_exist_and_get_the_default_value()
        {
            dictionary.Get("d").ShouldEqual(default(int));
        }

        [Test]
        public void get_a_dictionary_value_that_exists_2()
        {
            dictionary.Get("a", 5).ShouldEqual(1);
        }


        [Test]
        public void get_a_dictionary_value_that_does_not_exist_and_get_the_default_value_2()
        {
            dictionary.Get("d", 5).ShouldEqual(5);
        }
    }
}