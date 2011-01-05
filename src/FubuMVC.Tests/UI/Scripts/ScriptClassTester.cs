using System;
using System.Collections.Generic;
using FubuMVC.Core.UI.Scripts;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.UI.Scripts
{
    [TestFixture]
    public class ScriptGraphTester
    {
        private ScriptGraph theGraph;

        [SetUp] 
        public void SetUp()
        {
            theGraph = new ScriptGraph(new StubScriptFinder()); 
        }

        [Test]
        public void get_a_set_for_an_empty_graph_returns_an_empty_set_of_that_name()
        {
            theGraph.ScriptSetFor("some name")
                .ShouldNotBeNull()
                .Name.ShouldEqual("some name");
        }

        private IEnumerable<string> ScriptNamesFor(params string[] names)
        {
            return theGraph.GetScripts(names).Select(x => x.Name);
        }


        [Test]
        public void find_object_by_name_and_the_object_does_not_exist_use_script_by_name()
        {
            theGraph.ObjectFor("Name.js").ShouldBeOfType<IScript>()
                .Name.ShouldEqual("Name.js");
        }

        [Test]
        public void find_object_by_alias()
        {
            theGraph.Alias("Name.js", "Name");
            theGraph.ObjectFor("Name").ShouldBeOfType<IScript>()
                .Name.ShouldEqual("Name.js");
        }

        [Test]
        public void find_a_set_by_name()
        {
            theGraph.AddToSet("SetA", "A");
            theGraph.ObjectFor("SetA").ShouldBeOfType<ScriptSet>();
        }

        [Test]
        public void find_a_set_by_alias()
        {
            theGraph.AddToSet("SetA", "A");
            theGraph.Alias("SetA", "SetA-Alias");
            theGraph.ObjectFor("SetA-Alias").ShouldBeOfType<ScriptSet>().Name.ShouldEqual("SetA");
        }

    }

    public class StubScriptFinder : IScriptFinder
    {
        public IScript Find(string name)
        {
            return new Script(){
                Name = name
            };
        }
    }
}