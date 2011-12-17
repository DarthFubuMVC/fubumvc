using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Resources.Media;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Resources.Media
{
    [TestFixture]
    public class DictionaryMediaNodeTester
    {
        private DictionaryMediaNode theNode;

        [SetUp]
        public void SetUp()
        {
            theNode = new DictionaryMediaNode();
        }

        [Test]
        public void set_an_attribute_writes_to_the_dictionary()
        {
            theNode.SetAttribute("key", "value");

            theNode.Values["key"].ShouldEqual("value");
        }

        [Test]
        public void add_child_adds_a_dictionary_to_the_dictionary()
        {
            theNode.AddChild("child").SetAttribute("color", "red");

            theNode.Values["child"].As<IDictionary<string, object>>()
                ["color"].ShouldEqual("red");
        }
    }
}