using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Projections;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Projections
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

        [Test]
        public void add_list_returns_a_node_list()
        {
            theNode.AddList("nodes").ShouldBeOfType<DictionaryMediaNodeList>();
        }

        [Test]
        public void add_children_to_a_list()
        {
            var list = theNode.AddList("nodes");
            list.Add().SetAttribute("animal", "Dolphin");
            list.Add().SetAttribute("animal", "Horse");

            var nodes = theNode.Values["nodes"].As<IList<IDictionary<string, object>>>();
            nodes.First()["animal"].ShouldEqual("Dolphin");
            nodes.Last()["animal"].ShouldEqual("Horse");
        }
    }
}